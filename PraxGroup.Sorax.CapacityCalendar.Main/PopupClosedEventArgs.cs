using System;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class PopupClosedEventArgs : EventArgs
    {
        public Form Popup { get; }

        public PopupClosedEventArgs(Form popup)
        {
            Popup = popup;
        }


    }
}