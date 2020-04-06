using System;
using PraxGroup.Sorax.CapacityCalendar.Main;

namespace PraxGroup.Sorax.CapacityCalendar.Tests
{
    public class EmptyCapacityProvider : ICapacityProvider
    {
        public int[][][] GetCapacity(DateTime date)
        {
            throw new NotImplementedException();
        }

        public void GetCapacity(DateTime date, int[][][] buffer)
        {
            throw new NotImplementedException();
        }
    }
}