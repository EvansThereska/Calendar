using System;
using System.Diagnostics.CodeAnalysis;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class NativeCalls
    {
        public const int EM_SETMARGINS = 0x00d3;
        public const int EM_RIGHTMARGIN = 2;
        public const int EM_LEFTMARGIN = 1;



        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }
}