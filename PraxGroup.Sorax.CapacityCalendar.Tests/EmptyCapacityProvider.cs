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
    }
}