using System;
using PraxGroup.Sorax.CapacityCalendar.Main;

namespace PraxGroup.Sorax.CapacityCalendar.Demo
{
    public class DummyCapacityProvider : ICapacityProvider
    {
        public int[][][] GetCapacity(DateTime date)
        {
            var daysThisMonth = DateTime.DaysInMonth(date.Year, date.Month);
            
            var matrix = new int[daysThisMonth][][];

            for (var i = 0; i < daysThisMonth; i++)
            {
                matrix[i] = new int[2][];
                matrix[i][0] = new int[2];
                matrix[i][1] = new int[2];
                matrix[i][0][0] = 100;
                matrix[i][0][1] = 30;
                matrix[i][1][0] = 100;
                matrix[i][1][1] = 40;
            }

            return matrix;
        }
    }
}