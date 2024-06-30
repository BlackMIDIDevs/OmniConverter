using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OmniConverter
{
    internal class Program
    {
        private static readonly string settingsPath = AppContext.BaseDirectory + "/settings.json";
        public static Settings Settings { get; set; } = new();
        public static SoundFonts SoundFontsManager { get; set; } = new();
        public static DiscordRPC? RichPresence { get; set; }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            var aval = BuildAvaloniaApp();

            if (aval != null)
            {

                foreach (var item in args)
                {
                    switch (item.ToLowerInvariant())
                    {
#if !DEBUG
                        case "/debug":
                            Debug.EnableConsole();
                            break;
#endif
                    }
                }

#if DEBUG
                Debug.EnableConsole();
#endif

                LoadConfig(true);
                // RichPresence.SetPresence("Idle", "Idling");

                aval.StartWithClassicDesktopLifetime(args);
            }
        }

        public static void Stop(int code = 0)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
            {
                SaveConfig(true);
                desktopApp.Shutdown(code);
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();

        public static void SaveConfig(bool startup = false)
        {
            try
            {
                File.WriteAllText(settingsPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
            catch (Exception ex)
            {
                if (startup) Debug.PrintToConsole(Debug.LogType.Error, ex.ToString());
                else MessageBox.Show(ex.ToString(), "OmniConverter - ERROR", null, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            }
        }

        private static void CreateConfig(bool startup = false)
        {
            Settings = new Settings();
            SaveConfig(startup);
        }

        public static void LoadConfig(bool startup = false)
        {
            try
            {
                if (!File.Exists(settingsPath))
                {
                    CreateConfig(startup);
                    return;
                }

                string? settingsJSON = null;
                using (StreamReader sr = new(settingsPath))
                    settingsJSON = sr.ReadToEnd();

                var jsonSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore };
                var userSettings = JsonConvert.DeserializeObject<Settings?>(settingsJSON, jsonSettings);

                if (userSettings == null)
                {
                    CreateConfig(startup);
                    return;
                }

                Settings = userSettings;
            }
            catch
            {
                CreateConfig(startup);
            }
            finally
            {
                SoundFontsManager = new(Settings.SoundFontsList);
                Debug.PrintToConsole(Debug.LogType.Message, "Loaded configuration file!");
            }
        }
    }

    static class Debug
    {
        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        private static readonly object Lock = new object();
        public static bool DebugMode = false;

        public enum LogType
        {
            Unknown = -1,
            Message = 0,
            Warning = 1,
            Error = 2
        }

        public static void EnableConsole()
        {
            var CV = Assembly.GetExecutingAssembly().GetName().Version;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                AllocConsole();

            DebugMode = true;

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            if (CV != null)
            {
                Console.WriteLine($"OmniConverter v{CV.Major}.{CV.Minor}.{CV.Build}");
                Console.WriteLine($"Copyright (C) 2019-{DateTime.Today.Year} Keppy's Software and Arduano\nYou're not supposed to be here!\n");
            }     
        }

        public static void PrintToConsole(
            LogType Type, string Message,
            [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            if (!DebugMode) return;

            lock (Lock)
            {

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(string.Format($"[{(func == ".ctor" ? "Thread" : func)} -> {Path.GetFileName(file)}, L{line}] - "));

                switch (Type)
                {
                    case LogType.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                    case LogType.Message:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.Write($"{Message}\n");
            }
        }
    }
}
