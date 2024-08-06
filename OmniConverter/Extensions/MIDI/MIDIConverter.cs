using Avalonia.Controls;
using Avalonia.Threading;
using CSCore;
using CSCore.Codecs.WAV;
using FFMpegCore;
using MIDIModificationFramework;
using MIDIModificationFramework.MIDIEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            EncodingAudio,
            Crash
        }

        private Stopwatch _convElapsedTime = new Stopwatch();
        private AudioEngine? _audioRenderer;
        private MIDIValidator _validator;
        private CancellationTokenSource _cancToken;
        private ParallelOptions _parallelOptions;
        private Thread? _converterThread;

        private string _customTitle = string.Empty;
        private Window _winRef;
        private StackPanel _panelRef;
        private ObservableCollection<MIDI> _midis;
        private WaveFormat _waveFormat;
        private Settings _cachedSettings;

        private string _curStatus = string.Empty;
        private double _progress = 0;
        private double _tracksProgress = 0;
        private string _outputPath = string.Empty;
        private bool _pauseConversion = false;
        private int _threadsCount = 0;

        public MIDIConverter(string outputPath, AudioCodecType codec, int threads, Window winRef, StackPanel panel, ObservableCollection<MIDI> midis)
        {
            _winRef = winRef;
            _panelRef = panel;
            _midis = midis;
            _outputPath = outputPath;
            _cachedSettings = (Settings)Program.Settings.Clone();
            _cancToken = new CancellationTokenSource();
            _validator = new MIDIValidator((ulong)_midis.Count);
            _threadsCount = _cachedSettings.MultiThreadedMode ? threads.LimitToRange(1, 65536) : 1;

            _parallelOptions = new ParallelOptions { 
                MaxDegreeOfParallelism = _threadsCount, 
                CancellationToken = _cancToken.Token
            };

            _waveFormat = new WaveFormat(_cachedSettings.SampleRate, 32, 2, AudioEncoding.IeeeFloat);
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
            if (_cachedSettings.MaxVoices >= 10000000)
            {
                var biggestMidi = _midis.MaxBy(x => x.Tracks);

                var v = _cachedSettings.MaxVoices;
                var mem = ((ulong)v * 312) * (ulong)_cachedSettings.ThreadsCount;
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

            if (_cachedSettings.AudioCodec != AudioCodecType.PCM)
            {
                if (!Program.FFmpegAvailable)
                {
                    var re = MessageBox.Show(_winRef,
                        $"You selected {_cachedSettings.AudioCodec.ToExtension()} as your export format, but FFMpeg is not available!\n\n" +
                        $"Press Yes if you want to fall back to {AudioCodecType.PCM.ToExtension()}.",
                        "OmniConverter - Error", MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Error);
                    switch (re)
                    {
                        case MessageBox.MsgBoxResult.No:
                            return false;

                        default:
                            _cachedSettings.AudioCodec = AudioCodecType.PCM;
                            Program.SaveConfig();
                            break;
                    }
                }

                switch (_cachedSettings.AudioCodec)
                {
                    case AudioCodecType.LAME:
                        if (IsInvalidFormat(AudioCodecType.LAME, 48000, 320))
                            return false;

                        break;

                    case AudioCodecType.Vorbis:
                        if (IsInvalidFormat(AudioCodecType.Vorbis, 48000, 480))
                            return false;

                        break;

                    default:
                        break;
                }
            }

            _converterThread = new Thread(ConversionFunc);
            _converterThread.IsBackground = true;
            _converterThread.Start();
            return true;
        }

        public override void RestoreWork()
        {
            // We trust that the deserialized stuff is fine KAPPA
            _converterThread = new Thread(ConversionFunc);
            _converterThread.IsBackground = true;
            _converterThread.Start();
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

        public override void TogglePause(bool toggle) => _pauseConversion = toggle;

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

            ulong _midiEvents = _validator.GetProcessedMIDIEvents();
            ulong _totalMidiEvents = _validator.GetTotalMIDIEvents();

            ulong _processed = _validator.GetProcessedEvents();
            ulong _all = _validator.GetTotalEvents();

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
                    _curStatus = "Writing audio file to disk.\n\nPlease do not turn off the computer...";
                    _progress = Math.Round(_processed * 100.0 / _all);
                    _tracksProgress = Math.Round(_midiEvents * 100.0 / _totalMidiEvents);
                    break;

                case ConvStatus.EncodingAudio:
                    _curStatus = $"Encoding audio to {_cachedSettings.AudioCodec.ToExtension()}.\n\nPlease do not turn off the computer...";
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
                switch (_cachedSettings.Renderer)
                {
                    case EngineID.XSynth:
                        _audioRenderer = new XSynthEngine(_waveFormat, _cachedSettings);
                        break;

                    case EngineID.BASS:
                    default:
                        // do this hacky crap to get the voice change to work
                        _audioRenderer = new BASSEngine(_waveFormat, _cachedSettings);
                        _audioRenderer.Dispose();

                        _audioRenderer = new BASSEngine(_waveFormat, _cachedSettings);
                        break;
                }

                if (_audioRenderer.Initialized)
                {
                    if (_cachedSettings.PerTrackMode)
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
                _audioRenderer?.Dispose();
            }
        }

        private string GetOutputFilename(string midi, AudioCodecType codec, bool add_swp = true)
        {
            var filename = Path.GetFileNameWithoutExtension(midi);
            var outputFile = $"{_outputPath}/{filename}{codec.ToExtension()}";

            // Check if file already exists
            if (File.Exists(outputFile))
                outputFile = $"{_outputPath}/{filename} - {DateTime.Now:yyyy-MM-dd HHmmss}{codec.ToExtension()}";

            if (codec != AudioCodecType.PCM && add_swp)
                outputFile += ".swp";

            return outputFile;
        }

        private void GetTotalEventsCount()
        {
            List<ulong> totalEvents = new();
            foreach (MIDI midi in _midis)
                totalEvents.Add(midi.TotalEventCount);

            _validator.SetTotalEventsCount(totalEvents);
        }

        private bool IsInvalidFormat(AudioCodecType codec, int maxSampleRate, int maxBitrate)
        {
            string error = string.Empty;

            if (_cachedSettings.SampleRate > maxSampleRate || _cachedSettings.AudioBitrate > maxBitrate)
            {
                if (_cachedSettings.AudioEvents)
                    MiscFunctions.PlaySound(MiscFunctions.ConvSounds.Error, true);

                if (_cachedSettings.SampleRate > maxSampleRate)
                    error += $"{codec.ToExtension()} does not support sample rates above {maxSampleRate / 1000}kHz.";

                if (_cachedSettings.AudioBitrate > maxBitrate)
                    error += $"{(string.IsNullOrEmpty(error) ? "" : "\n\n")}{codec.ToExtension()} does not support bitrates above {maxBitrate}kbps.";


                MessageBox.Show(_winRef, error, "OmniConverter - Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);

                return true;
            }

            return false;
        }

        private bool CheckIfCodecCanFloat(AudioCodecType codec)
        {
            switch (codec)
            {
                case AudioCodecType.LAME:
                    return false;

                default:
                    return true;
            }
        }

        private void PerMIDIConversion()
        {
            if (_audioRenderer == null)
                return;

            // Cache settings
            var autoDevice = _audioRenderer is BASSEngine;
            var codec = _cachedSettings.AudioCodec;
            var audioLimiter = CheckIfCodecCanFloat(codec) ? _cachedSettings.AudioLimiter : true;

            AutoFillInfo(ConvStatus.Prep);
            GetTotalEventsCount();

            if (_cachedSettings.AudioEvents)
                MiscFunctions.PlaySound(MiscFunctions.ConvSounds.Start, !autoDevice);

            _convElapsedTime.Reset();
            _convElapsedTime.Start();

            Parallel.For(_midis.Count, _parallelOptions, nMidi =>
            {
                try
                {
                    if (_cancToken.IsCancellationRequested)
                        throw new OperationCanceledException();

                    var midi = _midis[nMidi];
                    midi.EnablePooling();

                    // Begin conversion
                    _validator.SetCurrentMIDI(midi.Path);

                    // Prepare the filename
                    string fallbackFile = GetOutputFilename(midi.Name, AudioCodecType.PCM, false);
                    string outputFile1 = GetOutputFilename(midi.Name, codec, true);
                    string outputFile2 = GetOutputFilename(midi.Name, codec, false);

                    Debug.PrintToConsole(Debug.LogType.Message, $"Output file: {outputFile1}");

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
                        using (var msm = new MultiStreamMerger(_waveFormat))
                        {
                            using (var eventsProcesser = new EventsProcesser(_cachedSettings, _audioRenderer, evs, midi))
                            {
                                AutoFillInfo(ConvStatus.SingleConv);

                                Dispatcher.UIThread.Post(() => midiPanel = new TaskStatus(midi.Name, _panelRef, eventsProcesser));

                                var cvThread = Task.Run(() => eventsProcesser.Process(msm.GetWriter(), _waveFormat, _cancToken.Token, f => _validator.AddEvent()));

                                while (!cvThread.IsCompleted)
                                {
                                    if (_cancToken.IsCancellationRequested)
                                        throw new OperationCanceledException();

                                    eventsProcesser.RefreshInfo();
                                    eventsProcesser.TogglePause(_pauseConversion);

                                    Thread.Sleep(100);
                                }

                                AutoFillInfo(ConvStatus.SingleConv);

                                if (cvThread != null)
                                {
                                    cvThread.Wait();
                                    cvThread.Dispose();
                                }

                                Dispatcher.UIThread.Post(() => midiPanel?.Dispose());
                            }

                            if (!_cancToken.IsCancellationRequested) 
                                _validator.AddValidMIDI();

                            Debug.PrintToConsole(Debug.LogType.Message, $"Thread for MIDI {outputFile1} is done rendering data.");

                            // Reset MSM position
                            msm.Position = 0;

                            IWaveSource MStream;
                            Limiter BAC;
                            if (audioLimiter && _waveFormat.BitsPerSample == 32)
                            {
                                Debug.PrintToConsole(Debug.LogType.Message, "LoudMax enabled.");
                                BAC = new Limiter(msm, 0.1);
                                MStream = BAC.ToWaveSource(_waveFormat.BitsPerSample);
                            }
                            else MStream = msm.ToWaveSource(_waveFormat.BitsPerSample);

                            FileStream FOpen = File.Open(outputFile1, FileMode.Create);
                            WaveWriter FDestination = new WaveWriter(FOpen, _waveFormat);
                            Debug.PrintToConsole(Debug.LogType.Message, "Output file is open.");

                            int FRead = 0;
                            byte[] FBuffer = new byte[1024 * 16];

                            Debug.PrintToConsole(Debug.LogType.Message, $"Writing data for {outputFile1} to disk...");
                            while ((FRead = MStream.Read(FBuffer, 0, FBuffer.Length)) != 0)
                                FDestination.Write(FBuffer, 0, FRead);
                            Debug.PrintToConsole(Debug.LogType.Message, $"Done writing {outputFile1}.");

                            FDestination.Dispose();
                            FOpen.Dispose();

                            if (codec != AudioCodecType.PCM)
                            {
                                try
                                {
                                    AutoFillInfo(ConvStatus.EncodingAudio);

                                    var ffcodec = codec.ToFFMpegCodec();

                                    if (ffcodec == null)
                                        throw new Exception();

                                    Debug.PrintToConsole(Debug.LogType.Message, $"Converting {outputFile1} to final user selected codec...");
                                    FFMpegArguments
                                    .FromFileInput(outputFile1)
                                    .OutputToFile(outputFile2, true,
                                        options => options.WithAudioCodec(ffcodec)
                                        .WithAudioBitrate(_cachedSettings.AudioBitrate))
                                    .ProcessSynchronously();

                                    File.Delete(outputFile1);
                                    Debug.PrintToConsole(Debug.LogType.Message, $"Done converting {outputFile2}.");
                                }
                                catch
                                {
                                    Debug.PrintToConsole(Debug.LogType.Message, $"ffmpeg errored out or wasn't found! Falling back to WAV output. ({fallbackFile})");
                                    File.Move(outputFile1, fallbackFile);
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Debug.PrintToConsole(Debug.LogType.Error, $"{ex.InnerException} - {ex.Message}");
                }
            });

            if (!_cancToken.IsCancellationRequested)
            {
                if (_cachedSettings.AudioEvents)
                    MiscFunctions.PlaySound(MiscFunctions.ConvSounds.Finish, !autoDevice);

                MiscFunctions.PerformShutdownCheck(_convElapsedTime);
            }
            else MiscFunctions.PlaySound(MiscFunctions.ConvSounds.Error, !autoDevice);

            Dispatcher.UIThread.Post(_winRef.Close);
        }

        private void PerTrackConversion()
        {
            if (_audioRenderer == null)
                return;

            // Cache settings
            var autoDevice = _audioRenderer is BASSEngine;
            var perTrackFile = _cachedSettings.PerTrackFile;
            var codec = _cachedSettings.AudioCodec;
            var audioLimiter = CheckIfCodecCanFloat(codec) ? _cachedSettings.AudioLimiter : true;

            if (_cachedSettings.AudioEvents)
                MiscFunctions.PlaySound(MiscFunctions.ConvSounds.Start, !autoDevice);

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

                _validator.SetTotalMIDIEvents(midi.TotalEventCount);
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

                    Parallel.For(midiData.Count(), _parallelOptions, track =>
                    {
                        try
                        {
                            if (_cancToken.IsCancellationRequested)
                                throw new OperationCanceledException();

                            var midiTrack = midiData.ElementAt(track);

                            var notes = midiTrack.Where(x => x is NoteOnEvent);

                            if (notes.Count() < 2)
                                return;

                            Task cvThread;
                            string fallbackFile = string.Empty;
                            string outputFile1 = string.Empty;
                            string outputFile2 = string.Empty;

                            TimeSpan TrackETA = TimeSpan.Zero;
                            TaskStatus? trackPanel = null;

                            ISampleWriter sampleWriter;

                            using (MultiStreamMerger trackMsm = new(_waveFormat))
                            {
                                Debug.PrintToConsole(Debug.LogType.Message, $"ConvertWorker => T{track}, {midi.Length.TotalSeconds}");

                                // Per track!
                                if (perTrackFile)
                                {
                                    // Prepare the filename
                                    fallbackFile = outputFile2 = string.Format("{0}Track {1}{2}",
                                        folder, track, AudioCodecType.PCM.ToExtension());
                                    outputFile1 = string.Format("{0}Track {1}{2}{3}",
                                        folder, track, codec.ToExtension(), codec != AudioCodecType.PCM ? ".swp" : "");
                                    outputFile2 = string.Format("{0}Track {1}{2}",
                                        folder, track, codec.ToExtension());

                                    // Check if file already exists
                                    if (File.Exists(outputFile1))
                                    {
                                        var date = DateTime.Now.ToString("dd-MM-yyyy HHmmsstt");

                                        fallbackFile = outputFile2 = string.Format("{0}Track {1} - {2}{3}",
                                            folder, track, date, AudioCodecType.PCM.ToExtension());
                                        outputFile1 = string.Format("{0}Track {1} - {2}{3}{4}",
                                            folder, track, date, codec.ToExtension(), codec != AudioCodecType.PCM ? ".swp" : "");
                                        outputFile2 = string.Format("{0}Track {1} - {2}{3}",
                                            folder, track, date, codec.ToExtension());
                                    }

                                    sampleWriter = trackMsm.GetWriter();
                                }
                                else sampleWriter = msm.GetWriter();

                                using (var eventsProcesser = new EventsProcesser(_cachedSettings, _audioRenderer, midiTrack, midi, track))
                                {
                                    Dispatcher.UIThread.Post(() => trackPanel = new TaskStatus($"Track {track}", _panelRef, eventsProcesser));

                                    cvThread = Task.Run(() => eventsProcesser.Process(sampleWriter, _waveFormat, _cancToken.Token, f =>
                                    {
                                        _validator.AddEvent();
                                        return _validator.AddMIDIEvent();
                                    }));
                                    Debug.PrintToConsole(Debug.LogType.Message, $"ConvThread started for T{track}");

                                    while (!cvThread.IsCompleted)
                                    {
                                        if (_cancToken.IsCancellationRequested)
                                            throw new OperationCanceledException();

                                        eventsProcesser.RefreshInfo();
                                        eventsProcesser.TogglePause(_pauseConversion);

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

                                    Dispatcher.UIThread.Post(() => trackPanel?.Dispose());
                                }

                                if (perTrackFile)
                                {
                                    // Reset MSM position
                                    trackMsm.Position = 0;

                                    IWaveSource exportSource;
                                    if (audioLimiter && _waveFormat.BitsPerSample == 32)
                                    {
                                        Debug.PrintToConsole(Debug.LogType.Message, "LoudMax enabled.");
                                        var BAC = new Limiter(trackMsm, 0.1);
                                        exportSource = BAC.ToWaveSource(_waveFormat.BitsPerSample);
                                    }
                                    else exportSource = trackMsm.ToWaveSource(_waveFormat.BitsPerSample);

                                    using (var targetFile = File.Open(outputFile1, FileMode.Create, FileAccess.Write))
                                    {
                                        WaveWriter fileWriter = new WaveWriter(targetFile, _waveFormat);
                                        Debug.PrintToConsole(Debug.LogType.Message, "Output file is open.");

                                        int bufRead = 0;
                                        byte[] buf = new byte[1024 * 16];

                                        Debug.PrintToConsole(Debug.LogType.Message, $"Writing data for {outputFile1} to disk...");

                                        while ((bufRead = exportSource.Read(buf, 0, buf.Length)) != 0)
                                            fileWriter.Write(buf, 0, bufRead);
                                        Debug.PrintToConsole(Debug.LogType.Message, $"Done writing {outputFile1}.");

                                        exportSource.Dispose();
                                        fileWriter.Dispose();
                                    }

                                    if (codec != AudioCodecType.PCM)
                                    {
                                        try
                                        {
                                            Debug.PrintToConsole(Debug.LogType.Message, $"Converting {outputFile1} to final user selected codec...");

                                            var ffcodec = codec.ToFFMpegCodec();

                                            if (ffcodec == null)
                                                throw new Exception();

                                            FFMpegArguments
                                            .FromFileInput(outputFile1)
                                            .OutputToFile(outputFile2, true,
                                                options => options.WithAudioCodec(ffcodec)
                                                .WithAudioBitrate(_cachedSettings.AudioBitrate))
                                            .ProcessSynchronously();

                                            File.Delete(outputFile1);
                                            Debug.PrintToConsole(Debug.LogType.Message, $"Done converting {outputFile2}.");
                                        }
                                        catch
                                        {
                                            Debug.PrintToConsole(Debug.LogType.Message, $"ffmpeg errored out or wasn't found! Falling back to WAV output. ({fallbackFile})");
                                            File.Move(outputFile1, fallbackFile);
                                        }
                                    }
                                }

                                if (!_cancToken.IsCancellationRequested)
                                    _validator.AddTrack();
                            }
                        }
                        catch (OperationCanceledException) { }
                        catch (Exception ex)
                        {
                            Debug.PrintToConsole(Debug.LogType.Error, $"{ex}");
                        }                 
                    });

                    try
                    {
                        if (!perTrackFile)
                        {
                            // Reset MSM position
                            msm.Position = 0;

                            // Time to save the file
                            string fallbackFile = GetOutputFilename(midi.Name, AudioCodecType.PCM, false);
                            var outputFile1 = GetOutputFilename(midi.Name, codec, true);
                            var outputFile2 = GetOutputFilename(midi.Name, codec, false);

                            Debug.PrintToConsole(Debug.LogType.Message, $"Output file: {outputFile1}");

                            // Prepare wave source
                            IWaveSource? MStream = null;
                            if (audioLimiter && _waveFormat.BitsPerSample == 32)
                            {
                                Debug.PrintToConsole(Debug.LogType.Message, "LoudMax enabled.");
                                var BAC = new Limiter(msm, 0.1);
                                MStream = BAC.ToWaveSource(_waveFormat.BitsPerSample);
                            }
                            else MStream = msm.ToWaveSource(_waveFormat.BitsPerSample);

                            using (var targetFile = File.Open(outputFile1, FileMode.Create, FileAccess.Write))
                            {
                                WaveWriter fileWriter = new WaveWriter(targetFile, _waveFormat);
                                Debug.PrintToConsole(Debug.LogType.Message, "Output file is open.");

                                int FRead = 0;
                                byte[] FBuffer = new byte[1024 * 16];

                                Debug.PrintToConsole(Debug.LogType.Message, $"Writing data for {outputFile1} to disk...");

                                AutoFillInfo(ConvStatus.AudioOut);

                                while ((FRead = MStream.Read(FBuffer, 0, FBuffer.Length)) != 0)
                                    fileWriter.Write(FBuffer, 0, FRead);

                                Debug.PrintToConsole(Debug.LogType.Message, $"Done writing {outputFile1}.");

                                MStream.Dispose();
                                fileWriter.Dispose();
                            }

                            if (codec != AudioCodecType.PCM)
                            {
                                try
                                {
                                    AutoFillInfo(ConvStatus.EncodingAudio);

                                    var ffcodec = codec.ToFFMpegCodec();

                                    if (ffcodec == null)
                                        throw new Exception();

                                    Debug.PrintToConsole(Debug.LogType.Message, $"Converting {outputFile1} to final user selected codec...");
                                    FFMpegArguments
                                    .FromFileInput(outputFile1)
                                    .OutputToFile(outputFile2, true,
                                        options => options.WithAudioCodec(ffcodec)
                                        .WithAudioBitrate(_cachedSettings.AudioBitrate))
                                    .ProcessSynchronously();

                                    File.Delete(outputFile1);
                                    Debug.PrintToConsole(Debug.LogType.Message, $"Done converting {outputFile2}.");
                                }
                                catch
                                {
                                    Debug.PrintToConsole(Debug.LogType.Message, $"ffmpeg errored out or wasn't found! Falling back to WAV output. ({fallbackFile})");
                                    File.Move(outputFile1, fallbackFile);
                                }
                            }
                        }

                        if (_cancToken.IsCancellationRequested)
                            break;
                        else _validator.AddValidMIDI();
                    }
                    catch (Exception ex)
                    {
                        Debug.PrintToConsole(Debug.LogType.Error, $"{ex}");
                    }
                }
            }

            if (!_cancToken.IsCancellationRequested)
            {
                if (_cachedSettings.AudioEvents)
                    MiscFunctions.PlaySound(MiscFunctions.ConvSounds.Finish, !autoDevice);

                MiscFunctions.PerformShutdownCheck(_convElapsedTime);
            }
            else MiscFunctions.PlaySound(MiscFunctions.ConvSounds.Error, !autoDevice);

            Dispatcher.UIThread.Post(_winRef.Close);
        }
    }

    public class EventsProcesser : OmniTask
    {
        MIDIRenderer? _midiRenderer = null;
        AudioEngine _audioRenderer;
        IEnumerable<MIDIEvent>? _events = new List<MIDIEvent>();
        MidiFile _file;
        Settings _cachedSettings;

        ulong _eventsCount = 0;
        ulong _processedEvents = 0;

        double _length = 0;
        double _converted = 0;

        ulong playedNotes = 0;
        bool rtsMode = false;
        bool pauseConversion = false;
        double curFrametime = 0.0;

        public override double Progress => (_converted / _length) * 100;
        public override double Remaining => _length - _converted;
        public override double Processed => _converted;
        public override double Length => _length;
        public override void RefreshInfo() => _midiRenderer?.RefreshInfo();
        public override void TogglePause(bool toggle) { if (pauseConversion != toggle) pauseConversion = toggle; }

        public ulong RemainingEvents => _eventsCount - _processedEvents;
        public ulong ProcessedEvents => _processedEvents;
        public ulong ActiveVoices => _midiRenderer != null ? _midiRenderer.ActiveVoices : 0;
        public float RenderingTime => _midiRenderer != null ? _midiRenderer.RenderingTime : 0.0f;
        public ulong PlayedNotes => playedNotes;
        public bool IsRTS => rtsMode;
        public double Framerate => 1 / curFrametime;

        public EventsProcesser(Settings cachedSettings, AudioEngine audioRenderer, IEnumerable<MIDIEvent> events, MIDI? midi, int track = -1)
        {
            if (midi == null)
                throw new NullReferenceException("MIDI is null");

            if (midi.LoadedFile == null)
                throw new NullReferenceException("MidiFile is null");

            _file = midi.LoadedFile;
            _audioRenderer = audioRenderer;
            _events = events;
            _eventsCount = track < 0 ? midi.TotalEventCount : midi.EventCounts[track];
            _cachedSettings = cachedSettings;
            _length = midi.Length.TotalSeconds;
        }

        double RoundToNearest(double n, double x)
        {
            return Math.Round(n / x) * x;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _events?.GetEnumerator().Dispose();
                _events = null;
                _midiRenderer?.Dispose();
            }

            _disposed = true;
        }

        public void Process(ISampleWriter output, WaveFormat waveFormat, CancellationToken cancToken, Func<ulong, ulong>? f = null)
        {
            var r = new Random();

            var volume = _cachedSettings.Volume;
            rtsMode = _cachedSettings.RTSMode;

            var rtsFps = _cachedSettings.RTSFPS;
            var rtsFluct = _cachedSettings.RTSFluct;

            var rtsFr = (1.0 / rtsFps);
            var percFps = (rtsFr / 100) * rtsFluct;
            var minFps = rtsFr - percFps;
            var maxFps = rtsFr + percFps;

            try
            {
                switch (_audioRenderer)
                {
                    case XSynthEngine xsynth:
                        _midiRenderer = new XSynthRenderer(xsynth);
                        break;

                    case BASSEngine bass:
                        _midiRenderer = new BASSRenderer(bass);
                        break;

                    default:
                        break;
                }

                if (_midiRenderer != null)
                {
                    _midiRenderer.ChangeVolume(volume);
                    _midiRenderer.SystemReset();

                    float[] buffer = new float[256 * waveFormat.BlockAlign];
                    long prevWriteTime = 0;
                    double deltaTime = 0;
                    byte[] scratch = new byte[16];

                    Debug.PrintToConsole(Debug.LogType.Message, $"Initialized {_midiRenderer.UniqueID}.");

                    // Prepare stream
                    if (_cachedSettings.OverrideEffects)
                    {
                        for (int i = 0; i <= 15; i++)
                            _midiRenderer.SendCustomFXEvents(i, _cachedSettings.ReverbVal, _cachedSettings.ChorusVal);
                    }

                    if (_events != null)
                    {
                        foreach (MIDIEvent e in _events)
                        {
                            if (cancToken.IsCancellationRequested)
                                return;

                            while (pauseConversion)
                                Thread.Sleep(500);

                            if (e is UndefinedEvent)
                                continue;

                            deltaTime += e.DeltaTime;
                            _converted = deltaTime;
                            var eb = e.GetData(scratch);

                            if (rtsMode)
                                curFrametime = r.NextDouble() * (maxFps - minFps) + minFps;

                            long writeTime = (long)((rtsMode ? RoundToNearest(deltaTime, curFrametime) : deltaTime) * waveFormat.SampleRate);

                            // If writeTime ends up being negative, clamp it to 0
                            if (writeTime < prevWriteTime)
                                writeTime = prevWriteTime;

                            var chunk = (int)((writeTime - prevWriteTime) * waveFormat.Channels);

                            // Never EVER go back in time!!!!!!
                            if (writeTime > prevWriteTime) // <<<<< EVER!!!!!!!!!
                                prevWriteTime = writeTime;

                            while (chunk > 0)
                            {
                                bool smallChunk = chunk < buffer.Length;
                                int readData = _midiRenderer.Read(buffer, 0, chunk, smallChunk ? chunk : buffer.Length);

                                if (readData > 0)
                                {
                                    output.Write(buffer, 0, smallChunk ? chunk : buffer.Length);
                                    chunk = smallChunk ? 0 : chunk - buffer.Length;
                                }
                            }

                            switch (e)
                            {
                                case ControlChangeEvent ev:
                                    if (!(_audioRenderer.CachedSettings.OverrideEffects && (ev.Controller == 0x5B || ev.Controller == 0x5D)))
                                        goto default;

                                    break;

                                case ProgramChangeEvent:
                                    if (!_audioRenderer.CachedSettings.IgnoreProgramChanges)
                                        goto default;

                                    break;

                                case NoteOnEvent:
                                    playedNotes++;
                                    goto default;

                                case MIDIPortEvent:
                                case ChannelPrefixEvent:
                                    break;

                                default:
                                    _midiRenderer.SendEvent(eb);
                                    break;
                            }

                            if (f != null)
                                _ = f(1);
                            //_renderingTime = bass.RenderingTime;

                            _file.Return(e);

                            _processedEvents++;
                        }
                    }

                    _midiRenderer.SendEndEvent();

                    while (_midiRenderer.ActiveVoices > 0)
                    {
                        _midiRenderer.Read(buffer, 0, 0, buffer.Length);
                        output.Write(buffer, 0, buffer.Length);
                    }
                }

                output.Flush();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Debug.PrintToConsole(Debug.LogType.Warning, $"{_midiRenderer?.UniqueID} - DataParsingError {ex.Message}");
            }
        }
    }
}
