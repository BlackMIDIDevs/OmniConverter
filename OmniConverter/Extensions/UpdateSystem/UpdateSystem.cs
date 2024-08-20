using Avalonia.Controls;
using Octokit;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using Color = Avalonia.Media.Color;

namespace OmniConverter
{
    public class UpdateSystem
    {
        public static GitHubClient UpdateClient = new GitHubClient(new ProductHeaderValue(ProductName));

        public const string ProductName = "OmniConverter";
        public const string GitHubPage = $"https://github.com/KaleidonKep99/{ProductName}";
        public const string SetupFile = $"{GitHubPage}/releases/download/{{0}}/OmniConverterSetup.exe";
        public const string UpdatePage = $"{GitHubPage}/releases/tag/{{0}}";

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
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(GitHubPage).Result;
                    response.EnsureSuccessStatusCode();
                    return true;
                }
            }
            catch { }

            return false;
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
            string UTitle = string.Empty;
            string UText = string.Empty;
            string RVal = "0.0.0.0";

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
                    Octokit.Release Release = UpdateClient.Repository.Release.GetLatest("KaleidonKep99", "OmniConverter").Result;
                    Process.Start(String.Format(UpdatePage, Release.TagName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_owner, "An error has occurred while trying to show you the latest changelog.\nPlease try again later.", $"{ProductName} - Unknown Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                }
            }
        }

        public static string GetCurrentBranch()
        {
            return Program.Settings.Program.UpdateBranch switch
            {
                Branch.Canary => "Canary branch",
                Branch.Release => "Release branch",
                Branch.Delay => "Delayed branch",
                _ => "No branch selected",
            };
        }

        public static Color GetCurrentBranchColor()
        {
            return Program.Settings.Program.UpdateBranch switch
            {
                Branch.Canary => Color.FromRgb(221, 172, 5),
                Branch.Release => Color.FromRgb(158, 14, 204),
                Branch.Delay => Color.FromRgb(84, 110, 122),
                _ => Color.FromRgb(182, 0, 0),
            };
        }

        public static string GetCurrentBranchToolTip()
        {
            return Program.Settings.Program.UpdateBranch switch
            {
                Branch.Canary => "Receive all updates.\nYou may get broken updates that haven't been fully tested.\nDesigned for testers and early adopters.",
                Branch.Release => "Receive occasional updates and urgent bugfixes (Eg. from version x.0.x.x to x.1.x.x).\nRecommended.",
                Branch.Delay => "You will only get major releases (Eg. from version 0.x.x.x to 1.x.x.x).\nFor those who do not wish to update often.\nNot recommended.",
                _ => "No branch selected.",
            };
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
                    Release Release = UpdateClient.Repository.Release.GetAll("KaleidonKep99", "OmniConverter").Result[0];

                    Version? convOnline = null;
                    Version.TryParse(Release.TagName, out convOnline);
                    Version? convCurrent = Assembly.GetExecutingAssembly().GetName().Version;

                    if (convCurrent != null && convOnline != null)
                    {
                        switch (Program.Settings.Program.UpdateBranch)
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
                catch (Exception ex)
                {
                    MessageBox.Show(_owner, "An error has occurred while checking for updates.\nPlease try again later.", $"{ProductName} - Unknown Error", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                    NoUpdates(startup, internetok);
                }
            }
        }
    }
}