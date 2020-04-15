using System;
using System.Collections.Generic;
using System.Drawing;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CalendarDayPoint
    {
        private readonly Rectangle _rectangle;

        private readonly List<CapacityDetail> _details = new List<CapacityDetail>();

        public CalendarDayPoint(DateTime date, Rectangle rectangle)
        {
            Date = date;
            _rectangle = rectangle;
        }

        public DateTime Date { get; }

        public bool Contains(int x, int y)
        {
            return _rectangle.Contains(x, y);
        }

        public void AddDetail(CapacityDetail detail)
        {
            _details.Add(detail);
        }

        public List<CapacityDetail> Details => _details;
    }
}