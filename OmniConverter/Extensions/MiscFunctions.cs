using Avalonia.Platform;
using ManagedBass;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace OmniConverter
{
    class MiscFunctions
    {
        public enum ConvSounds
        {
            Unknown = -1,
            Start,
            Finish,
            Error
        }

        private static ulong PowerOf(int pw)
        {
            return (ulong)Math.Pow(2, 10 * pw);
        }

        public static string BytesToHumanReadableSize(ulong length)
        {
            string size;
            try
            {
                if (length >= PowerOf(8)) return (length / PowerOf(8)).ToString("0.00 YiB");
                else if (length >= PowerOf(7)) return (length / PowerOf(7)).ToString("0.00 ZiB");
                else if (length >= PowerOf(6)) return (length / PowerOf(6)).ToString("0.00 EiB");
                else if (length >= PowerOf(5)) return (length / PowerOf(5)).ToString("0.00 PiB");
                else if (length >= PowerOf(4)) return (length / PowerOf(4)).ToString("0.00 TiB");
                else if (length >= PowerOf(3)) return (length / PowerOf(3)).ToString("0.00 GiB");
                else if (length >= PowerOf(2)) return (length / PowerOf(2)).ToString("0.00 MiB");
                else if (length >= PowerOf(1)) return (length / PowerOf(1)).ToString("0.00 KiB");
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
            (!string.IsNullOrEmpty(component)) ? $"{component} " : null,
                verInfo[0],
                string.Format(".{0}", verInfo[1]),
                string.Format(".{0}", verInfo[2]),
                (verInfo[3] < 1) ? "" : string.Format(" - {0}{1}", type, verInfo[3])
                );
        }

        // TODO: Maybe this should use actual platform-specific code instead of BASS
        // BASS devices are per-thread, so this should be fine
        public static void PlaySound(ConvSounds selectedSound, bool auto = false)
        {
            string sound = string.Empty;

            switch (selectedSound)
            {
                case ConvSounds.Finish:
                    sound = Program.Settings.Program.OldKMCScheme ? "convfinold.wav" : "convfin.wav";
                    break;

                case ConvSounds.Error:
                    sound = Program.Settings.Program.OldKMCScheme ? "convfailold.wav" : "convfail.wav";
                    break;

                case ConvSounds.Start:
                    sound = Program.Settings.Program.OldKMCScheme ? "convstartold.wav" : "convstart.wav";
                    break;

                default:
                    return;
            }

            // TODO: Hardcoding this to the first device is probably a bad idea
            // It doesn't seem to be possible to check what device *would* get chosen if the default device is given to BASS_Init
            if (Bass.Init(auto ? -1 : 1) || Bass.LastError == Errors.Already)
            {
                if (!auto) Bass.CurrentDevice = 1;

                int stream = Bass.CreateStream($"{AppContext.BaseDirectory}/CustomSounds/{sound}", Flags: BassFlags.AutoFree);
                if (stream == 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        var res = AssetLoader.Open(new Uri($"avares://OmniConverter/Assets/{sound}"));
                        res.CopyTo(ms);

                        var arr = ms.ToArray();
                        stream = Bass.CreateStream(arr, 0, arr.LongLength, BassFlags.AutoFree);
                    }
                }
                Bass.ChannelPlay(stream);

                if (!auto) Bass.CurrentDevice = 0;
            }
        }

        public static void PerformShutdownCheck(Stopwatch stopwatch)
        {
            var action = Program.Settings.Program.AfterRenderAction;

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

        public static Version ConvertIntToVersion(int version)
        {
            return new Version(version >> 16 & 0xff,
                               version >> 8 & 0xff,
                               version & 0xff,
                               0);
        }
    }
}
