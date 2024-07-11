using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace OmniConverter;

public partial class ChangeBranch : Window
{
    public ChangeBranch()
    {
        InitializeComponent();

        SelectedBranch.SelectedIndex = ((int)Program.Settings.UpdateBranch).LimitToRange((int)UpdateSystem.Branch.None, (int)UpdateSystem.Branch.Total);

        SolidColorBrush brchCol = new SolidColorBrush();
        brchCol.Color = UpdateSystem.GetCurrentBranchColor();
        CurrentBranch.Content = UpdateSystem.GetCurrentBranch();
        CurrentBranch.Foreground = brchCol;
    }

    private void ConfirmBranchChange(object? sender, RoutedEventArgs e)
    {
        Program.Settings.UpdateBranch = (UpdateSystem.Branch)SelectedBranch.SelectedIndex;
        Program.SaveConfig();
        Close();
    }
}