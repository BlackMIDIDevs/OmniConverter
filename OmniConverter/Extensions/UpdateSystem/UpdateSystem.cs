using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace OmniConverter
{
    class UpdateSystem
    {
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int connDescription, int ReservedValue);

        public static string ProductName = "OmniConverter";
        public static Octokit.GitHubClient UpdateClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(ProductName));

        public static string SetupFile = Properties.Settings.Default.ProjectLink + "/releases/download/{0}/OmniConverterSetup.exe";
        public static string UpdatePage = Properties.Settings.Default.ProjectLink + "/releases/tag/{0}";

        public const int NORMAL = 0x0;
        public const int USERFOLDER_PATH = 0x1;
        public const int WIPE_SETTINGS = 0xF;

        public static bool IsInternetAvailable()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        public static void CheckThenUpdate(String ReturnVal, Int32 InstallMode)
        {
            if (!ReturnVal.Equals("0.0.0.0"))
            {
                DLEngine frm = new DLEngine(ReturnVal, String.Format("Downloading update {0}...", ReturnVal, @"{0}"), null, null, InstallMode);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
            }
            else Debug.ShowMsgBox("Excuse me...", "No.", null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void TriggerUpdateWindow(Version CurVer, Version OLVer, String newestversion, bool forced, bool startup, bool isitfromthechangelogwindow)
        {
            String UTitle = null;
            String UText = null;
            String RVal = "0.0.0.0";

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

                DialogResult RES = Debug.ShowMsgBox(UTitle, UText, null, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (RES == DialogResult.Yes) CheckThenUpdate(RVal, UpdateSystem.NORMAL);
            }
        }

        public static void NoUpdates(bool startup, bool internetok)
        {
            if (!startup)
                Debug.ShowMsgBox("No updates found", "No updates for the converter have been found.", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void CheckChangelog()
        {
            bool internetok = IsInternetAvailable();
            if (internetok == false)
            {
                MessageBox.Show("There's no Internet connection.\n\nYou can't see the changelog without one.", String.Format("{0} - No Internet connection available", ProductName), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    Octokit.Release Release = UpdateClient.Repository.Release.GetLatest("KeppySoftware", "OmniConverter").Result;
                    Process.Start(String.Format(UpdatePage, Release.TagName));
                }
                catch (Exception ex)
                {
                    Debug.ShowMsgBox(
                        "Unknown error", 
                        "An error has occurred while trying to show you the latest changelog.\nPlease try again later.", 
                        ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static string GetCurrentBranch()
        {
            if (Properties.Settings.Default.UpdateBranch == "canary")
                return "Canary branch";
            else if (Properties.Settings.Default.UpdateBranch == "normal")
                return "Normal branch";
            else if (Properties.Settings.Default.UpdateBranch == "delay")
                return "Delayed branch";
            else if (Properties.Settings.Default.UpdateBranch == "choose")
                return "No branch selected";
            else
                return "Normal branch";
        }

        public static Color GetCurrentBranchColor()
        {
            if (Properties.Settings.Default.UpdateBranch == "canary")
                return Color.FromArgb(221, 172, 5);
            else if (Properties.Settings.Default.UpdateBranch == "normal")
                return Color.FromArgb(158, 14, 204);
            else if (Properties.Settings.Default.UpdateBranch == "delay")
                return Color.FromArgb(84, 110, 122);
            else if (Properties.Settings.Default.UpdateBranch == "choose")
                return Color.FromArgb(182, 0, 0);
            else
                return Color.FromArgb(255, 255, 255);
        }

        public static string GetCurrentBranchToolTip()
        {
            if (Properties.Settings.Default.UpdateBranch == "canary")
                return "Receive all updates.\nYou may get broken updates that haven't been fully tested.\nDesigned for testers and early adopters.";
            else if (Properties.Settings.Default.UpdateBranch == "normal")
                return "Receive occasional updates and urgent bugfixes (Eg. from version x.0.x.x to x.1.x.x).\nRecommended.";
            else if (Properties.Settings.Default.UpdateBranch == "delay")
                return "You will only get major releases (Eg. from version 0.x.x.x to 1.x.x.x).\nFor those who do not wish to update often.\nNot recommended.";
            else if (Properties.Settings.Default.UpdateBranch == "choose")
                return "No information, since you didn't chose a branch.";
            else
                return "Receive occasional updates and urgent bugfixes (Eg. from version x.0.x.x to x.1.x.x).\nRecommended.";
        }

        public static void CheckForUpdates(bool forced, bool startup, bool isitfromthechangelogwindow)
        {
            bool internetok = IsInternetAvailable();
            if (internetok == false)
                NoUpdates(startup, false);
            else
            {
                try
                {
                    Octokit.Release Release = UpdateClient.Repository.Release.GetLatest("KeppySoftware", "OmniConverter").Result;

                    Version DriverOnline = null;
                    Version.TryParse(Release.TagName, out DriverOnline);
                    Version DriverCurrent = Assembly.GetExecutingAssembly().GetName().Version; ;
 
                    if (Properties.Settings.Default.UpdateBranch == "canary")
                    {
                        if (DriverCurrent.Major < DriverOnline.Major || DriverCurrent.Minor < DriverOnline.Minor)
                        {
                            if ((DriverCurrent.Build >= DriverOnline.Build || DriverCurrent.Build < DriverOnline.Build))
                                TriggerUpdateWindow(DriverCurrent, DriverOnline, Release.TagName, forced, startup, isitfromthechangelogwindow);
                            else
                            {
                                if (forced)
                                    TriggerUpdateWindow(DriverCurrent, DriverOnline, DriverCurrent.ToString(), forced, startup, isitfromthechangelogwindow);
                                else
                                    NoUpdates(startup, internetok);
                            }
                        }
                        else
                        {
                            if (forced)
                                TriggerUpdateWindow(DriverCurrent, DriverOnline, DriverCurrent.ToString(), forced, startup, isitfromthechangelogwindow);
                            else
                                NoUpdates(startup, internetok);
                        }
                    }
                    else if (Properties.Settings.Default.UpdateBranch == "normal")
                    {
                        if (DriverCurrent.Major < DriverOnline.Major)
                        {
                            if ((DriverCurrent.Minor >= DriverOnline.Minor || DriverCurrent.Minor < DriverOnline.Minor))
                                TriggerUpdateWindow(DriverCurrent, DriverOnline, Release.TagName, forced, startup, isitfromthechangelogwindow);
                            else
                            {
                                if (forced)
                                    TriggerUpdateWindow(DriverCurrent, DriverOnline, DriverCurrent.ToString(), forced, startup, isitfromthechangelogwindow);
                                else
                                    NoUpdates(startup, internetok);
                            }
                        }
                        else
                        {
                            if (forced)
                                TriggerUpdateWindow(DriverCurrent, DriverOnline, DriverCurrent.ToString(), forced, startup, isitfromthechangelogwindow);
                            else
                                NoUpdates(startup, internetok);
                        }
                    }
                    else if (Properties.Settings.Default.UpdateBranch == "delay")
                    {
                        if (DriverCurrent.Major < DriverOnline.Major)
                            TriggerUpdateWindow(DriverCurrent, DriverOnline, Release.TagName, forced, startup, isitfromthechangelogwindow);
                        else
                        {
                            if (forced)
                                TriggerUpdateWindow(DriverCurrent, DriverOnline, DriverCurrent.ToString(), forced, startup, isitfromthechangelogwindow);
                            else
                                NoUpdates(startup, internetok);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.ShowMsgBox(
                        "Unknown error",
                        "An error has occurred while checking for updates.\nPlease try again later.",
                        ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NoUpdates(startup, internetok);
                }
            }
        }
    }
}