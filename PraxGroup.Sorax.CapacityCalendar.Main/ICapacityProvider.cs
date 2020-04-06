using System;
using System.Collections.Generic;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public interface ICapacityProvider
    {
        IEnumerable<int> GetTotal(DateTime date);

        IEnumerable<int> GetUsed(DateTime date);

        IEnumerable<int> GetAvailable(DateTime date);
    }
}