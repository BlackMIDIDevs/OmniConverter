using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using OmniConverter.Extensions;
using System;

namespace OmniConverter;

public partial class MIDIWindow : Window
{
    private MIDIWorker? _worker = null;
    private string[]? _files;
    private bool _import = true;
    private bool _silent = false;
    private DispatcherTimer _feeder = new DispatcherTimer();

    private MainWindow _winRef;

    public MIDIWindow(MainWindow winRef, string[]? files, bool silent = false, bool import = true)
    {
        InitializeComponent();
        Closing += MIDIWindow_OnClosing;

        _import = import;
        _silent = silent;
        _files = files;
        _winRef = winRef;

        LogArea.Loaded += FireUpThread;
    }

    private async void FireUpThread(object? sender, EventArgs e)
    {
        _feeder.Tick += FeederTick;
        _feeder.Interval = TimeSpan.FromMilliseconds(100);

        if (_import && _files != null)
        {
            Title = "MIDI analysis";
            _worker = new MIDIAnalysis(_files, _silent, Program.Settings.MultiThreadedMode ? Program.Settings.ThreadsCount : 1, this, LogArea, _winRef.MIDIs);
        }
        else
        {
            if (_winRef.MIDIs.Count > 0)
            {
                Title = "MIDI converter";
                string outputFolder = string.Empty;

                if (!Program.Settings.AutoExportToFolder)
                {
                    var topLevel = GetTopLevel(this);

                    if (topLevel != null)
                    {
                        var startPath = await StorageProvider.TryGetFolderFromPathAsync(Program.Settings.LastExportFolder);
                        var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                        {
                            SuggestedStartLocation = startPath,
                            Title = "Selected export folder"
                        });

                        if (folder.Count == 1)
                        {
                            LogAreaGroup.Header = "Active threads";

                            string? path = folder[0].TryGetLocalPath();

                            if (path != null)
                            {
                                outputFolder = path;
                                Program.Settings.LastExportFolder = path;
                                Program.SaveConfig();
                            }
                        }
                    }
                }
                else outputFolder = Program.Settings.AutoExportFolderPath;

                if (!string.IsNullOrEmpty(outputFolder))
                    _worker = new MIDIConverter(outputFolder, Program.Settings.AudioCodec, Program.Settings.MultiThreadedMode ? Program.Settings.ThreadsCount : 1, this, LogArea, _winRef.MIDIs);
            }
            else MessageBox.Show(this, "There are no MIDIs to convert.", "OmniConverter - Information");
        }

        if (_worker != null)
        {
            if (_worker.StartWork())
            {
                _feeder.Start();
                return;
            }   
        }

        Close();
    }

    private void FeederTick(object? sender, EventArgs e)
    {
        string customTitle = _worker?.GetCustomTitle() ?? string.Empty;

        if (!string.IsNullOrEmpty(customTitle))
            Title = customTitle;

        MIDIStatus.Content = _worker?.GetStatus();
        Progress.Value = _worker?.GetProgress() ?? 0;

        if (TrackProgress.IsVisible)
            TrackProgress.Value = ((MIDIConverter?)_worker).GetTracksProgress();
        
        Platform.SetTaskbarProgress(_winRef, Platform.TaskbarState.Normal, (ulong)Progress.Value, 100);
    }

    private void CancelBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_worker != null)
        {
            if (_worker.IsRunning())
            {
                var dr = MessageBox.Show(this, "Are you sure you want to terminate the current process?", "The converter is still busy", MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Warning);
                switch (dr)
                {
                    case MessageBox.MsgBoxResult.Yes:
                        _worker.CancelWork();
                        break;
                    default:
                    case MessageBox.MsgBoxResult.No:
                        return;
                }
            }

            while (_worker.IsRunning()) ;

            _worker.Dispose();
        }

        Close();
    }

    public void EnableTrackProgress(bool enable)
    {
        ProgressBars.RowDefinitions = enable ? new("1*, 1*") : new("1*, 0*");
        TrackProgress.IsVisible = enable;
    }

    private void MIDIWindow_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;

        if (_winRef != null)
            _winRef.NullMIDIWindow(this);
        
        _feeder.Tick -= FeederTick;
        _feeder.Stop();
        Platform.SetTaskbarProgress(_winRef, Platform.TaskbarState.NoProgress);

        Closing -= MIDIWindow_OnClosing;
        Close();
    }
}