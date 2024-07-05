using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace OmniConverter
{
    public enum SincInterType
    {
        Linear = 0,
        Point8,
        Point16,
        Point32,
        Point64,
        Max = Point64
    }

    public class Settings
    {
        [JsonProperty]
        public EngineID Renderer = EngineID.BASS;

        [JsonProperty]
        public float Volume = 100.0f;
        [JsonProperty]
        public int SampleRate = 48000;
        [JsonProperty]
        public AudioCodecType AudioCodec = AudioCodecType.PCM;
        [JsonProperty]
        public int AudioBitrate = 256; // kbps

        [JsonProperty]
        public int MaxVoices = 1000;
        [JsonProperty]
        public bool DisableEffects = true;
        [JsonProperty]
        public bool AudioLimiter = false;

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
        public bool NoteOff1 = false;
        [JsonProperty]
        public SincInterType SincInter = SincInterType.Point16;

        [JsonProperty]
        public bool RichPresence = true;

        [JsonProperty]
        public bool RTSMode = false;
        [JsonProperty]
        public double RTSFPS = 90.0;
        [JsonProperty]
        public double RTSFluct = 75.0;

        [JsonProperty]
        public bool AutoUpdateCheck = false;
        [JsonProperty]
        public UpdateSystem.Branch UpdateBranch = UpdateSystem.Branch.None;

        [JsonProperty]
        public bool AutoExportToFolder = false;
        [JsonProperty]
        public string AutoExportFolderPath = string.Empty;
        [JsonProperty]
        public int AfterRenderAction = -1;
        [JsonProperty]
        public bool AudioEvents = true;
        [JsonProperty]
        public bool OldKMCScheme = false;

        [JsonProperty]
        public string LastMIDIFolder = string.Empty;
        [JsonProperty]
        public string LastSoundFontFolder = string.Empty;
        [JsonProperty]
        public string LastExportFolder = string.Empty;

        [JsonProperty("SoundFonts")]
        public ObservableCollection<SoundFont> SoundFontsList = new();
    }
}
