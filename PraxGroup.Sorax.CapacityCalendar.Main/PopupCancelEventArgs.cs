using System;
using System.Drawing;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class PopupCancelEventArgs : EventArgs
    {
        public Point CursorLocation { get; set;  }

        public Form Popup { get; set;  }

        public bool Cancel { get; set; }

        public PopupCancelEventArgs(Form popup, Point location)
        {
            Popup = popup;
            CursorLocation = location;
            Cancel = false;
        }

    }

}