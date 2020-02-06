using CSCore;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;

// Written with help from Arduano

namespace OmniConverter
{
    public class BASSMIDI : ISampleSource
    {
        private readonly object Lock = new object();
        private BASSFlag Flags = BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_MIDI_DECAYEND;
        private int Handle;

        public string UniqueID { get; } = IDGenerator.GetID();
        public bool ErroredOut { get; } = false;
        public bool CanSeek { get; } = false;
        public WaveFormat WaveFormat { get; }

        // Special RTS mode
        private Random RTSR = new Random();
        private Boolean RTSMode { get; } = false;
        private UInt32 EventC;
        private BASS_MIDI_EVENT[] EventS;
        private Double CFMin { get; } = Properties.Settings.Default.CFValue - Properties.Settings.Default.CFFluctuation;
        private Double CFMax { get; } = Properties.Settings.Default.CFValue + Properties.Settings.Default.CFFluctuation;

        public BASSMIDI(String MIDI, Int32 TrackToExport, WaveFormat WF)
        {
            lock (Lock)
            {
                WaveFormat = WF;
                InitializeSettings(TrackToExport);

                Handle = BassMidi.BASS_MIDI_StreamCreateFile(MIDI, 0L, 0L, Flags, WaveFormat.SampleRate);
                if (!CheckError(MIDI, Handle, String.Format("Unable to create stream for {0} ({1}).", MIDI, UniqueID)))
                {
                    ErroredOut = true;
                    return;
                }
                if (Handle == 0)
                {
                    BASSError ERR = Bass.BASS_ErrorGetCode();

                    Debug.ShowMsgBox(
                        "BASSMIDI error", 
                        String.Format("Unable to create stream for {0} ({1}).\n\nError encountered: {2}", MIDI, UniqueID, ERR),
                        null, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    ErroredOut = true;
                    return;
                }
                Debug.PrintToConsole("ok", String.Format("{0} - Stream for file {1} is open.", UniqueID, MIDI));

                Int32 Tracks = BassMidi.BASS_MIDI_StreamGetTrackCount(Handle);
                Debug.PrintToConsole("ok", String.Format("{0} - Total tracks = {1}", UniqueID, Tracks));

                if (TrackToExport != -1)
                {
                    for (int i = 0; i < Tracks; i++)
                        Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_MIDI_TRACK_VOL + i, 0.0f);

                    Bass.BASS_ChannelSetAttribute(Handle, BASSAttribute.BASS_ATTRIB_MIDI_TRACK_VOL + TrackToExport, 1.0f);
                    Debug.PrintToConsole("ok", String.Format("{0} - Track {1} is ready for export.", UniqueID, Tracks));
                }

                SetSoundFonts();
            }
        }

        // This is a really unstable mode, touching anything in its code might break it
        public BASSMIDI(Boolean RTS, String MIDI, WaveFormat WF)
        {
            if (!RTS)
            {
                Debug.ShowMsgBox("Wait what!?", "The \"RTS\" argument should be set to TRUE!", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErroredOut = true;
                return;
            }

            lock (Lock)
            {
                Int32 MT = Properties.Settings.Default.MultiThreadedMode ? Properties.Settings.Default.MultiThreadedLimitV : 1;

                RTSMode = RTS;
                WaveFormat = WF;
                InitializeSettings(-1);

                // Remove DECAYEND since it's not applicable to this type of stream
                Flags &= ~BASSFlag.BASS_MIDI_DECAYEND;

                // Create stream which will feed the events to
                Handle = BassMidi.BASS_MIDI_StreamCreate(16, Flags, WaveFormat.SampleRate);
                if (!CheckError(MIDI, Handle, String.Format("Unable to create RTS stream for {0} ({1}).", MIDI, UniqueID)))
                {
                    ErroredOut = true;
                    return;
                }
                Debug.PrintToConsole("ok", String.Format("{0} - RTS stream for file {1} is open.", UniqueID, MIDI));

                // Parse events
                try
                {
                    // Load the MIDI from where we should extract the events
                    Int32 TempHandle = BassMidi.BASS_MIDI_StreamCreateFile(MIDI, 0L, 0L, Flags, WaveFormat.SampleRate);
                    if (!CheckError(MIDI, TempHandle, String.Format("Unable to load events for stream {1}.", MIDI, UniqueID)))
                    {
                        ErroredOut = true;
                        return;
                    }
                    Debug.PrintToConsole("ok", String.Format("{0} - Temporary stream for chunks copying is ready.", UniqueID, MIDI));

                    // Prepare events buffer and temporary chunk
                    BASS_MIDI_EVENT[] TChunk;
                    Int32 BMES = Marshal.SizeOf(typeof(BASS_MIDI_EVENT));
                    EventC = (UInt32)BassMidi.BASS_MIDI_StreamGetEvents(TempHandle, -1, 0, null);
                    EventS = new BASS_MIDI_EVENT[EventC];
                    Debug.PrintToConsole("ok", String.Format("{0} - Events buffer and temporary chunk prepared.", UniqueID));
                    Debug.PrintToConsole("ok", 
                        String.Format("{0} - Size: {1} ({2} events)", UniqueID, DataCheck.BytesToHumanReadableSize(Convert.ToUInt32(BMES) * EventC), EventC));

                    Parallel.For(0, EventC / 50000000, new ParallelOptions { MaxDegreeOfParallelism = MT }, Chunk =>
                    {
                        Debug.PrintToConsole("ok", String.Format("{0} - Preparing C{1}", UniqueID, Chunk));

                        Int32 SC = Math.Min(50000000, (Int32)EventC - ((Int32)Chunk * 50000000));
                        Debug.PrintToConsole("ok", 
                            String.Format("{0} C{1} - Size: {2} ({3} events)", 
                            UniqueID, Chunk, DataCheck.BytesToHumanReadableSize(Convert.ToUInt32(BMES) * Convert.ToUInt32(SC)), SC));

                        TChunk = new BASS_MIDI_EVENT[SC];
                        Int32 R = BassMidi.BASS_MIDI_StreamGetEvents(TempHandle, -1, 0, TChunk, (Int32)Chunk * 50000000, SC);
                        if (R < 1) return;

                        TChunk.CopyTo(EventS, Chunk * 50000000);
                        TChunk = null;
                    });
                }
                catch
                {
                    Debug.ShowMsgBox("BASSMIDI error", "This file is too big for RTS mode.", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErroredOut = true;
                    return;
                }

                Debug.PrintToConsole("ok", String.Format("{0} - Total tracks = {1}", UniqueID, BassMidi.BASS_MIDI_StreamGetTrackCount(Handle)));

                SetSoundFonts();
            }
        }

        private bool CheckError(String MIDI, Int32 H, String Error)
        {
            if (H == 0)
            {
                Error += String.Format("\n\nError encountered: {0}", Bass.BASS_ErrorGetCode());
                Debug.ShowMsgBox("BASSMIDI error",  Error, null, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
        }
        
        private void InitializeSettings(Int32 TrackToExport)
        {
            Debug.PrintToConsole("ok", String.Format("Stream unique ID: {0}", UniqueID));

            Debug.PrintToConsole(
                "ok",
                String.Format("{0} - Preparing stream for {1}...", UniqueID, TrackToExport == -1 ? "all tracks" : String.Format("track {0}", TrackToExport))
                );

            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_MIDI_VOICES, Properties.Settings.Default.VoiceLimit);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_MIDI_AUTOFONT, 0);
            Bass.BASS_SetConfigPtr(BASSConfig.BASS_CONFIG_MIDI_DEFFONT, IntPtr.Zero);
            Debug.PrintToConsole("ok", String.Format("{0} - Voice limit set to {1}.", UniqueID, Properties.Settings.Default.VoiceLimit));

            Flags |= Properties.Settings.Default.SincInter ? BASSFlag.BASS_MIDI_SINCINTER : BASSFlag.BASS_DEFAULT;
            Debug.PrintToConsole("ok", String.Format("{0} - SincInter = {1}", UniqueID, Properties.Settings.Default.SincInter.ToString()));

            Flags |= Properties.Settings.Default.DisableEffects ? BASSFlag.BASS_MIDI_NOFX : BASSFlag.BASS_DEFAULT;
            Debug.PrintToConsole("ok", String.Format("{0} - DisableEffects = {1}", UniqueID, Properties.Settings.Default.DisableEffects.ToString()));

            Flags |= Properties.Settings.Default.NoteOff1 ? BASSFlag.BASS_MIDI_NOTEOFF1 : BASSFlag.BASS_DEFAULT;
            Debug.PrintToConsole("ok", String.Format("{0} - NoteOff1 = {1}", UniqueID, Properties.Settings.Default.NoteOff1.ToString()));
        }

        private void SetSoundFonts()
        {
            BassMidi.BASS_MIDI_StreamSetFonts(Handle, Program.SFArray.BMFEArray.ToArray(), Program.SFArray.BMFEArray.Count);
            BassMidi.BASS_MIDI_StreamLoadSamples(Handle);
            Debug.PrintToConsole("ok", String.Format("{0} - Loaded {1} SoundFonts.", UniqueID, Program.SFArray.BMFEArray.Count));
        }

        public unsafe int NRead(float[] buffer, int offset, int count)
        {
            lock (Lock)
            {
                fixed (float* buff = buffer)
                {
                    var obuff = buff + offset;
                    int ret = Bass.BASS_ChannelGetData(Handle, (IntPtr)obuff, (count * 4) | (int)BASSData.BASS_DATA_FLOAT);

                    if (ret == 0)
                    {
                        BASSError BE = Bass.BASS_ErrorGetCode();
                        if (BE != BASSError.BASS_ERROR_ENDED)
                        {
                            Debug.ShowMsgBox(
                                "Data parsing error",
                                "An error has occured while parsing the audio data from the BASS stream.",
                                null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        Debug.PrintToConsole("wrn", BE.ToString());
                    }

                    return ret / 4;
                }
            }
        }

        public unsafe int RTRead(float[] buffer, int offset, int count)
        {
            lock (Lock)
            {
                fixed (float* buff = buffer)
                {
                    double FPSSim = RTSR.NextDouble() * (CFMin - CFMax) + CFMax;
                    int TLen = Convert.ToInt32(Bass.BASS_ChannelSeconds2Bytes(Handle, FPSSim));
                    byte[] TBuf = new byte[TLen];

                    int ret = Bass.BASS_ChannelGetData(Handle, TBuf, TLen);
                    if (ret == 0)
                    {
                        BASSError BE = Bass.BASS_ErrorGetCode();
                        if (BE != BASSError.BASS_ERROR_ENDED)
                        {
                            Debug.ShowMsgBox(
                                "Data parsing error",
                                "An error has occured while parsing the audio data from the BASS RTS stream.",
                                null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        Debug.PrintToConsole("wrn", BE.ToString());
                    }


                    Buffer.BlockCopy(TBuf, 0, buffer, 0, TLen);

                    return ret;
                }
            }
        }

        public unsafe int Read(float[] buffer, int offset, int count)
        {
            return RTSMode ? RTRead(buffer, offset, count) : NRead(buffer, offset, count);
        }

        public long Position
        {
            get { return Bass.BASS_ChannelGetPosition(Handle) / 4; }
            set { throw new NotSupportedException("Can't set position."); }
        }

        public long Length
        {
            get { return Bass.BASS_ChannelGetLength(Handle) / 4; }
        }

        public void Dispose()
        {
            lock (Lock)
                Bass.BASS_StreamFree(Handle);

            Debug.PrintToConsole("ok", String.Format("Stream {0} freed.", UniqueID));
        }
    }
}
