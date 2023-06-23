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
using ManagedBass;
using System.Xml;

namespace OmniConverter
{
    class MIDIValidation
    {
        private String CurrentMIDI;
        private UInt64 ValidMIDIs;
        private UInt64 InvalidMIDIs;
        private UInt64 TotalMIDIsCount;

        private int Tracks = 0;
        private int CurrentTrack = 0;

        public MIDIValidation()
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

        public void SetTotalTracks(int T) { Tracks = T; }
        public void AddTrack() { CurrentTrack++; }
        public void ResetCurrentTrack() { CurrentTrack = 0; }
        public int GetTotalTracks() { return Tracks; }
        public int GetCurrentTrack() { return CurrentTrack; }
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

        public void Convert(ISampleWriter output, CSCore.WaveFormat format, CancellationToken cancel)
        {
            BASSMIDI bass;

            Debug.PrintToConsole("ok", $"Initializing BASSMIDI for thread...");

            using (bass = new BASSMIDI(format))
            {
                ISampleSource bassSource;
                float[] buffer = new float[2048 * 16];
                long prevWriteTime = 0;
                double time = 0;
                int read;

                /*if (loudmax) bassSource = new AntiClipping(bass, 0.1);
                else bassSource = bass;*/
                bassSource = bass; //Why the hell was it running loudmax twice lol
                Debug.PrintToConsole("ok", $"Initialized {bass.UniqueID}.");

                // Prepare stream
                if (Properties.Settings.Default.RVOverrideToggle)
                {
                    for (int i = 0; i <= 15; i++)
                    {
                        bass.SendReverbEvent(i, Properties.Settings.Default.ReverbValue);
                        bass.SendChorusEvent(i, Properties.Settings.Default.ChorusValue);
                    }
                }

                try
                {
                    foreach (MIDIEvent e in events)
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
                            var ev = e as ControlChangeEvent;

                            if (Properties.Settings.Default.RVOverrideToggle && (ev.Controller == 0x5B || ev.Controller == 0x5D))
                                continue;

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
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    if (bass != null)
                        Debug.PrintToConsole("wrn", $"{bass.UniqueID} - DataParsingError {ex}");
                }

                bass.SendEndEvent();
                while ((read = bassSource.Read(buffer, 0, buffer.Length)) != 0)
                {
                    output.Write(buffer, 0, read);
                }
            }
        }
    }

    class Converter
    {
        public CancellationTokenSource CTS;
        public MIDIValidation MDV;

        private String Status = "prep";
        private String StError = "";
        private Boolean StopRequested = false;
        private Boolean IsCrash = false;

        private Thread CThread;

        public Converter(Control Form, Panel ThreadsPanel, String OPath)
        {
            MDV = new MIDIValidation();

            CThread = new Thread(() => MIDIConversion(Form, ThreadsPanel, OPath));
            Debug.PrintToConsole("ok", "CThread allocated.");

            CThread.IsBackground = true;
            CThread.Start();
            Debug.PrintToConsole("ok", "CThread started.");
        }

        public Boolean IsStillRendering() { return CThread.IsAlive; }
        public void RequestStop() { StopRequested = true; CTS.Cancel(); }
        public String GetStatus() { return Status; }
        public string GetError() { return StError; }

        private void PerMIDIConv(int MT, CSCore.WaveFormat WF, Panel ThreadsPanel, string OPath)
        {
            try
            {
                Debug.PrintToConsole("ok", "Preparing Parallel.ForEach loop...");
                CTS = new CancellationTokenSource();
                ParallelOptions PO = new ParallelOptions { MaxDegreeOfParallelism = MT, CancellationToken = CTS.Token };
                Debug.PrintToConsole("ok", String.Format("ParallelOptions prepared, MaxDegreeOfParallelism = {0}", MT));
                ParallelFor(0, Program.MIDIList.Count, MT, CTS.Token, T =>
                {
                    if (StopRequested)
                    {
                        Debug.PrintToConsole("ok", "Stop requested. Stopping ParallelFor...");
                        throw new OperationCanceledException();
                    }

                    MIDI MFile = Program.MIDIList[T];

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

                    TaskStatus MIDIT = new TaskStatus(MFile.Name);
                    MIDIT.Dock = DockStyle.Top;
                    ThreadsPanel.Invoke((MethodInvoker)delegate { ThreadsPanel.Controls.Add(MIDIT); });

                    ConvertWorker Worker = new ConvertWorker(MFile.GetFullMIDITimeBased(), MFile.TimeLength.TotalSeconds);

                    Stream FOpen = new BufferedStream(File.Open(OutputDir, FileMode.Create), 65536);
                    WaveWriter Destination = new WaveWriter(FOpen, WF);
                    ISampleWriter Writer = new WaveSampleWriter(Destination);
                    Debug.PrintToConsole("ok", "Output file is open.");

                    var ConvThread = Task.Run(() => Worker.Convert(Writer, WF, PO.CancellationToken));

                    int ov = 0;
                    while (!ConvThread.IsCompleted)
                    {
                        var v = Convert.ToInt32(Math.Round(Worker.Progress * 100));

                        if (StopRequested)
                            break;

                        if (ov != v)
                        {
                            ov = v;

                            MIDIT.UpdateTitle($"{v}%");
                            MIDIT.UpdatePB(v);
                        }
                    }

                    ConvThread.Wait();
                    ConvThread.Dispose();

                    Debug.PrintToConsole("ok", String.Format("Thread for MIDI {0} is done rendering data.", OutputDir));

                    ThreadsPanel.Invoke((MethodInvoker)delegate { MIDIT.Dispose(); });

                    if (!StopRequested) MDV.AddValidMIDI();

                    if (Destination != null) Destination.Dispose();
                    if (FOpen != null) FOpen.Dispose();
                });
            }
            catch (Exception ex)
            {
                Debug.PrintToConsole("err", String.Format("{0} - {1}", ex.InnerException.ToString(), ex.Message.ToString()));
            }
            finally { CTS.Dispose(); CTS = null; }
        }

        private void PerTrackConv(int MT, CSCore.WaveFormat WF, Panel ThreadsPanel, string OPath)
        {
            foreach (MIDI MFile in Program.MIDIList)
            {
                if (StopRequested)
                {
                    Debug.PrintToConsole("ok", "Stop requested. Stopping PerTrackConv...");
                    break;
                }

                MultiStreamMerger MSM = new MultiStreamMerger(WF);
                int t = 0;

                MDV.SetTotalTracks(MFile.Tracks);
                MDV.ResetCurrentTrack();

                // Begin conversion
                Status = "mconv";

                try
                {
                    CTS = new CancellationTokenSource();
                    Debug.PrintToConsole("ok", $"PerTrackConv => MaxDegreeOfParallelism = {MT}");

                    ParallelFor(0, MFile.Tracks, MT, CTS.Token, T =>
                    {
                        if (StopRequested)
                        {
                            Debug.PrintToConsole("ok", "Stop requested. Stopping ParallelFor...");
                            throw new OperationCanceledException();
                        }

                        if (MFile.NoteCount >= 0)
                        {
                            TaskStatus Trck = new TaskStatus($"Track {T}");
                            Trck.Dock = DockStyle.Top;
                            ThreadsPanel.Invoke((MethodInvoker)delegate { ThreadsPanel.Controls.Add(Trck); });

                            ConvertWorker Worker = new ConvertWorker(MFile.GetSingleTrackTimeBased(T), MFile.TimeLength.TotalSeconds);
                            ISampleWriter Writer;
                            WaveWriter SDestination = null;
                            FileStream SFOpen = null;
                            Debug.PrintToConsole("ok", $"ConvertWorker => T{T}, {MFile.TimeLength.TotalSeconds}");

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

                            var ConvThread = Task.Run(() => Worker.Convert(Writer, WF, CTS.Token));
                            Debug.PrintToConsole("ok", $"ConvThread started for T{T}");

                            int ov = 0;
                            while (!ConvThread.IsCompleted)
                            {
                                var v = Convert.ToInt32(Math.Round(Worker.Progress * 100));

                                if (StopRequested)
                                    break;

                                if (ov != v)
                                {
                                    ov = v;

                                    Trck.UpdateTitle($"{v}%");
                                    Trck.UpdatePB(v);
                                }
                            }

                            ConvThread.Wait();
                            ConvThread.Dispose();

                            if (Writer != null)
                            {
                                if (Writer.GetType() == typeof(MultiStreamMerger))
                                    ((MultiStreamMerger)Writer).Dispose();
                                else if (Writer.GetType() == typeof(WaveSampleWriter))
                                    ((WaveSampleWriter)Writer).Dispose();
                            }

                            ThreadsPanel.Invoke((MethodInvoker)delegate { Trck.Dispose(); });

                            if (SDestination != null) SDestination.Dispose();
                            if (SFOpen != null) SFOpen.Dispose();

                            if (!StopRequested) MDV.AddTrack();
                        }
                    });
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Debug.PrintToConsole("err", String.Format("{0} - {1}", ex.InnerException.ToString(), ex.Message.ToString()));
                }
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
                        DateTime.Now.ToString("yyyyMMdd HHmmsstt"), Properties.Settings.Default.Codec);

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
        }

        private void MIDIConversion(Control Form, Panel ThreadsPanel, string OPath)
        {
            var MT = Properties.Settings.Default.MultiThreadedMode ? Properties.Settings.Default.MultiThreadedLimitV : 1;
            var WF = new CSCore.WaveFormat(Properties.Settings.Default.Frequency, 32, 2, AudioEncoding.IeeeFloat);

            Status = "prep";

            Debug.PrintToConsole("ok", "Initializing BASS...");
          
            if (!Bass.Init(Bass.NoSoundDevice, WF.SampleRate, DeviceInitFlags.Default))
                throw new Exception("Unable to initialize BASS!");

            Debug.PrintToConsole("ok", $"BASS initialized. (ERR: {Bass.LastError})");

            try
            {
                if (Properties.Settings.Default.PerTrackExport)
                    PerTrackConv(MT, WF, ThreadsPanel, OPath);
                else
                    PerMIDIConv(MT, WF, ThreadsPanel, OPath);

                Debug.PrintToConsole("ok", "BASS freed.");
                Bass.Free();
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
                if (tasks.Count > threads) TryTake();
            }

            while (completed.Count > 0 || tasks.Count > 0) TryTake();
        }
    }
}
