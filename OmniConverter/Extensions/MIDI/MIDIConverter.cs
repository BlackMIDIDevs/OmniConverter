using Avalonia.Controls;
using Avalonia.Threading;
using CSCore;
using CSCore.Codecs.WAV;
using MIDIModificationFramework;
using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OmniConverter
{
    public class MIDIConverter : MIDIWorker
    {
        public enum ConvStatus
        {
            Dead = -2,
            Error,
            Idle,
            Prep,
            SingleConv,
            MultiConv,
            AudioOut,
            Crash
        }

        private AudioEngine _audioRenderer;
        private MIDIValidator _validator;
        private CancellationTokenSource _cancToken;
        private ParallelOptions _parallelOptions;
        private Thread? _converterThread;

        private Window _winRef;
        private StackPanel _panelRef;
        private ObservableCollection<MIDI> _midis;

        private string _curStatus = string.Empty;
        private double _progress = 0;
        private double _tracksProgress = 0;
        private string _outputPath = string.Empty;

        public MIDIConverter(string outputPath, int threads, Window winRef, StackPanel panel, ObservableCollection<MIDI> midis)
        {
            _winRef = winRef;
            _panelRef = panel;
            _midis = midis;
            _outputPath = outputPath;

            _cancToken = new CancellationTokenSource();
            _validator = new MIDIValidator((ulong)_midis.Count);

            _parallelOptions = new ParallelOptions { 
                MaxDegreeOfParallelism = Program.Settings.MultiThreadedMode ? threads.LimitToRange(1, Environment.ProcessorCount) : 1, 
                CancellationToken = _cancToken.Token
            };
        }

        public override void Dispose()
        {
            _cancToken?.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public override bool StartWork()
        {
            if (Program.Settings.MaxVoices >= 10000000)
            {
                var biggestMidi = _midis.MaxBy(x => x.Tracks);

                var v = Program.Settings.MaxVoices;
                var mem = (ulong)((Program.Settings.MaxVoices * 312) * biggestMidi.Tracks);
                var memusage = MiscFunctions.BytesToHumanReadableSize(mem);

                var re = MessageBox.Show(
                    $"You set your voice limit to {v}.\nThis will require {memusage} of physical memory.\n\nAre you sure you want to continue?",
                    "OmniConverter - Warning", _winRef, MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Warning);
                switch (re)
                {
                    case MessageBox.MsgBoxResult.No:
                        return false;

                    default:
                        break;
                }
            }

            if (Program.SoundFontsManager.GetSoundFontsCount() < 1)
            {
                MessageBox.Show("You need to add SoundFonts to the SoundFonts list to start the conversion process.", "OmniConverter - Error", _winRef, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                return false;
            }

            _converterThread = new Thread(ConversionFunc);
            _converterThread.IsBackground = true;
            _converterThread.Start();
            return true;
        }

        public override void CancelWork()
        {
            _cancToken.Cancel();
        }

        public override string GetStatus() => _curStatus;
        public override double GetProgress() => _progress;
        public double GetTracksProgress() => _tracksProgress;

        public override bool IsRunning() => _converterThread != null ? _converterThread.IsAlive : false;

        private void ChangeInfo(string newStatus, double newProgress = 0)
        {
            _curStatus = newStatus;
            _progress = newProgress;
        }

        private void AutoFillInfo(ConvStatus intStatus = ConvStatus.Idle)
        {
            int _tracks = _validator.GetTotalTracks();
            int _curTrack = _validator.GetCurrentTrack();

            ulong _valid = _validator.GetValidMIDIsCount();
            ulong _nonvalid = _validator.GetInvalidMIDIsCount();
            ulong _total = _validator.GetTotalMIDIsCount();

            switch (intStatus)
            {
                case ConvStatus.Prep:
                    _curStatus = "The converter is preparing itself for the conversion process...\n\nPlease wait.";
                    _progress = 0;
                    _tracksProgress = 0;
                    break;

                case ConvStatus.Dead:
                    _curStatus = "The MIDI converter system died.\n\nOops!";
                    _progress = 0;
                    _tracksProgress = 0;
                    break;

                case ConvStatus.SingleConv:
                    _curStatus = string.Format("{0} file(s) out of {1} have been converted.\n\nPlease wait...",
                                (_valid + _nonvalid).ToString("N0", new CultureInfo("is-IS")),
                                _total.ToString("N0", new CultureInfo("is-IS")));

                    _progress = Math.Round((_valid + _nonvalid) * 100.0 / _total);
                    break;

                case ConvStatus.MultiConv:
                    _curStatus = string.Format("{0} file(s) out of {1} have been converted.\nRendered {2} track(s) out of {3}.\nPlease wait...",
                                (_valid + _nonvalid).ToString("N0", new CultureInfo("is-IS")),
                                _total.ToString("N0", new CultureInfo("is-IS")),
                                _curTrack.ToString("N0", new CultureInfo("is-IS")), _tracks.ToString("N0", new CultureInfo("is-IS")));
                    _progress = Math.Round((_valid + _nonvalid) * 100.0 / _total);
                    _tracksProgress = Math.Round(_curTrack * 100.0 / _tracks);
                    break;

                case ConvStatus.AudioOut:
                    _curStatus = "Writing final audio file to disk.\n\nPlease do not turn off the computer...";
                    _progress = Math.Round((_valid + _nonvalid) * 100.0 / _total);
                    _tracksProgress = Math.Round(_curTrack * 100.0 / _tracks);
                    break;

                default:
                    break;
            }
        }

        private void ConversionFunc()
        {
            try
            {
                ChangeInfo("Initializing BASS...\n\nPlease wait.");
                var waveFormat = new CSCore.WaveFormat(Program.Settings.SampleRate, 32, 2, AudioEncoding.IeeeFloat);

                // later I will add support for more engines
                switch (Program.Settings.Renderer)
                {
                    case EngineID.BASS:
                    default:
                        _audioRenderer = new BASS(waveFormat, Program.Settings.MaxVoices, Program.Settings.SoundFontsList);

                        // do this hacky crap to get the voice change to work
                        _audioRenderer.Dispose();
                        _audioRenderer = new BASS(waveFormat, Program.Settings.MaxVoices, Program.Settings.SoundFontsList);
                        break;
                }

                if (_audioRenderer.Initialized)
                {
                    if (Program.Settings.PerTrackMode)
                    {
                        Dispatcher.UIThread.Post(() => ((MIDIWindow)_winRef).EnableTrackProgress(true));
                        PerTrackConversion(waveFormat);
                    }
                    else PerMIDIConversion(waveFormat);
                }
                else throw new Exception("Unable to initialize audio renderer!");
            }
            catch (Exception ex)
            {
                Debug.PrintToConsole(Debug.LogType.Error, ex.ToString());
                ChangeInfo($"An error has occurred while starting the conversion process!!!\n\nError: {ex.Message}");
            }
            finally
            {
                if (_audioRenderer.Initialized)
                    _audioRenderer.Dispose();
            }
        }

        private void PerMIDIConversion(CSCore.WaveFormat waveFormat)
        {
            // Cache settings
            var audioLimiter = Program.Settings.AudioLimiter;
            var codec = Program.Settings.Codec;

            AutoFillInfo(ConvStatus.Prep);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.For(0, _midis.Count, _parallelOptions, nMidi =>
            {
                try
                {

                    if (_cancToken.IsCancellationRequested)
                    {
                        Debug.PrintToConsole(Debug.LogType.Message, "Stop requested. Stopping ParallelFor...");
                        throw new OperationCanceledException();
                    }

                    var midi = _midis[nMidi];

                    // Begin conversion
                    AutoFillInfo(ConvStatus.SingleConv);
                    _validator.SetCurrentMIDI(midi.Path);

                    // Prepare the filename
                    string outputFile = string.Format("{0}/{1}.{2}",
                        _outputPath, Path.GetFileNameWithoutExtension(midi.Name), codec);

                    // Check if file already exists
                    if (File.Exists(outputFile))
                        outputFile = string.Format("{0}/{1} - {2}.{3}",
                            _outputPath, Path.GetFileNameWithoutExtension(midi.Name),
                            DateTime.Now.ToString("dd-MM-yyyy HHmmsstt"), codec);

                    Debug.PrintToConsole(Debug.LogType.Message, String.Format("Output file: {0}", outputFile));

                    TaskStatus? midiPanel = null;

                    Dispatcher.UIThread.Post(() => midiPanel = new TaskStatus(midi.Name, _panelRef));

                    IEnumerable<MIDIEvent> evs = [];
                    try
                    {
                        evs = midi.GetFullMIDITimeBased();
                    }
                    catch (Exception ex)
                    {
                        Debug.PrintToConsole(Debug.LogType.Error, $"{ex.Message}");
                    }

                    if (evs.Count() > 0)
                    {
                        var eventsProcesser = new EventsProcesser(_audioRenderer, evs, midi.Length.TotalSeconds);

                        // Initialize memory stream
                        var msm = new MultiStreamMerger(waveFormat);
                        var sampleWriter = msm.GetWriter();

                        var cvThread = Task.Run(() => eventsProcesser.Process(sampleWriter, waveFormat, _cancToken.Token));

                        double cachedPerc = 0;
                        while (!cvThread.IsCompleted)
                        {
                            var perc = Math.Round(eventsProcesser.Progress * 100.0);

                            if (_cancToken.IsCancellationRequested)
                                break;

                            midiPanel?.UpdateTitle(eventsProcesser);
                            //midiPanel?.UpdateRemainingTime(midiRenderer);

                            if (cachedPerc != perc)
                            {
                                midiPanel?.UpdateProgress(cachedPerc = perc);
                                AutoFillInfo(ConvStatus.SingleConv);
                            }
                        }

                        midiPanel?.UpdateProgress(100.0);
                        AutoFillInfo(ConvStatus.SingleConv);

                        if (cvThread != null)
                        {
                            cvThread.Wait();
                            cvThread.Dispose();
                        }

                        eventsProcesser.Dispose();

                        Debug.PrintToConsole(Debug.LogType.Message, String.Format("Thread for MIDI {0} is done rendering data.", outputFile));

                        Dispatcher.UIThread.Post(() => midiPanel?.Dispose());

                        if (!_cancToken.IsCancellationRequested) _validator.AddValidMIDI();

                        // Reset MSM position
                        msm.Position = 0;

                        IWaveSource MStream;
                        AudioLimiter BAC;
                        if (audioLimiter && waveFormat.BitsPerSample == 32)
                        {
                            Debug.PrintToConsole(Debug.LogType.Message, "LoudMax enabled.");
                            BAC = new AudioLimiter(msm, 0.1);
                            MStream = BAC.ToWaveSource(waveFormat.BitsPerSample);
                        }
                        else MStream = msm.ToWaveSource(waveFormat.BitsPerSample);

                        FileStream FOpen = File.Open(outputFile, FileMode.Create);
                        WaveWriter FDestination = new WaveWriter(FOpen, waveFormat);
                        Debug.PrintToConsole(Debug.LogType.Message, "Output file is open.");

                        int FRead = 0;
                        byte[] FBuffer = new byte[1024 * 16];

                        Debug.PrintToConsole(Debug.LogType.Message, String.Format("Writing data for {0} to disk...", outputFile));
                        while ((FRead = MStream.Read(FBuffer, 0, FBuffer.Length)) != 0)
                            FDestination.Write(FBuffer, 0, FRead);
                        Debug.PrintToConsole(Debug.LogType.Message, String.Format("Done writing {0}.", outputFile));

                        msm.Dispose();
                        FDestination.Dispose();
                        FOpen.Dispose();
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Debug.PrintToConsole(Debug.LogType.Error, string.Format("{0} - {1}", ex.InnerException?.ToString(), ex.Message.ToString()));
                }
            });

            if (!_cancToken.IsCancellationRequested)
                MiscFunctions.PerformShutdownCheck(stopwatch);

            Dispatcher.UIThread.Post(_winRef.Close);
        }

        private void PerTrackConversion(CSCore.WaveFormat waveFormat)
        {
            // Cache settings
            var perTrackFile = Program.Settings.PerTrackFile;
            var audioLimiter = Program.Settings.AudioLimiter;
            var codec = Program.Settings.Codec;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (MIDI midi in _midis)
            {
                AutoFillInfo(ConvStatus.Prep);

                if (_cancToken.IsCancellationRequested)
                    break;

                string folder = _outputPath;

                using (MultiStreamMerger msm = new(waveFormat))
                {
                    _validator.SetTotalTracks(midi.Tracks);
                    _validator.ResetCurrentTrack();

                    // Per track!
                    if (perTrackFile)
                    {
                        // We do, create folder
                        folder += string.Format("/{0}", Path.GetFileNameWithoutExtension(midi.Name));

                        if (Directory.Exists(folder))
                            folder += string.Format(" - {0}", DateTime.Now.ToString("dd-MM-yyyy HHmmsstt"));

                        Directory.CreateDirectory(folder);
                    }

                    folder += "/";

                    Parallel.For(0, midi.Tracks, _parallelOptions, track =>
                    {
                        if (_cancToken.IsCancellationRequested)
                            return;

                        Task cvThread;
                        string fOutputDir = string.Empty;

                        TaskStatus? trackPanel = null;

                        ISampleWriter sampleWriter;

                        try
                        {
                            using (MultiStreamMerger trackMsm = new(waveFormat))
                            {
                                Dispatcher.UIThread.Post(() => trackPanel = new TaskStatus($"Track {track}", _panelRef));

                                trackPanel?.UpdateTitle("Loading...");
                                trackPanel?.UpdateProgress(0.0);

                                var eventsProcesser = new EventsProcesser(_audioRenderer, midi.GetSingleTrackTimeBased(track), midi.Length.TotalSeconds);
                                Debug.PrintToConsole(Debug.LogType.Message, $"ConvertWorker => T{track}, {midi.Length.TotalSeconds}");

                                // Per track!
                                if (perTrackFile)
                                {
                                    // Prepare the filename
                                    fOutputDir = string.Format("{0}Track {1}.{2}",
                                        folder, track, codec);

                                    // Check if file already exists
                                    if (File.Exists(fOutputDir))
                                        fOutputDir = string.Format("{0}Track {1} - {2}.{3}",
                                            folder, track, DateTime.Now.ToString("dd-MM-yyyy HHmmsstt"), codec);

                                    sampleWriter = trackMsm.GetWriter();
                                }
                                else sampleWriter = msm.GetWriter();

                                cvThread = Task.Run(() => eventsProcesser.Process(sampleWriter, waveFormat, _cancToken.Token));
                                Debug.PrintToConsole(Debug.LogType.Message, $"ConvThread started for T{track}");

                                double cachedPerc = 0;

                                while (!cvThread.IsCompleted)
                                {
                                    var perc = Math.Round(eventsProcesser.Progress * 100.0);

                                    if (_cancToken.IsCancellationRequested)
                                        break;

                                    trackPanel?.UpdateTitle(eventsProcesser);
                                    //trackPanel?.UpdateRemainingTime(midiRenderer);

                                    if (cachedPerc != perc)
                                    {
                                        trackPanel?.UpdateProgress(cachedPerc = perc);
                                        AutoFillInfo(ConvStatus.MultiConv);
                                    }
                                }

                                // Update panel to 100%
                                trackPanel?.UpdateProgress(100.0);
                                AutoFillInfo(ConvStatus.MultiConv);

                                if (cvThread != null)
                                {
                                    cvThread.Wait();
                                    cvThread.Dispose();                                
                                }

                                eventsProcesser.Dispose();

                                if (perTrackFile)
                                {
                                    // Reset MSM position
                                    trackMsm.Position = 0;

                                    IWaveSource exportSource;
                                    if (audioLimiter && waveFormat.BitsPerSample == 32)
                                    {
                                        Debug.PrintToConsole(Debug.LogType.Message, "LoudMax enabled.");
                                        var BAC = new AudioLimiter(trackMsm, 0.1);
                                        exportSource = BAC.ToWaveSource(waveFormat.BitsPerSample);
                                    }
                                    else exportSource = trackMsm.ToWaveSource(waveFormat.BitsPerSample);

                                    FileStream targetFile = File.Open(fOutputDir, FileMode.Create, FileAccess.Write);
                                    WaveWriter fileWriter = new WaveWriter(targetFile, waveFormat);
                                    Debug.PrintToConsole(Debug.LogType.Message, "Output file is open.");

                                    int bufRead = 0;
                                    byte[] buf = new byte[1024 * 16];

                                    Debug.PrintToConsole(Debug.LogType.Message, $"Writing data for {fOutputDir} to disk...");

                                    while ((bufRead = exportSource.Read(buf, 0, buf.Length)) != 0)
                                        fileWriter.Write(buf, 0, bufRead);
                                    Debug.PrintToConsole(Debug.LogType.Message, $"Done writing {fOutputDir}.");

                                    fileWriter.Dispose();
                                    targetFile.Dispose();
                                }

                                if (!_cancToken.IsCancellationRequested)
                                    _validator.AddTrack();
                            }
                        }
                        catch (OperationCanceledException) { }
                        catch (Exception ex)
                        {
                            Debug.PrintToConsole(Debug.LogType.Error, string.Format("{0} - {1}", ex.InnerException?.ToString(), ex.Message.ToString()));
                        }

                        Dispatcher.UIThread.Post(() => trackPanel?.Dispose());
                    });

                    try
                    {
                        if (_cancToken.IsCancellationRequested)
                            break;

                        else _validator.AddValidMIDI();

                        if (!perTrackFile)
                        {
                            // Reset MSM position
                            msm.Position = 0;

                            // Time to save the file
                            var OutputDir = string.Format("{0}/{1}.{2}",
                            _outputPath, Path.GetFileNameWithoutExtension(midi.Name), codec);

                            // Check if file already exists
                            if (File.Exists(OutputDir))
                                OutputDir = string.Format("{0}/{1} - {2}.{3}",
                                    _outputPath, Path.GetFileNameWithoutExtension(midi.Name),
                                    DateTime.Now.ToString("yyyyMMdd HHmmsstt"), codec);

                            Debug.PrintToConsole(Debug.LogType.Message, String.Format("Output file: {0}", OutputDir));

                            // Prepare wave source
                            IWaveSource? MStream = null;
                            if (audioLimiter && waveFormat.BitsPerSample == 32)
                            {
                                Debug.PrintToConsole(Debug.LogType.Message, "LoudMax enabled.");
                                var BAC = new AudioLimiter(msm, 0.1);
                                MStream = BAC.ToWaveSource(waveFormat.BitsPerSample);
                            }
                            else MStream = msm.ToWaveSource(waveFormat.BitsPerSample);

                            FileStream targetFile = File.Open(OutputDir, FileMode.Create, FileAccess.Write);
                            WaveWriter fileWriter = new WaveWriter(targetFile, waveFormat);
                            Debug.PrintToConsole(Debug.LogType.Message, "Output file is open.");

                            int FRead = 0;
                            byte[] FBuffer = new byte[1024 * 16];

                            Debug.PrintToConsole(Debug.LogType.Message, String.Format("Writing data for {0} to disk...", OutputDir));
                            AutoFillInfo(ConvStatus.AudioOut);

                            while ((FRead = MStream.Read(FBuffer, 0, FBuffer.Length)) != 0)
                                fileWriter.Write(FBuffer, 0, FRead);
                            Debug.PrintToConsole(Debug.LogType.Message, String.Format("Done writing {0}.", OutputDir));

                            MStream.Dispose();
                            fileWriter.Dispose();
                            targetFile.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.PrintToConsole(Debug.LogType.Error, string.Format("{0} - {1}", ex.InnerException?.ToString(), ex.Message.ToString()));
                    }
                }
            }

            if (!_cancToken.IsCancellationRequested)
                MiscFunctions.PerformShutdownCheck(stopwatch);

            Dispatcher.UIThread.Post(_winRef.Close);
        }
    }

    public class EventsProcesser : IDisposable
    {
        int _activeVoices = 0;
        float _renderingTime = 0;
        protected bool _disposed = false;

        MIDIRenderer? midiRenderer = null;
        AudioEngine audioRenderer;
        IEnumerable<MIDIEvent>? events = new List<MIDIEvent>();

        bool rtsMode = false;
        double curFrametime = 0.0;
        double length = 0;
        double converted = 0;

        public double Progress => converted / length;
        public double RemainingTime => length - converted;
        public double ConvertedTime => converted;

        public int ActiveVoices => _activeVoices;
        public float RenderingTime => _renderingTime;
        public bool IsRTS => rtsMode;
        public double Framerate => 1 / curFrametime;

        public EventsProcesser(AudioEngine audioRenderer, IEnumerable<MIDIEvent> events, double length)
        {
            this.audioRenderer = audioRenderer;
            this.events = events;
            this.length = length;
        }
        double RoundToNearest(double n, double x)
        {
            return Math.Round(n / x) * x;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                events?.GetEnumerator().Dispose();
                events = null;
                midiRenderer?.Dispose();
            }

            _disposed = true;
        }

        public void Process(ISampleWriter output, CSCore.WaveFormat waveFormat, CancellationToken cancToken)
        {
            Random r = new Random();

            var volume = Program.Settings.Volume;
            rtsMode = Program.Settings.RTSMode;

            var rtsFps = Program.Settings.RTSFPS;
            var rtsFluct = Program.Settings.RTSFluct;

            var rtsFr = (1.0 / rtsFps);
            var percFps = (rtsFr / 100) * rtsFluct;
            var minFps = rtsFr - percFps;
            var maxFps = rtsFr + percFps;

            switch (audioRenderer)
            {
                case BASS bass:
                    midiRenderer = new BASSMIDI(bass);
                    break;

                default:
                    break;
            }

            if (midiRenderer != null)
            {
                midiRenderer.ChangeVolume(volume);

                float[] buffer = new float[64 * waveFormat.BlockAlign];
                long prevWriteTime = 0;
                double deltaTime = 0;

                Debug.PrintToConsole(Debug.LogType.Message, $"Initialized {midiRenderer.UniqueID}.");

                // Prepare stream
                if (Program.Settings.OverrideEffects)
                {
                    for (int i = 0; i <= 15; i++)
                        midiRenderer.SendCustomFXEvents(i, Program.Settings.ReverbVal, Program.Settings.ChorusVal);
                }

                try
                {
                    if (events != null)
                    {
                        foreach (MIDIEvent e in events)
                        {
                            if (cancToken.IsCancellationRequested)
                                return;

                            if (e is UndefinedEvent)
                                continue;

                            deltaTime += e.DeltaTime;
                            converted = deltaTime;
                            var eb = e.GetData();

                            if (rtsMode)
                                curFrametime = r.NextDouble() * (maxFps - minFps) + minFps;

                            long writeTime = (long)((rtsMode ? RoundToNearest(deltaTime, curFrametime) : deltaTime) * waveFormat.SampleRate);

                            // If writeTime ends up being negative, clamp it to 0
                            if (writeTime < prevWriteTime)
                                writeTime = prevWriteTime;

                            var offset = (int)((writeTime - prevWriteTime) * 2);

                            // Never EVER go back in time!!!!!!
                            if (writeTime > prevWriteTime) // <<<<< EVER!!!!!!!!!
                                prevWriteTime = writeTime;

                            while (offset > 0)
                            {
                                bool smallOffset = offset < buffer.Length;

                                midiRenderer.Read(buffer, 0, smallOffset ? offset : buffer.Length);
                                output.Write(buffer, 0, smallOffset ? offset : buffer.Length);
                                offset = smallOffset ? 0 : offset - buffer.Length;
                            }

                            switch (e)
                            {
                                case ControlChangeEvent ev:
                                    if (!(Program.Settings.OverrideEffects && (ev.Controller == 0x5B || ev.Controller == 0x5D)))
                                        midiRenderer.SendEvent(eb);

                                    break;

                                case NoteOnEvent:
                                case NoteOffEvent:
                                case PitchWheelChangeEvent:
                                case ChannelPressureEvent:
                                case ProgramChangeEvent:
                                case ChannelModeMessageEvent:
                                    midiRenderer.SendEvent(eb);
                                    break;

                                default:
                                    Debug.PrintToConsole(Debug.LogType.Warning, $"The synthesizer can't read this: {e.GetType()}");
                                    break;
                            }

                            _activeVoices = midiRenderer.ActiveVoices;
                            //_renderingTime = bass.RenderingTime;
                        }
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Debug.PrintToConsole(Debug.LogType.Warning, $"{midiRenderer.UniqueID} - DataParsingError {ex.Message}");
                }

                // MIDI renderer does support end event,
                // wait for the audio to reach a stop
                if (midiRenderer.SendEndEvent())
                {
                    int read;

                    while ((read = midiRenderer.Read(buffer, 0, buffer.Length)) != 0)
                        output.Write(buffer, 0, read);
                }              
            }

            output.Flush();
        }
    }
}
