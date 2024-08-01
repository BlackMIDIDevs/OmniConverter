using CSCore;
using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Midi;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

// Written with help from Arduano

namespace OmniConverter
{
    public class XSynth
    {
        private const string XSynthLib = "xsynth";

        public enum EventType
        {
            NoteOn = 0,
            NoteOff = 1,
            AllNotesOff = 2,
            AllNotesKilled = 3,
            ResetControl = 4,
            Control = 5,
            ProgramChange = 6,
            Pitch = 7
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StreamParams
        {
            public uint sample_rate;
            public ushort audio_channels;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GroupOptions
        {
            public StreamParams stream_params;
            public uint channels;
            public IntPtr drum_channels;
            public uint drum_channels_count;
            public bool use_threadpool;
            public bool fade_out_killing;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SoundfontOptions
        {
            public StreamParams stream_params;
            public short bank;
            public short preset;
            public bool linear_release;
            public bool use_effects;
            public byte interpolator;
        }

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_StreamParams", CallingConvention = CallingConvention.Cdecl)]
        public static extern StreamParams GenDefault_StreamParams();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_GroupOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern GroupOptions GenDefault_GroupOptions();

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_Create", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ChannelGroup_Create(GroupOptions options);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_VoiceCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ChannelGroup_VoiceCount(ulong id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SendEvent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_SendEvent(ulong id, uint channel, ushort evt, ushort param);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_ReadSamples", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void ChannelGroup_ReadSamples(ulong id, nint buffer, ulong length);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_GetStreamParams", CallingConvention = CallingConvention.Cdecl)]
        public static extern StreamParams ChannelGroup_GetStreamParams(ulong id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SetLayerCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_SetLayerCount(ulong id, ulong layers);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SetSoundfonts", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ChannelGroup_SetSoundfonts(ulong id, nint sf_ids, ulong count);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_ClearSoundfonts", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_ClearSoundfonts(ulong id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_Remove", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_Remove(ulong id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_RemoveAll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_RemoveAll();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_SoundfontOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern SoundfontOptions GenDefault_SoundfontOptions();

        [DllImport(XSynthLib, EntryPoint = "XSynth_Soundfont_LoadNew", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Soundfont_LoadNew(string path, SoundfontOptions options);

        [DllImport(XSynthLib, EntryPoint = "XSynth_Soundfont_Remove", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Soundfont_Remove(ulong id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_Soundfont_RemoveAll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Soundfont_RemoveAll();
    }

    public class XSynthEngine : AudioEngine
    {
        private ulong[]? _sfArray;
        private List<ulong> channelGroups = new();
        private XSynth.StreamParams _streamParams;
        private XSynth.GroupOptions _groupOptions;
        private XSynth.SoundfontOptions _soundfontOptions;

        public XSynthEngine(CSCore.WaveFormat waveFormat, ObservableCollection<SoundFont>? initSFs = null) : base(waveFormat, false)
        {
            _streamParams = XSynth.GenDefault_StreamParams();
            _groupOptions = XSynth.GenDefault_GroupOptions();
            _soundfontOptions = XSynth.GenDefault_SoundfontOptions();

            _streamParams.audio_channels = (ushort)waveFormat.Channels;
            _streamParams.sample_rate = (uint)waveFormat.SampleRate;

            _groupOptions.stream_params = _streamParams;
            _groupOptions.channels = 16;
            _groupOptions.fade_out_killing = Program.Settings.KilledNoteFading;
            _groupOptions.use_threadpool = Program.Settings.MultiThreadedMode;

            _soundfontOptions.stream_params = _groupOptions.stream_params;

            _sfArray = InitializeSoundFonts(initSFs);

            Debug.PrintToConsole(Debug.LogType.Message, $"XSynth >> AC: {_groupOptions.stream_params.audio_channels} | SR: {_groupOptions.stream_params.sample_rate}");
            Initialized = true;

            return;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (_sfArray != null)
                FreeSoundFontsArray();

            if (Initialized)
            {
                for (int i = 0; i < channelGroups.Count(); i++)
                    XSynth.ChannelGroup_Remove(channelGroups[i]);

                XSynth.Soundfont_RemoveAll();
            }
                
            Initialized = false;
            _disposed = true;
        }

        private ulong[]? InitializeSoundFonts(ObservableCollection<SoundFont> sfList)
        {
            var _tmpSfArray = new List<ulong>();

            foreach (SoundFont sf in sfList)
            {
                if (!sf.Enabled)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, "SoundFont is disabled, there's no need to load it.");
                    continue;
                }

                Debug.PrintToConsole(Debug.LogType.Message, $"Preparing ulong for {sf.SoundFontPath}...");

                _soundfontOptions.bank = sf.SourceBank;
                _soundfontOptions.preset = sf.SourcePreset;

                ulong handle = XSynth.Soundfont_LoadNew(sf.SoundFontPath, _soundfontOptions);

                Debug.PrintToConsole(Debug.LogType.Message, $"SoundFont handle initialized. Handle = {handle:X8}");
                _tmpSfArray.Add(handle);
            }

            if (_tmpSfArray.Count > 0) 
                return _tmpSfArray.ToArray();
            
            return null;
        }

        private void FreeSoundFontsArray()
        {
            if (_sfArray != null)
            {
                XSynth.Soundfont_RemoveAll();
            }

            _sfArray = null;
        }

        public void AddChannel(ulong channel) => channelGroups.Add(channel);
        public void RemoveChannel(ulong channel) => channelGroups.Remove(channel);

        public ulong[]? GetSoundFontsArray() => _sfArray;
        public XSynth.GroupOptions GetGroupOptions() => _groupOptions;
    }

    public class XSynthRenderer : MIDIRenderer
    {
        private readonly object Lock = new object();
        private readonly BassFlags Flags;

        // Special RTS mode
        private Random RTSR = new Random();
        private bool RTSMode { get; } = false;

        public ulong handle { get; private set; } = 0;
        private ulong[]? sfArray = [];
        private XSynthEngine reference;
        private XSynth.GroupOptions groupOptions;

        public XSynthRenderer(XSynthEngine xsynth) : base(xsynth.WaveFormat, false)
        {
            if (UniqueID == string.Empty)
                return;

            Debug.PrintToConsole(Debug.LogType.Message, $"Stream unique ID: {UniqueID}");

            reference = xsynth;
            groupOptions = reference.GetGroupOptions();
            sfArray = reference.GetSoundFontsArray();

            if (sfArray != null)
            {
                handle = XSynth.ChannelGroup_Create(groupOptions);

                reference.AddChannel(handle);
                XSynth.ChannelGroup_SetLayerCount(handle, 2);

                nint sfArrPtr = Marshal.AllocHGlobal(sfArray.Length * sizeof(ulong));
                MarshalExt.CopyToManaged(sfArray, sfArrPtr, 0, sfArray.Length);

                ulong count = XSynth.ChannelGroup_SetSoundfonts(handle, sfArrPtr, (ulong)sfArray.Count());

                if ((int)count != sfArray.Count())
                {
                    Debug.PrintToConsole(Debug.LogType.Message, "WHAT!");
                }

                Marshal.FreeHGlobal(sfArrPtr);

                Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - Stream is open.");

                Initialized = true;
            }
        }

        private bool IsError(string Error)
        {
            if (Bass.LastError != 0)
            {
                Debug.PrintToConsole(Debug.LogType.Error, $"{UniqueID} - {Error}.");
                return true;
            }

            return false;
        }

        public override unsafe int Read(float[] buffer, int offset, int count)
        {
            return NotSupportedVal;

            lock (Lock)
            {
                fixed (float* buff = buffer)
                {
                    var offsetBuff = buff + offset;

                    XSynth.ChannelGroup_ReadSamples(handle, (nint)offsetBuff, (ulong)count);

                    return count;
                }
            }
        }

        public override unsafe int ReadSamples(float[] buffer, int offset, long delta, int count)
        {
            lock (Lock)
            {
                fixed (float* buff = buffer)
                {
                    var offsetBuff = buff + offset;
                    var len = count + offset;

                    XSynth.ChannelGroup_ReadSamples(handle, (nint)offsetBuff, (ulong)len);

                    return count;
                }
            }
        }

        public override void ChangeVolume(float volume)
        {
            return;
        }

        public override bool SendCustomFXEvents(int channel, short reverb, short chorus)
        {
            return true;
        }

        public override void SendEvent(byte[] data)
        {
            var status = data[0];
            var param1 = data[1];
            var param2 = data.Length >= 3 ? data[2] : (byte)0;

            var eventType = XSynth.EventType.NoteOn;
            int eventParams;

            switch ((MIDIEventType)(status & 0xF0))
            {
                case MIDIEventType.NoteOn:
                    if (Program.Settings.FilterVelocity && param2 >= Program.Settings.VelocityLow && param2 <= Program.Settings.VelocityHigh)
                        return;
                    if (Program.Settings.FilterKey && (param1 < Program.Settings.KeyLow || param1 > Program.Settings.KeyHigh))
                        return;

                    eventParams = (param2 << 8) | param1;
                    break;

                case MIDIEventType.NoteOff:
                    if (Program.Settings.FilterKey && (param1 < Program.Settings.KeyLow || param1 > Program.Settings.KeyHigh))
                        return;

                    eventType = XSynth.EventType.NoteOff;
                    eventParams = param1;
                    break;

                case MIDIEventType.PatchChange:
                    eventType = XSynth.EventType.ProgramChange;
                    eventParams = param1;
                    break;

                case MIDIEventType.CC:
                    eventType = XSynth.EventType.Control;
                    eventParams = (param2 << 8) | param1;
                    break;

                case MIDIEventType.PitchBend:
                    eventType = XSynth.EventType.Pitch;
                    eventParams = (param2 << 7) | param1;
                    break;

                default:
                    return;
            }

            XSynth.ChannelGroup_SendEvent(handle, (uint)(status & 0xF), (ushort)eventType, (ushort)eventParams);
        }

        public override bool SendEndEvent()
        {
            return false;
        }

        public override void RefreshInfo()
        {
            ActiveVoices = XSynth.ChannelGroup_VoiceCount(handle);
            RenderingTime = 0.0f;
        }

        public override long Position
        {
            get { return 0; }
            set { throw new NotSupportedException("Can't set position."); }
        }

        public override long Length
        {
            get { return 0; }
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                lock (Lock)
                {
                    reference.RemoveChannel(handle);
                    XSynth.ChannelGroup_Remove(handle);
                }
            }

            UniqueID = string.Empty;
            CanSeek = false;

            Initialized = false;
            Disposed = true;
        }
    }
}
