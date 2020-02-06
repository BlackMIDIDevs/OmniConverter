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

        public BASSMIDI(WaveFormat WF)
        {
            lock (Lock)
            {
                WaveFormat = WF;
                InitializeSettings();

                Handle = BassMidi.BASS_MIDI_StreamCreate(16, Flags, WaveFormat.SampleRate);
                if (!CheckError(Handle, "Unable to create stream."))
                {
                    ErroredOut = true;
                    return;
                }
                if (Handle == 0)
                {
                    BASSError ERR = Bass.BASS_ErrorGetCode();

                    Debug.ShowMsgBox(
                        "BASSMIDI error",
                        String.Format("Unable to create stream.\n\nError encountered: {0}", ERR),
                        null, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    ErroredOut = true;
                    return;
                }
                Debug.PrintToConsole("ok", String.Format("{0} - Stream is open.", UniqueID));

                Int32 Tracks = BassMidi.BASS_MIDI_StreamGetTrackCount(Handle);
                Debug.PrintToConsole("ok", String.Format("{0} - Total tracks = {1}", UniqueID, Tracks));

                SetSoundFonts();
            }
        }

        // This is a really unstable mode, touching anything in its code might break it
        public BASSMIDI(Boolean RTS, WaveFormat WF)
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
                InitializeSettings();

                // Remove DECAYEND since it's not applicable to this type of stream
                Flags &= ~BASSFlag.BASS_MIDI_DECAYEND;

                // Create stream which will feed the events to
                Handle = BassMidi.BASS_MIDI_StreamCreate(16, Flags, WaveFormat.SampleRate);
                if (!CheckError(Handle, String.Format("Unable to create RTS stream ({0}).", UniqueID)))
                {
                    ErroredOut = true;
                    return;
                }
                Debug.PrintToConsole("ok", String.Format("{0} - RTS stream.", UniqueID));

                SetSoundFonts();
            }
        }

        private bool CheckError(Int32 H, String Error)
        {
            if (H == 0)
            {
                Error += String.Format("\n\nError encountered: {0}", Bass.BASS_ErrorGetCode());
                Debug.ShowMsgBox("BASSMIDI error", Error, null, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
        }

        private void InitializeSettings()
        {
            Debug.PrintToConsole("ok", String.Format("Stream unique ID: {0}", UniqueID));

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

        public unsafe int Read(float[] buffer, int offset, int count)
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

        public unsafe int SendEventRaw(uint data, int channel)
        {
            var mode = BASSMIDIEventMode.BASS_MIDI_EVENTS_RAW | BASSMIDIEventMode.BASS_MIDI_EVENTS_NORSTATUS;
            return BassMidi.BASS_MIDI_StreamEvents(Handle, mode, channel, (IntPtr)(&data), 3);
        }

        public unsafe int SendEndEvent()
        {
            var ev = new[] { 
                new BASS_MIDI_EVENT(BASSMIDIEvent.MIDI_EVENT_END_TRACK, 0, 0, 0, 0),
                new BASS_MIDI_EVENT(BASSMIDIEvent.MIDI_EVENT_END, 0, 0, 0, 0),
            };
            var mode = BASSMIDIEventMode.BASS_MIDI_EVENTS_TIME | BASSMIDIEventMode.BASS_MIDI_EVENTS_STRUCT;
            return BassMidi.BASS_MIDI_StreamEvents(Handle, mode, ev);
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
