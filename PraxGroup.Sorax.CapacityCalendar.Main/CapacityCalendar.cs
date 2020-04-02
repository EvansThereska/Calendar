using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CapacityCalendar : UserControl
    {
        private readonly Size DefaultSize = new Size(512, 440);

        private readonly Font _dayOfWeekFont = new Font("Arial", 10, FontStyle.Regular);
        private readonly Font _dateHeaderFont = new Font("Arial", 10, FontStyle.Regular);

        private DateTime _calendarDate = DateTime.Now;

        // private bool _showArrowControls;

        private const int MarginSize = 5;
        private const int NumberOfDaysAWeek = 7;

        public CapacityCalendar()
        {
            InitialiseComponents();
        }

        private void InitialiseComponents()
        {
            SuspendLayout();
            Paint += CalendarPaint;
            Name = nameof(CapacityCalendar);
            Size = DefaultSize;
            DoubleBuffered = true;
            ResumeLayout(false);
        }

        private void CalendarPaint(object sender, PaintEventArgs e)
        {
            RenderCalendar(e);
        }

        private void RenderCalendar(PaintEventArgs e)
        {
            using (var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    string monthAsString = _calendarDate.ToString("MMMM");
                    string yearAsString = _calendarDate.Year.ToString(CultureInfo.InvariantCulture);
                    int dateHeaderSize =
                        (int) g.MeasureString(monthAsString + " " + yearAsString, _dateHeaderFont).Height;

                    int daySpace = (int) CalculateDayFontSizes(g).Select(f => f.Height).Max();

                    int effectiveWidth = ClientSize.Width - MarginSize;
                    int effectiveHeight = ClientSize.Height - (daySpace + dateHeaderSize + MarginSize);
                    var alignInfo = CalculateNumberOfWeeks(_calendarDate.Year, _calendarDate.Month);
                    int cellWidth = effectiveWidth / NumberOfDaysAWeek;
                    int cellHeight = effectiveHeight / alignInfo.NumberOfWeeks;

                    int xStart = MarginSize;
                    int yStart = MarginSize + dateHeaderSize + daySpace;

                    // Draw grid and dates
                    var gray = new SolidBrush(Color.FromArgb(170, 170, 170));
                    var black = Brushes.Black;

                    var dayCount = 0;
                    foreach (var day in alignInfo.Days)
                    {
                        var brush = day.IsRogue ? gray : black;
                        g.DrawString(day.ToString(), _dayOfWeekFont, brush, xStart, yStart);
                        xStart += cellWidth;
                        if (++dayCount % NumberOfDaysAWeek == 0)
                        {
                            xStart = MarginSize;
                            yStart += cellHeight;
                        }
                    }
                }
                e.Graphics.DrawImage(bitmap, 0, 0, ClientSize.Width, ClientSize.Height);
            }
        }

        internal MonthInfo CalculateNumberOfWeeks(int year, int month)
        {
            // This isn't just days / 7 because we can get a number
            // of 4, 5, or 6 weeks per month depending on what days
            // the first and last day fall.

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var firstOfMonth = new DateTime(year, month, 1);
            var lastOfMonth = new DateTime(year, month, daysInMonth);

            var dayOnFirstWeek = (int) firstOfMonth.DayOfWeek;
            var dayOnLastWeek = (int) lastOfMonth.DayOfWeek;

            var leftPadding = (dayOnFirstWeek == 0 ? NumberOfDaysAWeek : dayOnFirstWeek) - 1;
            var rightPadding = dayOnLastWeek == 0 ? 0 : NumberOfDaysAWeek - dayOnLastWeek;

            var paddedDays = daysInMonth + leftPadding + rightPadding;

            Debug.Assert(paddedDays % NumberOfDaysAWeek == 0);

            var days = PopulateDays(leftPadding, rightPadding, daysInMonth);

            var numberOfWeeks = paddedDays / NumberOfDaysAWeek;

            return new MonthInfo(daysInMonth, dayOnFirstWeek, dayOnLastWeek, leftPadding, rightPadding, numberOfWeeks, days);
        }

        private IEnumerable<Day> PopulateDays(int leftPadding, int rightPadding, int daysInMonth)
        {
            var days = new List<Day>();
            if (leftPadding > 0)
            {
                var prevMonth = _calendarDate.AddMonths(-1);
                var prevNoOfDays = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
                while (leftPadding-- > 0)
                {
                    days.Add(new Day(prevNoOfDays - leftPadding, true));
                }
            }

            for (var i = 1; i <= daysInMonth; i++)
            {
                days.Add(new Day(i, false));
            }

            if (rightPadding > 0)
            {
                while (rightPadding-- > 0)
                {
                    days.Add(new Day(rightPadding + 1, true));
                }
            }

            return days;
        }

        private IEnumerable<SizeF> CalculateDayFontSizes(Graphics g)
        {
            return new[]
            {
                g.MeasureString("Mon", _dayOfWeekFont),
                g.MeasureString("Tue", _dayOfWeekFont),
                g.MeasureString("Wed", _dayOfWeekFont),
                g.MeasureString("Thu", _dayOfWeekFont),
                g.MeasureString("Fri", _dayOfWeekFont),
                g.MeasureString("Sat", _dayOfWeekFont),
                g.MeasureString("Sun", _dayOfWeekFont),
            };
        }

        internal struct MonthInfo
        {
            internal MonthInfo(int daysInMonth, int start, int end, int rogueBefore, int rogueAfter, int numberOfWeeks, IEnumerable<Day> days)
            {
                Days = days;
                DaysInMonth = daysInMonth;
                Start = start;
                End = end;
                RogueBefore = rogueBefore;
                RogueAfter = rogueAfter;
                NumberOfWeeks = numberOfWeeks;
            }

            public IEnumerable<Day> Days { get; set; }
            public int DaysInMonth { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
            public int RogueBefore { get; set; }
            public int RogueAfter { get; set; }
            public int NumberOfWeeks { get; set; }
        }

        internal class Day
        {
            public Day(int value, bool isRogue)
            {
                Value = value;
                IsRogue = isRogue;
            }

            public int Value { get; private set; }
            
            public DayOfWeek DayOfWeek { get; set; }
            
            public bool IsRogue { get; set; }

            public override string ToString()
            {
                return Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}