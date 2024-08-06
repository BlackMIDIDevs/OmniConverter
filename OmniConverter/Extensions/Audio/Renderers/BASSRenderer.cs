using CSCore;
using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Midi;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Written with help from Arduano

namespace OmniConverter
{
    public class BASSEngine : AudioEngine
    {
        private MidiFontEx[]? _bassArray;

        public BASSEngine(CSCore.WaveFormat waveFormat, Settings settings) : base(waveFormat, settings, false)
        {
            /*                                                                                                                                                                            
                                -+++------.                             
                           -++###+#++++++++++#+++-                      
                       ++##+-..        -#+-    .++#++                   
                    -+#+.              .++.        .+#+-                
                   +#+.                              .+++               
                  ++#+++.                              .+++             
                   ##++++####+-.                         -#+-           
                         +#- -+++##++-                    .+++-         
                               .+- ++.                      .##+        
                           .   +- +#+                         +##.      
                     ++.  ++++++++#+          KHANG!!!         .+++     
                    -++#+++++-.                                  -++    
                     ++-                                           +    
                     -++++++++++++++++.                                 
                        .------------#+                                 
                                    -++.                                
                                     +#-        --                      
                                   +#+#+     .++##+                     
                                -+#+- ++.   +++. .#+.                   
                               +++..-+#+-  +#- -++#+#.                  
                               ++++#+++++ -+++#+-.                      
                                 .    .#+  -+-                          
                                       ++-                              
                                       -++.                             
            */

            if (Bass.Init(Bass.NoSoundDevice, waveFormat.SampleRate, DeviceInitFlags.Default))
            {
                _bassArray = InitializeSoundFonts();

                var tmp = BassMidi.CreateStream(16, BassFlags.Default, 0);

                if (tmp != 0)
                {
                    Bass.Configure(Configuration.MidiVoices, CachedSettings.MaxVoices);
                    Bass.Configure(Configuration.SRCQuality, ((int)CachedSettings.SincInter).LimitToRange((int)SincInterType.Linear, (int)SincInterType.Max));
                    Bass.Configure(Configuration.SampleSRCQuality, ((int)CachedSettings.SincInter).LimitToRange((int)SincInterType.Linear, (int)SincInterType.Max));

                    Bass.StreamFree(tmp);

                    Initialized = true;

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

            if (Initialized)
                Bass.Free();

            Initialized = false;
            _disposed = true;
        }

        private MidiFontEx[]? InitializeSoundFonts()
        {
            var _bassArray = new List<MidiFontEx>();

            foreach (SoundFont sf in CachedSettings.SoundFontsList)
            {
                if (!sf.Enabled)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, "SoundFont is disabled, there's no need to load it.");
                    continue;
                }

                MidiFontEx bsf;
                Debug.PrintToConsole(Debug.LogType.Message, $"Preparing BASS_MIDI_FONTEX for {sf.SoundFontPath}...");

                var sfHandle = BassMidi.FontInit(sf.SoundFontPath, sf.XGMode ? FontInitFlags.XGDrums : (FontInitFlags)0);

                if (sfHandle != 0)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, $"SoundFont handle initialized. Handle = {sfHandle:X8}");

                    bsf.Handle = sfHandle;
                    bsf.SoundFontPreset = sf.SourcePreset;
                    bsf.SoundFontBank = sf.SourceBank;
                    bsf.DestinationPreset = sf.DestinationPreset;
                    bsf.DestinationBank = sf.DestinationBank;
                    bsf.DestinationBankLSB = sf.DestinationBankLSB;
                    Debug.PrintToConsole(Debug.LogType.Message,
                        string.Format(
                            "spreset = {0}, sbank = {1}, dpreset = {2}, dbank = {3}, dbanklsb = {4}, xg = {5}",
                            bsf.SoundFontPreset, bsf.SoundFontBank, bsf.DestinationPreset, bsf.DestinationBank, bsf.DestinationBankLSB, sf.XGMode
                            )
                        );

                    BassMidi.FontLoad(bsf.Handle, bsf.SoundFontPreset, bsf.SoundFontBank);
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
                    BassMidi.FontFree(bsf.Handle);

                Debug.PrintToConsole(Debug.LogType.Message, "Handles freed.");
                _bassArray = null;
            }
        }

        public MidiFontEx[]? GetSoundFontsArray() => _bassArray;
    }

    public class BASSRenderer : MIDIRenderer
    {
        private readonly object Lock = new object();
        private readonly BassFlags Flags;
        public int Handle { get; private set; } = 0;

        private int VolHandle;
        private BASSEngine reference;
        private VolumeFxParameters? VolParam = null;
        private MidiFontEx[]? SfArray = [];

        public BASSRenderer(BASSEngine bass) : base(bass.WaveFormat, false)
        {
            if (UniqueID == string.Empty)
                return;

            reference = bass;

            bool isFloat = WaveFormat.WaveFormatTag == AudioEncoding.IeeeFloat;
            Flags = BassFlags.Decode | BassFlags.MidiDecayEnd;
            Debug.PrintToConsole(Debug.LogType.Message, $"Stream unique ID: {UniqueID}");

            Flags |= (reference.CachedSettings.SincInter > SincInterType.Linear) ? BassFlags.SincInterpolation : BassFlags.Default;
            Flags |= reference.CachedSettings.DisableEffects ? BassFlags.MidiNoFx : BassFlags.Default;
            Flags |= reference.CachedSettings.NoteOff1 ? BassFlags.MidiNoteOff1 : BassFlags.Default;
            Flags |= isFloat ? BassFlags.Float : BassFlags.Default;

            Handle = BassMidi.CreateStream(16, Flags, WaveFormat.SampleRate);
            if (IsError("Unable to open MIDI stream."))
                return;

            Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - Stream is open.");

            VolHandle = Bass.ChannelSetFX(Handle, EffectType.Volume, 1);
            if (IsError("Unable to set volume FX."))
                return;

            Bass.ChannelSetAttribute(Handle, ChannelAttribute.MidiKill, Convert.ToDouble(reference.CachedSettings.KilledNoteFading));

            SfArray = reference.GetSoundFontsArray();
            if (SfArray != null)
            {
                BassMidi.StreamSetFonts(Handle, SfArray, SfArray.Length);
                BassMidi.StreamLoadSamples(Handle);
                Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - Loaded {SfArray.Length} SoundFonts");
            }

            Initialized = true;
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
            int ret = 0;

            lock (Lock)
            {
                fixed (float* buff = buffer)
                {
                    var offsetBuff = buff + offset;
                    var len = (count * sizeof(float)) | (WaveFormat.BitsPerSample == 32 ? (int)DataFlags.Float : 0);

                    ret = Bass.ChannelGetData(Handle, (nint)offsetBuff, len);
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
            BassMidi.StreamEvent(Handle, 0, MidiEventType.System, (int)MidiSystem.GS);
        }

        public override void ChangeVolume(float volume)
        {
            if (VolParam == null)
                VolParam = new VolumeFxParameters();

            VolParam.fCurrent = 1.0f;
            VolParam.fTarget = volume;
            VolParam.fTime = 0.0f;
            VolParam.lCurve = 1;
            Bass.FXSetParameters(VolHandle, VolParam);
        }

        public override bool SendCustomFXEvents(int channel, short reverb, short chorus) 
        {
            var b1 = BassMidi.StreamEvent(Handle, channel, MidiEventType.Reverb, reverb);
            var b2 = BassMidi.StreamEvent(Handle, channel, MidiEventType.Chorus, chorus);
            return b1 && b2;
        }

        public override void SendEvent(byte[] data)
        {
            var status = data[0];
            var param1 = data[1];
            var param2 = data.Length >= 3 ? data[2] : 0;
            
            int eventParams;
            var eventType = MidiEventType.Note;

            switch ((MIDIEventType)(status & 0xF0))
            {
                case MIDIEventType.NoteOn:
                    if (reference.CachedSettings.FilterVelocity && param2 >= reference.CachedSettings.VelocityLow && param2 <= reference.CachedSettings.VelocityHigh)
                        return;
                    if (reference.CachedSettings.FilterKey && (param1 < reference.CachedSettings.KeyLow || param1 > reference.CachedSettings.KeyHigh))
                        return;
                    eventParams = param2 << 8 | param1;
                    break;

                case MIDIEventType.NoteOff:
                    if (reference.CachedSettings.FilterKey && (param1 < reference.CachedSettings.KeyLow || param1 > reference.CachedSettings.KeyHigh))
                        return;
                    eventParams = param1;
                    break;

                case MIDIEventType.Aftertouch:
                    eventType = MidiEventType.KeyPressure;
                    eventParams = param2 << 8 | param1;
                    break;

                case MIDIEventType.PatchChange:
                    eventType = MidiEventType.Program;
                    eventParams = param1;
                    break;

                case MIDIEventType.ChannelPressure:
                    eventType = MidiEventType.ChannelPressure;
                    eventParams = param1;
                    break;

                case MIDIEventType.PitchBend:
                    eventType = MidiEventType.Pitch;
                    eventParams = param2 << 7 | param1;
                    break;

                default:
                    BassMidi.StreamEvents(Handle, MidiEventsMode.Raw | MidiEventsMode.NoRunningStatus, data);
                    return;
            }

            BassMidi.StreamEvent(Handle, status & 0xF, eventType, eventParams);
        }

        public override void RefreshInfo()
        {
            float output = 0.0f;
            Bass.ChannelGetAttribute(Handle, ChannelAttribute.MidiVoicesActive, out output);
            ActiveVoices = (ulong)output;

            Bass.ChannelGetAttribute(Handle, ChannelAttribute.CPUUsage, out output);
            RenderingTime = output;
        }

        public override void SendEndEvent()
        {
            var ev = new[]
            {
                new MidiEvent() {EventType = MidiEventType.EndTrack, Channel = 0, Parameter = 0, Position = 0, Ticks = 0 },
                new MidiEvent() {EventType = MidiEventType.End, Channel = 0, Parameter = 0, Position = 0, Ticks = 0 },
            };

            BassMidi.StreamEvents(Handle, MidiEventsMode.Raw | MidiEventsMode.Struct, ev);
        }

        public override long Position
        {
            get { return Bass.ChannelGetPosition(Handle) / 4; }
            set { throw new NotSupportedException("Can't set position."); }
        }

        public override long Length
        {
            get { return Bass.ChannelGetLength(Handle) / 4; }
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                lock (Lock)
                    Bass.StreamFree(Handle);
            }

            UniqueID = string.Empty;
            CanSeek = false;

            Initialized = false;
            Disposed = true;
        }
    }
}
