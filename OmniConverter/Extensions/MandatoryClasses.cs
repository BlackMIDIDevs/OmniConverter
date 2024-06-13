using ManagedBass.Midi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniConverter
{
    public class SoundFont
    {
        public string SoundFontPath { get; private set; }
        public int SourcePreset { get; private set; }
        public int SourceBank { get; private set; }
        public int DestinationPreset { get; private set; }
        public int DestinationBank { get; private set; }
        public int DestinationBankLSB { get; private set; }
        public bool Enabled { get; private set; }
        public bool XGMode { get; private set; }

        public int SoundFontHandle;

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
    }

    public class VST
    {
        private string VSTPath;
        private Int32 VSTHandle { get; set; }

        public VST()
        {
            VSTPath = null;
        }
    }

    public class SoundFonts
    {
        public List<SoundFont> List;
        public MidiFontEx[] BMFEArray;

        public SoundFonts()
        {
            List = new List<SoundFont>();
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

    public class ParallelLoopExt
    {
        public static void ParallelFor(int from, int to, int threads, CancellationToken cancel, Action<int> func)
        {
            Dictionary<int, Task> tasks = new Dictionary<int, Task>();
            BlockingCollection<int> completed = new BlockingCollection<int>();

            void RunTask(int i)
            {
                var t = new Task(() =>
                {
                    func(i);
                    completed.Add(i);
                });
                tasks.Add(i, t);
                t.Start();
            }

            void TryTake()
            {
                var t = completed.Take(cancel);
                tasks[t].Wait();
                tasks.Remove(t);
            }

            for (int i = from; i < to; i++)
            {
                RunTask(i);
                if (tasks.Count >= threads) TryTake();
            }

            while (completed.Count > 0 || tasks.Count > 0) TryTake();
        }
    }

    public enum MoveDirection
    {
        Up = -1,
        Down = 1
    }
}
