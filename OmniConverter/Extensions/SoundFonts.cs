using Avalonia.Platform.Storage;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace OmniConverter
{
    public class SoundFont
    {
        [JsonProperty("Path")]
        public string SoundFontPath { get; private set; }
        [JsonProperty]
        public int SourcePreset { get; private set; }
        [JsonProperty]
        public int SourceBank { get; private set; }
        [JsonProperty]
        public int DestinationPreset { get; private set; }
        [JsonProperty]
        public int DestinationBank { get; private set; }
        [JsonProperty]
        public int DestinationBankLSB { get; private set; }
        [JsonProperty]
        public bool Enabled { get; private set; }
        [JsonProperty]
        public bool XGMode { get; private set; }

        public SoundFont()
        {
            SoundFontPath = string.Empty;
            SourcePreset = -1;
            SourceBank = -1;
            DestinationPreset = -1;
            DestinationBank = 0;
            DestinationBankLSB = 0;
            Enabled = false;
            XGMode = false;
        }

        public SoundFont(string SFP, int SP, int SB, int DP, int DB, int DBLSB, bool E, bool XGM)
        {
            SoundFontPath = SFP;
            SetNewValues(SP, SB, DP, DB, DBLSB, E, XGM);
        }

        public void ChangePath(string SFP)
        {
            SoundFontPath = SFP;
            SetNewValues(SourcePreset, SourceBank, DestinationPreset, DestinationBank, DestinationBankLSB, Enabled, XGMode);
        }

        public void SetNewValues(int SP, int SB, int DP, int DB, int DBLSB, bool E, bool XGM)
        {
            SourcePreset = IsSFZ() && SP < 0 ? 0 : SP;
            SourceBank = IsSFZ() && SB < 0 ? 0 : SB;
            DestinationPreset = IsSFZ() && DP < 0 ? 0 : DP;
            DestinationBank = DB;
            DestinationBankLSB = DBLSB;
            Enabled = E;
            XGMode = XGM;
        }

        public bool IsSFZ()
        {
            return Path.GetExtension(SoundFontPath).Equals(".sfz");
        }

        public static FilePickerFileType SoundFontAll { get; } = new("SoundFonts")
        {
            Patterns = new[] { "*.sf2", "*.sf3", "*.sfz" },
            AppleUniformTypeIdentifiers = new[] { "soundfont" },
            MimeTypes = new[] { "audio/x-soundfont" }
        };
    }

    public class SoundFonts
    {
        private ObservableCollection<SoundFont> _sfList;

        public SoundFonts()
        {
            _sfList = new ObservableCollection<SoundFont>();
        }

        public SoundFonts(ObservableCollection<SoundFont> sfList)
        {
            _sfList = sfList;
        }

        public ObservableCollection<SoundFont> GetSoundFontList() => _sfList;

        public void Add(SoundFont? sf)
        {
            if (sf != null)
                _sfList.Add(sf);
        }

        public void Remove(SoundFont? sf)
        {
            if (sf != null)
                _sfList.Remove(sf);
        }

        public void Move(SoundFont? sf, MoveDirection direction)
        {
            if (sf != null)
            {
                List<int> itemsBefore = new List<int>();

                int oldIndex = _sfList.IndexOf(sf);
                _sfList.Move(oldIndex, direction);
            }
        }
    }

}
