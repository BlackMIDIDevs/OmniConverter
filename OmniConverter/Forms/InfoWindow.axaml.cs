using System;
using System.Diagnostics;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using ManagedBass;
using ManagedBass.Midi;

namespace OmniConverter;

public partial class InfoWindow : Window
{
    public InfoWindow()
    {
        InitializeComponent();

        var cv = Assembly.GetExecutingAssembly().GetName().Version;

        var dummy = new Version(0, 0, 0, 0);
        Version? bassVer = dummy;
        Version? bmidiVer = dummy;
        Version convVer = cv != null ? cv : dummy;

        try { bassVer = Bass.Version; } catch { }
        try { bmidiVer = BassMidi.Version; } catch { }

        ConvBrand.Content = MiscFunctions.ReturnAssemblyVersion("OmniConverter", "CR", [convVer.Major, convVer.Minor, convVer.Build, convVer.MinorRevision]);
        BASSVersion.Content = MiscFunctions.ReturnAssemblyVersion(string.Empty, "Rev. ", [bassVer.Major, bassVer.Minor, bassVer.Build, bassVer.MinorRevision]);
        BMIDIVersion.Content = MiscFunctions.ReturnAssemblyVersion(string.Empty, "Rev. ", [bmidiVer.Major, bmidiVer.Minor, bmidiVer.Build, bmidiVer.MinorRevision]);

        SetBranchColor();
    }

    private void SetBranchColor()
    {
        SolidColorBrush brchCol = new SolidColorBrush();
        brchCol.Color = UpdateSystem.GetCurrentBranchColor();
        CurUpdateBranch.Content = UpdateSystem.GetCurrentBranch();
        CurUpdateBranch.Foreground = brchCol;
    }

    private void GitHubPage(object? sender, PointerPressedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/KaleidonKep99/OmniConverter") { UseShellExecute = true });
    }

    private void LicensePage(object? sender, PointerPressedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/KaleidonKep99/OmniConverter/blob/master/LICENSE.md") { UseShellExecute = true });
    }

    private async void ChangeBranch(object? sender, RoutedEventArgs e)
    {
        await new ChangeBranch().ShowDialog(this);
        SetBranchColor();
    }

    private void CheckForUpdates(object? sender, RoutedEventArgs e)
    {
        UpdateSystem.CheckForUpdates(false, false, this);
    }

    private void CloseWindow(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}