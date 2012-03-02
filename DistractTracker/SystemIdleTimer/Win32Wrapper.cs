using System;
using System.Runtime.InteropServices;

namespace DistractTracker.SystemIdleTimer
{
    public class Win32Wrapper
    {
        public static uint GetIdle()
        {
            LASTINPUTINFO structure = new LASTINPUTINFO();
            structure.cbSize = Convert.ToUInt32(Marshal.SizeOf(structure));
            GetLastInputInfo(ref structure);
            return (Convert.ToUInt32(Environment.TickCount) - structure.dwTime);
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO lii);

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
    }
}

