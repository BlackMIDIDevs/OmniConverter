using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace OmniConverter;

public partial class ChangeBranch : Window
{
    public ChangeBranch()
    {
        InitializeComponent();

        SelectedBranch.SelectedIndex = ((int)Program.Settings.Program.UpdateBranch).LimitToRange(UpdateSystem.Branch.None, UpdateSystem.Branch.Total);

        SolidColorBrush brchCol = new SolidColorBrush();
        brchCol.Color = UpdateSystem.GetCurrentBranchColor();
        CurrentBranch.Content = UpdateSystem.GetCurrentBranch();
        CurrentBranch.Foreground = brchCol;
    }

    private void ConfirmBranchChange(object? sender, RoutedEventArgs e)
    {
        Program.Settings.Program.UpdateBranch = (UpdateSystem.Branch)SelectedBranch.SelectedIndex;
        Program.SaveConfig();
        Close();
    }
}