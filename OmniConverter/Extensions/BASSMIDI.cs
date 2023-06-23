using CSCore;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagedBass;
using ManagedBass.Midi;
using ManagedBass.Fx;
using System.Threading.Channels;
using MIDIModificationFramework;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;

// Written with help from Arduano

namespace OmniConverter
{
    public class CMIDIEvent
    {
        private uint WholeEvent = 0;

        private byte Status = 0;
        private byte LRS = 0;

        private byte EventType = 0;
        private byte Channel = 0;

        private byte Param1 = 0;
        private byte Param2 = 0;

        private byte Secret = 0;

        public CMIDIEvent(uint DWORD)
        {
            SetNewEvent(DWORD);
        }

        public void SetNewEvent(uint DWORD)
        {
            WholeEvent = DWORD;

            Status = (byte)(WholeEvent & 0xFF);
            if ((Status & 0x80) != 0)
                LRS = Status;
            else
                WholeEvent = WholeEvent << 8 | LRS;

            EventType = (byte)(LRS & 0xF0);
            Channel = (byte)(LRS & 0x0F);

            Param1 = (byte)(WholeEvent >> 8);
            Param2 = (byte)(WholeEvent >> 16);

            Secret = (byte)(WholeEvent >> 24);
        }

        public uint GetWholeEvent() { return WholeEvent; }

        public byte GetStatus() { return Status; }

        public byte GetEventType() { return EventType; }
        public byte GetChannel() { return Channel; }

        public int GetParams() { return Param1 | Param2 << 8; }
        public byte GetFirstParam() { return Param1; }
        public byte GetSecondParam() { return Param2; }

        public byte GetSecret() { return Secret; }
    }

    public class BASSMIDI : ISampleSource
    {
        private readonly object Lock = new object();
        private BassFlags Flags;
        private int Handle;
        private bool _disposed = false;

        public string UniqueID { get; private set; } = IDGenerator.GetID();
        public bool ErroredOut { get; private set; } = false;
        public bool CanSeek { get; private set; } = false;
        public CSCore.WaveFormat WaveFormat { get; private set; }

        // Special RTS mode
        private Random RTSR = new Random();
        private bool RTSMode { get; } = false;
        private uint EventC;
        private MidiEvent[] EventS;

        private int VolHandle;
        private VolumeFxParameters VolParam;

        private double CFMin { get; } = Properties.Settings.Default.CFValue - Properties.Settings.Default.CFFluctuation;
        private double CFMax { get; } = Properties.Settings.Default.CFValue + Properties.Settings.Default.CFFluctuation;

        public BASSMIDI(CSCore.WaveFormat WF)
        {
            lock (Lock)
            {
                WaveFormat = WF;
                InitializeSettings();
                Flags = BassFlags.Decode | BassFlags.Float | BassFlags.MidiDecayEnd;

                Handle = BassMidi.CreateStream(16, Flags, WaveFormat.SampleRate);
                if (!CheckError(Handle, "Unable to create stream."))
                {
                    ErroredOut = true;
                    return;
                }
                if (Handle == 0)
                {
                    Errors ERR = Bass.LastError;

                    Debug.ShowMsgBox(
                        "BASSMIDI error",
                        String.Format("Unable to create stream.\n\nError encountered: {0}", ERR),
                        null, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    ErroredOut = true;
                    return;
                }
                Debug.PrintToConsole("ok", String.Format("{0} - Stream is open.", UniqueID));

                Bass.ChannelSetAttribute(Handle, ChannelAttribute.MidiCPU, 0);

                VolHandle = Bass.ChannelSetFX(Handle, EffectType.Volume, 1);
                if (!CheckError(Handle, "Unable to set volume FX."))
                {
                    ErroredOut = true;
                    return;
                }

                ChangeVolume(Properties.Settings.Default.Volume);

                SetSoundFonts();
            }
        }

        // This is a really unstable mode, touching anything in its code might break it
        public BASSMIDI(CSCore.WaveFormat WF, bool RTS = true)
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
                Flags = BassFlags.Decode | BassFlags.Float;

                // Create stream which will feed the events to
                Handle = BassMidi.CreateStream(16, Flags, WaveFormat.SampleRate);
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
                Error += String.Format("\n\nError encountered: {0}", Bass.LastError);
                Debug.ShowMsgBox("BASSMIDI error", Error, null, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
        }

        private void InitializeSettings()
        {
            Debug.PrintToConsole("ok", String.Format("Stream unique ID: {0}", UniqueID));

            Bass.Configure(Configuration.MidiVoices, Properties.Settings.Default.VoiceLimit);
            Bass.Configure(Configuration.MidiAutoFont, 0);
            Bass.Configure(Configuration.MidiDefaultFont, IntPtr.Zero);
            Debug.PrintToConsole("ok", $"{UniqueID} - VoiceLimit = {Properties.Settings.Default.VoiceLimit}.");

            Flags |= Properties.Settings.Default.SincInter ? BassFlags.SincInterpolation : BassFlags.Default;
            Debug.PrintToConsole("ok", $"{UniqueID} - SincInter = {Properties.Settings.Default.SincInter}.");

            Flags |= Properties.Settings.Default.DisableEffects ? BassFlags.MidiNoFx : BassFlags.Default;
            Debug.PrintToConsole("ok", $"{UniqueID} - DisableEffects = {Properties.Settings.Default.DisableEffects}.");

            Flags |= Properties.Settings.Default.NoteOff1 ? BassFlags.MidiNoteOff1 : BassFlags.Default;
            Debug.PrintToConsole("ok", $"{UniqueID} - NoteOff1 = {Properties.Settings.Default.NoteOff1}.");
        }

        private void SetSoundFonts()
        {
            BassMidi.StreamSetFonts(Handle, Program.SFArray.BMFEArray, Program.SFArray.BMFEArray.Length);
            Debug.PrintToConsole("ok", $"{UniqueID} - Loaded {Program.SFArray.BMFEArray.Length} SoundFonts.");
        }

        private void SetVSTs()
        {
            return;

/*
            foreach (VST iVST in Program.VSTArray)
            {
                //
            }
*/
        }

        public unsafe int Read(float[] buffer, int offset, int count)
        {
            lock (Lock)
            {
                fixed (float* buff = buffer)
                {
                    var obuff = buff + offset;
                    int ret = Bass.ChannelGetData(Handle, (IntPtr)obuff, (count * 4) | 0x40000000);

                    if (ret == 0)
                    {
                        var BE = Bass.LastError;
                        if (BE != Errors.Ended)
                            Debug.PrintToConsole("wrn", $"{UniqueID} - DataParsingError {(count * 4) | 0x40000000}");

                        Debug.PrintToConsole("wrn", BE.ToString());
                    }

                    return ret / 4;
                }
            }
        }

        public void ChangeVolume(float volume)
        {
            if (VolParam == null)
                VolParam = new VolumeFxParameters();

            VolParam.fCurrent = 1.0f;
            VolParam.fTarget = volume;
            VolParam.fTime = 0.0f;
            VolParam.lCurve = 1;
            Bass.FXSetParameters(VolHandle, VolParam);
        }

        public bool SetAttribute(ChannelAttribute attrib, float value)
        {
            return Bass.ChannelSetAttribute(Handle, attrib, value);
        }

        public unsafe bool SendReverbEvent(int channel, int param)
        {
            return BassMidi.StreamEvent(Handle, channel, MidiEventType.Reverb, param);
        }

        public unsafe bool SendChorusEvent(int channel, int param)
        {
            return BassMidi.StreamEvent(Handle, channel, MidiEventType.Chorus, param);
        }

        public int SendEventStruct(MidiEvent data) 
        {
            return BassMidi.StreamEvents(Handle, MidiEventsMode.Struct, new MidiEvent[] { data });
        }

        public int SendEventRaw(byte[] data)
        {
            return BassMidi.StreamEvents(Handle, MidiEventsMode.Raw | MidiEventsMode.NoRunningStatus, data);
        }

        public unsafe int SendEventRaw(uint data)
        {
            IntPtr idata = (IntPtr)(&data);
            return BassMidi.StreamEvents(Handle, MidiEventsMode.Raw | MidiEventsMode.NoRunningStatus, idata, 3);
        }

        public unsafe int SendEndEvent()
        {
            bool a = BassMidi.StreamEvent(Handle, 0, MidiEventType.End, 0);
            bool b = BassMidi.StreamEvent(Handle, 0, MidiEventType.EndTrack, 0);
            return (a && b) ? 0 : -1;
        }

        public long Position
        {
            get { return Bass.ChannelGetPosition(Handle) / 4; }
            set { throw new NotSupportedException("Can't set position."); }
        }

        public long Length
        {
            get { return Bass.ChannelGetLength(Handle) / 4; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                lock (Lock)
                    Bass.StreamFree(Handle);
            }

            UniqueID = string.Empty;
            ErroredOut = false;
            CanSeek = false;
            WaveFormat = null;

            _disposed = true;
        }
    }
}
