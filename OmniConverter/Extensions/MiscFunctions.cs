using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OmniConverter
{
    class MiscFunctions
    {
        public static string BytesToHumanReadableSize(ulong length)
        {
            string size;
            try
            {
                if (length >= Math.Pow(2, 80)) return (length / Math.Pow(2, 80)).ToString("0.00 YiB");
                else if (length >= Math.Pow(2, 70)) return (length / Math.Pow(2, 70)).ToString("0.00 ZiB");
                else if (length >= Math.Pow(2, 60)) return (length / Math.Pow(2, 60)).ToString("0.00 EiB");
                else if (length >= Math.Pow(2, 50)) return (length / Math.Pow(2, 50)).ToString("0.00 PiB");
                else if (length >= Math.Pow(2, 40)) return (length / Math.Pow(2, 40)).ToString("0.00 TiB");
                else if (length >= Math.Pow(2, 30)) return (length / Math.Pow(2, 30)).ToString("0.00 GiB");
                else if (length >= Math.Pow(2, 20)) return (length / Math.Pow(2, 20)).ToString("0.00 MiB");
                else if (length >= Math.Pow(2, 10)) return (length / Math.Pow(2, 10)).ToString("0.00 KiB");
                else size = length.ToString("0.00 B");
            }
            catch { size = "-"; }

            if (length > 0) return size;
            else return "Black hole";
        }

        public static string TimeSpanToHumanReadableTime(TimeSpan time)
        {
            if (time.TotalMinutes > 60) return $"{time.TotalHours:0}:{time.Minutes:00}:{time.Seconds:00}.{time.Milliseconds:000}";
            else return $"{time.Minutes}:{time.Seconds:00}.{time.Milliseconds:000}";
        }
        public static string ReturnAssemblyVersion(string component, string type, int[] verInfo)
        {
            return string.Format("{0}{1}{2}{3}{4}",
            (!string.IsNullOrEmpty(component)) ? component + " " : null,
                verInfo[0],
                string.Format(".{0}", verInfo[1]),
                string.Format(".{0}", verInfo[2]),
                (verInfo[3] < 1) ? "" : string.Format(" - {0}{1}", type, verInfo[3])
                );
        }

        public static void PerformShutdownCheck(Stopwatch stopwatch)
        {
            var action = Program.Settings.AfterRenderAction;

            // Currently only under Windows, sorry!
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && action < 4)
            {
                if (action >= 0)
                {
                    ProcessStartInfo? PSI = null;
                    Version OSVer = Environment.OSVersion.Version;
                    string hybridShutdown = "";

                    if (OSVer.Major == 6 && OSVer.Minor >= 2 || OSVer.Major >= 6)
                        hybridShutdown = " /hybrid";

                    switch (action)
                    {
                        case 0:
                        case 1:
                            PSI = new ProcessStartInfo("rundll32", $"powrprof.dll, SetSuspendState {action},1,0");
                            break;
                        case 2:
                            PSI = new ProcessStartInfo("shutdown", $"/s{hybridShutdown} /t 15 /c \"Automatic shutdown through OmniConverter.\"");
                            break;
                        case 3:
                            PSI = new ProcessStartInfo("shutdown", "/r /t 15 /c \"Automatic restart through OmniConverter.\"");
                            break;
                        default:
                            break;
                    }

                    if (PSI != null)
                    {
                        PSI.CreateNoWindow = true;
                        PSI.UseShellExecute = false;
                        Process.Start(PSI);
                    }
                }
            }
            else
            {
                switch (action)
                {
                    case 4:
                        Program.Stop();
                        break;
                    case 5:
                        MessageBox.Show($"Completed in {stopwatch}", "OmniConverter");
                        break;
                }
            }
        }
    }
}
