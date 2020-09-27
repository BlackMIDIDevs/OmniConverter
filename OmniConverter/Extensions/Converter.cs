using CSCore;
using CSCore.Codecs.WAV;
using MIDIModificationFramework;
using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

    class WaveSampleWriter : ISampleWriter, IDisposable
    {
        WaveWriter writer;

        public WaveSampleWriter(WaveWriter writer)
        {
            this.writer = writer;
        }

        public int Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            writer.Dispose();
        }

        public void Write(float[] buffer, int offset, int count)
        {
            writer.WriteSamples(buffer, offset, count);
        }

        public unsafe void Write(float* buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    class ConvertWorker
    {
        IEnumerable<MIDIEvent> events;
        double length;
        double converted;

        public double Progress => converted / length;

        public ConvertWorker(IEnumerable<MIDIEvent> events, double length)
        {
            this.events = events;
            this.length = length;
        }

        public void Convert(ISampleWriter output, WaveFormat format, bool loudmax, CancellationToken cancel)
        {
            try
            {
                using (var bass = new BASSMIDI(format))
                {
                    ISampleSource bassSource;
                    /*if (loudmax) bassSource = new AntiClipping(bass, 0.1);
                    else bassSource = bass;*/
                    bassSource = bass; //Why the hell was it running loudmax twice lol
                    float[] buffer = new float[2048 * 16];
                    long prevWriteTime = 0;
                    double time = 0;
                    int read;
                    foreach (var e in events)
                    {
                        cancel.ThrowIfCancellationRequested();

                        time += e.DeltaTime;
                        converted = time;
                        var writeTime = (long)(time * format.SampleRate);
                        var offset = (int)((writeTime - prevWriteTime) * 2);
                        prevWriteTime = writeTime;

                        while (offset > 0)
                        {
                            if (offset < buffer.Length)
                            {
                                bassSource.Read(buffer, 0, offset);
                                output.Write(buffer, 0, offset);
                                offset = 0;
                            }
                            else
                            {
                                bassSource.Read(buffer, 0, buffer.Length);
                                output.Write(buffer, 0, buffer.Length);
                                offset -= buffer.Length;
                            }
                        }

                        if (e is NoteOnEvent)
                        {
                            var ev = e as NoteOnEvent;
                            bass.SendEventRaw((uint)(ev.Channel | 0x90 | (ev.Key << 8) | (ev.Velocity << 16)), 0);
                        }
                        else if (e is NoteOffEvent)
                        {
                            var ev = e as NoteOffEvent;
                            bass.SendEventRaw((uint)(ev.Channel | 0x80 | (ev.Key << 8)), 0);
                        }
                        else if (e is PolyphonicKeyPressureEvent)
                        {
                            var ev = e as PolyphonicKeyPressureEvent;
                            bass.SendEventRaw((uint)(ev.Channel | 0xA0 | (ev.Key << 8) | (ev.Velocity << 16)), 0);
                        }
                        else if (e is ControlChangeEvent)
                        {
                            if(Properties.Settings.Default.RVOverrideToggle)
                            {
                                for(int i = 0; i <= 15; i++)
                                {
                                    bass.SendReverbEvent(i, Properties.Settings.Default.ReverbValue);
                                    bass.SendChorusEvent(i, Properties.Settings.Default.ChorusValue);
                                }
                            }
                            
                            var ev = e as ControlChangeEvent;
                            bass.SendEventRaw((uint)(0xB0 | (ev.Controller << 8) | (ev.Value << 16)), ev.Channel + 1);
                        }
                        else if (e is ProgramChangeEvent)
                        {
                            var ev = e as ProgramChangeEvent;
                            bass.SendEventRaw((uint)(ev.Channel | 0xC0 | (ev.Program << 8)), 0);
                        }
                        else if (e is ChannelPressureEvent)
                        {
                            var ev = e as ChannelPressureEvent;
                            bass.SendEventRaw((uint)(ev.Channel | 0xD0 | (ev.Pressure << 8)), 0);
                        }
                        else if (e is PitchWheelChangeEvent)
                        {
                            var ev = e as PitchWheelChangeEvent;
                            var val = ev.Value + 8192;
                            bass.SendEventRaw((uint)(ev.Channel | 0xE0 | ((val & 0x7F) << 8) | (((val >> 7) & 0x7F) << 16)), 0);
                        }
                    }
                    bass.SendEndEvent();
                    while ((read = bassSource.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        output.Write(buffer, 0, read);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }
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
                        MDV.SetCurrentMIDI(MFile.Path);

                        // Prepare the filename
                        String OutputDir = String.Format("{0}\\{1}.{2}",
                            OPath, Path.GetFileNameWithoutExtension(MFile.Name), Properties.Settings.Default.Codec);

                        // Check if file already exists
                        if (File.Exists(OutputDir))
                            OutputDir = String.Format("{0}\\{1} - {2}.{3}",
                                OPath, Path.GetFileNameWithoutExtension(MFile.Name),
                                DateTime.Now.ToString("dd-MM-yyyy HHmmsstt"), Properties.Settings.Default.Codec);

                        Debug.PrintToConsole("ok", String.Format("Output file: {0}", OutputDir));

                        MIDIThreadStatus MIDIT = new MIDIThreadStatus(MFile.Name);
                        MIDIT.Dock = DockStyle.Top;
                        ThreadsPanel.Invoke((MethodInvoker)delegate
                        {
                            Debug.PrintToConsole("ok", "Added MIDIThreadStatus control for MIDI.");
                            ThreadsPanel.Controls.Add(MIDIT);
                        });

                        ConvertWorker Worker = new ConvertWorker(MFile.GetFullMIDITimeBased(), MFile.TimeLength.TotalSeconds);

                        Stream FOpen = new BufferedStream(File.Open(OutputDir, FileMode.Create), 65536);
                        WaveWriter Destination = new WaveWriter(FOpen, WF);
                        ISampleWriter Writer = new WaveSampleWriter(Destination);
                        Debug.PrintToConsole("ok", "Output file is open.");

                        Task ConvThread = Task.Run(() =>
                        {
                            Worker.Convert(Writer, WF, Properties.Settings.Default.LoudMax, PO.CancellationToken);
                        });

                        while (!ConvThread.IsCompleted)
                        {
                            if (StopRequested)
                                break;

                            MIDIT.Invoke((MethodInvoker)delegate
                            {
                                MIDIT.UpdatePB(Convert.ToInt32(Math.Round(Worker.Progress * 100)));
                            });

                            Thread.Sleep(200);
                        }

                        ConvThread.Wait();

                        Debug.PrintToConsole("ok", String.Format("Thread for MIDI {0} is done rendering data.", OutputDir));

                        ThreadsPanel.Invoke((MethodInvoker)delegate
                        {
                            Debug.PrintToConsole("ok", "Removed MIDIThreadStatus control for MIDI.");
                            ThreadsPanel.Controls.Remove(MIDIT);
                        });

                        if (!StopRequested) MDV.AddValidMIDI();

                        Destination.Dispose();
                        FOpen.Dispose();

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

                    MDV.SetTotalTracks(MFile.Tracks);
                    MDV.ResetCurrentTrack();

                    // Begin conversion
                    Status = "mconv";

                    try
                    {
                        Debug.PrintToConsole("ok", "Preparing Parallel.For loop...");
                        CTS = new CancellationTokenSource();
                        ParallelOptions PO = new ParallelOptions { MaxDegreeOfParallelism = MT, CancellationToken = CTS.Token };
                        Debug.PrintToConsole("ok", String.Format("ParallelOptions prepared, MaxDegreeOfParallelism = {0}", MT));

                        ParallelFor(0, MFile.Tracks, Environment.ProcessorCount, new CancellationToken(false), T =>
                        {
                            if (StopRequested)
                            {
                                Debug.PrintToConsole("ok", "Stop requested. Stopping Parallel.For...");
                                //LS.Stop();
                                return;
                            }

                            TrackThreadStatus Trck = new TrackThreadStatus(T);
                            Trck.Dock = DockStyle.Top;
                            ThreadsPanel.Invoke((MethodInvoker)delegate
                            {
                                Debug.PrintToConsole("ok", "Added TrackThreadStatus control for MIDI.");
                                ThreadsPanel.Controls.Add(Trck);
                            });

                            ConvertWorker Worker = new ConvertWorker(MFile.GetSingleTrackTimeBased(T), MFile.TimeLength.TotalSeconds);
                            ISampleWriter Writer;
                            WaveWriter SDestination = null;
                            FileStream SFOpen = null;
                            if (Properties.Settings.Default.PerTrackSeparateFiles)
                            {
                                // Check if we need to export each track to a file
                                String Folder = OPath;
                                if (Properties.Settings.Default.PerTrackSeparateFiles)
                                {
                                    // We do, create folder
                                    Folder += String.Format("\\{0}\\", Path.GetFileNameWithoutExtension(MFile.Name));

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

                                Debug.PrintToConsole("ok", String.Format("{0} - Output file: {1}", T, SOutputDir));

                                SFOpen = File.Open(SOutputDir, FileMode.Create);
                                SDestination = new WaveWriter(SFOpen, WF);
                                Writer = new WaveSampleWriter(SDestination);
                            }
                            else Writer = MSM.GetWriter();

                            Task ConvThread = Task.Run(() =>
                            {
                                Worker.Convert(Writer, WF, false, PO.CancellationToken);
                            });

                            while (!ConvThread.IsCompleted)
                            {
                                if (StopRequested)
                                    break;

                                Trck.Invoke((MethodInvoker)delegate
                                {
                                    Trck.UpdatePB(Convert.ToInt32(Math.Round(Worker.Progress * 100)));
                                });

                                Thread.Sleep(200);
                            }

                            ConvThread.Wait();

                            if (SDestination != null) SDestination.Dispose();
                            if (SFOpen != null) SFOpen.Dispose();

                            ThreadsPanel.Invoke((MethodInvoker)delegate
                            {
                                Debug.PrintToConsole("ok", String.Format("{0} - Removed TrackThreadStatus control for MIDI.", T));
                                ThreadsPanel.Controls.Remove(Trck);
                            });

                            if (!StopRequested) MDV.AddTrack();

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
                        OPath, Path.GetFileNameWithoutExtension(MFile.Name), Properties.Settings.Default.Codec);

                    // Check if file already exists
                    if (File.Exists(OutputDir))
                        OutputDir = String.Format("{0}\\{1} - {2}.{3}",
                            OPath, Path.GetFileNameWithoutExtension(MFile.Name),
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

        static void ParallelFor(int from, int to, int threads, CancellationToken cancel, Action<int> func)
        {
            Dictionary<int, Task> tasks = new Dictionary<int, Task>();
            BlockingCollection<int> completed = new BlockingCollection<int>();

            void RunTask(int i)
            {
                var t = new Task(() =>
                {
                    try
                    {
                        func(i);
                        completed.Add(i);
                    }
                    catch (Exception e) { }
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
                if (tasks.Count > threads) TryTake();
            }

            while (completed.Count > 0 || tasks.Count > 0) TryTake();
        }
    }
}
