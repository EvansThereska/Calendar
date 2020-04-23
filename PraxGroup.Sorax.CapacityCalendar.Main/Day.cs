using System;
using System.Globalization;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class Day
    {
        public Day(int value, bool isRogue, DateTime date)
        {
            Value = value;
            IsRogue = isRogue;
            Date = date;
        }

        public int Value { get; }

        public DayOfWeek DayOfWeek { get; set; }

        public bool IsRogue { get; set; }

        public DateTime Date { get; }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}