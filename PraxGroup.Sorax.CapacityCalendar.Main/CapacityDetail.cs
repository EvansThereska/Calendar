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
    }

}