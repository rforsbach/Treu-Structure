using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Canguro.Utility
{
    class NativeMethods
    {
        // Import GetFocus() from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int MsgWaitForMultipleObjects(
            int nCount,		    // number of handles in array
            IntPtr pHandles,    // object-handle array
            bool bWaitAll,	    // wait option
            int dwMilliseconds,	// time-out interval
            int dwWakeMask	    // input-event type
            );

        [DllImport("user32.dll")]
        internal static extern void MessageBeep(uint uType);

        internal const uint MB_OK = 0x00000000;
        internal const uint MB_ICONHAND = 0x00000010;
        internal const uint MB_ICONQUESTION = 0x00000020;
        internal const uint MB_ICONEXCLAMATION = 0x00000030;
        internal const uint MB_ICONASTERISK = 0x00000040;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        //public static extern bool SendMessage(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)] int Msg, IntPtr wParam, IntPtr lParam);
        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Message
        {
            public IntPtr hWnd;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
