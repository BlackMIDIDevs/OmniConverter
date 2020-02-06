using System;
using System.Collections.Generic;
using Un4seen.Bass.AddOn.Midi;

namespace OmniConverter
{
    public class MIDI
    {
        private Int64 ID;
        private String FileName;
        private String FilePath;
        private TimeSpan TimeLength;
        private Int32 Tracks;
        private Int64 NoteCount;
        private UInt64 Size;

        public MIDI(Int64 I, String FN, String FP, TimeSpan TL, Int32 T, Int64 NC, UInt64 S)
        {
            ID = I;
            FileName = FN;
            FilePath = FP;
            TimeLength = TL;
            Tracks = T;
            NoteCount = NC;
            Size = S;
        }

        public Int64 GetID { get { return ID; } }
        public String GetName { get { return FileName; } }
        public String GetPath { get { return FilePath; } }
        public TimeSpan GetTimeLength { get { return TimeLength; } }
        public Int32 GetTracks { get { return Tracks; } }
        public Int64 GetNoteCount { get { return NoteCount; } }
        public UInt64 GetSize { get { return Size; } }
    }

    public class SoundFont
    {
        private String SoundFontPath;
        private Int32 SourcePreset;
        private Int32 SourceBank;
        private Int32 DestinationPreset;
        private Int32 DestinationBank;
        private Int32 DestinationBankLSB;
        private Boolean Enabled;
        private Boolean XGMode;
        public Int32 SoundFontHandle { get; set; }

        public SoundFont()
        {
            SoundFontPath = null;
            SourcePreset = 0;
            SourceBank = 0;
            DestinationPreset = 0;
            DestinationBank = 0;
            DestinationBankLSB = 0;
            Enabled = false;
            XGMode = false;
        }

        public SoundFont(String SFP, Int32 SP, Int32 SB, Int32 DP, Int32 DB, Int32 DBLSB, Boolean E, Boolean XGM)
        {
            SoundFontPath = SFP;
            SourcePreset = SP;
            SourceBank = SB;
            DestinationPreset = DP;
            DestinationBank = DB;
            DestinationBankLSB = DBLSB;
            Enabled = E;
            XGMode = XGM;
        }

        public void ChangePath(String SFP)
        {
            SoundFontPath = SFP;
        }

        public void SetNewValues(Int32 SP, Int32 SB, Int32 DP, Int32 DB, Int32 DBLSB, Boolean E, Boolean XGM)
        {
            SourcePreset = SP;
            SourceBank = SB;
            DestinationPreset = DP;
            DestinationBank = DB;
            DestinationBankLSB = DBLSB;
            Enabled = E;
            XGMode = XGM;
        }

        public String GetSoundFontPath { get { return SoundFontPath; } }
        public Int32 GetSourcePreset { get { return SourcePreset; } }
        public Int32 GetSourceBank { get { return SourceBank; } }
        public Int32 GetDestinationPreset { get { return DestinationPreset; } }
        public Int32 GetDestinationBank { get { return DestinationBank; } }
        public Int32 GetDestinationBankLSB { get { return DestinationBankLSB; } }
        public Boolean IsEnabled { get { return Enabled; } }
        public Boolean GetXGMode { get { return XGMode; } }
    }

    public class SoundFonts
    {
        public List<SoundFont> List;
        public List<BASS_MIDI_FONTEX> BMFEArray;

        public SoundFonts()
        {
            List = new List<SoundFont>();
            BMFEArray = new List<BASS_MIDI_FONTEX>();
        }
    }

    internal static class ListExtensions
    {
        public static int Move<T>(this IList<T> LINQList, int ITM, MoveDirection D)
        {
            // If there are few than 2 items, ignore
            if (LINQList.Count <= 1) return -1;

            // Store the expected new index for the item
            Int32 FinalDir = ITM + (int)D;

            // Do some logic to prevent crashes
            Int32 wIndexToMove = ((D == MoveDirection.Up) ? ((FinalDir < 0) ? LINQList.Count : ITM) : ((FinalDir >= LINQList.Count)) ? -1 : ITM) + (int)D;

            // Beep boop it should be all good, take a copy of the item
            var OldItem = LINQList[wIndexToMove];

            // If the item to move is at the end, move it back to the top
            if (ITM == LINQList.Count)
            {
                LINQList[wIndexToMove] = LINQList[0];
                LINQList[0] = OldItem;
            }
            // Else it's all good
            else
            {
                LINQList[wIndexToMove] = LINQList[ITM];
                LINQList[ITM] = OldItem;
            }

            // Return the new index value
            return wIndexToMove;
        }
    }

    public enum MoveDirection
    {
        Up = -1,
        Down = 1
    }
}
