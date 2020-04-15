using System;
using System.Drawing;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CapacityDetail
    {
        public Shift Shift { get; }
        public int Total { get; }
        public int Used { get; }
        public Rectangle DrawArea { get; set; }

        public CapacityDetail(Shift shift, int total, int used)
        {
            Shift = shift;
            Total = total;
            Used = used;
        }

        public string ShiftValue
        {
            get
            {
                switch (Shift)
                {
                    case Shift.Am:
                        return "AM";
                    case Shift.Pm:
                        return "PM";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool Contains(int x, int y)
        {
            return DrawArea.Contains(x, y);
        }

    }

}