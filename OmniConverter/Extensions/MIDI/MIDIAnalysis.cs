using Avalonia.Controls;
using Avalonia.Threading;
using Octokit;
using Monad.FLParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Project = Monad.FLParser.Project;

namespace OmniConverter
{
    public class MIDIAnalysis : MIDIWorker
    {
        private string[] _files;
        private bool _silent = false;
        private ParallelOptions _parallelOptions;
        private CancellationTokenSource _cancToken = null;
        private Thread? _midiAnalysis, _pathChecker;

        private ulong _valid = 0;
        private ulong _notvalid = 0;
        private ulong _total = 0;

        private ObservableCollection<MIDI> _midiRef;
        private Window _winRef;
        private StackPanel _panelRef;

        private string _curStatus = string.Empty;
        private double _progress = 0;

        public MIDIAnalysis(string[] files, bool silent, int threads, Window winRef, StackPanel panelRef, ObservableCollection<MIDI> midiRef)
        {
            _files = files;
            _silent = silent;
            _midiRef = midiRef;
            _winRef = winRef;
            _panelRef = panelRef;

            _cancToken = new CancellationTokenSource();
            _parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Program.Settings.MultiThreadedMode ? threads.LimitToRange(1, Environment.ProcessorCount) : 1,
                CancellationToken = _cancToken.Token
            };
        }

        public override void Dispose()
        {
            _cancToken.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public override bool StartWork()
        {
            _pathChecker = new Thread(() =>
            {
                foreach (string file in _files)
                {
                    if (_cancToken.IsCancellationRequested) break;
                    CheckCount(file);
                }

                if (!_cancToken.IsCancellationRequested)
                {
                    _midiAnalysis = new Thread(midiAnalysisFunc);
                    _midiAnalysis.IsBackground = true;
                    _midiAnalysis.Start();
                }
            });

            _pathChecker.Start();
            return true;
        }

        public override void CancelWork() => _cancToken?.Cancel();

        public override string GetStatus() => _curStatus;
        public override double GetProgress() => _progress;

        public override bool IsRunning() => _midiAnalysis != null ? _midiAnalysis.IsAlive : false;

        private void UpdateInfo()
        {
            _curStatus = String.Format("Parsed {0} file(s) out of {1}.\nPlease wait...",
                (_valid + _notvalid).ToString("N0", new CultureInfo("is-IS")),
                _total.ToString("N0", new CultureInfo("is-IS")));
            _progress = Math.Round((_valid + _notvalid) * 100.0 / _total);
        }

        private void midiAnalysisFunc()
        {
            // Get last ID from array
            Int32 Index = 0;
            Int64 CurrentMaxIndex = 0;

            if (_midiRef.Count > 0)
            {
                Index = Enumerable.Range(0, _midiRef.Count).Aggregate((max, i) => (_midiRef[max]).ID > (_midiRef[i]).ID ? max : i);
                CurrentMaxIndex = (_midiRef[Index]).ID;
            }

            try
            {
                // Clear
                UpdateInfo();

                Parallel.For(0, _files.Length, _parallelOptions, T =>
                {
                    try
                    {
                        if (_cancToken.Token.IsCancellationRequested)
                            return;

                        CheckDirectory(ref CurrentMaxIndex, _files[T]);

                        UpdateInfo();
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                });
            }
            catch (OperationCanceledException) { }

            if (_notvalid > 0 && !_silent && !_cancToken.IsCancellationRequested)
            {
                _curStatus = string.Format("Out of {0} files, {1} were valid and {2} were not.",
                            (_valid + _notvalid).ToString("N0", new CultureInfo("is-IS")),
                            _valid.ToString("N0", new CultureInfo("is-IS")),
                            _notvalid.ToString("N0", new CultureInfo("is-IS")));
                _progress = 100;
            }
            else Dispatcher.UIThread.Post(_winRef.Close);
        }

        // Check if file is valid
        private string GetInfoMIDI(ref long CMI, string str, out MIDI? MIDIStruct)
        {
            MIDIStruct = null;

            if (_cancToken.IsCancellationRequested)
                return string.Empty;

            // Set MIDIStruct as null first
            TaskStatus? midiPanel = null;
            string ID = IDGenerator.GetID();

            try
            {
                Dispatcher.UIThread.Post(() => midiPanel = new TaskStatus(Path.GetFileName(str), _panelRef));

                midiPanel?.UpdateTitle($"Loading...");

                var ext = Path.GetExtension(str);

                switch (ext)
                {
                    case ".flp":
                        MIDIStruct = FLP.Load(CMI, str, Path.GetFileName(str), _parallelOptions, (p, t) =>
                        {
                            midiPanel?.UpdateTitle($"{p}/{t}");
                            midiPanel?.UpdateProgress(100 * p / t);
                        });
                        break;

                    default:
                        MIDIStruct = MIDI.Load(CMI, str, Path.GetFileName(str), _parallelOptions, (p, t) =>
                        {
                            midiPanel?.UpdateTitle($"{p}/{t}");
                            midiPanel?.UpdateProgress(100 * p / t);
                        });
                        break;
                }

                Dispatcher.UIThread.Post(() => midiPanel?.Dispose());

                return "No error.";
            }
            catch (Exception ex)
            {
                return $"Unable to load \"{str}\". Reason: {ex.Message}";
            }
        }

        private void CheckFile(ref long CMI, string str)
        {
            MIDI MIDIInfo = null;
            string infoMidiError = string.Empty;

            if (Path.GetExtension(str).ToLower() == ".mid" ||
                Path.GetExtension(str).ToLower() == ".midi" ||
                Path.GetExtension(str).ToLower() == ".kar" ||
                Path.GetExtension(str).ToLower() == ".rmi" ||
                Path.GetExtension(str).ToLower() == ".riff" ||
                Path.GetExtension(str).ToLower() == ".flp")
            {
                for (int i = 0; i < _midiRef.Count; i++)
                {
                    if (_midiRef[i].Path == str)
                    {
                        Debug.PrintToConsole(Debug.LogType.Error, $"The MIDI {str} is already present in the conversion list!");
                        _notvalid++;
                        return;
                    }
                }

                infoMidiError = GetInfoMIDI(ref CMI, str, out MIDIInfo);

                if (MIDIInfo != null && !_cancToken.IsCancellationRequested)
                {
                    _midiRef.Add(MIDIInfo);
                    _valid++;
                    return;
                }
            }

            Debug.PrintToConsole(Debug.LogType.Error, infoMidiError);
            _notvalid++;
        }

        private void CheckDirectory(ref long CMI, string Target)
        {
            try
            {
                foreach (String file in GetFiles(Target))
                {
                    if (_cancToken.IsCancellationRequested) return;
                    CheckFile(ref CMI, file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "OmniConverter - Import Error", _winRef);
            }
        }

        private void CheckCount(string Target)
        {
            try
            {
                foreach (string A in GetFiles(Target))
                    _total++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "OmniConverter - Import Error", _winRef);
            }
        }

        // Code by Mac Gravell, edited by Keppy
        // https://stackoverflow.com/a/929418
        private IEnumerable<string> GetFiles(string target)
        {
            Queue<string> analysisQueue = new Queue<string>();

            // Add target of queue to the queue
            analysisQueue.Enqueue(target);

            // Do this while the queue list still contains items
            while (analysisQueue.Count > 0)
            {
                // Dequeue the item that is going to be analyzed
                target = analysisQueue.Dequeue();

                try
                {
                    // Add each subdir to the queue
                    if (_cancToken.IsCancellationRequested) break;
                    foreach (string subDir in Directory.GetDirectories(target))
                    {
                        if (_cancToken.IsCancellationRequested) break;
                        analysisQueue.Enqueue(subDir);
                    }
                }
                catch { }

                string[] files = null;
                try
                {
                    // Add files from the directory of the queued item
                    files = Directory.GetFiles(target);
                }
                catch { }

                // If the function detected items, return them to the calling foreach loop
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (_cancToken.IsCancellationRequested) break;
                        yield return files[i];
                    }
                }

                // If the queued item is actually a direct path to the file, return it to the foreach loop
                if (File.Exists(target)) yield return target;
            }
        }
    }
}
