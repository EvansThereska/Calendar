using System;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public interface ICapacityProvider
    {
        int[][][] GetCapacity(DateTime date);
    }
}