using System;
using System.Collections.Generic;
using System.Linq;
using PraxGroup.Sorax.CapacityCalendar.Main;

namespace PraxGroup.Sorax.CapacityCalendar.Tests
{
    public class RandomCapacityProvider : ICapacityProvider
    {

        private readonly DateTime _now;
        private readonly Random _random = new Random();

        private Dictionary<string, Capacity[]> _capacities = new Dictionary<string, Capacity[]>();

        public RandomCapacityProvider()
        {
            Init();
        }

        private void Init()
        {
            for (var i = 0; i < 10 /* days */; i++)
            {
                var day = _now.AddDays(i);
                _capacities.Add(day.ToString("yyyyMMdd"), GetDayCapacity(day));
            }
        }

        private Capacity[] GetDayCapacity(DateTime day)
        {
            return new Capacity[]
            {
                new Capacity(_random.Next(5, 10), _random.Next(0, 5), _random.Next(5, 10) - _random.Next(0, 5)), 
                new Capacity(_random.Next(5, 10), _random.Next(0, 5), _random.Next(5, 10) - _random.Next(0, 5)) 
            };
        }

        public IEnumerable<int> GetTotal(DateTime date)
        {
            return _capacities[date.ToString("yyyyMMdd")].Select(x => x.Total);
        }

        public IEnumerable<int> GetUsed(DateTime date)
        {
            return _capacities[date.ToString("yyyyMMdd")].Select(x => x.Used);
        }

        public IEnumerable<int> GetAvailable(DateTime date)
        {
            return _capacities[date.ToString("yyyyMMdd")].Select(x => x.Free);
        }

        private struct Capacity
        {
            public Capacity(int total, int used, int free)
            {
                _total = total;
                _used = used;
                _free = free;
            }

            private int _total;

            public int Total => _total;

            public int Used => _used;

            public int Free => _free;

            private int _used;
            private int _free;


        }
    }
}