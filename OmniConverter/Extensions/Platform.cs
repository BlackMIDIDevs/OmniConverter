using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace OmniConverter.Extensions
{
    public class Platform
    {
        // https://stackoverflow.com/questions/1295890/windows-7-progress-bar-in-taskbar-in-c
        public enum TaskbarState
        {
            NoProgress = 0,
            Indeterminate = 1,
            Normal = 2,
            Error = 4,
            Paused = 8
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
    }
}
