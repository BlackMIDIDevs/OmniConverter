using CSCore;
using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

// Written with help from Arduano

namespace OmniConverter
{
    public struct MidiFontOC
    {
        public MidiFontEx2 bmStruct;
        public byte port;
    }

    public class BASSEngine : AudioEngine
    {
        private int FlacPlug = 0;
        private MidiFontOC[]? _bassArray;

        public BASSEngine(Settings settings) : base(settings, false)
        {
            if (Bass.Init(Bass.NoSoundDevice, _waveFormat.SampleRate, DeviceInitFlags.Default))
            {
                var tmp = BassMidi.CreateStream(16, BassFlags.Default, 0);
                FlacPlug = Bass.PluginLoad("bassflac");

                if (tmp != 0)
                {
                    // Subtract 1 because BASS uses -1 for no interpolation, 0 for linear and so on
                    var interp = ((int)_cachedSettings.Synth.Interpolation)
                        .LimitToRange((int)GlobalSynthSettings.InterpolationType.None,
                                      (int)GlobalSynthSettings.InterpolationType.Max) - 1;

                    Bass.Configure(Configuration.MidiVoices, _cachedSettings.Synth.MaxVoices);
                    Bass.Configure(Configuration.SRCQuality, interp);
                    Bass.Configure(Configuration.SampleSRCQuality, interp);

                    Bass.StreamFree(tmp);

                    if (FlacPlug == 0)
                        Debug.PrintToConsole(Debug.LogType.Warning, "BASSFLAC failed to load, this could lead to incorrect opcode handling when using SFZ based SoundFonts using FLAC samples.");
                    else
                        Debug.PrintToConsole(Debug.LogType.Message, "BASSFLAC loaded");

                    _bassArray = InitializeSoundFonts();

                    _init = true;

                    return;
                }
            }

            throw new BassException(Bass.LastError);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (_bassArray != null)
                FreeSoundFontsArray();

            if (FlacPlug != 0)
                Bass.PluginFree(FlacPlug);

            if (_init)
                Bass.Free();

            _init = false;
            _disposed = true;
        }

        private MidiFontOC[]? InitializeSoundFonts()
        {
            var _bassArray = new List<MidiFontOC>();

            foreach (SoundFont sf in _cachedSettings.SoundFontsList)
            {
                if (!sf.Enabled)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, "SoundFont is disabled, there's no need to load it.");
                    continue;
                }

                MidiFontOC bsf = new MidiFontOC();
                FontInitFlags bsfl = 0;

                // 0x40000 = BASS_MIDI_FONT_XGDRUMS
                // 0x100000 = BASS_MIDI_FONT_LINATTMOD
                // 0x200000 = BASS_MIDI_FONT_LINDECVOL
                // 0x400000 = BASS_MIDI_FONT_NORAMPIN
                // 0x800000 = BASS_MIDI_FONT_NOSBLIMITS
                // 0x1000000 = BASS_MIDI_FONT_MINFX
                // 0x2000000 = BASS_MIDI_FONT_SB_LIMITS

                bsfl |= sf.XGDrums ? (FontInitFlags)0x40000 : 0;
                bsfl |= sf.LinAttMod ? (FontInitFlags)0x100000 : 0;
                bsfl |= sf.LinDecVol ? (FontInitFlags)0x200000 : 0;
                bsfl |= sf.MinFx ? (FontInitFlags)0x1000000 : 0;
                bsfl |= sf.EnforceSBLimits ? (FontInitFlags)0x2000000 : (FontInitFlags)0x800000;
                bsfl |= sf.NoRampIn ? (FontInitFlags)0x400000 : 0;

                Debug.PrintToConsole(Debug.LogType.Message, $"Preparing BASS_MIDI_FONTEX for {sf.SoundFontPath}...");

                var sfHandle = BassMidi.FontInit(sf.SoundFontPath, bsfl);
                if (sfHandle != 0)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, $"SoundFont handle initialized. Handle = {sfHandle:X8}");

                    bsf.bmStruct.Handle = sfHandle;
                    bsf.bmStruct.SoundFontPreset = sf.SourcePreset;
                    bsf.bmStruct.SoundFontBank = sf.SourceBank;
                    bsf.bmStruct.DestinationPreset = sf.DestinationPreset;
                    bsf.bmStruct.DestinationBank = sf.DestinationBank;
                    bsf.bmStruct.DestinationBankLSB = sf.DestinationBankLSB;
                    bsf.port = sf.MIDIPort;

                    Debug.PrintToConsole(Debug.LogType.Message,
                        string.Format(
                            "spreset = {0}, sbank = {1}, dpreset = {2}, dbank = {3}, dbanklsb = {4}",
                            bsf.bmStruct.SoundFontPreset, bsf.bmStruct.SoundFontBank, bsf.bmStruct.DestinationPreset, bsf.bmStruct.DestinationBank, bsf.bmStruct.DestinationBankLSB, sf.XGDrums
                            )
                        );

                    Debug.PrintToConsole(Debug.LogType.Message,
                        string.Format(
                            "xg = {0}, lam = {0}, ldv = {0}, mfx = {0}, sl = {0}, nri = {0}",
                            sf.XGDrums, sf.LinAttMod, sf.LinDecVol, sf.MinFx, sf.EnforceSBLimits, sf.NoRampIn
                            )
                        );

                    BassMidi.FontLoad(bsf.bmStruct.Handle, bsf.bmStruct.SoundFontPreset, bsf.bmStruct.SoundFontBank);
                    _bassArray.Add(bsf);
                    Debug.PrintToConsole(Debug.LogType.Message, "SoundFont loaded and added to BASS_MIDI_FONTEX array.");
                }
                else Debug.PrintToConsole(Debug.LogType.Error, $"Could not load {sf.SoundFontPath}. BASSERR: {Bass.LastError}");
            }

            if (_bassArray.Count > 0) 
            {
                Debug.PrintToConsole(Debug.LogType.Message, "Reversing array...");
                _bassArray.Reverse();

                return _bassArray.ToArray();
            }
            else return null;
        }

        private void FreeSoundFontsArray()
        {
            if (_bassArray != null)
            {
                Debug.PrintToConsole(Debug.LogType.Message, "Freeing SoundFont handles...");
                foreach (var bsf in _bassArray)
                    BassMidi.FontFree(bsf.bmStruct.Handle);

                Debug.PrintToConsole(Debug.LogType.Message, "Handles freed.");
                _bassArray = null;
            }
        }

        public MidiFontOC[]? GetSoundFontsArray() => _bassArray;
    }

    public class BASSRenderer : AudioRenderer
    {
        private readonly BassFlags Flags;

        private int _streamHandle = 0;
        private int _volFx = 0;

        private VolumeFxParameters? VolParam = null;
        private List<MidiFontEx2> bmStructs = new List<MidiFontEx2>();
        private MidiFontOC[]? SfArray = [];

        public BASSRenderer(BASSEngine bass) : base(bass, false)
        {
            if (UniqueID == string.Empty)
                return;

            bool isFloat = WaveFormat.WaveFormatTag == AudioEncoding.IeeeFloat;
            Flags = BassFlags.Decode | BassFlags.MidiDecayEnd;
            Debug.PrintToConsole(Debug.LogType.Message, $"Stream unique ID: {UniqueID}");

            Flags |= (_cachedSettings.Synth.Interpolation > GlobalSynthSettings.InterpolationType.Linear) ? BassFlags.SincInterpolation : BassFlags.Default;
            Flags |= _cachedSettings.Synth.DisableEffects ? BassFlags.MidiNoFx : BassFlags.Default;
            Flags |= _cachedSettings.BASS.NoteOff1 ? BassFlags.MidiNoteOff1 : BassFlags.Default;
            Flags |= isFloat ? BassFlags.Float : BassFlags.Default;

            _streamHandle = BassMidi.CreateStream(16, Flags, WaveFormat.SampleRate);
            if (IsError("Unable to open MIDI stream."))
                return;

            Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - Stream is open.");

            _volFx = Bass.ChannelSetFX(_streamHandle, EffectType.Volume, 1);
            if (IsError("Unable to set volume FX."))
                return;

            Bass.ChannelSetAttribute(_streamHandle, ChannelAttribute.MidiKill, Convert.ToDouble(_cachedSettings.Synth.KilledNoteFading));

            bmStructs.Clear();
            SfArray = bass.GetSoundFontsArray();
            if (SfArray != null)
            {
                foreach (var sf in SfArray)
                {
                    if (sf.port == 0)
                    {
                        bmStructs.Add(sf.bmStruct);
                    }
                }

                BassMidi.StreamSetFonts(_streamHandle, bmStructs.ToArray(), bmStructs.Count);
                BassMidi.StreamLoadSamples(_streamHandle);
                Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - Loaded {SfArray.Length} SoundFonts");
            }

            if (VolParam == null)
                VolParam = new VolumeFxParameters();

            VolParam.fCurrent = 1.0f;
            VolParam.fTarget = (float)_cachedSettings.Synth.Volume;
            VolParam.fTime = 0.0f;
            VolParam.lCurve = 1;
            Bass.FXSetParameters(_volFx, VolParam);

            Initialized = true;
        }

        private bool IsError(string Error)
        {
            var beId = Bass.LastError;

            if (beId != 0)
            {
                Debug.PrintToConsole(Debug.LogType.Error, $"{UniqueID} (BE{beId:X}) - {Error}.");
                return true;
            }

            return false;
        }

        public override unsafe int Read(float[] buffer, int offset, long delta, int count)
        {
            int ret = 0;

            lock (_lock)
            {
                fixed (float* buff = buffer)
                {
                    var offsetBuff = buff + offset;
                    var len = (count * sizeof(float)) | (WaveFormat.BitsPerSample == 32 ? (int)DataFlags.Float : 0);

                    ret = Bass.ChannelGetData(_streamHandle, (nint)offsetBuff, len);
                    if (ret == 0)
                    {
                        var BE = Bass.LastError;

                        if (BE != Errors.Ended)
                            Debug.PrintToConsole(Debug.LogType.Warning, $"{UniqueID} - Data parsing error {BE} with length {len}");
                    }                 
                }
            }

            return ret / 4;
        }

        public override void SystemReset()
        {
            BassMidi.StreamEvent(_streamHandle, 0, MidiEventType.System, (int)MidiSystem.GS);
        }

        public override bool SendCustomCC(int channel, short reverb, short chorus) 
        {
            var b1 = BassMidi.StreamEvent(_streamHandle, channel, MidiEventType.Reverb, reverb);
            var b2 = BassMidi.StreamEvent(_streamHandle, channel, MidiEventType.Chorus, chorus);
            return b1 && b2;
        }

        public override void SendEvent(byte[] data)
        {
            var status = data[0];

            var ret = -1;
            var isSysEx = (EventType)status == EventType.SystemExclusive;
            var type = (EventType)(status & 0xF0);
            var chan = status & 0xF;
            var param1 = data.Length > 1 ? data[1] : 0;
            var param2 = data.Length > 2 ? data[2] : 0;

            int? eventParams = null;
            var eventType = MidiEventType.Note;

            switch (type)
            {
                case EventType.NoteOn:
                    eventParams = param2 << 8 | param1;
                    break;

                case EventType.NoteOff:
                    eventParams = param1;
                    break;

                case EventType.KeyPressure:
                    eventType = MidiEventType.KeyPressure;
                    eventParams = param2 << 8 | param1;
                    break;

                case EventType.ProgramChange:
                    eventType = MidiEventType.Program;
                    eventParams = param1;
                    break;

                case EventType.ChannelPressure:
                    eventType = MidiEventType.ChannelPressure;
                    eventParams = param1;
                    break;

                case EventType.PitchBend:
                    eventType = MidiEventType.Pitch;
                    eventParams = param2 << 7 | param1;
                    break;

                case EventType.MetaEvent:
                    switch (param1)
                    {
                        case 21:
                            {
                                if (SfArray == null)
                                {
                                    Debug.PrintToConsole(Debug.LogType.Error, $"Huh?");
                                    return;
                                }

                                if (data.Length != 4)
                                {
                                    Debug.PrintToConsole(Debug.LogType.Error, $"Received MIDI port event, but data length is {data.Length} instead of 4!");
                                    return;
                                }

                                var port = data[3];

                                if (data[3] != 0)
                                {
                                    var tgtSf = SfArray.Where(item => item.port == port).Select(item => item.bmStruct);

                                    if (tgtSf.Any())
                                    {
                                        bmStructs.Clear();
                                        foreach (var bmStruct in tgtSf)
                                        {
                                            bmStructs.Add(bmStruct);
                                        }

                                        BassMidi.StreamSetFonts(_streamHandle, bmStructs.ToArray(), bmStructs.Count());
                                    }              
                                }
                            }
                            return;

                        default:
                            ret = 0;
                            break;
                    }
                    break;

                default:
                    ret = 0;
                    break;
            }

            if (ret == 0)
            {
                ret = BassMidi.StreamEvents(_streamHandle, MidiEventsMode.Raw | MidiEventsMode.NoRunningStatus, data, data.Length);

                if (ret == -1 || (isSysEx && ret < 1))
                    Debug.PrintToConsole(Debug.LogType.Error, $"Unsupported {(isSysEx ? "SysEx" : "data")}! >> {BitConverter.ToString(data).Replace("-", "")}");
            }

            if (ret < 1 && eventParams != null)
                ret = Convert.ToInt32(BassMidi.StreamEvent(_streamHandle, chan, eventType, (int)eventParams));

            if (ret < 1)
            {
                switch (type)
                {
                    case EventType.Controller:
                        Debug.PrintToConsole(Debug.LogType.Error, $"Unsupported CC! >> {(ControllerType)param1}");
                        break;

                    default:
                        break;
                }
            }
        }

        public override void RefreshInfo()
        {
            float output = 0.0f;
            Bass.ChannelGetAttribute(_streamHandle, ChannelAttribute.MidiVoicesActive, out output);
            ActiveVoices = (ulong)output;

            Bass.ChannelGetAttribute(_streamHandle, ChannelAttribute.CPUUsage, out output);
            RenderingTime = output;
        }

        public override void SendEndEvent()
        {
            var ev = new[]
            {
                new MidiEvent() {EventType = MidiEventType.EndTrack, Channel = 0, Parameter = 0, Position = 0, Ticks = 0 },
                new MidiEvent() {EventType = MidiEventType.End, Channel = 0, Parameter = 0, Position = 0, Ticks = 0 },
            };

            BassMidi.StreamEvents(_streamHandle, MidiEventsMode.Raw | MidiEventsMode.Struct, ev);
        }

        public override long Position
        {
            get { return Bass.ChannelGetPosition(_streamHandle) / 4; }
            set { throw new NotSupportedException("Can't set position."); }
        }

        public override long Length
        {
            get { return Bass.ChannelGetLength(_streamHandle) / 4; }
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                lock (_lock)
                    Bass.StreamFree(_streamHandle);
            }

            UniqueID = string.Empty;
            CanSeek = false;

            Initialized = false;
            Disposed = true;
        }
    }
}
