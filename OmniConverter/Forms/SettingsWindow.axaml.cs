using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Linq;

namespace OmniConverter;

public partial class SettingsWindow : Window
{
    private bool ForceLimitAudio = false;
    private bool NoFFMPEGFound = false;

    public SettingsWindow()
    {
        InitializeComponent();

        Loaded += CheckSettings;
    }

    private void CheckSettings(object? sender, RoutedEventArgs e)
    {
        // Jank
        for (int i = 0; i < SampleRate.Items.Count; i++)
        {
            object? item = SampleRate.Items[i];

            if (item != null)
            {
                int freq = Convert.ToInt32(((ComboBoxItem)item).Content);

                if (freq == Program.Settings.Synth.SampleRate)
                {
                    SampleRate.SelectedIndex = i;
                    break;
                }
            }
        }

        var maxCodec = Program.FFmpegAvailable ? AudioCodecType.Max : AudioCodecType.PCM;
        if (maxCodec == AudioCodecType.PCM)
        {
            var items = AudioCodec.Items.Where(item => !((ComboBoxItem)item).Name.Contains("WAV"));

            foreach (var item in items.ToList())
                AudioCodec.Items.Remove(item);

            NoFFMPEGFound = true;
            AudioCodec.IsEnabled = false;
        }

        SelectedRenderer.SelectedIndex = ((int)Program.Settings.Renderer).LimitToRange((int)EngineID.BASS, (int)EngineID.MAX);
        AudioRendererChanged(sender, new SelectionChangedEventArgs(e.RoutedEvent, null, null));

        AudioCodec.SelectedIndex = ((int)Program.Settings.Encoder.AudioCodec).LimitToRange((int)AudioCodecType.PCM, (int)maxCodec);
        AudioBitrate.Value = Program.Settings.Encoder.AudioBitrate.LimitToRange(1, (int)AudioBitrate.Maximum);
        
        InterpolationSelection.SelectedIndex = ((int)Program.Settings.Synth.Interpolation)
            .LimitToRange((int)GlobalSynthSettings.InterpolationType.None,
                          (int)GlobalSynthSettings.InterpolationType.Max);

        XSynth_ThreadingSelection.SelectedIndex = ((int)Program.Settings.XSynth.Threading)
            .LimitToRange((int)XSynthSettings.ThreadingType.None, (int)XSynthSettings.ThreadingType.Max);

        BASS_MaxVoices.Value = Program.Settings.BASS.MaxVoices;
        XSynth_MaxLayers.Value = Program.Settings.XSynth.MaxLayers;
        BASS_DisableFX.IsChecked = Program.Settings.BASS.DisableEffects;
        BASS_NoteOff1.IsChecked = Program.Settings.BASS.NoteOff1;
        XSynth_UseEffects.IsChecked = Program.Settings.XSynth.UseEffects;
        XSynth_LinearEnv.IsChecked = Program.Settings.XSynth.LinearEnvelope;
        KilledNoteFading.IsChecked = Program.Settings.Synth.KilledNoteFading;
        AudioLimiter.IsChecked = Program.Settings.Synth.AudioLimiter;
        AudioCodecChanged(sender, new SelectionChangedEventArgs(e.RoutedEvent, null, null));

        RTSMode.IsChecked = Program.Settings.Synth.RTSMode;
        RTSFPS.Value = (decimal)Program.Settings.Synth.RTSFPS;
        RTSFluct.Value = (decimal)Program.Settings.Synth.RTSFluct;
        RTSModeCheck(sender, e);

        FilterVelocity.IsChecked = Program.Settings.Event.FilterVelocity;
        VelocityLowValue.Value = Program.Settings.Event.VelocityLow;
        VelocityHighValue.Value = Program.Settings.Event.VelocityHigh;
        FilterVelocityCheck(sender, e);

        FilterKey.IsChecked = Program.Settings.Event.FilterKey;
        KeyLowValue.Value = Program.Settings.Event.KeyLow;
        KeyHighValue.Value = Program.Settings.Event.KeyHigh;
        FilterKeyCheck(sender, e);

        OverrideEffects.IsChecked = Program.Settings.Event.OverrideEffects;
        ReverbValue.Value = Program.Settings.Event.ReverbVal;
        ChorusValue.Value = Program.Settings.Event.ChorusVal;
        OverrideEffectsCheck(sender, e);

        IgnoreProgramChanges.IsChecked = Program.Settings.Event.IgnoreProgramChanges;

        MTMode.IsChecked = Program.Settings.Render.MultiThreadedMode;
        PerTrackMode.IsChecked = Program.Settings.Render.PerTrackMode;
        PerTrackFile.IsChecked = Program.Settings.Render.PerTrackFile;
        PerTrackStorage.IsChecked = Program.Settings.Render.PerTrackStorage;

        NoLimitThreadsOnCPU.IsChecked = Program.Settings.Render.ThreadsCount > Environment.ProcessorCount;   
        NoLimitThreadsOnCPUCheck(sender, e);
        MaxThreads.Value = Program.Settings.Render.ThreadsCount.LimitToRange(1, (int)MaxThreads.Maximum);

        if ((bool)NoLimitThreadsOnCPU.IsChecked)
            LimitThreads.IsChecked = true;
        else
            LimitThreads.IsChecked = MaxThreads.Value < Environment.ProcessorCount;

        MaxThreadsPanel.IsEnabled = (bool)LimitThreads.IsChecked;

        AutoExportToFolder.IsChecked = Program.Settings.Render.AutoExportToFolder;
        AutoExportFolderPath.Text = Program.Settings.Render.AutoExportFolderPath;
        AutoExportFolderCheck(sender, e);

        AfterRenderAction.IsChecked = Program.Settings.Program.AfterRenderAction >= 0;
        AfterRenderSelectedAction.SelectedIndex = Program.Settings.Program.AfterRenderAction.LimitToRange(0, AfterRenderSelectedAction.Items.Count);
        AfterRenderActionCheck(sender, e);

        NoFFMPEG.IsVisible = NoFFMPEGFound;
        AudioEvents.IsChecked = Program.Settings.Program.AudioEvents;
        OldKMCScheme.IsChecked = Program.Settings.Program.OldKMCScheme;
        AudioEventsCheck(sender, e);
    }

    private async void AutoExportFolderSelection(object? sender, RoutedEventArgs e)
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
                string? path = folder[0].TryGetLocalPath();

                if (path != null && AutoExportFolderPath.Text != null)
                    AutoExportFolderPath.Text = path;
            }
        }
    }

    private void AudioEventsCheck(object? sender, RoutedEventArgs e)
    {
        if (AudioEvents.IsChecked != null)
            OldKMCScheme.IsEnabled = (bool)AudioEvents.IsChecked;
    }

    private void MaxVoicesChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (BASS_MaxVoices.Value > BASS_MaxVoices.Maximum)
            BASS_MaxVoices.Value = BASS_MaxVoices.Maximum;
    }

    private void AudioCodecChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (AudioCodec != null)
        {
            AudioBitrate.IsEnabled = AudioCodec.SelectedIndex > 1;

            switch ((AudioCodecType)AudioCodec.SelectedIndex)
            {
                case AudioCodecType.LAME:
                    ForceLimitAudio = true;
                    AudioLimiter.IsChecked = ForceLimitAudio;
                    break;

                default:
                    ForceLimitAudio = false;
                    AudioLimiter.IsChecked = Program.Settings.Synth.AudioLimiter;
                    break;
            }

            AudioLimiter.IsEnabled = !ForceLimitAudio;
        }
    }

    private void KhangModCheck(object? sender, RoutedEventArgs e)
    {
        if (BASS_NoVoiceLimit.IsChecked != null)
        {
            BASS_MaxVoices.Maximum = (bool)BASS_NoVoiceLimit.IsChecked ? int.MaxValue : 100000;
            MaxVoicesChanged(null, new NumericUpDownValueChangedEventArgs(e.RoutedEvent, null, null));
        }
    }

    private void UnlimitedLayersCheck(object? sender, RoutedEventArgs e)
    {
        if (XSynth_NoLayerLimit.IsChecked != null)
        {
            if ((bool)XSynth_NoLayerLimit.IsChecked)
                XSynth_MaxLayers.Minimum = 0;
            else
                XSynth_MaxLayers.Minimum = 1;
            XSynth_MaxLayers.Value = (bool)XSynth_NoLayerLimit.IsChecked ? 0 : Program.Settings.XSynth.MaxLayers;
            XSynth_MaxLayers.IsEnabled = !(bool)XSynth_NoLayerLimit.IsChecked;
        }
    }

    private void AudioRendererChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SelectedRenderer != null)
        {
            switch ((EngineID)SelectedRenderer.SelectedIndex)
            {
                case EngineID.XSynth:
                    XSynth_NoLayerLimit.IsChecked = Program.Settings.XSynth.MaxLayers == 0;
                    XSynth_MaxLayers.Value = Program.Settings.XSynth.MaxLayers;
                    BASSSettingsPanel.IsVisible = false;
                    XSynthSettingsPanel.IsVisible = true;
                    break;

                case EngineID.BASS:
                    BASS_NoVoiceLimit.IsChecked = Program.Settings.BASS.MaxVoices > 100000;
                    BASS_MaxVoices.Value = Program.Settings.BASS.MaxVoices;
                    BASSSettingsPanel.IsVisible = true;
                    XSynthSettingsPanel.IsVisible = false;
                    break;
                
                default:
                    break;
            }
        }
    }

    private void NoFFMPEGWarning(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        MessageBox.Show(this, "To use additional formats, you need ffmpeg.\n\n" +
            "Please install it on your system, or move the ffmpeg binary to the same folder as the converter.",
            "OmniConverter - Warning", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Warning);
    }

    private void MTModeCheck(object? sender, RoutedEventArgs e)
    {
        if (MTMode.IsChecked != null)
            LimitThreadsPanel.IsEnabled = (bool)MTMode.IsChecked;
    }

    private void OverrideEffectsCheck(object? sender, RoutedEventArgs e)
    {
        if (OverrideEffects.IsChecked != null)
        {
            ReverbValue.IsEnabled = (bool)OverrideEffects.IsChecked;
            ChorusValue.IsEnabled = (bool)OverrideEffects.IsChecked;
        }
    }

    private void FilterVelocityCheck(object? sender, RoutedEventArgs e)
    {
        if (FilterVelocity.IsChecked != null)
        {
            VelocityLowValue.IsEnabled = (bool)FilterVelocity.IsChecked;
            VelocityHighValue.IsEnabled = (bool)FilterVelocity.IsChecked;
        }
    }

    private void FilterKeyCheck(object? sender, RoutedEventArgs e)
    {
        if (FilterKey.IsChecked != null)
        {
            KeyLowValue.IsEnabled = (bool)FilterKey.IsChecked;
            KeyHighValue.IsEnabled = (bool)FilterKey.IsChecked;
        }
    }

    private void PerTrackModeCheck(object? sender, RoutedEventArgs e)
    {
        if (PerTrackMode.IsEnabled)
        {
            if (PerTrackMode.IsChecked != null)
            {
                PerTrackFile.IsEnabled = (bool)PerTrackMode.IsChecked;
                PerTrackStorage.IsEnabled = (bool)PerTrackMode.IsChecked;
            }

            PerTrackFileCheck(sender, e);
        }
    }

    private void PerTrackFileCheck(object? sender, RoutedEventArgs e)
    {
        if (PerTrackFile.IsEnabled)
        {
            if (PerTrackFile.IsChecked != null)
                PerTrackStorage.IsEnabled = (bool)PerTrackFile.IsChecked;
        }
    }

    private void LimitThreadsCheck(object? sender, RoutedEventArgs e)
    {
        if (LimitThreads.IsChecked != null)
            MaxThreadsPanel.IsEnabled = (bool)LimitThreads.IsChecked;
    }

    private void NoLimitThreadsOnCPUCheck(object? sender, RoutedEventArgs e)
    {
        if (NoLimitThreadsOnCPU.IsChecked != null)
        {
            bool val = (bool)NoLimitThreadsOnCPU.IsChecked;

            MaxThreads.Maximum = val ? 65536 : Environment.ProcessorCount;

            if (val && MaxThreads.Value > Environment.ProcessorCount)
                MaxThreads.Value = Environment.ProcessorCount;
        }
    }

    private void RTSModeCheck(object? sender, RoutedEventArgs e)
    {
        if (RTSMode.IsChecked != null)
            RTSPanel.IsEnabled = (bool)RTSMode.IsChecked;
    }

    private void AutoExportFolderCheck(object? sender, RoutedEventArgs e)
    {
        if (AutoExportToFolder.IsChecked != null)
            AutoExportFolderPanel.IsEnabled = (bool)AutoExportToFolder.IsChecked;
    }

    private void AfterRenderActionCheck(object? sender, RoutedEventArgs e)
    {
        if (AfterRenderAction.IsChecked != null)
            AfterRenderSelectedAction.IsEnabled = (bool)AfterRenderAction.IsChecked;
    }

    private void ApplySettings(object? sender, RoutedEventArgs e)
    {
        bool success = true;

        object? item = SampleRate.Items[SampleRate.SelectedIndex];
        if (item != null)
            Program.Settings.Synth.SampleRate = Convert.ToInt32(((ComboBoxItem)item).Content);

        if (BASS_MaxVoices.Value != null)
            Program.Settings.BASS.MaxVoices = (int)BASS_MaxVoices.Value;
        if (XSynth_MaxLayers.Value != null)
            Program.Settings.XSynth.MaxLayers = (ulong)XSynth_MaxLayers.Value;

        Program.Settings.Renderer = (EngineID)SelectedRenderer.SelectedIndex;

        Program.Settings.Synth.Interpolation = (GlobalSynthSettings.InterpolationType)InterpolationSelection.SelectedIndex;
        Program.Settings.XSynth.Threading = (XSynthSettings.ThreadingType)XSynth_ThreadingSelection.SelectedIndex;

        Program.Settings.Encoder.AudioCodec = (AudioCodecType)AudioCodec.SelectedIndex;
        if (AudioBitrate.Value != null)
            Program.Settings.Encoder.AudioBitrate = (int)AudioBitrate.Value;

        if (BASS_DisableFX.IsChecked != null)
            Program.Settings.BASS.DisableEffects = (bool)BASS_DisableFX.IsChecked;
        if (BASS_NoteOff1.IsChecked != null)
            Program.Settings.BASS.NoteOff1 = (bool)BASS_NoteOff1.IsChecked;

        if (XSynth_LinearEnv.IsChecked != null)
            Program.Settings.XSynth.LinearEnvelope = (bool)XSynth_LinearEnv.IsChecked;
        if (XSynth_UseEffects.IsChecked != null)
            Program.Settings.XSynth.UseEffects = (bool)XSynth_UseEffects.IsChecked;

        if (KilledNoteFading.IsChecked != null)
            Program.Settings.Synth.KilledNoteFading = (bool)KilledNoteFading.IsChecked;

        if (AudioLimiter.IsChecked != null && !ForceLimitAudio)
            Program.Settings.Synth.AudioLimiter = (bool)AudioLimiter.IsChecked;

        if (RTSMode.IsChecked != null)
            Program.Settings.Synth.RTSMode = (bool)RTSMode.IsChecked;
        if (RTSFPS.Value != null)
            Program.Settings.Synth.RTSFPS = (double)RTSFPS.Value;
        if (RTSFluct.Value != null)
            Program.Settings.Synth.RTSFluct = (double)RTSFluct.Value;

        if (FilterVelocity.IsChecked != null)
            Program.Settings.Event.FilterVelocity = (bool)FilterVelocity.IsChecked;
        if (VelocityLowValue.Value != null)
            Program.Settings.Event.VelocityLow = (int)VelocityLowValue.Value;
        if (VelocityHighValue.Value != null)
            Program.Settings.Event.VelocityHigh = (int)VelocityHighValue.Value;
        
        if (FilterKey.IsChecked != null)
            Program.Settings.Event.FilterKey = (bool)FilterKey.IsChecked;
        if (KeyLowValue.Value != null)
            Program.Settings.Event.KeyLow = (int)KeyLowValue.Value;
        if (KeyHighValue.Value != null)
            Program.Settings.Event.KeyHigh = (int)KeyHighValue.Value;

        if (OverrideEffects.IsChecked != null)
            Program.Settings.Event.OverrideEffects = (bool)OverrideEffects.IsChecked;
        if (ReverbValue.Value != null)
            Program.Settings.Event.ReverbVal = (short)ReverbValue.Value;
        if (ChorusValue.Value != null)
            Program.Settings.Event.ChorusVal = (short)ChorusValue.Value;

        if (IgnoreProgramChanges.IsChecked != null)
            Program.Settings.Event.IgnoreProgramChanges = (bool)IgnoreProgramChanges.IsChecked;

        if (MTMode.IsChecked != null)
            Program.Settings.Render.MultiThreadedMode = (bool)MTMode.IsChecked;
        if (PerTrackMode.IsChecked != null)
            Program.Settings.Render.PerTrackMode = (bool)PerTrackMode.IsChecked;
        if (PerTrackFile.IsChecked != null)
            Program.Settings.Render.PerTrackFile = (bool)PerTrackFile.IsChecked;
        if (PerTrackStorage.IsChecked != null)
            Program.Settings.Render.PerTrackStorage = (bool)PerTrackStorage.IsChecked;

        if (LimitThreads.IsChecked != null)
        {
            if ((bool)LimitThreads.IsChecked && MaxThreads.Value != null)
                Program.Settings.Render.ThreadsCount = (int)MaxThreads.Value;
            else Program.Settings.Render.ThreadsCount = Environment.ProcessorCount;
        }

        if (AutoExportToFolder.IsChecked != null)
            Program.Settings.Render.AutoExportToFolder = (bool)AutoExportToFolder.IsChecked;

        if (AfterRenderAction.IsChecked != null && (bool)AfterRenderAction.IsChecked)
            Program.Settings.Program.AfterRenderAction = AfterRenderSelectedAction.SelectedIndex;
        else Program.Settings.Program.AfterRenderAction = -1;

        if (AudioEvents.IsChecked != null)
            Program.Settings.Program.AudioEvents = (bool)AudioEvents.IsChecked;

        if (OldKMCScheme.IsChecked != null)
            Program.Settings.Program.OldKMCScheme = (bool)OldKMCScheme.IsChecked;

        var newExportPath = AutoExportFolderPath.Text;
        if (newExportPath != null && !newExportPath.Equals(Program.Settings.Render.AutoExportFolderPath))
        {
            if (!Directory.Exists(newExportPath))
            {
                MessageBox.Show(this, $"\"{newExportPath}\" does not exist.\n\nPlease pick a valid export path.", "OmniConverter - Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                success = false;
            }

            // Test the directory for permissions
            if (success)
            {
                FileStream? testStream = null;
                string testFile = $"{newExportPath}/testFile.123";
                try
                {
                    testStream = File.Create(testFile);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(this, $"You do not have enough permissions to write to \"{newExportPath}\".\n\nPlease pick a path you have write access to.", "OmniConverter - Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                    success = false;
                }
                catch (IOException)
                {
                    MessageBox.Show(this, $"An I/O error has occurred while testing the path \"{newExportPath}\" for write permissions.\n\nPlease pick another path.", "OmniConverter - Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                    success = false;
                }
                finally
                {
                    if (testStream != null)
                    {
                        testStream.Dispose();
                        File.Delete(testFile);
                    }

                    if (success) Program.Settings.Render.AutoExportFolderPath = newExportPath;
                }
            }
        }

        if (success)
        {
            Program.SaveConfig();
            Close();
            return;
        }

        Program.LoadConfig();
        CheckSettings(sender, e);
    }
}