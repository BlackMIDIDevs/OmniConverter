using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Color = Avalonia.Media.Color;

namespace OmniConverter
{
    public class UpdateSystem
    {
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int connDescription, int ReservedValue);

        public static string ProductName = "OmniConverter";
        public static Octokit.GitHubClient UpdateClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(ProductName));

        public static string SetupFile = "https://github.com/KaleidonKep99/OmniConverter/releases/download/{0}/OmniConverterSetup.exe";
        public static string UpdatePage = "https://github.com/KaleidonKep99/OmniConverter/releases/tag/{0}";

        public const int NORMAL = 0x0;
        public const int USERFOLDER_PATH = 0x1;
        public const int WIPE_SETTINGS = 0xF;

        private static Window? _owner = null;

        public enum Branch
        {
            None = -1,
            Delay,
            Release,
            Canary,
            Total = Canary
        }

        public static bool IsInternetAvailable()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        public static void CheckThenUpdate(String ReturnVal, Int32 InstallMode)
        {
            if (!ReturnVal.Equals("0.0.0.0"))
            {
                /*
                DLEngine frm = new DLEngine(ReturnVal, String.Format("Downloading update {0}...", ReturnVal, @"{0}"), null, null, InstallMode);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                */

                CheckChangelog();
            }
        }

        public static void TriggerUpdateWindow(Version CurVer, Version OLVer, String newestversion, bool forced, bool startup)
        {
            string? UTitle = null;
            string? UText = null;
            string? RVal = "0.0.0.0";

            if (forced && startup) CheckThenUpdate(newestversion, UpdateSystem.NORMAL);
            else
            {
                if (OLVer == CurVer && forced)
                {
                    UTitle = String.Format("Reinstall version ({0})", CurVer.ToString());
                    UText = String.Format("Would you like to reinstall OmniConverter?\nCurrent version online is {0}, the same as yours.\n\nPress Yes to confirm, or No to close the window.", CurVer.ToString());
                    RVal = CurVer.ToString();
                }
                else if (OLVer < CurVer && forced)
                {
                    UTitle = String.Format("Downgrade to version {0}", OLVer.ToString());
                    UText = String.Format("Are you sure you want to downgrade OmniConverter?\nCurrent version online is {0}, you have {1}.\n\nPress Yes to confirm, or No to close the window.", OLVer.ToString(), CurVer.ToString());
                    RVal = OLVer.ToString();
                }
                else
                {
                    UTitle = String.Format("Update found ({0})", OLVer.ToString());
                    UText = String.Format("A new update for OmniConverter has been found.\nCurrent version online is {0}, you have {1}.\n\nWould you like to update now?", OLVer.ToString(), CurVer.ToString());
                    RVal = OLVer.ToString();
                }

                var msg = MessageBox.Show(_owner, UText, UTitle, MsBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Question);
                if (msg == MessageBox.MsgBoxResult.Yes) CheckThenUpdate(RVal, UpdateSystem.NORMAL);
            }
        }

        public static void NoUpdates(bool startup, bool internetok)
        {
            if (!startup)
                MessageBox.Show(_owner, "No updates for the converter have been found.", $"{ProductName} - No updates found", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);
        }

        public static void CheckChangelog()
        {
            bool internetok = IsInternetAvailable();
            if (internetok == false)
            {
                MessageBox.Show(_owner, "There's no Internet connection.\n\nYou can't see the changelog without one.", $"{ProductName} - No Internet connection available", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            }
            else
            {
                try
                {
                    Octokit.Release Release = UpdateClient.Repository.Release.GetLatest("KeppySoftware", "OmniConverter").Result;
                    Process.Start(String.Format(UpdatePage, Release.TagName));
                }
                catch (Exception)
                {
                    MessageBox.Show(_owner, "An error has occurred while trying to show you the latest changelog.\nPlease try again later.", $"{ProductName} - Unknown Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                }
            }
        }

        public static string GetCurrentBranch()
        {
            switch (Program.Settings.UpdateBranch)
            {
                case Branch.Canary:
                    return "Canary branch";

                case Branch.Release:
                    return "Release branch";

                case Branch.Delay:
                    return "Delayed branch";

                case Branch.None:
                default:
                    return "No branch selected";
            }
        }

        public static Color GetCurrentBranchColor()
        {
            switch (Program.Settings.UpdateBranch)
            {
                case Branch.Canary:
                    return Color.FromRgb(221, 172, 5);

                case Branch.Release:
                    return Color.FromRgb(158, 14, 204);

                case Branch.Delay:
                    return Color.FromRgb(84, 110, 122);

                case Branch.None:
                default:
                    return Color.FromRgb(182, 0, 0);
            }
        }

        public static string GetCurrentBranchToolTip()
        {
            switch (Program.Settings.UpdateBranch)
            {
                case Branch.Canary:
                    return "Receive all updates.\nYou may get broken updates that haven't been fully tested.\nDesigned for testers and early adopters.";

                case Branch.Release:
                    return "Receive occasional updates and urgent bugfixes (Eg. from version x.0.x.x to x.1.x.x).\nRecommended.";

                case Branch.Delay:
                    return "You will only get major releases (Eg. from version 0.x.x.x to 1.x.x.x).\nFor those who do not wish to update often.\nNot recommended.";

                case Branch.None:
                default:
                    return "No information, since you didn't chose a branch.";
            }
        }

        public static void CheckForUpdates(bool forced, bool startup, Window? owner = null)
        {
            _owner = owner;

            bool internetok = IsInternetAvailable();
            if (internetok == false)
                NoUpdates(startup, false);
            else
            {
                try
                {
                    Octokit.Release Release = UpdateClient.Repository.Release.GetAll("KaleidonKep99", "OmniConverter").Result[0];

                    Version? convOnline = null;
                    Version.TryParse(Release.TagName, out convOnline);
                    Version? convCurrent = Assembly.GetExecutingAssembly().GetName().Version;

                    if (convCurrent != null && convOnline != null)
                    {
                        switch (Program.Settings.UpdateBranch)
                        {
                            case Branch.Canary:
                                if (convCurrent.Major < convOnline.Major || convCurrent.Minor < convOnline.Minor)
                                {
                                    if ((convCurrent.Build >= convOnline.Build || convCurrent.Build < convOnline.Build))
                                        TriggerUpdateWindow(convCurrent, convOnline, Release.TagName, forced, startup);
                                    else
                                    {
                                        if (forced)
                                            TriggerUpdateWindow(convCurrent, convOnline, convCurrent.ToString(), forced, startup);
                                        else
                                            NoUpdates(startup, internetok);
                                    }
                                }
                                else
                                {
                                    if (forced)
                                        TriggerUpdateWindow(convCurrent, convOnline, convCurrent.ToString(), forced, startup);
                                    else
                                        NoUpdates(startup, internetok);
                                }
                                break;

                            case Branch.Delay:
                                if (convCurrent.Major < convOnline.Major)
                                    TriggerUpdateWindow(convCurrent, convOnline, Release.TagName, forced, startup);
                                else
                                {
                                    if (forced)
                                        TriggerUpdateWindow(convCurrent, convOnline, convCurrent.ToString(), forced, startup);
                                    else
                                        NoUpdates(startup, internetok);
                                }
                                break;

                            case Branch.Release:
                            default:
                                if (convCurrent.Major < convOnline.Major)
                                {
                                    if ((convCurrent.Minor >= convOnline.Minor || convCurrent.Minor < convOnline.Minor))
                                        TriggerUpdateWindow(convCurrent, convOnline, Release.TagName, forced, startup);
                                    else
                                    {
                                        if (forced)
                                            TriggerUpdateWindow(convCurrent, convOnline, convCurrent.ToString(), forced, startup);
                                        else
                                            NoUpdates(startup, internetok);
                                    }
                                }
                                else
                                {
                                    if (forced)
                                        TriggerUpdateWindow(convCurrent, convOnline, convCurrent.ToString(), forced, startup);
                                    else
                                        NoUpdates(startup, internetok);
                                }
                                break;

                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(_owner, "An error has occurred while checking for updates.\nPlease try again later.", $"{ProductName} - Unknown Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                    NoUpdates(startup, internetok);
                }
            }
        }
    }
}