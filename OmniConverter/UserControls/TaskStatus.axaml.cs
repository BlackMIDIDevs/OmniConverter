using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace OmniConverter;

public partial class TaskStatus : UserControl
{
    private DispatcherTimer _feeder = new DispatcherTimer();
    private string _ogTitle = string.Empty;
    private StackPanel? _parent = null;

    private string _title = string.Empty;
    private double _progress = 0.0;

    private DateTime _dtStart;
    private TimeSpan _eta;

    private OmniTask? _proc = null;
    private bool _trackMode = false;

    public TimeSpan ETA => _eta;

    public TaskStatus()
    {
        InitializeComponent();
    }

    public TaskStatus(string? title, StackPanel? parent, OmniTask? proc = null)
    {
        InitializeComponent();

        if (title != null)
            _ogTitle = title;

        if (parent != null)
        {
            _parent = parent;
            _parent.Children.Add(this);
        }

        _dtStart = DateTime.Now;

        _trackMode = title?.ToLower().Contains("track") ?? false;
        _proc = proc;   
        switch (_proc)
        {
            case EventsProcesser:
                _feeder.Tick += evProcTick;
                break;

            default:
                _feeder.Tick += genProcTick;
                break;
        }

        _feeder.Interval = TimeSpan.FromMilliseconds(100);
        _feeder.Start();
    }

    public void Dispose()
    {
        _feeder.Stop();

        if (_parent != null)
            _parent.Children.Remove(this);
    }

    public void UpdateTitle(string text)
    {
        if (_proc == null)
            _title = GetFinalTitle(text);
    }

    public void UpdateProgress(double progress)
    {
        if (_proc == null)
            _progress = progress > 100.0 ? 100.0 : progress;
    }

    private string GetFinalTitle(string text)
    {
        if (!string.IsNullOrEmpty(text))
            return _trackMode ? $"{_ogTitle} - {text}" : $"{text} - {_ogTitle}";
        else
            return _ogTitle;
    }

    private void evProcTick(object? sender, EventArgs e)
    {
        var evProc = (EventsProcesser?)_proc;

        var dtCurrent = DateTime.Now;
        var playedNotes = evProc?.PlayedNotes;
        var rtsMode = evProc?.IsRTS ?? false;
        var progress = evProc?.Progress * 100 ?? 0;
        var processed = evProc?.Processed ?? 0;
        var remaining = evProc?.Remaining ?? 0;
        var activeVoices = evProc?.ActiveVoices ?? 0;
        var framerate = evProc?.Framerate ?? 0;

        var title = $"{activeVoices.ToString("n0")} voices";

        if (rtsMode)
            title += $" @ {framerate:n}FPS";

        var speed = processed / (dtCurrent - _dtStart).TotalSeconds;
        _eta = TimeSpan.FromSeconds(remaining / (speed == 0 ? 0.001 : speed));

        title += $", ETA {MiscFunctions.TimeSpanToHumanReadableTime(ETA)}, {playedNotes:n0} notes";

        JobDescription.Content = GetFinalTitle(title);
        Progress.Value = progress > 100.0 ? 100.0 : progress;
    }

    private void genProcTick(object? sender, EventArgs e)
    {
        JobDescription.Content = _title;
        Progress.Value = _progress;
    }
}