using CSCore;
using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Midi;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
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
            public nint drum_channels;
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
            public byte interpolator;
        }

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_StreamParams", CallingConvention = CallingConvention.Cdecl)]
        public static extern StreamParams GenDefault_StreamParams();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_GroupOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern GroupOptions GenDefault_GroupOptions();

        [DllImport(XSynthLib, EntryPoint = "XSynth_GenDefault_SoundfontOptions", CallingConvention = CallingConvention.Cdecl)]
        public static extern SoundfontOptions GenDefault_SoundfontOptions();

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
        public static extern void ChannelGroup_SetSoundfonts(ulong id, nint sf_ids, ulong count);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_ClearSoundfonts", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_ClearSoundfonts(ulong id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_Remove", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_Remove(ulong id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_ChannelGroup_RemoveAll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ChannelGroup_RemoveAll();

        [DllImport(XSynthLib, EntryPoint = "XSynth_Soundfont_LoadNew", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Soundfont_LoadNew(string path, SoundfontOptions options);

        [DllImport(XSynthLib, EntryPoint = "XSynth_Soundfont_Remove", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Soundfont_Remove(ulong id);

        [DllImport(XSynthLib, EntryPoint = "XSynth_Soundfont_RemoveAll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Soundfont_RemoveAll();
    }

    public class XSynthEngine : AudioEngine
    {
        private ulong _sfCount = 0;
        private nint _sfArray = nint.Zero;
        private List<ulong> channelGroups = new();
        private XSynth.StreamParams _streamParams;
        private XSynth.GroupOptions _groupOptions;

        public XSynthEngine(CSCore.WaveFormat waveFormat, ObservableCollection<SoundFont>? initSFs = null) : base(waveFormat, false)
        {
            _streamParams = XSynth.GenDefault_StreamParams();
            _groupOptions = XSynth.GenDefault_GroupOptions();

            _streamParams.audio_channels = (ushort)waveFormat.Channels;
            _streamParams.sample_rate = (uint)waveFormat.SampleRate;

            _groupOptions.stream_params = _streamParams;
            _groupOptions.channels = 16;
            _groupOptions.fade_out_killing = Program.Settings.KilledNoteFading;
            _groupOptions.use_threadpool = Program.Settings.MultiThreadedMode;

            var tmp = InitializeSoundFonts(initSFs);

            if (tmp != null)
            {
                _sfCount = (ulong)tmp.Length;
                _sfArray = Marshal.AllocHGlobal(tmp.Length * sizeof(ulong));
                MarshalExt.CopyToManaged(tmp, _sfArray, 0, tmp.Length);
            }
            
            Debug.PrintToConsole(Debug.LogType.Message, $"XSynth >> AC: {_groupOptions.stream_params.audio_channels} | SR: {_groupOptions.stream_params.sample_rate}");
            Initialized = true;

            return;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

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

        private ulong[]? InitializeSoundFonts(ObservableCollection<SoundFont>? sfList)
        {
            if (sfList == null)
                return null;

            var _tmpSfArray = new List<ulong>();

            foreach (SoundFont sf in sfList)
            {
                if (!sf.Enabled)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, "SoundFont is disabled, there's no need to load it.");
                    continue;
                }

                Debug.PrintToConsole(Debug.LogType.Message, $"Preparing ulong for {sf.SoundFontPath}...");
                
                XSynth.SoundfontOptions _soundfontOptions = XSynth.GenDefault_SoundfontOptions();
                _soundfontOptions.stream_params.sample_rate = _streamParams.sample_rate;
                _soundfontOptions.stream_params.audio_channels = _streamParams.audio_channels;
                _soundfontOptions.bank = sf.SourceBank;
                _soundfontOptions.preset = sf.SourcePreset;
                _soundfontOptions.interpolator = 1;
                _soundfontOptions.linear_release = false;
                _soundfontOptions.use_effects = !Program.Settings.DisableEffects;

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
            if (_sfArray != nint.Zero)
            {
                XSynth.Soundfont_RemoveAll();
                Marshal.FreeHGlobal(_sfArray);
            }

            _sfArray = nint.Zero;
        }

        public void AddChannel(ulong channel) => channelGroups.Add(channel);
        public void RemoveChannel(ulong channel) => channelGroups.Remove(channel);

        public nint GetSoundFontsArray(out ulong count)
        {
            count = _sfCount;
            return _sfArray;
        }

        public XSynth.GroupOptions GetGroupOptions() => _groupOptions;
    }

    public class XSynthRenderer : MIDIRenderer
    {
        private readonly object Lock = new object();

        public ulong handle { get; private set; } = 0;
        private ulong sfCount = 0;
        private nint sfArray = nint.Zero;
        private XSynthEngine reference;
        private XSynth.GroupOptions groupOptions;

        private const double maxDb = 1.1220185;
        private double dbVolume = maxDb;
        private float volume = 1.0f;

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
                handle = XSynth.ChannelGroup_Create(groupOptions);

                reference.AddChannel(handle);
                XSynth.ChannelGroup_SetLayerCount(handle, (ulong)Program.Settings.MaxVoices);    
                XSynth.ChannelGroup_SetSoundfonts(handle, sfArray, sfCount);

                var tmp = XSynth.ChannelGroup_GetStreamParams(handle);

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
            if (Bass.LastError != 0)
            {
                Debug.PrintToConsole(Debug.LogType.Error, $"{UniqueID} - {Error}.");
                return true;
            }

            return false;
        }

        public override unsafe int Read(float[] buffer, int offset, long delta, int count)
        {
            lock (Lock)
            {
                fixed (float* buff = buffer)
                {
                    var offsetBuff = buff + offset;
                    XSynth.ChannelGroup_ReadSamples(handle, (nint)offsetBuff, (ulong)count);

                    if (volume != 1.0f)
                    {
                        for (int i = 0; i < count; i++)
                            offsetBuff[i] = offsetBuff[i] * (float)dbVolume;
                    }
                }
            }

            return count;
        }

        public override void SystemReset()
        {
            for (uint i = 0; i < 16; i++)
            {
                XSynth.ChannelGroup_SendEvent(handle, i, (ushort)XSynth.EventType.ResetControl, 0);
                XSynth.ChannelGroup_SendEvent(handle, i, (ushort)XSynth.EventType.Control, 0);
                XSynth.ChannelGroup_SendEvent(handle, i, (ushort)XSynth.EventType.ProgramChange, 0);
            }
        }

        public override void ChangeVolume(float volume)
        {
            this.volume = volume;
            dbVolume = Math.Pow(10.0f, (this.volume / 100.0f) * 0.05f);
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

                    if (param1 == 0)
                    {
                        eventType = XSynth.EventType.NoteOff;
                        eventParams = param1;
                    }
                    else eventParams = (param2 << 8) | param1;
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
        }

        public override void SetRenderingTime(float rt)
        {
            RenderingTime = rt;
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
