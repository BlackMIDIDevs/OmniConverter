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
using System.Threading;
using System.Reflection.Metadata;

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

        public BASSMIDI(CSCore.WaveFormat WF, CancellationToken CTS)
        {
            if (UniqueID == string.Empty)
                return;

            WaveFormat = WF;
            Flags = BassFlags.Decode | BassFlags.Float | BassFlags.MidiDecayEnd;

            Debug.PrintToConsole("ok", String.Format("Stream unique ID: {0}", UniqueID));
            Debug.PrintToConsole("ok", $"{UniqueID} - VoiceLimit = {Properties.Settings.Default.VoiceLimit}.");

            Flags |= Properties.Settings.Default.SincInter ? BassFlags.SincInterpolation : BassFlags.Default;
            Debug.PrintToConsole("ok", $"{UniqueID} - SincInter = {Properties.Settings.Default.SincInter}.");

            Flags |= Properties.Settings.Default.DisableEffects ? BassFlags.MidiNoFx : BassFlags.Default;
            Debug.PrintToConsole("ok", $"{UniqueID} - DisableEffects = {Properties.Settings.Default.DisableEffects}.");

            Flags |= Properties.Settings.Default.NoteOff1 ? BassFlags.MidiNoteOff1 : BassFlags.Default;
            Debug.PrintToConsole("ok", $"{UniqueID} - NoteOff1 = {Properties.Settings.Default.NoteOff1}.");

            Handle = BassMidi.CreateStream(16, Flags, WaveFormat.SampleRate);
            if (!CheckError("Unable to open MIDI stream."))
            {
                ErroredOut = true;
                return;
            }
            else Debug.PrintToConsole("ok", String.Format("{0} - Stream is open.", UniqueID));

            VolHandle = Bass.ChannelSetFX(Handle, (EffectType)9, 1);
            if (!CheckError("Unable to set volume FX."))
            {
                ErroredOut = true;
                return;
            }

            ChangeVolume(Properties.Settings.Default.Volume);

            SetSoundFonts();
        }

        private bool CheckError(string Error)
        {
            if (Bass.LastError != 0)
            {
#if DEBUG
                Error += String.Format("\n\nError encountered: {0}", Bass.LastError);
                Debug.ShowMsgBox("BASSMIDI error", Error, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                Debug.PrintToConsole("err", $"{UniqueID} - {Error}.");   
#endif

                return false;
            }

            return true;
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

        public int SendEventStruct(MidiEvent[] data) 
        {
            return BassMidi.StreamEvents(Handle, MidiEventsMode.Struct, data);
        }

        public int SendEventRaw(byte[] data)
        {
            return BassMidi.StreamEvents(Handle, MidiEventsMode.Raw | MidiEventsMode.NoRunningStatus, data);
        }

        public int SendEndEvent()
        {
            var ev = new[]
            {
                new MidiEvent() {EventType = MidiEventType.EndTrack, Channel = 0, Parameter = 0, Position = 0, Ticks = 0 },
                new MidiEvent() {EventType = MidiEventType.End, Channel = 0, Parameter = 0, Position = 0, Ticks = 0 },
            };

            return BassMidi.StreamEvents(Handle, MidiEventsMode.Raw | MidiEventsMode.Struct, ev);
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
