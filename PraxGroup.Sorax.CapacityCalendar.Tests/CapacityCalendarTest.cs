using System;
using Xunit;

namespace PraxGroup.Sorax.CapacityCalendar.Tests
{
    public class CapacityCalendarTest
    {
        private readonly Main.CapacityCalendar _calendar = new Main.CapacityCalendar();

        [Fact]
        public void CalculateNumberOfWeeksApril2020()
        {
            Assert.Equal(5, _calendar.CalculateNumberOfWeeks(2020, 4).NumberOfWeeks);
            Assert.Equal((int) DayOfWeek.Wednesday, _calendar.CalculateNumberOfWeeks(2020, 4).Start);
            Assert.Equal((int) DayOfWeek.Thursday, _calendar.CalculateNumberOfWeeks(2020, 4).End);
            Assert.Equal(2, _calendar.CalculateNumberOfWeeks(2020, 4).RogueBefore);
            Assert.Equal(3, _calendar.CalculateNumberOfWeeks(2020, 4).RogueAfter);
        }
        
        [Fact]
        public void CalculateNumberOfWeeksMarch2020()
        {
            Assert.Equal(6, _calendar.CalculateNumberOfWeeks(2020, 3).NumberOfWeeks);
            Assert.Equal((int) DayOfWeek.Sunday, _calendar.CalculateNumberOfWeeks(2020, 3).Start);
            Assert.Equal((int) DayOfWeek.Tuesday, _calendar.CalculateNumberOfWeeks(2020, 3).End);
            Assert.Equal(6, _calendar.CalculateNumberOfWeeks(2020, 3).RogueBefore);
            Assert.Equal(5, _calendar.CalculateNumberOfWeeks(2020, 3).RogueAfter);
        }

        [Fact]
        public void CalculateNumberOfWeeksFebruary2020()
        {
            Assert.Equal(5, _calendar.CalculateNumberOfWeeks(2020, 2).NumberOfWeeks);
            
        }

        [Fact]
        public void CalculateNumberOfWeeksJuly2019()
        {
            Assert.Equal(5, _calendar.CalculateNumberOfWeeks(2020, 2).NumberOfWeeks);
        }

        [Fact]
        public void CalculateNumberOfWeeksFebrurary2021()
        {
            // Is fully aligned
            Assert.Equal(4, _calendar.CalculateNumberOfWeeks(2021, 2).NumberOfWeeks);
            Assert.Equal((int) DayOfWeek.Monday, _calendar.CalculateNumberOfWeeks(2021, 2).Start);
            Assert.Equal((int) DayOfWeek.Sunday, _calendar.CalculateNumberOfWeeks(2021, 2).End);
            Assert.Equal(0, _calendar.CalculateNumberOfWeeks(2021, 2).RogueBefore);
            Assert.Equal(0, _calendar.CalculateNumberOfWeeks(2021, 2).RogueAfter);
            Assert.Equal(28, _calendar.CalculateNumberOfWeeks(2021, 2).DaysInMonth);
        }
    }
}