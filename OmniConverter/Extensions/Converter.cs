using CSCore;
using CSCore.Codecs.WAV;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;

namespace OmniConverter
{
    class MIDIsValidity
    {
        private String CurrentMIDI;
        private UInt64 ValidMIDIs;
        private UInt64 InvalidMIDIs;
        private UInt64 TotalMIDIsCount;

        private Int32 Tracks = 0;
        private Int32 CurrentTrack = 0;

        public MIDIsValidity()
        {
            CurrentMIDI = "";
            ValidMIDIs = 0;
            InvalidMIDIs = 0;
            Tracks = 0;
            CurrentTrack = 0;
            TotalMIDIsCount = (ulong)Program.MIDIList.Count;
        }

        public void SetCurrentMIDI(String S) { CurrentMIDI = S; }
        public String GetCurrentMIDI() { return CurrentMIDI; }
        public void AddValidMIDI() { ValidMIDIs++; }
        public void AddInvalidMIDI() { InvalidMIDIs++; }
        public UInt64 GetValidMIDIsCount() { return ValidMIDIs; }
        public UInt64 GetInvalidMIDIsCount() { return InvalidMIDIs; }
        public UInt64 GetTotalMIDIsCount() { return TotalMIDIsCount; }

        public void SetTotalTracks(Int32 T) { Tracks = T; }
        public void AddTrack() { CurrentTrack++; }
        public void ResetCurrentTrack() { CurrentTrack = 0; }
        public Int32 GetTotalTracks() { return Tracks; }
        public Int32 GetCurrentTrack() { return CurrentTrack; }
    }

    class Converter
    {
        public CancellationTokenSource CTS;
        public MIDIsValidity MDV;

        private String Status = "prep";
        private String StError = "";
        private Boolean StopRequested = false;
        private Boolean IsCrash = false;

        private Thread CThread;

        public Converter(Control Form, Panel ThreadsPanel, String OPath)
        {
            MDV = new MIDIsValidity();

            if (Properties.Settings.Default.PerTrackExport)
            {
                CThread = new Thread(() => MIDIConversionTbT(Form, ThreadsPanel, OPath));
                Debug.PrintToConsole("ok", "CThread = MIDIConversionTbT");
            }
            else
            {
                CThread = new Thread(() => MIDIConversion(Form, ThreadsPanel, OPath));
                Debug.PrintToConsole("ok", "CThread = MIDIConversion");
            }

            CThread.IsBackground = true;
            CThread.Start();
            Debug.PrintToConsole("ok", "CThread started.");
        }

        public Boolean IsStillRendering() { return CThread.IsAlive; }
        public void RequestStop() { StopRequested = true; CTS.Cancel(); }
        public void ForceStop() { CThread.Abort(); }
        public String GetStatus() { return Status; }
        public string GetError() { return StError; }

        private void LoadSoundFonts()
        {
            foreach (SoundFont SF in Program.SFArray.List)
            {
                if (!SF.IsEnabled)
                {
                    Debug.PrintToConsole("ok", "SoundFont is disabled, there's no need to load it.");
                    continue;
                }

                BASS_MIDI_FONTEX TSF;
                Debug.PrintToConsole("ok", String.Format("Preparing BASS_MIDI_FONTEX for {0}...", SF.GetSoundFontPath));

                TSF.font = BassMidi.BASS_MIDI_FontInit(SF.GetSoundFontPath, SF.GetXGMode ? BASSFlag.BASS_MIDI_FONT_XGDRUMS : BASSFlag.BASS_DEFAULT);
                Debug.PrintToConsole("ok", String.Format("SoundFont handle initialized. Handle = {0:X8}", TSF.font));

                TSF.spreset = SF.GetSourcePreset;
                TSF.sbank = SF.GetSourceBank;
                TSF.dpreset = SF.GetDestinationPreset;
                TSF.dbank = SF.GetDestinationBank;
                TSF.dbanklsb = SF.GetDestinationBankLSB;
                Debug.PrintToConsole("ok",
                    String.Format(
                        "spreset = {0}, sbank = {1}, dpreset = {2}, dbank = {3}, dbanklsb = {4}, xg = {5}",
                        TSF.spreset, TSF.sbank, TSF.dpreset, TSF.dbank, TSF.dbanklsb, SF.GetXGMode
                        )
                    );

                if (TSF.font != 0)
                {
                    Program.SFArray.BMFEArray.Add(TSF);
                    Debug.PrintToConsole("ok", "SoundFont loaded and added to BASS_MIDI_FONTEX array.");
                }
                else Debug.PrintToConsole("err", String.Format("Could not load {0}. BASSERR: {1}", SF.GetSoundFontPath, Bass.BASS_ErrorGetCode()));
            }

            Debug.PrintToConsole("ok", "Reversing array...");
            Program.SFArray.BMFEArray.Reverse();
        }

        private void FreeSoundFonts()
        {
            Debug.PrintToConsole("ok", "Freeing SoundFont handles...");
            foreach (BASS_MIDI_FONTEX SF in Program.SFArray.BMFEArray)
                BassMidi.BASS_MIDI_FontFree(SF.font);

            Debug.PrintToConsole("ok", "Handles freed.");
            Program.SFArray.BMFEArray.Clear();
        }

        private void MIDIConversion(Control Form, Panel ThreadsPanel, String OPath)
        {
            try
            {
                Status = "prep";
                Int32 MT = Properties.Settings.Default.MultiThreadedMode ? Properties.Settings.Default.MultiThreadedLimitV : 1;
                WaveFormat WF = new WaveFormat(Properties.Settings.Default.Frequency, 32, 2, AudioEncoding.IeeeFloat);

                Debug.PrintToConsole("ok", "Initializing BASS...");
                if (!Bass.BASS_Init(0, WF.SampleRate, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                    throw new Exception("Unable to initialize BASS!");

                LoadSoundFonts();

                try
                {
                    Debug.PrintToConsole("ok", "Preparing Parallel.ForEach loop...");
                    CTS = new CancellationTokenSource();
                    ParallelOptions PO = new ParallelOptions { MaxDegreeOfParallelism = MT, CancellationToken = CTS.Token };
                    Debug.PrintToConsole("ok", String.Format("ParallelOptions prepared, MaxDegreeOfParallelism = {0}", MT));

                    Parallel.ForEach(Program.MIDIList, PO, (MFile, LS) =>
                    {
                        if (StopRequested)
                        {
                            Debug.PrintToConsole("ok", "Stop requested. Stopping Parallel.ForEach...");
                            LS.Stop();
                            return;
                        }

                        IWaveSource Stream;

                        // Begin conversion
                        Status = "sconv";
                        MDV.SetCurrentMIDI(MFile.GetPath);

                        // Prepare the filename
                        String OutputDir = String.Format("{0}\\{1}.{2}",
                            OPath, Path.GetFileNameWithoutExtension(MFile.GetName), Properties.Settings.Default.Codec);

                        // Check if file already exists
                        if (File.Exists(OutputDir))
                            OutputDir = String.Format("{0}\\{1} - {2}.{3}",
                                OPath, Path.GetFileNameWithoutExtension(MFile.GetName),
                                DateTime.Now.ToString("dd-MM-yyyy HHmmsstt"), Properties.Settings.Default.Codec);

                        Debug.PrintToConsole("ok", String.Format("Output file: {0}", OutputDir));

                        MIDIThreadStatus MIDIT = new MIDIThreadStatus(MFile.GetName);
                        MIDIT.Dock = DockStyle.Top;
                        ThreadsPanel.Invoke((MethodInvoker)delegate {
                            Debug.PrintToConsole("ok", "Added MIDIThreadStatus control for MIDI.");
                            ThreadsPanel.Controls.Add(MIDIT);
                        });

                        BASSMIDI BASS = new BASSMIDI(true, MFile.GetPath, WF);
                        if (!BASS.ErroredOut)
                        {
                            FileStream FOpen = File.Open(OutputDir, FileMode.Create);
                            WaveWriter Destination = new WaveWriter(FOpen, WF);
                            Debug.PrintToConsole("ok", "Output file is open.");

                            if (Properties.Settings.Default.LoudMax)
                            {
                                Debug.PrintToConsole("ok", "LoudMax enabled.");
                                AntiClipping BAC = new AntiClipping(BASS, 0.1);
                                Stream = BAC.ToWaveSource(32);
                            }
                            else Stream = BASS.ToWaveSource(32);

                            Int32 FRead = 0;
                            byte[] FBuffer = new byte[1024 * 16];

                            Debug.PrintToConsole("ok", String.Format("Thread for MIDI {0} is now rendering data...", OutputDir));
                            while ((FRead = Stream.Read(FBuffer, 0, FBuffer.Length)) != 0)
                            {
                                if (StopRequested)
                                    break;

                                MIDIT.Invoke((MethodInvoker)delegate {
                                    MIDIT.UpdatePB(Convert.ToInt32(Math.Round(BASS.Position * 100.0 / BASS.Length)));
                                });

                                Destination.Write(FBuffer, 0, FRead);
                            }
                            Debug.PrintToConsole("ok", String.Format("Thread for MIDI {0} is done rendering data.", OutputDir));

                            ThreadsPanel.Invoke((MethodInvoker)delegate {
                                Debug.PrintToConsole("ok", "Removed MIDIThreadStatus control for MIDI.");
                                ThreadsPanel.Controls.Remove(MIDIT);
                            });

                            if (!StopRequested) MDV.AddValidMIDI();

                            Destination.Dispose();
                            FOpen.Dispose();
                        }

                        if (BASS.ErroredOut)
                        {
                            Debug.PrintToConsole("err", String.Format("Unable to render {0}.", OutputDir));
                            File.Delete(OutputDir);
                        }

                        BASS.Dispose();

                        PO.CancellationToken.ThrowIfCancellationRequested();
                    });
                }
                catch (OperationCanceledException) { }
                finally { CTS.Dispose(); CTS = null; }

                FreeSoundFonts();

                Debug.PrintToConsole("ok", "BASS freed.");
                Bass.BASS_Free();
            }
            catch (Exception ex)
            {
                Status = "crsh";
                StError = String.Format("The converter encountered an error during the conversion process.\nError: {0}", ex.Message.ToString());
                IsCrash = true;

                Debug.PrintToConsole("err", String.Format("{0} - {1}", ex.InnerException.ToString(), ex.Message.ToString()));
            }

            if (!StopRequested && !IsCrash)
                Form.Invoke((MethodInvoker)delegate { ((Form)Form).Close(); });
        }

        private void MIDIConversionTbT(Control Form, Panel ThreadsPanel, String OPath)
        {
            try
            {
                Status = "prep";
                Int32 MT = Properties.Settings.Default.MultiThreadedMode ? Properties.Settings.Default.MultiThreadedLimitV : 1;
                WaveFormat WF = new WaveFormat(Properties.Settings.Default.Frequency, 32, 2, AudioEncoding.IeeeFloat);

                // Initialize BASS
                Debug.PrintToConsole("ok", "Initializing BASS...");
                if (!Bass.BASS_Init(0, WF.SampleRate, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                    throw new Exception("Unable to initialize BASS!");

                LoadSoundFonts();

                foreach (MIDI MFile in Program.MIDIList)
                {
                    MultiStreamMerger MSM = new MultiStreamMerger(WF);

                    if (StopRequested)
                    {
                        Debug.PrintToConsole("ok", "Stop requested. Stopping foreach loop...");
                        break;
                    }

                    MDV.SetTotalTracks(MFile.GetTracks);
                    MDV.ResetCurrentTrack();

                    // Begin conversion
                    Status = "mconv";

                    try
                    {
                        Debug.PrintToConsole("ok", "Preparing Parallel.For loop...");
                        CTS = new CancellationTokenSource();
                        ParallelOptions PO = new ParallelOptions { MaxDegreeOfParallelism = MT, CancellationToken = CTS.Token };
                        Debug.PrintToConsole("ok", String.Format("ParallelOptions prepared, MaxDegreeOfParallelism = {0}", MT));

                        Parallel.For(0, MFile.GetTracks, PO, (T, LS) =>
                        {
                            if (StopRequested)
                            {
                                Debug.PrintToConsole("ok", "Stop requested. Stopping Parallel.For...");
                                LS.Stop();
                                return;
                            }

                            TrackThreadStatus Trck = new TrackThreadStatus(T);
                            Trck.Dock = DockStyle.Top;
                            ThreadsPanel.Invoke((MethodInvoker)delegate {
                                Debug.PrintToConsole("ok", "Added TrackThreadStatus control for MIDI.");
                                ThreadsPanel.Controls.Add(Trck);
                            });

                            BASSMIDI BASS = new BASSMIDI(MFile.GetPath, T, WF);
                            String UniqueID = BASS.UniqueID;
                            if (!BASS.ErroredOut)
                            {
                                if (!Properties.Settings.Default.PerTrackSeparateFiles)
                                {
                                    ISampleWriter Writer = MSM.GetWriter();
                                    Int32 CRead = 0;
                                    float[] CBuffer = new float[2048];

                                    Debug.PrintToConsole("ok", String.Format("{0} - Thread for track {1} is now rendering data...", UniqueID, T));
                                    while ((CRead = BASS.Read(CBuffer, 0, CBuffer.Length)) != 0)
                                    {
                                        if (StopRequested)
                                            break;

                                        Trck.Invoke((MethodInvoker)delegate {
                                            Trck.UpdatePB(Convert.ToInt32(Math.Round(BASS.Position * 100.0 / BASS.Length)));
                                        });

                                        Writer.Write(CBuffer, 0, CRead);
                                    }
                                    Debug.PrintToConsole("ok", String.Format("{0} - Thread for track {1} is done rendering data.", UniqueID, T));
                                }
                                else
                                {
                                    // Each track needs its own wave source
                                    IWaveSource AStream;

                                    // Check if we need to export each track to a file
                                    String Folder = OPath;
                                    if (Properties.Settings.Default.PerTrackSeparateFiles)
                                    {
                                        // We do, create folder
                                        Folder += String.Format("\\{0}\\", Path.GetFileNameWithoutExtension(MFile.GetName));

                                        if (!Directory.Exists(Folder))
                                            Directory.CreateDirectory(Folder);
                                    }
                                    else Folder += " ";

                                    // Prepare the filename
                                    String SOutputDir = String.Format("{0}Track {1}.{2}",
                                        Folder, T, Properties.Settings.Default.Codec);

                                    // Check if file already exists
                                    if (File.Exists(SOutputDir))
                                        SOutputDir = String.Format("{0}Track {1} - {2}.{3}",
                                            Folder, T, DateTime.Now.ToString("dd-MM-yyyy HHmmsstt"), Properties.Settings.Default.Codec);

                                    Debug.PrintToConsole("ok", String.Format("{0} - Output file: {1}", UniqueID, SOutputDir));

                                    FileStream SFOpen = File.Open(SOutputDir, FileMode.Create);
                                    WaveWriter SDestination = new WaveWriter(SFOpen, WF);
                                    Debug.PrintToConsole("ok", String.Format("{0} - Output file is open.", UniqueID));

                                    if (Properties.Settings.Default.LoudMax)
                                    {
                                        Debug.PrintToConsole("ok", String.Format("{0} - LoudMax enabled.", UniqueID));
                                        AntiClipping BAC = new AntiClipping(BASS, 0.1);
                                        AStream = BAC.ToWaveSource(32);
                                    }
                                    else AStream = BASS.ToWaveSource(32);

                                    Int32 SFRead = 0;
                                    byte[] SFBuffer = new byte[1024 * 16];

                                    Debug.PrintToConsole("ok", String.Format("{0} - Thread for track {1} is now rendering data...", UniqueID, T));
                                    while ((SFRead = AStream.Read(SFBuffer, 0, SFBuffer.Length)) != 0)
                                    {
                                        if (StopRequested)
                                            break;

                                        Trck.Invoke((MethodInvoker)delegate {
                                            Trck.UpdatePB(Convert.ToInt32(Math.Round(BASS.Position * 100.0 / BASS.Length)));
                                        });

                                        SDestination.Write(SFBuffer, 0, SFRead);
                                    }
                                    Debug.PrintToConsole("ok", String.Format("{0} - Thread for track {1} is done rendering data.", UniqueID, T));

                                    SDestination.Dispose();
                                    SFOpen.Dispose();
                                }

                                ThreadsPanel.Invoke((MethodInvoker)delegate {
                                    Debug.PrintToConsole("ok", String.Format("{0} - Removed TrackThreadStatus control for MIDI.", UniqueID));
                                    ThreadsPanel.Controls.Remove(Trck);
                                });

                                if (!StopRequested) MDV.AddTrack();
                            }

                            BASS.Dispose();

                            PO.CancellationToken.ThrowIfCancellationRequested();
                        });
                    }
                    catch (OperationCanceledException) { }
                    finally { CTS.Dispose(); CTS = null; }

                    if (StopRequested)
                        break;
                    else MDV.AddValidMIDI();

                    // Time to save the file
                    String OutputDir = String.Format("{0}\\{1}.{2}",
                        OPath, Path.GetFileNameWithoutExtension(MFile.GetName), Properties.Settings.Default.Codec);

                    // Check if file already exists
                    if (File.Exists(OutputDir))
                        OutputDir = String.Format("{0}\\{1} - {2}.{3}",
                            OPath, Path.GetFileNameWithoutExtension(MFile.GetName),
                            DateTime.Now.ToString("dd-MM-yyyy HHmmsstt"), Properties.Settings.Default.Codec);

                    Debug.PrintToConsole("ok", String.Format("Output file: {0}", OutputDir));

                    // Reset MSM position
                    MSM.Position = 0;

                    // Prepare wave source
                    IWaveSource MStream;
                    if (Properties.Settings.Default.LoudMax)
                    {
                        Debug.PrintToConsole("ok", "LoudMax enabled.");
                        AntiClipping BAC = new AntiClipping(MSM, 0.1);
                        MStream = BAC.ToWaveSource(32);
                    }
                    else MStream = MSM.ToWaveSource(32);

                    FileStream FOpen = File.Open(OutputDir, FileMode.Create);
                    WaveWriter Destination = new WaveWriter(FOpen, WF);
                    Debug.PrintToConsole("ok", "Output file is open.");

                    Int32 FRead = 0;
                    byte[] FBuffer = new byte[1024 * 16];

                    Status = "aout";
                    Debug.PrintToConsole("ok", String.Format("Writing data for {0} to disk...", OutputDir));
                    while ((FRead = MStream.Read(FBuffer, 0, FBuffer.Length)) != 0)
                        Destination.Write(FBuffer, 0, FRead);
                    Debug.PrintToConsole("ok", String.Format("Done writing {0}.", OutputDir));

                    Destination.Dispose();
                    FOpen.Dispose();
                }

                FreeSoundFonts();

                Debug.PrintToConsole("ok", "BASS freed.");
                Bass.BASS_Free();
            }
            catch (Exception ex)
            {
                Status = "crsh";
                StError = String.Format("The converter encountered an error during the conversion process.\nError: {0}", ex.Message.ToString());
                IsCrash = true;

                Debug.PrintToConsole("err", String.Format("{0} - {1}", ex.InnerException.ToString(), ex.Message.ToString()));
            }

            if (!StopRequested && !IsCrash)
                Form.Invoke((MethodInvoker)delegate { ((Form)Form).Close(); });
        }
    }
}
