﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SmartTaskbar.Core
{
    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        #region Taskbar Buttons Size

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SendNotifyMessage([In] IntPtr hWnd, uint msg, UIntPtr wParam, string lParam);

        #endregion

        #region PostMessage

        [DllImport("user32.dll", EntryPoint = "PostMessageW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PostMessage([In] IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region PostThreadMessage

        [DllImport("user32.dll", EntryPoint = "PostThreadMessageW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PostThreadMessage(int idThread, uint msg,
            IntPtr wParam, IntPtr lParam);

        #endregion

        #region MonitorFromWindow

        [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
        internal static extern IntPtr MonitorFromWindow([In] IntPtr hwnd, uint dwFlags);

        #endregion

        #region GetParentWindow

        [DllImport("user32.dll", EntryPoint = "GetAncestor")]
        internal static extern IntPtr GetAncestor([In] IntPtr hwnd, uint gaFlags);

        #endregion

        #region GetCursorPos

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out Point lpPoint);

        #endregion

        #region GetDesktopWindow

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        internal static extern IntPtr GetDesktopWindow();

        #endregion

        #region IsWindowVisible

        [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible([In] IntPtr hWnd);

        #endregion

        #region DwmGetWindowAttribute

        [DllImport("dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute,
            [MarshalAs(UnmanagedType.Bool)] out bool pvAttribute, int cbAttribute);

        #endregion

        #region GetForegroundWindow

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        internal static extern IntPtr GetForegroundWindow();

        #endregion

        #region GetClassName

        [DllImport("user32.dll", EntryPoint = "GetClassNameW")]
        internal static extern int GetClassName([In] IntPtr hWnd,
            [Out] [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder lpClassName, int nMaxCount);

        #endregion

        #region GetWindowThreadProcessId

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        internal static extern uint GetWindowThreadProcessId([In] IntPtr hWnd, out int lpdwProcessId);

        #endregion

        #region List of Value

        internal static List<Taskbar> taskbars = new List<Taskbar>(4);

        internal static Dictionary<IntPtr, string> cacheName = new Dictionary<IntPtr, string>(64);

        #endregion

        #region Taskbar Display State

        [StructLayout(LayoutKind.Sequential)]
        internal struct Appbardata
        {
            public uint cbSize;

            public IntPtr hWnd;

            public uint uCallbackMessage;

            public uint uEdge;

            public Tagrect rc;

            public int lParam;
        }

        [DllImport("shell32.dll", EntryPoint = "SHAppBarMessage", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr SHAppBarMessage(uint dwMessage, ref Appbardata pData);

        #endregion

        #region Taskbar Animation

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetSystemParameters(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetSystemParameters(uint uiAction, uint uiParam, out bool pvParam, uint fWinIni);

        #endregion

        #region FindWindow

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow([In] string strClassName, [In] string strWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowEx([In] IntPtr parentHandle, [In] IntPtr childAfter,
            [In] string lclassName,
            [In] string windowTitle);

        #endregion

        #region GetWindowRect

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect([In] IntPtr hWnd, out Tagrect lpRect);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Tagrect
        {
            public int left;

            public int top;

            public int right;

            public int bottom;

            public static implicit operator Rectangle(Tagrect rect) =>
                Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);

            public static implicit operator Tagrect(Rectangle rectangle) => new Tagrect
            {
                left = rectangle.Left,
                top = rectangle.Top,
                right = rectangle.Right,
                bottom = rectangle.Bottom
            };
        }

        #endregion

        #region WindowFromPoint

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        internal static extern IntPtr WindowFromPoint(Point point);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Point
        {
            public int x;

            public int y;
        }

        #endregion

        #region GetWindowPlacement

        [StructLayout(LayoutKind.Sequential)]
        internal struct Tagwindowplacement
        {
            public uint length;

            public uint flags;

            public uint showCmd;

            public System.Drawing.Point ptMinPosition;

            public System.Drawing.Point ptMaxPosition;

            public Tagrect rcNormalPosition;
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowPlacement")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement([In] IntPtr hWnd, ref Tagwindowplacement lpwndpl);

        #endregion

        #region EnumWindows

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal delegate bool Wndenumproc(IntPtr param0, IntPtr param1);

        [DllImport("user32.dll", EntryPoint = "EnumWindows")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumWindows(Wndenumproc lpEnumFunc, IntPtr lParam);

        #endregion

        //#region GetModuleBaseName

        //[DllImport("kernel32.dll", EntryPoint = "GetModuleFileNameExW")]
        //internal static extern uint GetModuleFileName([In] IntPtr hProcess, [In] IntPtr hModule, [Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpFilename, uint nSize);

        //#endregion
    }
}