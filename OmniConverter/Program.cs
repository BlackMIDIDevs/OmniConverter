using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;

namespace OmniConverter
{
    static class Program
    {
        private const int IDC_HAND = 32649;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        public static readonly Cursor SystemHandCursor = new Cursor(LoadCursor(IntPtr.Zero, IDC_HAND));

        public static readonly Color Good = Color.DarkGreen;
        public static readonly Color Error = Color.DarkRed;

        public static List<MIDI> MIDIList = new List<MIDI>();
        public static List<VST> VSTArray = new List<VST>();
        public static SoundFonts SFArray = new SoundFonts();

        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main(String[] Arguments)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          
            if (Properties.Settings.Default.PreRelease)
                MessageBox.Show("You are using a pre-release version of the converter.\n" +
                    "Anything you might try and see here might not make it into the final release.\n\n" +
                    "You have been warned.\n\nPress OK to continue.", "OmniConverter - Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (Properties.Settings.Default.SUP)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.SUP = false;
                Properties.Settings.Default.Save();
                Debug.PrintToConsole("ok", "Restored settings from old version.");
            }

            if (Properties.Settings.Default.MultiThreadedLimitV == -1)
            {
                Properties.Settings.Default.MultiThreadedLimitV = Environment.ProcessorCount;
                Properties.Settings.Default.Save();
                Debug.PrintToConsole("ok", String.Format("MTL = {0}", Properties.Settings.Default.MultiThreadedLimit));
            }

            if (Properties.Settings.Default.UpdateBranch == "choose")
            {
                SelectBranch frm = new SelectBranch();
                frm.ShowInTaskbar = true;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                frm.Dispose();
            }

            foreach (String Arg in Arguments)
            {
                switch (Arg.ToLowerInvariant())
                {
                    case "/debug":
                    case "/mode":
                        Debug.EnableConsole();
                        Debug.PrintToConsole("ok", "Restored settings from old version.");
                        break;
                    case "/reset":
                        if (MessageBox.Show("Are you sure you want to reset OmniConverter's settings?") == DialogResult.Yes)
                        {
                            Properties.Settings.Default.Reset();
                            Properties.Settings.Default.SUP = false;
                            Properties.Settings.Default.Save();
                            break;
                        }
                        else return;
                }
            }

            CommonSoundFonts.LoadCSF();

            Application.Run(new MainWindow(Arguments));
        }
    }

    static class Debug
    {
        private static readonly object Lock = new object();
        public static Boolean DebugMode = false;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        public static DialogResult ShowMsgBox(String Title, String Error, String Exception, MessageBoxButtons Btns, MessageBoxIcon Icns)
        {
            String FinalTitle = String.Format("OmniConverter - {0}", Title);
            String FinalError = Error;

            if (!String.IsNullOrEmpty(Exception))
                FinalError += String.Format("\n\nException:\n{0}", Exception);

            PrintToConsole(String.IsNullOrEmpty(Exception) ? "err" : "wrn", String.Format("{0} - {1}", FinalError, String.IsNullOrEmpty(Exception) ? Exception : "null"));

            if (Btns == MessageBoxButtons.OK)
                FinalError += "\n\nPress OK to continue.";

            return MessageBox.Show(FinalError, FinalTitle, Btns, Icns);
        }

        public static void EnableConsole()
        {
            Version CV = Assembly.GetExecutingAssembly().GetName().Version;

            AllocConsole();
            DebugMode = true;

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(String.Format("OmniConverter v{0}.{1}.{2}", CV.Major, CV.Minor, CV.Build));
            Console.WriteLine(String.Format("Copyright (C) 2019-{0} Keppy's Software and Arduano\nYou're not supposed to be here!\n", DateTime.Today.Year));
        }

        public static void PrintToConsole(String Type, String Message)
        {
            if (!DebugMode) return;

            lock (Lock)
            {

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(String.Format("{0} - ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff tt")));

                switch (Type.ToLowerInvariant())
                {
                    case "wrn":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case "err":
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                    case "ok":
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.Write(String.Format("{0}\n", Message));
            }
        }
    }

    internal static class IDGenerator
    {
        private static Random RND = new Random();
        public static string GetID() => RND.Next(Int32.MinValue, Int32.MaxValue).ToString("X8");
    }

    internal static class IconSystem
    {
        public static Bitmap Add = IconSystem.GetResizedIcon(Properties.Resources.Add, 24, 24);
        public static Bitmap Remove = IconSystem.GetResizedIcon(Properties.Resources.Remove, 24, 24);
        public static Bitmap Clear = IconSystem.GetResizedIcon(Properties.Resources.Clear, 24, 24);
        public static Bitmap Sleep = IconSystem.GetResizedIcon(Properties.Resources.Sleep, 24, 24);
        public static Bitmap Info = IconSystem.GetResizedIcon(Properties.Resources.Info, 24, 24);
        public static Bitmap Download = IconSystem.GetResizedIcon(Properties.Resources.Download, 24, 24);
        public static Bitmap Empty = IconSystem.GetResizedIcon(Properties.Resources.Empty, 24, 24);
        public static Bitmap Up = IconSystem.GetResizedIcon(Properties.Resources.Up, 24, 24);
        public static Bitmap Down = IconSystem.GetResizedIcon(Properties.Resources.Down, 24, 24);
        public static Bitmap Octocat = IconSystem.GetResizedIcon(Properties.Resources.Octocat, 24, 24);

        public static Bitmap GetResizedIcon(Image IMG, int W, int H)
        {
            Rectangle DestinationRect = new Rectangle(0, 0, W, H);
            Bitmap DestinationImg = new Bitmap(W, H);

            DestinationImg.SetResolution(IMG.HorizontalResolution, IMG.VerticalResolution);

            using (Graphics GRP = Graphics.FromImage(DestinationImg))
            {
                GRP.CompositingMode = CompositingMode.SourceCopy;
                GRP.CompositingQuality = CompositingQuality.HighQuality;
                GRP.InterpolationMode = InterpolationMode.HighQualityBicubic;
                GRP.SmoothingMode = SmoothingMode.HighQuality;
                GRP.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (ImageAttributes WM = new ImageAttributes())
                {
                    WM.SetWrapMode(WrapMode.TileFlipXY);
                    GRP.DrawImage(IMG, DestinationRect, 0, 0, IMG.Width, IMG.Height, GraphicsUnit.Pixel, WM);
                }
            }

            return DestinationImg;
        }
    }
}
