using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace OmniConverter
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<MIDI> MIDIs = new();

        private DispatcherTimer _volumeWatcher = new DispatcherTimer();

        private MIDIWindow? _midiWindow = null;

        public MainWindow()
        {
            InitializeComponent();

            _volumeWatcher.Interval = TimeSpan.FromSeconds(1);
            _volumeWatcher.Tick += VolumeWatcherFunc;

            OutputVolumeSlider.Value = Program.Settings.Volume * 100.0f;

            AddHandler(DragDrop.DropEvent, FileDropInit);
            AddHandler(DragDrop.DragEnterEvent, FileDropEnter);
            // AddHandler(DragDrop.DragLeaveEvent, FileDropLeave);

            if (Program.Settings.AutoUpdateCheck)
                UpdateSystem.CheckForUpdates(false, true);

            Loaded += CheckBuildTarget;
            Loaded += CheckBranch;

            CheckWatermark();
            // Program.SoundFonts.Add(new(@"/mnt/Seagate2TB/TEST/gm.sf2", -1, -1, -1, 0, 0, true, false));
        }

        public void NullMIDIWindow(Window death)
        {
            if (death == _midiWindow)
                _midiWindow = null;
        }

        private void CheckWatermark()
        {
            // Reassign the list, to make sure we update the indexes
            MIDIListView.ItemsSource = MIDIs;

            // Check if the label should be visible
            AddMIDIsLabel.IsVisible = MIDIs.Count == 0;
        }

        private void CheckBranch(object? sender, RoutedEventArgs e)
        {
            if (Program.Settings.UpdateBranch == UpdateSystem.Branch.None)
                new ChangeBranch().ShowDialog(this);
        }

        private void CheckBuildTarget(object? sender, RoutedEventArgs e)
        {
#if !WINDOWS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                MessageBox.Show(this, "We're running on Windows, but OmniConverter was compiled for a generic target. Please compile with net8.0-windows instead!");
#endif
        }

        private async Task AddMIDICheck(IEnumerable<IStorageItem>? files, bool dragndrop = false)
        {
            if (files == null)
                return;

            if (_midiWindow != null)
            {
                MessageBox.Show(this, "The converter is already rendering.", "OmniConverter - Hey");
                return;
            }

            if (files.Count() >= 1)
            {
                List<string> filenames = new();

                foreach (var file in files)
                {
                    var p = file.TryGetLocalPath();
                    if (p != null) filenames.Add(p);
                }

                if (filenames.Count > 0)
                {
                    if (!dragndrop)
                    {
                        var folder = Path.GetDirectoryName(filenames[0]);

                        if (folder != null)
                        {
                            Program.Settings.LastMIDIFolder = folder;
                            Program.SaveConfig();
                        }
                    }

                    _midiWindow = new MIDIWindow(this, filenames.ToArray(), false);
                    await _midiWindow.ShowDialog(this);
                    _midiWindow = null;
                }
            }

            CheckWatermark();
        }

        public void OutputVolumeSliderChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (OutputVolumeSlider != null)
            {
                if (!_volumeWatcher.IsEnabled)
                    _volumeWatcher.Start();

                double v = OutputVolumeSlider.Value / 100.0f;
                OutputVolumeLab.Content = $"({20 * Math.Log10(v):0.00}dB) {OutputVolumeSlider.Value:000.00}%";
            }
        }

        public void SelectedMIDIChanged(object? sender, SelectionChangedEventArgs e)
        {
            string emptys = "-";
            int index = MIDIListView.SelectedIndex;

            if (index != -1)
            {
                var item = MIDIs[index];
                var len = item.Length;

                InfoFullPath.Content = item.Path;
                InfoName.Content = item.Name;
                InfoNoteCount.Content = item.Notes.ToString("N0");
                InfoLength.Content = item.HumanReadableTime;
                InfoTracks.Content = item.Tracks;
                InfoSize.Content = item.HumanReadableSize;
                return;
            }

            InfoFullPath.Content = emptys;
            InfoName.Content = emptys;
            InfoNoteCount.Content = emptys;
            InfoLength.Content = emptys;
            InfoTracks.Content = "-:--.---";
            InfoSize.Content = "-.-- -";
        }

        private async void AddMIDI(object? sender, RoutedEventArgs e)
        {
            var topLevel = GetTopLevel(this);

            if (topLevel != null)
            {
                var startPath = await StorageProvider.TryGetFolderFromPathAsync(Program.Settings.LastMIDIFolder);
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    SuggestedStartLocation = startPath,
                    Title = "Import MIDIs",
                    FileTypeFilter = [MIDI.MidiAll],
                    AllowMultiple = true
                });

                await AddMIDICheck(files);
            }
        }

        private void RemoveMIDI(object? sender, RoutedEventArgs e)
        {
            if (MIDIListView.SelectedItems != null && MIDIListView.SelectedItems.Count > 0)
            {
                // Let's copy the references to an array
                MIDI[] list = new MIDI[MIDIListView.SelectedItems.Count];
                MIDIListView.SelectedItems.CopyTo(list, 0);

                // Delete the items
                foreach (MIDI midi in list)
                    MIDIs.Remove(midi);

                CheckWatermark();
            }
        }

        private void ClearList(object? sender, RoutedEventArgs e)
        {
            var dr = MessageBox.Show(this, "Are you sure you want to clear the list?", "OmniConverter - Warning", MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Warning);

            switch (dr)
            {
                case MessageBox.MsgBoxResult.Yes:
                    MIDIs.Clear();
                    break;

                case MessageBox.MsgBoxResult.No:
                default:
                    break;
            }

            CheckWatermark();
        }

        private void CloseConverter(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FileDropEnter(object? sender, DragEventArgs e)
        {
            e.DragEffects = e.Data.GetDataFormats().Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private async void FileDropInit(object? sender, DragEventArgs e)
        {
            await AddMIDICheck(e.Data.GetFiles(), true);
        }

        private async void ConvertMIDIs(object? sender, RoutedEventArgs e)
        {
            await new MIDIWindow(this, null, false, false).ShowDialog(this);
        }

        private async void OpenSettings(object? sender, RoutedEventArgs e)
        {
            await new SettingsWindow().ShowDialog(this);
        }

        private async void OpenSoundFontsManager(object? sender, RoutedEventArgs e)
        {
            await new SoundFontsManager().ShowDialog(this);
        }

        private async void OpenInfoWindow(object? sender, RoutedEventArgs e)
        {
            await new InfoWindow().ShowDialog(this);
        }

        private void CheckForUpdatesBtn(object? sender, RoutedEventArgs e)
        {
            UpdateSystem.CheckForUpdates(false, false, this);
        }

        private void VolumeWatcherFunc(object? sender, EventArgs e)
        {
            double v = OutputVolumeSlider.Value / 100.0f;
            Program.Settings.Volume = (float)v;
            Program.SaveConfig();
            _volumeWatcher.Stop();
        }
    }
}