using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static OmniConverter.XSynth;

// Written with help from Arduano

namespace OmniConverter
{
    public class XSynth
    {
        private const string XSynthLib = "xsynth";

        public const uint APIVersion = 0x300;

        public enum AudioEvent : ushort
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

        public enum ConfigEvent : ushort
        {
            SetLayers = 0,
            SetPercussionMode = 1,
        }

        public enum Interpolation : ushort
        {
            Nearest = 0,
            Linear = 1
        }

        public enum ChannelCount : ushort
        {
            Mono = 1,
            Stereo = 2
        }

        public enum EnvelopeCurve : byte
        {
            Linear = 0,
            Exponential = 1,
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
        public struct ParallelismOptions
        {
            public int channel;
            public int key;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GroupOptions
        {
            public StreamParams stream_params;
            public uint channels;
            [MarshalAs(UnmanagedType.U1)]
            public bool fade_out_killing;
            public ParallelismOptions parallelism;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EnvelopeOptions
        {
            public EnvelopeCurve attack_curve;
            public EnvelopeCurve decay_curve;
            public EnvelopeCurve release_curve;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SoundfontOptions
        {
            public StreamParams stream_params;
            public short bank;
            public short preset;
            public EnvelopeOptions vol_envelope_options;
            [MarshalAs(UnmanagedType.U1)]
            public bool use_effects;
            public Interpolation interpolator;
        }

        [DllImport(XSynthLib, EntryPoint = "XSynth_GetVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetVersionInt();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_StreamParams", CallingConvention = CallingConvention.Cdecl)]
        public static extern StreamParams GenDefault_StreamParams();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_ParallelismOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern ParallelismOptions GenDefault_ParallelismOptions();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_GroupOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern GroupOptions GenDefault_GroupOptions();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_EnvelopeOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern EnvelopeOptions GenDefault_EnvelopeOptions();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_SoundfontOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern SoundfontOptions GenDefault_SoundfontOptions();

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_Create", CallingConvention = CallingConvention.Cdecl)]
        public static extern XSynth_ChannelGroup ChannelGroup_Create(GroupOptions options);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_VoiceCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ChannelGroup_VoiceCount(XSynth_ChannelGroup id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SendAudioEvent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_SendAudioEvent(XSynth_ChannelGroup id, uint channel, AudioEvent evt, ushort param);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SendAudioEventAll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_SendAudioEventAll(XSynth_ChannelGroup id, AudioEvent evt, ushort param);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SendConfigEvent", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_SendConfigEvent(XSynth_ChannelGroup id, uint channel, ConfigEvent evt, uint param);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_SendAudioEventAll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_SendConfigEventAll(XSynth_ChannelGroup id, ConfigEvent evt, uint param);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_ReadSamples", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void ChannelGroup_ReadSamples(XSynth_ChannelGroup id, nint buffer, ulong length);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_GetStreamParams", CallingConvention = CallingConvention.Cdecl)]
        public static extern StreamParams ChannelGroup_GetStreamParams(XSynth_ChannelGroup id);

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

        public static Version Version => MiscFunctions.ConvertIntToVersion((int)GetVersionInt());
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
            var libraryVersion = GetVersionInt();
            if (libraryVersion >> 8 != APIVersion >> 8)
            {
                var neededVer = MiscFunctions.ConvertIntToVersion((int)APIVersion);
                throw new Exception($"Unsupported version of XSynth loaded. Please use version {neededVer.Major}.{neededVer.Minor}.x");
            }

            Debug.PrintToConsole(Debug.LogType.Message, $"Preparing XSynth...");

            _streamParams = GenDefault_StreamParams();
            _groupOptions = GenDefault_GroupOptions();

            _streamParams.audio_channels = (ChannelCount)waveFormat.Channels;
            _streamParams.sample_rate = (uint)waveFormat.SampleRate;

            _groupOptions.stream_params = _streamParams;
            _groupOptions.channels = 16;
            _groupOptions.fade_out_killing = !CachedSettings.Synth.KilledNoteFading;

            _groupOptions.parallelism = GenDefault_ParallelismOptions();
            switch (CachedSettings.XSynth.Threading)
            {
                case XSynthSettings.ThreadingType.None:
                    _groupOptions.parallelism.channel = -1;
                    _groupOptions.parallelism.key = -1;
                    break;
                case XSynthSettings.ThreadingType.PerChannel:
                    _groupOptions.parallelism.channel = CachedSettings.Render.ThreadsCount;
                    _groupOptions.parallelism.key = -1;
                    break;
                case XSynthSettings.ThreadingType.PerKey:
                    _groupOptions.parallelism.channel = CachedSettings.Render.ThreadsCount;
                    _groupOptions.parallelism.key = CachedSettings.Render.ThreadsCount;
                    break;
                default:
                    break;
            }

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
                if (CachedSettings.XSynth.LinearEnvelope) {
                    // Linear in amplitude -> Exponential in dB (XSynth uses dB units)
                    _soundfontOptions.vol_envelope_options.decay_curve = EnvelopeCurve.Exponential;
                    _soundfontOptions.vol_envelope_options.release_curve = EnvelopeCurve.Exponential;
                }
                _soundfontOptions.use_effects = CachedSettings.XSynth.UseEffects;
                _soundfontOptions.interpolator = CachedSettings.Synth.Interpolation switch {
                    GlobalSynthSettings.InterpolationType.None => Interpolation.Nearest,
                    _ => Interpolation.Linear,
                };

                XSynth_Soundfont handle = Soundfont_LoadNew(sf.SoundFontPath, _soundfontOptions);

                if (handle.soundfont != IntPtr.Zero)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, $"SoundFont handle initialized. Handle = {handle:X8}");
                    _managedSfArray.Add(handle);
                }
                else
                {
                    Debug.PrintToConsole(Debug.LogType.Message, $"Error loading SoundFont in XSynth: {sf.SoundFontPath}");
                }
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
        public XSynth_ChannelGroup? handle { get; private set; } = null;
        private long length = 0;
        private ulong sfCount = 0;
        private nint sfArray = nint.Zero;
        private XSynthEngine reference;
        private GroupOptions groupOptions;
        private double dbVolume = 1.0;

        public XSynthRenderer(XSynthEngine xsynth) : base(xsynth.WaveFormat, xsynth.CachedSettings.Synth.Volume, false)
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
                ChannelGroup_SendConfigEventAll((XSynth_ChannelGroup)handle, ConfigEvent.SetLayers, (uint)reference.CachedSettings.XSynth.MaxLayers);
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

                    if (Volume < 1.0)
                    {
                        for (int i = 0; i < count; i++)
                            offsetBuff[i] = offsetBuff[i] * (float)Volume;
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

            ChannelGroup_SendAudioEventAll((XSynth_ChannelGroup)handle, AudioEvent.AllNotesKilled, 0);
            ChannelGroup_SendAudioEventAll((XSynth_ChannelGroup)handle, AudioEvent.ResetControl, 0);
            ChannelGroup_SendAudioEventAll((XSynth_ChannelGroup)handle, AudioEvent.Control, 0);
            ChannelGroup_SendAudioEventAll((XSynth_ChannelGroup)handle, AudioEvent.ProgramChange, 0);
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

            var eventType = AudioEvent.NoteOn;
            int eventParams;

            switch ((MIDIEventType)(status & 0xF0))
            {
                case MIDIEventType.NoteOn:
                    if (reference.CachedSettings.Event.FilterVelocity && param2 >= reference.CachedSettings.Event.VelocityLow && param2 <= reference.CachedSettings.Event.VelocityHigh)
                        return;
                    if (reference.CachedSettings.Event.FilterKey && (param1 < reference.CachedSettings.Event.KeyLow || param1 > reference.CachedSettings.Event.KeyHigh))
                        return;

                    if (param1 == 0)
                    {
                        eventType = AudioEvent.NoteOff;
                        eventParams = param1;
                    }
                    else eventParams = (param2 << 8) | param1;
                    break;

                case MIDIEventType.NoteOff:
                    if (reference.CachedSettings.Event.FilterKey && (param1 < reference.CachedSettings.Event.KeyLow || param1 > reference.CachedSettings.Event.KeyHigh))
                        return;

                    eventType = AudioEvent.NoteOff;
                    eventParams = param1;
                    break;

                case MIDIEventType.PatchChange:
                    eventType = AudioEvent.ProgramChange;
                    eventParams = param1;
                    break;

                case MIDIEventType.CC:
                    eventType = AudioEvent.Control;
                    eventParams = (param2 << 8) | param1;
                    break;

                case MIDIEventType.PitchBend:
                    eventType = AudioEvent.Pitch;
                    eventParams = (param2 << 7) | param1;
                    break;

                default:
                    return;
            }

            ChannelGroup_SendAudioEvent((XSynth_ChannelGroup)handle, (uint)(status & 0xF), eventType, (ushort)eventParams);
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

            ChannelGroup_SendAudioEventAll((XSynth_ChannelGroup)handle, AudioEvent.AllNotesOff, 0);
            ChannelGroup_SendAudioEventAll((XSynth_ChannelGroup)handle, AudioEvent.ResetControl, 0);
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
