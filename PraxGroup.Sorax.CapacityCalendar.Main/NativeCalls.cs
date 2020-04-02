using System;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public static class NativeCalls
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }
}