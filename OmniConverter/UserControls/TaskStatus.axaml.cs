using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace OmniConverter;

public partial class TaskStatus : UserControl
{
    private DispatcherTimer _feeder = new DispatcherTimer();
    private string _extTitle = string.Empty;
    private StackPanel? _parent = null;

    private string _title = string.Empty;
    private double _progress = 0;
    private double _estimatedRenderTime = 0;

    public TaskStatus()
    {
        InitializeComponent();
    }

    public TaskStatus(string? text, StackPanel? parent)
    {
        InitializeComponent();

        if (text != null)
            _extTitle = text;

        if (parent != null)
        {
            _parent = parent;
            _parent.Children.Add(this);
        }
            
        _feeder.Tick += FeederTick;
        _feeder.Interval = TimeSpan.FromMilliseconds(100);
        _feeder.Start();

    }

    public void Dispose()
    {
        _feeder.Stop();

        if (_parent != null)
            _parent.Children.Remove(this);
    }

    private void FeederTick(object? sender, EventArgs e)
    {
        JobDescription.Content = _title;
        Progress.Value = _progress;
    }

    public void UpdateTitle(string text)
    {
        if (!string.IsNullOrEmpty(text))
            _title = $"{text} - {_extTitle}";
        else
            _title = _extTitle;
    }

    public void UpdateTitle(EventsProcesser renderer)
    {
        string title = $"{renderer.ActiveVoices.ToString(",0", CultureInfo.InvariantCulture)} voices";

        if (renderer.IsRTS)
            title += $", RTS @ {renderer.Framerate.ToString("0.0")}FPS";

        if (_estimatedRenderTime != 0 && (!double.IsNaN(_estimatedRenderTime) && !double.IsInfinity(_estimatedRenderTime)))
            title += $" >> Estimated time left is {TimeSpan.FromSeconds(_estimatedRenderTime)}";

        UpdateTitle(title);
    }

    public void UpdateProgress(double progress)
    {
        _progress = progress > 100.0 ? 100.0 : progress;
    }

    public void UpdateRemainingTime(EventsProcesser renderer)
    {
        //_estimatedRenderTime = renderer.EstimatedRenderTime;
    }
}