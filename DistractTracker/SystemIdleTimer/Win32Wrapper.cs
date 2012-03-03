using System;
using System.Runtime.InteropServices;

namespace DistractTracker.SystemIdleTimer
{
    public class Win32Wrapper
    {
        // note MM: http://msdn.microsoft.com/en-us/library/system.environment.tickcount.aspx
        // GetLastInputInfo info also goes round this way(although i couldn't find proof on the web)
        // but there's a risk of Overflow exception, so i cast them to long to avoid it
        // nevertheless, i'm not sure how this code will work after machine working for more than 50 days.
        // it depends whether GetLastInputInfo is synchronized with TickCount(behaves same way)

        public static uint GetIdle()
        {
            LASTINPUTINFO structure = new LASTINPUTINFO();
            structure.cbSize = Convert.ToUInt32(Marshal.SizeOf(structure));
            GetLastInputInfo(ref structure);
            // to exclude Overflow exception
            return Convert.ToUInt32((long)Environment.TickCount - (long)structure.dwTime);
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO lii);

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            public uint cbSize;
            public int dwTime;
        }
    }
}

