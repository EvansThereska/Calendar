using System;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class DummyCapacityProvider : ICapacityProvider
    {
        public int[][][] GetCapacity(DateTime date)
        {
            // var daysThisMonth = DateTime.DaysInMonth(date.Year, date.Month);

            const int maxDays = 31;
            const int shifts = 2;
            const int stats = 2;
            
            var matrix = new int[maxDays][][];

            for (var i = 0; i < maxDays; i++)
            {
                matrix[i] = new int[shifts][];
                matrix[i][0] = new int[stats];
                matrix[i][1] = new int[stats];
                matrix[i][0][0] = 100;
                matrix[i][0][1] = 30;
                matrix[i][1][0] = 100;
                matrix[i][1][1] = 100;
            }

            return matrix;
        }

        public void GetCapacity(DateTime date, int[][][] matrix)
        {
            EmptyBuffer(matrix);

            for (var i = 0; i < matrix.Length; i++)
            {
                matrix[i][0][0] = 100;
                matrix[i][0][1] = 30;
                matrix[i][1][0] = 100;
                matrix[i][1][1] = 40;
            }
        }

        private void EmptyBuffer(int[][][] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                for (int j = 0; j < buffer[i].Length; j++)
                {
                    for (int k = 0; k < buffer[j].Length; k++) {
                        buffer[i][j][k] = 0;
                    } 
                }
            }
        }
    }
}