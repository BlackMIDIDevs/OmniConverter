using CSCore;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
using System.Diagnostics;
using Avalonia.Threading;
using System.Collections.ObjectModel;
using Avalonia.Media;

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

    public class BASS : AudioEngine
    {
        private MidiFontEx[]? _bassArray;

        public BASS(CSCore.WaveFormat waveFormat, int maxVoices, ObservableCollection<SoundFont>? initSFs = null) : base(waveFormat, false)
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

            // spam it so it actually parses the value
            Bass.Configure(Configuration.MidiVoices, maxVoices);

            if (Bass.Init(Bass.NoSoundDevice, waveFormat.SampleRate, DeviceInitFlags.Default))
            {
                if (initSFs != null)
                    _bassArray = InitializeSoundFonts(initSFs);

                Initialized = true;
            }
            else throw new BassException(Bass.LastError);
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

        private MidiFontEx[]? InitializeSoundFonts(ObservableCollection<SoundFont> sfList)
        {
            var _bassArray = new List<MidiFontEx>();

            foreach (SoundFont sf in sfList)
            {
                if (!sf.Enabled)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, "SoundFont is disabled, there's no need to load it.");
                    continue;
                }

                MidiFontEx bsf;
                Debug.PrintToConsole(Debug.LogType.Message, String.Format("Preparing BASS_MIDI_FONTEX for {0}...", sf.SoundFontPath));

                var sfHandle = BassMidi.FontInit(sf.SoundFontPath, sf.XGMode ? FontInitFlags.XGDrums : (FontInitFlags)0);

                if (sfHandle != 0)
                {
                    Debug.PrintToConsole(Debug.LogType.Message, String.Format("SoundFont handle initialized. Handle = {0:X8}", sfHandle));

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

                    _bassArray.Add(bsf);
                    Debug.PrintToConsole(Debug.LogType.Message, "SoundFont loaded and added to BASS_MIDI_FONTEX array.");
                }
                else Debug.PrintToConsole(Debug.LogType.Error, String.Format("Could not load {0}. BASSERR: {1}", sf.SoundFontPath, Bass.LastError));
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

    public class BASSMIDI : MIDIRenderer
    {
        private readonly object Lock = new object();
        private BassFlags Flags;
        public int Handle { get; private set; } = 0;

        // Special RTS mode
        private Random RTSR = new Random();
        private bool RTSMode { get; } = false;

        private int VolHandle;
        private VolumeFxParameters? VolParam = null;
        private MidiFontEx[]? SfArray = [];

        public BASSMIDI(BASS bass) : base(bass.WaveFormat, false)
        {
            if (UniqueID == string.Empty)
                return;

            bool isFloat = WaveFormat.WaveFormatTag == AudioEncoding.IeeeFloat;
            Flags = BassFlags.Decode | BassFlags.MidiDecayEnd;

            Debug.PrintToConsole(Debug.LogType.Message, String.Format("Stream unique ID: {0}", UniqueID));
            Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - VoiceLimit = {Program.Settings.MaxVoices}.");

            Flags |= Program.Settings.SincInter ? BassFlags.SincInterpolation : BassFlags.Default;
            Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - SincInter = {Program.Settings.SincInter}.");

            Flags |= Program.Settings.DisableEffects ? BassFlags.MidiNoFx : BassFlags.Default;
            Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - DisableEffects = {Program.Settings.DisableEffects}.");

            Flags |= Program.Settings.NoteOff1 ? BassFlags.MidiNoteOff1 : BassFlags.Default;
            Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - NoteOff1 = {Program.Settings.NoteOff1}.");

            Flags |= isFloat ? BassFlags.Float : BassFlags.Default;
            Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - isFloat = {isFloat}.");

            Handle = BassMidi.CreateStream(16, Flags, WaveFormat.SampleRate);
            if (!CheckError("Unable to open MIDI stream."))
            {
                return;
            }
            else Debug.PrintToConsole(Debug.LogType.Message, String.Format("{0} - Stream is open.", UniqueID));

            VolHandle = Bass.ChannelSetFX(Handle, EffectType.Volume, 1);
            if (!CheckError("Unable to set volume FX."))
            {
                return;
            }

            SfArray = bass.GetSoundFontsArray();
            if (SfArray != null)
            {
                BassMidi.StreamSetFonts(Handle, SfArray, SfArray.Length);
                Debug.PrintToConsole(Debug.LogType.Message, $"{UniqueID} - Loaded {SfArray.Length} SoundFonts.");
            }

            Initialized = true;
        }

        private bool CheckError(string Error)
        {
            if (Bass.LastError != 0)
            {
                Debug.PrintToConsole(Debug.LogType.Error, $"{UniqueID} - {Error}.");
                return false;
            }

            return true;
        }

        public override unsafe int Read(float[] buffer, int offset, int count)
        {
            lock (Lock)
            {
                fixed (float* buff = buffer)
                {
                    var offsetBuff = buff + offset;
                    var len = (count * 4) | (WaveFormat.BitsPerSample == 32 ? (int)DataFlags.Float : 0);

                    int ret = Bass.ChannelGetData(Handle, (IntPtr)offsetBuff, len);
                    if (ret == 0)
                    {
                        var BE = Bass.LastError;

                        if (BE != Errors.Ended)
                            Debug.PrintToConsole(Debug.LogType.Warning, $"{UniqueID} - Data parsing error {BE} with length {len}");
                    }

                    return ret / 4;
                }
            }
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
                    eventParams = param2 << 8 | param1;
                    break;

                case MIDIEventType.NoteOff:
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

        public override bool SendEndEvent()
        {
            var ev = new[]
            {
                new MidiEvent() {EventType = MidiEventType.EndTrack, Channel = 0, Parameter = 0, Position = 0, Ticks = 0 },
                new MidiEvent() {EventType = MidiEventType.End, Channel = 0, Parameter = 0, Position = 0, Ticks = 0 },
            };

            return BassMidi.StreamEvents(Handle, MidiEventsMode.Raw | MidiEventsMode.Struct, ev) != -1 ? true : false;
        }

        public override int ActiveVoices
        {
            get
            {
                float voices;
                Bass.ChannelGetAttribute(Handle, ChannelAttribute.MidiVoicesActive, out voices);

                return (int)voices;
            }
        }

        public override float RenderingTime
        {
            get
            {
                float cpuusage;
                Bass.ChannelGetAttribute(Handle, ChannelAttribute.CPUUsage, out cpuusage);

                return cpuusage;
            }
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
