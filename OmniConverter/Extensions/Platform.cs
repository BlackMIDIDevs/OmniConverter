using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using ManagedBass;

namespace OmniConverter.Extensions
{
    public class Platform
    {
        // https://stackoverflow.com/questions/1295890/windows-7-progress-bar-in-taskbar-in-c
        public enum TaskbarState
        {
            NoProgress    = 0,
            Indeterminate = 1,
            Normal        = 2,
            Error         = 4,
            Paused        = 8
        }
        
#if WINDOWS
        [ComImport()]
        [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList3
        {
            // ITaskbarList
            [PreserveSig]
            void HrInit();
            [PreserveSig]
            void AddTab(IntPtr hwnd);
            [PreserveSig]
            void DeleteTab(IntPtr hwnd);
            [PreserveSig]
            void ActivateTab(IntPtr hwnd);
            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            [PreserveSig]
            void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
            [PreserveSig]
            void SetProgressState(IntPtr hwnd, TaskbarState state);
        }
        
        [ComImport()]    
        [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class TaskbarInstance
        {
        }

        private static ITaskbarList3 _taskbarInstance = (ITaskbarList3)new TaskbarInstance();
        private static TaskbarState _lastState = TaskbarState.NoProgress;
        
        public static void SetTaskbarProgress(Window window, TaskbarState state, ulong cur = 0, ulong total = 0)
        {
            var handle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
            if (handle == IntPtr.Zero) return;

            _taskbarInstance.SetProgressState(handle, state);
            _taskbarInstance.SetProgressValue(handle, cur, total);
        }
#else
        public static void SetTaskbarProgress(Window window, TaskbarState state, ulong cur = 0, ulong total = 0)
        { }
#endif

        // TODO: Maybe this should use actual platform-specific code instead of BASS
        // BASS devices are per-thread, so this should be fine
        public static void PlaySound(string filename)
        {
            // TODO: Hardcoding this to the first device is probably a bad idea
            // It doesn't seem to be possible to check what device *would* get chosen if the default device is given to BASS_Init
            if (Bass.Init(1) || Bass.LastError == Errors.Already)
            {
                Bass.CurrentDevice = 1;

                var programDir = AppContext.BaseDirectory;
                int stream = Bass.CreateStream($"{programDir}/CustomSounds/{filename}", Flags: BassFlags.AutoFree);
                if (stream == 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        var res = AssetLoader.Open(new Uri($"avares://OmniConverter/Assets/{filename}"));
                        res.CopyTo(ms);

                        var arr = ms.ToArray();
                        stream = Bass.CreateStream(arr, 0, arr.LongLength, BassFlags.AutoFree);
                    }
                }
                Bass.ChannelPlay(stream);

                Bass.CurrentDevice = 0;
            }
        }
    }
}
