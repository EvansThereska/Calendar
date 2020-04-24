using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class NativeCalls
    {
        public const int EM_SETMARGINS = 0x00d3;
        public const int EM_RIGHTMARGIN = 2;
        public const int EM_LEFTMARGIN = 1;

        public const int WM_ACTIVATE = 0x006;
        public const int WM_ACTIVATEAPP = 0x01C;
        public const int WM_NCACTIVATE = 0x086;



        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll")]
        public static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr handle, int msg, int wParam, IntPtr lParam);


    }
}