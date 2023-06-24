using ManagedBass;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniConverter
{
    public partial class InfoWindow : Form
    {
        private Version Converter = new Version(0, 0, 10, 0);

        private ToolTip DynamicToolTip = new ToolTip();
        private RegistryKey WVerKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", false);

        private Version BASS = Bass.Version;
        private FileVersionInfo BASSMIDI = FileVersionInfo.GetVersionInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\bassmidi.dll");

        private string ReturnDriverAssemblyVersion(String Component, String Type, Int32[] VI)
        {
            return String.Format("{0}{1}{2}{3}{4}",
                (!String.IsNullOrEmpty(Component)) ? Component + " " : null,
                VI[0],
                String.Format(".{0}", VI[1]),
                String.Format(".{0}", VI[2]),
                (VI[3] < 1) ? "" : String.Format(" - {0}{1}", Type, VI[3])
                );
        }

        public InfoWindow()
        {
            InitializeComponent();

            VerLabel.Text = ReturnDriverAssemblyVersion(
                "OmniConverter",
                "CR",
                new int[] { Converter.Major, Converter.Minor, Converter.Build, 0 }
                );

            BASSVer.Text = ReturnDriverAssemblyVersion(
                null,
                "Rev. ",
                new int[] { BASS.Major, BASS.Minor, BASS.Build, BASS.Revision }
                );

            BASSMIDIVer.Text = ReturnDriverAssemblyVersion(
                null,
                "Rev. ",
                new int[] { BASSMIDI.FileMajorPart, BASSMIDI.FileMinorPart, BASSMIDI.FileBuildPart, BASSMIDI.FilePrivatePart }
                );

            CopyrightLabel.Text = String.Format(CopyrightLabel.Text, DateTime.Today.Year);

            CurBranch.Text = UpdateSystem.GetCurrentBranch();
            CurBranch.ForeColor = UpdateSystem.GetCurrentBranchColor();
            BranchToolTip.SetToolTip(CurBranch, UpdateSystem.GetCurrentBranchToolTip());
            if (Properties.Settings.Default.PreRelease) VerLabel.Text += " (PR)";

            // Date check :^)

            OCBigLogo.Image = IconSystem.GetResizedIcon(Properties.Resources.OCLogo, OCBigLogo.Width, OCBigLogo.Height);

            GitHubPage.Cursor = Program.SystemHandCursor;
            GitHubPage.Image = IconSystem.GetResizedIcon(Properties.Resources.Octocat, GitHubPage.Width, GitHubPage.Height);

            OMLicense.Cursor = Program.SystemHandCursor;
            OMLicense.Image = IconSystem.GetResizedIcon(Properties.Resources.License, OMLicense.Width, OMLicense.Height);

            WinName.Text = String.Format("{0}", OSInfo.Name.Replace("Microsoft ", ""));
            RAMAmount.Text = DataCheck.BytesToHumanReadableSize(Convert.ToUInt64((new ComputerInfo()).TotalPhysicalMemory));
            switch (Environment.OSVersion.Version.Major)
            {
                case 10:
                    WinVer.Text = String.Format(
                        "Version {0} ({1})\nRelease {2}, Revision {3}",
                        WVerKey.GetValue("ReleaseId", 0).ToString(),
                        Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit",
                        Environment.OSVersion.Version.Build,
                        WVerKey.GetValue("UBR", 0).ToString()
                        );
                    break;
                case 6:
                    if (Environment.OSVersion.Version.Minor > 1)
                    {
                        WinVer.Text = String.Format(
                            "Version {0}.{1}\nBuild {2}",
                            Environment.OSVersion.Version.Major,
                            Environment.OSVersion.Version.Minor,
                            Environment.OSVersion.Version.Build
                            );
                    }
                    else
                    {
                        if (Int32.Parse(Regex.Match(Environment.OSVersion.ServicePack, @"\d+").Value, NumberFormatInfo.InvariantInfo) > 0)
                        {
                            WinVer.Text = String.Format("{0}.{1}\nBuild {2}, Service Pack {3}",
                                Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor,
                                Environment.OSVersion.Version.Build, Environment.OSVersion.ServicePack);
                        }
                        else
                        {
                            WinVer.Text = String.Format("{0}.{1}\nBuild {2}",
                                Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor,
                                Environment.OSVersion.Version.Build);
                        }
                    }
                    break;
            }
        }

        private void InfoWindow_Load(object sender, EventArgs e)
        {
            // Nothing lul
        }

        private void VerLabel_Click(object sender, EventArgs e)
        {
            // new ChangelogWindow(Driver.FileVersion.ToString(), false).ShowDialog();
        }

        private void GitHubPage_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(Properties.Settings.Default.ProjectLink) { UseShellExecute = true });
        }

        private void DonateKep_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.paypal.me/Keppy99") { UseShellExecute = true });
        }

        private void DonateArd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.paypal.me/arduano") { UseShellExecute = true });
        }

        private void OMLicense_Click(object sender, EventArgs e)
        {
            // String License = File.ReadAllText(LicensePath, Encoding.UTF8);
            // new TextReader("License", License).ShowDialog();
        }

        private void ChangeBranch_Click(object sender, EventArgs e)
        {
            new SelectBranch().ShowDialog();

            CurBranch.Text = UpdateSystem.GetCurrentBranch();
            CurBranch.ForeColor = UpdateSystem.GetCurrentBranchColor();
            BranchToolTip.SetToolTip(CurBranch, UpdateSystem.GetCurrentBranchToolTip());
        }

        private void CheckForUpdates_Click(object sender, EventArgs e)
        {
            UpdateSystem.CheckForUpdates((Control.ModifierKeys == Keys.Shift), false, false);
        }

        private void GitHubPage_MouseHover(object sender, EventArgs e)
        {
            DynamicToolTip.Dispose();
            DynamicToolTip = new ToolTip();
            DynamicToolTip.ToolTipIcon = ToolTipIcon.Info;
            DynamicToolTip.ToolTipTitle = "GitHub";
            DynamicToolTip.SetToolTip(GitHubPage, "Open the official GitHub project page");
        }

        private void OMLicense_MouseHover(object sender, EventArgs e)
        {
            DynamicToolTip.Dispose();
            DynamicToolTip = new ToolTip();
            DynamicToolTip.ToolTipIcon = ToolTipIcon.Info;
            DynamicToolTip.ToolTipTitle = "License";
            DynamicToolTip.SetToolTip(OMLicense, "Read the license for this piece of software");
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
