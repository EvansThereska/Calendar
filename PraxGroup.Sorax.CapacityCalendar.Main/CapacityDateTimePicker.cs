using System.Drawing;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CapacityDateTimePicker
    {
        private ButtonTextBox _textBox;
        private CapacityCalendar _calendar;

        public Point Location
        {
            get => _textBox.Location;
            set => _textBox.Location = value;
        }


    }
}