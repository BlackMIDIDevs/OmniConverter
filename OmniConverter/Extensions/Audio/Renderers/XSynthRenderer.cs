using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static OmniConverter.XSynth;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            Pitch = 7,
            FineTune = 8,
            CoarseTune = 9
        }

        public enum Interpolation: ushort
        {
            Nearest = 100,
            Linear = 101
        }

        public enum ChannelCount: ushort
        {
            Mono = 1,
            Stereo = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XSynth_Soundfont
        {
            public nint soundfont;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XSynth_ChannelGroup
        {
            public nint group;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StreamParams
        {
            public uint sample_rate;
            public ChannelCount audio_channels;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct GroupOptions
        {
            public StreamParams stream_params;
            public uint channels;
            public uint* drum_channels;
            public uint drum_channels_count;
            [MarshalAs(UnmanagedType.U1)]
            public bool use_threadpool;
            [MarshalAs(UnmanagedType.U1)]
            public bool fade_out_killing;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SoundfontOptions
        {
            public StreamParams stream_params;
            public short bank;
            public short preset;
            [MarshalAs(UnmanagedType.U1)]
            public bool linear_release;
            [MarshalAs(UnmanagedType.U1)]
            public bool use_effects;
            public Interpolation interpolator;
        }

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_StreamParams", CallingConvention = CallingConvention.Cdecl)]
        public static extern StreamParams GenDefault_StreamParams();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_GroupOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern GroupOptions GenDefault_GroupOptions();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_SoundfontOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern SoundfontOptions GenDefault_SoundfontOptions();

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_Create", CallingConvention = CallingConvention.Cdecl)]
        public static extern XSynth_ChannelGroup ChannelGroup_Create(GroupOptions options);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_VoiceCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ChannelGroup_VoiceCount(XSynth_ChannelGroup id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SendEvent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_SendEvent(XSynth_ChannelGroup id, uint channel, ushort evt, ushort param);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_ReadSamples", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void ChannelGroup_ReadSamples(XSynth_ChannelGroup id, nint buffer, ulong length);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_GetStreamParams", CallingConvention = CallingConvention.Cdecl)]
        public static extern StreamParams ChannelGroup_GetStreamParams(XSynth_ChannelGroup id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SetLayerCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_SetLayerCount(XSynth_ChannelGroup id, ulong layers);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SetSoundfonts", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_SetSoundfonts(XSynth_ChannelGroup id, nint sf_ids, ulong count);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_ClearSoundfonts", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_ClearSoundfonts(XSynth_ChannelGroup id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_Drop", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_Drop(XSynth_ChannelGroup id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_Soundfont_LoadNew", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern XSynth_Soundfont Soundfont_LoadNew(string path, SoundfontOptions options);

        [DllImport(XSynthLib, EntryPoint = "XSynth_Soundfont_Remove", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Soundfont_Remove(XSynth_Soundfont id);
    }

    public class XSynthEngine : AudioEngine
    {
        private ulong _sfCount = 0;
        private ulong _layerCount = 0;
        private nint _sfArray = nint.Zero;
        private List<XSynth_Soundfont> _managedSfArray = [];
        private List<XSynth_ChannelGroup> _channelGroups = [];
        private StreamParams _streamParams;
        private GroupOptions _groupOptions;

        public unsafe XSynthEngine(CSCore.WaveFormat waveFormat, Settings settings) : base(waveFormat, settings, false)
        {
            Debug.PrintToConsole(Debug.LogType.Message, $"Preparing XSynth...");

            _streamParams = GenDefault_StreamParams();
            _groupOptions = GenDefault_GroupOptions();

            _streamParams.audio_channels = (ChannelCount)waveFormat.Channels;
            _streamParams.sample_rate = (uint)waveFormat.SampleRate;

            _groupOptions.stream_params = _streamParams;
            _groupOptions.channels = 16;
            _groupOptions.fade_out_killing = !CachedSettings.KilledNoteFading;
            _groupOptions.use_threadpool = !(CachedSettings.MultiThreadedMode && CachedSettings.PerTrackMode);

            var tmp = InitializeSoundFonts();
            if (tmp == null)
            {
                string exp = "Failed to allocate SoundFonts!!!";
                Debug.PrintToConsole(Debug.LogType.Error, exp);
                throw new Exception(exp);
            }

            _sfCount = (ulong)tmp.Length;
            _sfArray = Marshal.AllocHGlobal(tmp.Length * sizeof(XSynth_Soundfont));
            MarshalExt.CopyToManaged(tmp, _sfArray, 0, tmp.Length);

            Initialized = true;

            Debug.PrintToConsole(Debug.LogType.Message, $"XSynth set up for {_streamParams.audio_channels}ch output at {_streamParams.sample_rate}Hz ");
            Debug.PrintToConsole(Debug.LogType.Message, $"ThreadPool = {_groupOptions.use_threadpool}");
            Debug.PrintToConsole(Debug.LogType.Message, $"FadeOutKilling = {_groupOptions.fade_out_killing}");
            Debug.PrintToConsole(Debug.LogType.Message, $"LayerCount = {_layerCount}");

            return;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (Initialized)
            {
                foreach (var group in _channelGroups)
                    ChannelGroup_Drop(group);

                FreeSoundFontsArray();

                Initialized = false;
            }

            _disposed = true;
        }

        private XSynth_Soundfont[]? InitializeSoundFonts()
        {
            foreach (SoundFont sf in CachedSettings.SoundFontsList)
            {
                if (!sf.Enabled)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, "SoundFont is disabled, there's no need to load it.");
                    continue;
                }

                Debug.PrintToConsole(Debug.LogType.Message, $"Preparing ulong for {sf.SoundFontPath}...");
                
                SoundfontOptions _soundfontOptions = GenDefault_SoundfontOptions();
                _soundfontOptions.stream_params = _streamParams;
                _soundfontOptions.bank = sf.SourceBank;
                _soundfontOptions.preset = sf.SourcePreset;
                _soundfontOptions.interpolator = Interpolation.Linear;
                _soundfontOptions.linear_release = false;
                _soundfontOptions.use_effects = !CachedSettings.DisableEffects;

                XSynth_Soundfont handle = Soundfont_LoadNew(sf.SoundFontPath, _soundfontOptions);

                Debug.PrintToConsole(Debug.LogType.Message, $"SoundFont handle initialized. Handle = {handle:X8}");
                _managedSfArray.Add(handle);
            }

            if (_managedSfArray.Count > 0) 
                return _managedSfArray.ToArray();
            
            return null;
        }

        private void FreeSoundFontsArray()
        {
            if (_sfArray != nint.Zero)
            {
                foreach (var sf in _managedSfArray)
                    Soundfont_Remove(sf);

                Marshal.FreeHGlobal(_sfArray);
            }

            _sfArray = nint.Zero;
        }

        public void AddChannel(XSynth_ChannelGroup channel) => _channelGroups.Add(channel);
        public void RemoveChannel(XSynth_ChannelGroup channel) => _channelGroups.Remove(channel);

        public nint GetSoundFontsArray(out ulong count)
        {
            count = _sfCount;
            return _sfArray;
        }

        public GroupOptions GetGroupOptions() => _groupOptions;
    }

    public class XSynthRenderer : MIDIRenderer
    {
        private readonly object Lock = new object();

        public XSynth_ChannelGroup? handle { get; private set; } = null;
        private long length = 0;
        private ulong sfCount = 0;
        private nint sfArray = nint.Zero;
        private XSynthEngine reference;
        private GroupOptions groupOptions;

        private const double maxDb = 1.1220185;
        private double dbVolume = maxDb;
        private double volume = 1.0f;

        public XSynthRenderer(XSynthEngine xsynth) : base(xsynth.WaveFormat, false)
        {
            reference = xsynth;

            if (UniqueID == string.Empty)
                return;

            if (xsynth == null)
                return;

            Debug.PrintToConsole(Debug.LogType.Message, $"Stream unique ID: {UniqueID}");

            groupOptions = reference.GetGroupOptions();
            sfArray = reference.GetSoundFontsArray(out sfCount);

            if (sfArray != nint.Zero)
            {
                handle = ChannelGroup_Create(groupOptions);

                reference.AddChannel((XSynth_ChannelGroup)handle);
                ChannelGroup_SetLayerCount((XSynth_ChannelGroup)handle, reference.CachedSettings.MaxLayers);    
                ChannelGroup_SetSoundfonts((XSynth_ChannelGroup)handle, sfArray, sfCount);

                var tmp = ChannelGroup_GetStreamParams((XSynth_ChannelGroup)handle);

                if (tmp.sample_rate != groupOptions.stream_params.sample_rate)
                    throw new AccessViolationException();

                if (tmp.audio_channels != groupOptions.stream_params.audio_channels)
                    throw new AccessViolationException();

                Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - Stream is open.");

                Initialized = true;
            }
        }

        private bool IsError(string Error)
        {
            return false;
        }

        public override unsafe int Read(float[] buffer, int offset, long delta, int count)
        {
            if (handle == null)
                return 0;

            lock (Lock)
            {
                fixed (float* buff = buffer)
                {
                    var offsetBuff = buff + offset;
                    ChannelGroup_ReadSamples((XSynth_ChannelGroup)handle, (nint)offsetBuff, (ulong)count);

                    if (volume < 1.0f)
                    {
                        for (int i = 0; i < count; i++)
                            offsetBuff[i] = offsetBuff[i] * (float)dbVolume;
                    }
                }
            }

            length += count;
            return count;
        }

        public override void SystemReset()
        {
            if (handle == null)
                return;

            for (uint i = 0; i < 16; i++)
            {
                ChannelGroup_SendEvent((XSynth_ChannelGroup)handle, i, (ushort)EventType.AllNotesKilled, 0);
                ChannelGroup_SendEvent((XSynth_ChannelGroup)handle, i, (ushort)EventType.ResetControl, 0);
                ChannelGroup_SendEvent((XSynth_ChannelGroup)handle, i, (ushort)EventType.Control, 0);
                ChannelGroup_SendEvent((XSynth_ChannelGroup)handle, i, (ushort)EventType.ProgramChange, 0);
            }
        }

        public override void ChangeVolume(float volume)
        {
            this.volume = volume / 100.0f;
            dbVolume = Math.Pow(10.0f, this.volume * 0.05f);
        }

        public override bool SendCustomFXEvents(int channel, short reverb, short chorus)
        {
            return true;
        }

        public override void SendEvent(byte[] data)
        {
            if (handle == null)
                return;

            var status = data[0];
            var param1 = data[1];
            var param2 = data.Length >= 3 ? data[2] : (byte)0;

            var eventType = EventType.NoteOn;
            int eventParams;

            switch ((MIDIEventType)(status & 0xF0))
            {
                case MIDIEventType.NoteOn:
                    if (reference.CachedSettings.FilterVelocity && param2 >= reference.CachedSettings.VelocityLow && param2 <= reference.CachedSettings.VelocityHigh)
                        return;
                    if (reference.CachedSettings.FilterKey && (param1 < reference.CachedSettings.KeyLow || param1 > reference.CachedSettings.KeyHigh))
                        return;

                    if (param1 == 0)
                    {
                        eventType = EventType.NoteOff;
                        eventParams = param1;
                    }
                    else eventParams = (param2 << 8) | param1;
                    break;

                case MIDIEventType.NoteOff:
                    if (reference.CachedSettings.FilterKey && (param1 < reference.CachedSettings.KeyLow || param1 > reference.CachedSettings.KeyHigh))
                        return;

                    eventType = EventType.NoteOff;
                    eventParams = param1;
                    break;

                case MIDIEventType.PatchChange:
                    eventType = EventType.ProgramChange;
                    eventParams = param1;
                    break;

                case MIDIEventType.CC:
                    eventType = EventType.Control;
                    eventParams = (param2 << 8) | param1;
                    break;

                case MIDIEventType.PitchBend:
                    eventType = EventType.Pitch;
                    eventParams = (param2 << 7) | param1;
                    break;

                default:
                    return;
            }

            ChannelGroup_SendEvent((XSynth_ChannelGroup)handle, (uint)(status & 0xF), (ushort)eventType, (ushort)eventParams);
        }

        public override void RefreshInfo()
        {
            if (handle == null)
                return;

            ActiveVoices = ChannelGroup_VoiceCount((XSynth_ChannelGroup)handle);
        }

        public override void SendEndEvent()
        {
            if (handle == null)
                return;

            for (uint i = 0; i < 16; i++)
            {
                ChannelGroup_SendEvent((XSynth_ChannelGroup)handle, i, (ushort)EventType.AllNotesOff, 0);
                ChannelGroup_SendEvent((XSynth_ChannelGroup)handle, i, (ushort)EventType.ResetControl, 0);
            }
        }

        public override long Position
        {
            get { return length; }
            set { throw new NotSupportedException("Can't set position."); }
        }

        public override long Length
        {
            get { return length; }
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            UniqueID = string.Empty;
            CanSeek = false;

            Initialized = false;
            Disposed = true;
        }
    }
}
