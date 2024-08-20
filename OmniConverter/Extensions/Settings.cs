using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace OmniConverter
{
    [JsonObject]
    public class GlobalSynthSettings : ICloneable
    {
        public enum InterpolationType
        {
            None = 0,
            Linear,
            Point8,
            Point16,
            Point32,
            Point64,
            Max = Point64
        }

        [JsonProperty]
        public double Volume = 1.0;

        [JsonProperty]
        public int SampleRate = 48000;

        [JsonProperty]
        public InterpolationType Interpolation = InterpolationType.Point16;

        [JsonProperty]
        public bool KilledNoteFading = false;

        [JsonProperty]
        public bool AudioLimiter = false;

        [JsonProperty]
        public bool RTSMode = false;

        [JsonProperty]
        public double RTSFPS = 90.0;

        [JsonProperty]
        public double RTSFluct = 75.0;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [JsonObject]
    public class BASSSettings : ICloneable
    {
        [JsonProperty]
        public int MaxVoices = 2048;

        [JsonProperty]
        public bool DisableEffects = true;

        [JsonProperty]
        public bool NoteOff1 = false;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [JsonObject]
    public class XSynthSettings : ICloneable
    {
        public enum ThreadingType
        {
            None = 0,
            PerChannel,
            PerKey,
            Max = PerKey,
        }

        [JsonProperty]
        public ulong MaxLayers = 32;

        [JsonProperty]
        public bool UseEffects = true;

        [JsonProperty]
        public ThreadingType Threading = ThreadingType.PerKey;

        [JsonProperty]
        public bool LinearEnvelope = false;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [JsonObject]
    public class EncoderSettings : ICloneable
    {
        [JsonProperty]
        public AudioCodecType AudioCodec = AudioCodecType.PCM;

        [JsonProperty]
        public int AudioBitrate = 256; // kbps

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [JsonObject]
    public class EventSettings : ICloneable
    {
        [JsonProperty]
        public bool FilterVelocity = false;

        [JsonProperty]
        public int VelocityLow = 1;

        [JsonProperty]
        public int VelocityHigh = 1;

        [JsonProperty]
        public bool FilterKey = false;

        [JsonProperty]
        public int KeyLow = 0;

        [JsonProperty]
        public int KeyHigh = 127;

        [JsonProperty]
        public bool OverrideEffects = false;

        [JsonProperty]
        public short ReverbVal = 64;

        [JsonProperty]
        public short ChorusVal = 64;

        [JsonProperty]
        public bool IgnoreProgramChanges = false;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

     [JsonObject]
    public class RenderSettings : ICloneable
    {
        // Threads count is set to ProcessorCount / 2 ,
        // to avoid having the converter hog up all the resources
        [JsonProperty]
        public bool MultiThreadedMode = true;

        [JsonProperty]
        public int ThreadsCount = Environment.ProcessorCount / 2;

        [JsonProperty]
        public bool PerTrackMode = false;

        [JsonProperty]
        public bool PerTrackFile = false;

        [JsonProperty]
        public bool PerTrackStorage = false;

        [JsonProperty]
        public bool AutoExportToFolder = false;

        [JsonProperty]
        public string AutoExportFolderPath = string.Empty;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [JsonObject]
    public class ProgramSettings : ICloneable
    {
        [JsonProperty]
        public bool AutoSaveState = false;

        [JsonProperty]
        public bool RichPresence = true;

        [JsonProperty]
        public bool AutoUpdateCheck = false;

        [JsonProperty]
        public UpdateSystem.Branch UpdateBranch = UpdateSystem.Branch.None;

        [JsonProperty]
        public int AfterRenderAction = -1;

        [JsonProperty]
        public bool AudioEvents = true;

        [JsonProperty]
        public bool OldKMCScheme = false;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class Settings : ICloneable
    {
        [JsonProperty]
        public EngineID Renderer = EngineID.BASS;

        [JsonProperty]
        public GlobalSynthSettings Synth = new();

        [JsonProperty]
        public BASSSettings BASS = new();

        [JsonProperty]
        public XSynthSettings XSynth = new();

        [JsonProperty]
        public EncoderSettings Encoder = new();

        [JsonProperty]
        public EventSettings Event = new();

        [JsonProperty]
        public RenderSettings Render = new();

        [JsonProperty]
        public ProgramSettings Program = new();

        [JsonProperty]
        public string LastMIDIFolder = string.Empty;

        [JsonProperty]
        public string LastSoundFontFolder = string.Empty;
        
        [JsonProperty]
        public string LastExportFolder = string.Empty;

        [JsonProperty("SoundFonts")]
        public ObservableCollection<SoundFont> SoundFontsList = [];

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
