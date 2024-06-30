using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ManagedBass.Midi;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OmniConverter;

public partial class SoundFontsManager : Window
{
    public SoundFontsManager()
    {
        InitializeComponent();

        SoundFontListView.ItemsSource = Program.SoundFontsManager.GetSoundFontList();

        AddHandler(DragDrop.DropEvent, FileDropInit);
        AddHandler(DragDrop.DragEnterEvent, FileDropEnter);
        // AddHandler(DragDrop.DragLeaveEvent, FileDropLeave);
    }

    private void AddSFCheck(IEnumerable<IStorageItem>? files, bool dragndrop = false)
    {
        if (files == null)
            return;

        if (files.Count() >= 1)
        {
            List<string> filenames = new();

            foreach (var file in files)
            {
                var p = file.TryGetLocalPath();
                if (p != null) filenames.Add(p);
            }

            if (filenames.Count < 1)
                return;

            if (!dragndrop)
            {
                var folder = Path.GetDirectoryName(filenames[0]);

                if (folder != null)
                {
                    Program.Settings.LastSoundFontFolder = folder;
                    Program.SaveConfig();
                }
            }

            foreach (var filename in filenames)
            {
                var err = BassMidi.FontInit(filename, FontInitFlags.Unicode);

                if (err != 0)
                {
                    BassMidi.FontFree(err);
                    Program.SoundFontsManager.Add(new SoundFont(filename, -1, -1, -1, 0, 0, true, false));
                }
            }
        }
    }

    private void RefreshList()
    {
        // Reassign the list, to make sure we update the indexes
        Program.SaveConfig();
        SoundFontListView.ItemsSource = Program.SoundFontsManager.GetSoundFontList();
    }

    public void SelectedSFChanged(object? sender, SelectionChangedEventArgs e)
    {
        int index = SoundFontListView.SelectedIndex;

        if (index != -1)
        {
            var item = Program.SoundFontsManager.GetSoundFontList()[index];

            XGMode.IsChecked = item.XGMode;
            Enabled.IsChecked = item.Enabled;

            SourceBank.Minimum = item.IsSFZ() ? 0 : -1;
            SourcePreset.Minimum = item.IsSFZ() ? 0 : -1;
            DestinationPreset.Minimum = item.IsSFZ() ? 0 : -1;

            SourceBank.Value = item.SourceBank;
            SourcePreset.Value = item.SourcePreset;
            DestinationBank.Value = item.DestinationBank;
            DestinationBankLSB.Value = item.DestinationBankLSB;
            DestinationPreset.Value = item.DestinationPreset;
            return;
        }

        XGMode.IsChecked = false;
        Enabled.IsChecked = false;

        SourceBank.Minimum = -1;
        SourcePreset.Minimum = -1;
        DestinationPreset.Minimum = -1;

        SourceBank.Value = -1;
        SourcePreset.Value = -1;
        DestinationBank.Value = 0;
        DestinationBankLSB.Value = 0;
        DestinationPreset.Value = -1;
    }

    private void ApplySoundFontSettings(object? sender, RoutedEventArgs e)
    {
        int index = SoundFontListView.SelectedIndex;

        if (index != -1)
        {
            var item = (SoundFont)SoundFontListView.Items[index];

            item.SetNewValues(
                     (int)SourceBank.Value, (int)SourceBank.Value,
                     (int)DestinationPreset.Value, (int)DestinationBank.Value, (int)DestinationBankLSB.Value,
                     (bool)Enabled.IsChecked, (bool)XGMode.IsChecked);
            return;
        }

        RefreshList();
    }

    private void FileDropEnter(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.GetDataFormats().Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void FileDropInit(object? sender, DragEventArgs e)
    {
        AddSFCheck(e.Data.GetFiles());
    }

    private async void AddSoundFont(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);

        if (topLevel != null)
        {
            var startPath = await StorageProvider.TryGetFolderFromPathAsync(Program.Settings.LastSoundFontFolder);
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                SuggestedStartLocation = startPath,
                Title = "Import SoundFonts",
                FileTypeFilter = [SoundFont.SoundFontAll],
                AllowMultiple = true
            });

            AddSFCheck(files);
        }

        RefreshList();
    }

    private void RemoveSoundFont(object? sender, RoutedEventArgs e)
    {
        if (SoundFontListView.SelectedItems != null && SoundFontListView.SelectedItems.Count > 0)
        {
            // Let's copy the references to an array
            SoundFont[] list = new SoundFont[SoundFontListView.SelectedItems.Count];
            SoundFontListView.SelectedItems.CopyTo(list, 0);

            // Delete the items
            foreach (SoundFont sf in list)
                Program.SoundFontsManager.Remove(sf);
        }

        RefreshList();
    }

    private void MoveSoundFontUp(object? sender, RoutedEventArgs e)
    {
        if (SoundFontListView.SelectedItems != null && SoundFontListView.SelectedItems.Count > 0)
        {
            // Let's copy the references to an array
            SoundFont[] list = new SoundFont[SoundFontListView.SelectedItems.Count];
            SoundFontListView.SelectedItems.CopyTo(list, 0);

            // Move the items
            foreach (SoundFont sf in list)
                Program.SoundFontsManager.Move(sf, MoveDirection.Up);
        }

        RefreshList();
    }

    private void MoveSoundFontDown(object? sender, RoutedEventArgs e)
    {
        if (SoundFontListView.SelectedItems != null && SoundFontListView.SelectedItems.Count > 0)
        {
            // Let's copy the references to an array
            SoundFont[] list = new SoundFont[SoundFontListView.SelectedItems.Count];
            SoundFontListView.SelectedItems.CopyTo(list, 0);

            // Move the items
            foreach (SoundFont sf in list)
                Program.SoundFontsManager.Move(sf, MoveDirection.Down);
        }

        RefreshList();
    }

    private void CloseSoundFontManager(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}