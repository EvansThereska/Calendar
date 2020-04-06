using System;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public interface ICapacityProvider
    {
        int[][][] GetCapacity(DateTime date);
        void GetCapacity(DateTime date, int[][][] buffer);
    }
}