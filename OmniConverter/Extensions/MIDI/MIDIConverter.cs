using Avalonia.Controls;
using Avalonia.Threading;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.MediaFoundation;
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

        private Stopwatch _convElapsedTime = new Stopwatch();
        private AudioEngine _audioRenderer;
        private MIDIValidator _validator;
        private CancellationTokenSource _cancToken;
        private ParallelOptions _parallelOptions;
        private Thread? _converterThread;

        private string _customTitle = string.Empty;
        private Window _winRef;
        private StackPanel _panelRef;
        private ObservableCollection<MIDI> _midis;
        private WaveFormat _waveFormat;

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

            _waveFormat = new WaveFormat(Program.Settings.SampleRate, 32, 2, AudioEncoding.IeeeFloat);
        }

        public override void Dispose()
        {
            _cancToken?.Cancel();
            _converterThread?.Join();
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
                var mem = ((ulong)v * 312) * (ulong)Program.Settings.ThreadsCount;
                var memusage = MiscFunctions.BytesToHumanReadableSize(mem);

                var re = MessageBox.Show(_winRef,
                    $"You set your voice limit to {v}.\nThis will require {memusage} of physical memory.\n\nAre you sure you want to continue?",
                    "OmniConverter - Warning", MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Warning);
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
                MessageBox.Show(_winRef, "You need to add SoundFonts to the SoundFonts list to start the conversion process.", "OmniConverter - Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
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

        public override string GetCustomTitle() => _customTitle;
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

            ulong _valid = _validator.GetValidMIDIs();
            ulong _nonvalid = _validator.GetInvalidMIDIs();
            ulong _total = _validator.GetTotalMIDIs();

            int _midiEvents = _validator.GetProcessedMIDIEvents();
            int _totalMidiEvents = _validator.GetTotalMIDIEvents();

            int _processed = _validator.GetProcessedEvents();
            int _all = _validator.GetTotalEvents();

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
                    _curStatus = 
                        $"{_valid + _nonvalid:N0} file(s) out of {_total:N0} have been converted.\n\n" +
                        $"Please wait..." +
                        $"\nElapsed time: {MiscFunctions.TimeSpanToHumanReadableTime(_convElapsedTime.Elapsed)}";
                    _progress = Math.Round(_processed * 100.0 / _all);
                    break;

                case ConvStatus.MultiConv:
                    _curStatus = 
                        $"{_valid + _nonvalid:N0} file(s) out of {_total:N0} have been converted.\n" +
                        $"Rendered {_curTrack:N0} track(s) out of {_tracks:N0}.\n" +
                        $"Please wait..." +
                        $"\nElapsed time: {MiscFunctions.TimeSpanToHumanReadableTime(_convElapsedTime.Elapsed)}";
                    _progress = Math.Round(_processed * 100.0 / _all);
                    _tracksProgress = Math.Round(_midiEvents * 100.0 / _totalMidiEvents);
                    break;

                case ConvStatus.AudioOut:
                    _curStatus = "Writing final audio file to disk.\n\nPlease do not turn off the computer...";
                    _progress = Math.Round(_processed * 100.0 / _all);
                    _tracksProgress = Math.Round(_midiEvents * 100.0 / _totalMidiEvents);
                    break;

                default:
                    break;
            }
        }

        private void ConversionFunc()
        {
            try
            {
                ChangeInfo("Initializing renderer...\n\nPlease wait.");

                // later I will add support for more engines
                switch (Program.Settings.Renderer)
                {
                    case EngineID.BASS:
                    default:
                        _audioRenderer = new BASS(_waveFormat, Program.Settings.MaxVoices, null);

                        // do this hacky crap to get the voice change to work
                        _audioRenderer.Dispose();
                        _audioRenderer = new BASS(_waveFormat, Program.Settings.MaxVoices, Program.Settings.SoundFontsList);
                        break;
                }

                if (_audioRenderer.Initialized)
                {
                    if (Program.Settings.PerTrackMode)
                    {
                        if (_midis.Count > 1)
                            Dispatcher.UIThread.Post(() => ((MIDIWindow)_winRef).EnableTrackProgress(true));

                        PerTrackConversion();
                    }
                    else PerMIDIConversion();
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

        private string GetOutputFilename(string midi, string codec = ".wav")
        {
            var filename = Path.GetFileNameWithoutExtension(midi);
            var outputFile = $"{_outputPath}/{filename}{codec}";

            // Check if file already exists
            if (File.Exists(outputFile))
                outputFile = $"{_outputPath}/{filename} - {DateTime.Now:yyyy-MM-dd HHmmss}{codec}";

            return outputFile;
        }

        private void GetTotalEventsCount()
        {
            List<int> totalEvents = new();
            foreach (MIDI midi in _midis)
                totalEvents.Add(midi.GetFullMIDITimeBased().Count());

            _validator.SetTotalEventsCount(totalEvents);
        }

        private void PerMIDIConversion()
        {
            // Cache settings
            var audioLimiter = Program.Settings.AudioLimiter;
            var codec = Program.Settings.SelectedCodec;

            AutoFillInfo(ConvStatus.Prep);
            GetTotalEventsCount();

            _convElapsedTime.Reset();
            _convElapsedTime.Start();

            Parallel.For(0, _midis.Count, _parallelOptions, nMidi =>
            {
                try
                {
                    if (_cancToken.IsCancellationRequested)
                        throw new OperationCanceledException();

                    var midi = _midis[nMidi];
                    midi.EnablePooling();

                    // Begin conversion
                    AutoFillInfo(ConvStatus.SingleConv);
                    _validator.SetCurrentMIDI(midi.Path);

                    // Prepare the filename
                    string outputFile = GetOutputFilename(midi.Name, codec);

                    Debug.PrintToConsole(Debug.LogType.Message, $"Output file: {outputFile}");

                    TaskStatus? midiPanel = null;

                    IEnumerable<MIDIEvent> evs = [];
                    bool loaded = false;
                    try
                    {
                        evs = midi.GetFullMIDITimeBased();
                        loaded = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.PrintToConsole(Debug.LogType.Error, $"{ex.Message}");
                    }

                    if (loaded)
                    {
                        var eventsProcesser = new EventsProcesser(_audioRenderer, evs, midi.Length.TotalSeconds, midi.LoadedFile);
                        Dispatcher.UIThread.Post(() => midiPanel = new TaskStatus(midi.Name, _panelRef, eventsProcesser));

                        // Initialize memory stream
                        var msm = new MultiStreamMerger(_waveFormat);
                        var sampleWriter = msm.GetWriter();

                        var cvThread = Task.Run(() => eventsProcesser.Process(sampleWriter, _waveFormat, _cancToken.Token, f => _validator.AddEvent()));

                        while (!cvThread.IsCompleted)
                        {
                            if (_cancToken.IsCancellationRequested)
                                throw new OperationCanceledException();

                            Thread.Sleep(100);
                        }

                        AutoFillInfo(ConvStatus.SingleConv);

                        if (cvThread != null)
                        {
                            cvThread.Wait();
                            cvThread.Dispose();
                        }

                        eventsProcesser.Dispose();

                        Debug.PrintToConsole(Debug.LogType.Message, $"Thread for MIDI \"{midi.Name}\" is done rendering data.");

                        Dispatcher.UIThread.Post(() => midiPanel?.Dispose());

                        if (!_cancToken.IsCancellationRequested) _validator.AddValidMIDI();

                        // Reset MSM position
                        msm.Position = 0;

                        IWaveSource MStream;
                        AudioLimiter BAC;
                        if (audioLimiter && _waveFormat.BitsPerSample == 32)
                        {
                            Debug.PrintToConsole(Debug.LogType.Message, "LoudMax enabled.");
                            BAC = new AudioLimiter(msm, 0.1);
                            MStream = BAC.ToWaveSource(_waveFormat.BitsPerSample);
                        }
                        else MStream = msm.ToWaveSource(_waveFormat.BitsPerSample);

                        FileStream FOpen = File.Open(outputFile, FileMode.Create);
                        WaveWriter FDestination = new WaveWriter(FOpen, _waveFormat);
                        Debug.PrintToConsole(Debug.LogType.Message, "Output file is open.");

                        int FRead = 0;
                        byte[] FBuffer = new byte[1024 * 16];

                        Debug.PrintToConsole(Debug.LogType.Message, $"Writing data for {outputFile} to disk...");
                        while ((FRead = MStream.Read(FBuffer, 0, FBuffer.Length)) != 0)
                            FDestination.Write(FBuffer, 0, FRead);
                        Debug.PrintToConsole(Debug.LogType.Message, $"Done writing {outputFile}.");

                        msm.Dispose();
                        FDestination.Dispose();
                        FOpen.Dispose();
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Debug.PrintToConsole(Debug.LogType.Error, $"{ex.InnerException} - {ex.Message}");
                }
            });

            if (!_cancToken.IsCancellationRequested)
                MiscFunctions.PerformShutdownCheck(_convElapsedTime);

            Dispatcher.UIThread.Post(_winRef.Close);
        }

        private void PerTrackConversion()
        {
            // Cache settings
            var perTrackFile = Program.Settings.PerTrackFile;
            var audioLimiter = Program.Settings.AudioLimiter;
            var codec = Program.Settings.SelectedCodec;

            GetTotalEventsCount();
            _convElapsedTime.Reset();
            _convElapsedTime.Start();

            foreach (MIDI midi in _midis)
            {
                midi.EnablePooling();
                AutoFillInfo(ConvStatus.Prep);

                if (_cancToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                _customTitle = midi.Name;
                string folder = _outputPath;

                var midiData = midi.GetIterateTracksTimeBased();
                var temp = 0;

                for (int i = 0; i < midiData.Count(); i++)
                    temp += midiData.ElementAt(i).Count();

                _validator.SetTotalMIDIEvents(temp);
                _validator.SetTotalTracks(midiData.Count());

                using (MultiStreamMerger msm = new(_waveFormat))
                {

                    // Per track!
                    if (perTrackFile)
                    {
                        // We do, create folder
                        folder += string.Format("/{0}", Path.GetFileNameWithoutExtension(midi.Name));

                        if (Directory.Exists(folder))
                            folder += string.Format(" - {0}", DateTime.Now.ToString("yyyy-MM-dd HHmmss"));

                        Directory.CreateDirectory(folder);
                    }

                    folder += "/";

                    Parallel.For(0, midiData.Count(), _parallelOptions, track =>
                    {
                        try
                        {
                            if (_cancToken.IsCancellationRequested)
                                throw new OperationCanceledException();

                            Task cvThread;
                            string fOutputDir = string.Empty;

                            TimeSpan TrackETA = TimeSpan.Zero;
                            TaskStatus? trackPanel = null;

                            ISampleWriter sampleWriter;

                            using (MultiStreamMerger trackMsm = new(_waveFormat))
                            {
                                if (_cancToken.IsCancellationRequested)
                                    return;

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

                                var trackData = midiData.ElementAt(track);

                                var eventsProcesser = new EventsProcesser(_audioRenderer, trackData, midi.Length.TotalSeconds, midi.LoadedFile);
                                Dispatcher.UIThread.Post(() => trackPanel = new TaskStatus($"Track {track}", _panelRef, eventsProcesser));

                                cvThread = Task.Run(() => eventsProcesser.Process(sampleWriter, _waveFormat, _cancToken.Token, f =>
                                {
                                    try
                                    {
                                        _validator.AddEvent();
                                        return _validator.AddMIDIEvent();
                                    }
                                    catch (ObjectDisposedException) { return 0; }
                                }));
                                Debug.PrintToConsole(Debug.LogType.Message, $"ConvThread started for T{track}");

                                while (!cvThread.IsCompleted)
                                {
                                    if (_cancToken.IsCancellationRequested)
                                        throw new OperationCanceledException();

                                    AutoFillInfo(ConvStatus.MultiConv);

                                    Thread.Sleep(100);
                                }

                                // Update panel to 100%
                                AutoFillInfo(ConvStatus.MultiConv);

                                if (cvThread != null)
                                {
                                    cvThread.Wait();
                                    cvThread.Dispose();                                
                                }

                                eventsProcesser.Dispose();

                                Dispatcher.UIThread.Post(() => trackPanel?.Dispose());

                                if (perTrackFile)
                                {
                                    // Reset MSM position
                                    trackMsm.Position = 0;

                                    IWaveSource exportSource;
                                    if (audioLimiter && _waveFormat.BitsPerSample == 32)
                                    {
                                        Debug.PrintToConsole(Debug.LogType.Message, "LoudMax enabled.");
                                        var BAC = new AudioLimiter(trackMsm, 0.1);
                                        exportSource = BAC.ToWaveSource(_waveFormat.BitsPerSample);
                                    }
                                    else exportSource = trackMsm.ToWaveSource(_waveFormat.BitsPerSample);

                                    FileStream targetFile = File.Open(fOutputDir, FileMode.Create, FileAccess.Write);
                                    WaveWriter fileWriter = new WaveWriter(targetFile, _waveFormat);
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
                            Debug.PrintToConsole(Debug.LogType.Error, $"{ex.InnerException} - {ex.Message}");
                        }                 
                    });

                    try
                    {
                        if (!perTrackFile)
                        {
                            // Reset MSM position
                            msm.Position = 0;

                            // Time to save the file
                            var OutputDir = GetOutputFilename(midi.Name, codec);

                            Debug.PrintToConsole(Debug.LogType.Message, $"Output file: {OutputDir}");

                            // Prepare wave source
                            IWaveSource? MStream = null;
                            if (audioLimiter && _waveFormat.BitsPerSample == 32)
                            {
                                Debug.PrintToConsole(Debug.LogType.Message, "LoudMax enabled.");
                                var BAC = new AudioLimiter(msm, 0.1);
                                MStream = BAC.ToWaveSource(_waveFormat.BitsPerSample);
                            }
                            else MStream = msm.ToWaveSource(_waveFormat.BitsPerSample);

                            FileStream targetFile = File.Open(OutputDir, FileMode.Create, FileAccess.Write);
                            WaveWriter fileWriter = new WaveWriter(targetFile, _waveFormat);
                            Debug.PrintToConsole(Debug.LogType.Message, "Output file is open.");

                            int FRead = 0;
                            byte[] FBuffer = new byte[1024 * 16];

                            Debug.PrintToConsole(Debug.LogType.Message, $"Writing data for {OutputDir} to disk...");
                            AutoFillInfo(ConvStatus.AudioOut);

                            while ((FRead = MStream.Read(FBuffer, 0, FBuffer.Length)) != 0)
                                fileWriter.Write(FBuffer, 0, FRead);
                            Debug.PrintToConsole(Debug.LogType.Message, $"Done writing {OutputDir}.");

                            MStream.Dispose();
                            fileWriter.Dispose();
                            targetFile.Dispose();
                        }

                        if (_cancToken.IsCancellationRequested)
                            break;
                        else _validator.AddValidMIDI();
                    }
                    catch (Exception ex)
                    {
                        Debug.PrintToConsole(Debug.LogType.Error, $"{ex.InnerException} - {ex.Message}");
                    }
                }
            }

            if (!_cancToken.IsCancellationRequested)
                MiscFunctions.PerformShutdownCheck(_convElapsedTime);

            Dispatcher.UIThread.Post(_winRef.Close);
        }
    }

    public class EventsProcesser : OmniTask
    {
        MIDIRenderer? midiRenderer = null;
        AudioEngine audioRenderer;
        IEnumerable<MIDIEvent>? events = new List<MIDIEvent>();
        MidiFile file;

        int totalEvents = 0;
        int processedEvents = 0;

        double length = 0;

        ulong noteCount = 0;
        ulong playedNotes = 0;
        bool rtsMode = false;
        double curFrametime = 0.0;

        public override double Progress => ((double)processedEvents / totalEvents) * 100;
        public override double Remaining => totalEvents - processedEvents;
        public override double Processed => processedEvents;
        public override double Length => length;

        public int ActiveVoices => midiRenderer != null ? midiRenderer.ActiveVoices : 0;
        public ulong PlayedNotes => playedNotes;
        public float RenderingTime => midiRenderer != null ? midiRenderer.RenderingTime : 0.0f;
        public bool IsRTS => rtsMode;
        public double Framerate => 1 / curFrametime;

        public EventsProcesser(AudioEngine audioRenderer, IEnumerable<MIDIEvent> events, double length, MidiFile file)
        {
            this.audioRenderer = audioRenderer;
            this.events = events;
            this.length = length;
            this.file = file;

            totalEvents = events.Count();
        }

        double RoundToNearest(double n, double x)
        {
            return Math.Round(n / x) * x;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                events?.GetEnumerator().Dispose();
                events = null;
                midiRenderer?.Dispose();
            }

            _disposed = true;
        }

        public void Process(ISampleWriter output, WaveFormat waveFormat, CancellationToken cancToken, Func<int, int>? f = null)
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
                byte[] scratch = new byte[16];

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

                            processedEvents++;

                            if (e is UndefinedEvent)
                                continue;

                            deltaTime += e.DeltaTime;
                            var eb = e.GetData(scratch);

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
                                    playedNotes++;
                                    midiRenderer.SendEvent(eb);
                                    break;

                                case NoteOffEvent:                     
                                case PitchWheelChangeEvent:
                                case ChannelPressureEvent:
                                case ProgramChangeEvent:
                                case ChannelModeMessageEvent:
                                    midiRenderer.SendEvent(eb);
                                    break;

                                default:
                                    break;
                            }

                            if (f != null)
                                _ = f(1);
                            //_renderingTime = bass.RenderingTime;

                            file.Return(e);
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
