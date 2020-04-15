using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CapacityCalendar : UserControl
    {
        private readonly ICapacityProvider _capacityProvider;
        private readonly Font _dayOfWeekFont = DefaultFont;
        private readonly Font _dateHeaderFont = DefaultFont;

        private readonly string[] _dayNames = {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"};

        private DateTime _calendarDate = DateTime.Now;

        private int[][][] _capacity;

        private bool IsCapacityLoaded { get; set; }

        private const int MarginSize = 5;
        private const int NumberOfDaysAWeek = 7;

        public bool HighlightCurrentDay { get; set; }

        public bool ShowToolTips { get; set; }

        private CapacityToolTip _toolTip;

        private readonly List<CalendarDayPoint> _calendarDayPoints = new List<CalendarDayPoint>();

        private TodayButton _btnToday;
        private NavigateLeftButton _btnLeft;
        private NavigateRightButton _btnRight;

        public CapacityCalendar(ICapacityProvider capacityProvider)
        {
            _capacityProvider = capacityProvider;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            SuspendLayout();

            _btnToday = new TodayButton {Name = "_btnToday"};
            _btnLeft = new NavigateLeftButton {Name = "_btnLeft"};
            _btnRight = new NavigateRightButton {Name = "_btnRight"};
            _toolTip = new CapacityToolTip();
            
            _btnToday.ButtonClicked += OnTodayButtonClicked;
            _btnLeft.ButtonClicked += OnLeftButtonClicked;
            _btnRight.ButtonClicked += OnRightButtonClicked;

            Load += OnCustomCalendarLoad;

            Size = new Size(512, 440);
            DoubleBuffered = true;
            
            Paint += CalendarPaint;
            
            Size = new Size(512, 440);

            Controls.Add(_btnRight);
            Controls.Add(_btnLeft);
            Controls.Add(_btnToday);
            Controls.Add(_toolTip);

            MouseMove += OnCalendarMouseMove;

            ResumeLayout(false);
        }

        private void OnRightButtonClicked(object sender)
        {
            _calendarDate = _calendarDate.AddMonths(1);
            _capacityProvider.GetCapacity(_calendarDate, _capacity);
            Refresh();
        }

        private void OnLeftButtonClicked(object sender)
        {
            _calendarDate = _calendarDate.AddMonths(-1);
            _capacityProvider.GetCapacity(_calendarDate, _capacity);
            Refresh();
        }

        private void OnTodayButtonClicked(object sender)
        {
            _calendarDate = DateTime.Now;
            Refresh();
        }

        private void OnCalendarMouseMove(object sender, MouseEventArgs e)
        {
            if (!ShowToolTips)
            {
                return;
            }

            _toolTip.Hide();

            var day = GetDayFromCoordinate(e.X, e.Y);
            if (day == null)
            {
                return;
            }

            foreach (var detail in day.Details)
            {
                if (detail.Contains(e.X, e.Y))
                {
                    _toolTip.Shift = detail.ShiftValue;
                    _toolTip.Total = detail.Total;
                    _toolTip.Used = detail.Used;

                    _toolTip.ToolTipText = @"moo";
                    _toolTip.Location = new Point(e.X + 5, e.Y - _toolTip.Size.Height);
                    _toolTip.Show();


                    return;
                }
            }
        }

        public override void Refresh()
        {
            _calendarDayPoints.Clear();
            base.Refresh();
        }

        private void CalendarPaint(object sender, PaintEventArgs e)
        {
            _calendarDayPoints.Clear();
            RenderCalendar(e);
        }

        public void AddTodayHandler(CustomButton.ButtonClickedArgs args)
        {
            _btnToday.ButtonClicked += args;
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
                    var heightOfMonthAndYear = (int) g.MeasureString(monthAsString + " " + yearAsString, _dateHeaderFont).Height;
                    int dateHeaderSize = heightOfMonthAndYear + 20;
                    int daySpace = MaxDaySize(g).Height;
                    int effectiveWidth = ClientSize.Width - MarginSize;
                    int effectiveHeight = ClientSize.Height - (daySpace + dateHeaderSize + MarginSize);
                    var monthInfo = CalculateNumberOfWeeks(_calendarDate.Year, _calendarDate.Month);
                    int cellWidth = effectiveWidth / NumberOfDaysAWeek;
                    int cellHeight = (effectiveHeight - 30) / monthInfo.NumberOfWeeks;

                    int xStart = MarginSize;
                    int yStart = MarginSize + dateHeaderSize + daySpace + 10;
                    

                    // Draw grid and dates
                    var gray = new SolidBrush(Color.FromArgb(170, 170, 170));
                    var black = Brushes.Black;

                    var dayCount = 0;
                    var thisMonthDayOffset = 0;
                    foreach (var day in monthInfo.Days)
                    {
                        var brush = day.IsRogue ? gray : black;
                        g.DrawString(day.ToString(), _dayOfWeekFont, brush, xStart + (cellWidth - g.MeasureString(day.ToString(), _dayOfWeekFont).Width) / 2, yStart + (cellHeight - g.MeasureString(day.ToString(), _dayOfWeekFont).Height) / 2 - 1);

                        var dayPoint = new CalendarDayPoint(day.Date, new Rectangle(xStart, yStart, cellWidth, cellHeight));

                        _calendarDayPoints.Add(dayPoint);

                        if (HighlightCurrentDay & IsToday(_calendarDate, day))
                        {
                            g.CompositingQuality = CompositingQuality.GammaCorrected;
                            var dashed = new Pen(Color.Black, 0.5f) {DashStyle = DashStyle.Dot};
                            var bold = new Pen(Color.Blue, 1.5f);
                            g.DrawRectangle(bold, xStart, yStart, cellWidth - 1, cellHeight - 1);
                            g.DrawRectangle(dashed, xStart + 1f, yStart + 1f, cellWidth - 3f, cellHeight - 3f);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(128, 204, 229, 255)), xStart, yStart, cellWidth, cellHeight);
                        }

                        // We do the capacity drawing here
                        if (!day.IsRogue && thisMonthDayOffset < monthInfo.DaysInMonth)
                        {
                            var dayCapacity = _capacity[thisMonthDayOffset];
                            // 0...am,  1...pm,  0.. total ... 1.. available
                            var amRow = dayCapacity[0];
                            var pmRow = dayCapacity[1];
                            if (amRow[0] > 0)   // AM Total
                            {
                                var x = xStart;
                                var y = yStart;
                                var width = cellWidth;
                                var height = cellHeight / 2;

                                var fractionUsed = (decimal)amRow[1] / amRow[0];
                                g.FillRectangle(new SolidBrush(Color.FromArgb(128, 91, 182, 146)), x, y, (int)(width * fractionUsed), height);

                                dayPoint.AddDetail(new CapacityDetail(Shift.Am, amRow[0], amRow[1]) {DrawArea = new Rectangle(x, y, width, height)});
                            }

                            if (pmRow[0] > 0)   // PM Total
                            {
                                var x = xStart;
                                var y = yStart + cellHeight / 2;
                                var width = cellWidth;
                                var height = cellHeight / 2;

                                var fractionUsed = (decimal)pmRow[1] / pmRow[0];

                                g.FillRectangle(new SolidBrush(Color.FromArgb(128, 72, 37, 152)), x, y, (int)(width * fractionUsed), height);

                                dayPoint.AddDetail(new CapacityDetail(Shift.Pm, amRow[0], amRow[1]) { DrawArea = new Rectangle(x, y, width, height)});
                            }

                            thisMonthDayOffset++;
                        }
                        


                        if (++dayCount % NumberOfDaysAWeek == 0)
                        {
                            xStart = MarginSize;
                            yStart += cellHeight;
                        }
                        else
                        {
                            xStart += cellWidth;
                        }
                    }

                    var endOfGrid = yStart;

                    // Draw day names
                    int xMondayBegin = 0, xSundayEnd = 0;
                    yStart = MarginSize + dateHeaderSize;
                    foreach (var dayName in _dayNames)
                    {
                        var measure = g.MeasureString(dayName, _dayOfWeekFont);
                        var x = (int) (xStart + (cellWidth - measure.Width) / 2);
                        g.DrawString(dayName, _dateHeaderFont, Brushes.Black, x, yStart);
                        if (dayName.Equals("Mon"))
                        {
                            xMondayBegin = x;
                        } 
                        else if (dayName.Equals("Sun"))
                        {
                            xSundayEnd = x + (int) measure.Width;
                        }
                        
                        xStart += cellWidth;
                    }

                    // Draw line
                    var pen = Pens.LightGray;
                    yStart = MarginSize + dateHeaderSize + daySpace + 5;
                    g.DrawLine(pen, xMondayBegin, yStart, xSundayEnd, yStart);

                    // Position header and left/right buttons
                    _btnLeft.Location = new Point(xMondayBegin -(int) (_btnLeft.Width * 0.25), MarginSize);
                    _btnRight.Location = new Point(xSundayEnd - (int) (_btnRight.Width * 0.75), MarginSize);
                    
                    // Draw month
                    var monthAndYear = _calendarDate.ToString("MMMM") + " " + _calendarDate.ToString("yyyy");
                    var monthAndYearWidth = (int) g.MeasureString(monthAndYear, _dateHeaderFont).Width;
                    g.DrawString(monthAndYear, _dateHeaderFont, Brushes.Black, (ClientSize.Width - monthAndYearWidth) / 2 + 1, MarginSize + 5.5f);

                    // Draw Today button
                    _btnToday.Location = new Point((ClientSize.Width - _btnToday.Width) / 2, endOfGrid);

                }
                e.Graphics.DrawImage(bitmap, 0, 0, ClientSize.Width, ClientSize.Height);
            }
        }

        // private void OnCalendarMouseMove(object sender, MouseEventArgs e)
        // {
        //     throw new NotImplementedException();
        // }

        private void OnCustomCalendarLoad(object sender, EventArgs e)
        {
            // Here we load our capacity data.. do this async as some point
            _capacity = _capacityProvider.GetCapacity(_calendarDate);

        }

        public DateTime? GetDateFromCoordinate(int x, int y)
        {
            return GetDayFromCoordinate(x, y)?.Date;
        }

        public CalendarDayPoint GetDayFromCoordinate(int x, int y)
        {
            foreach (var dayPoint in _calendarDayPoints)
            {
                if (dayPoint.Contains(x, y))
                {
                    return dayPoint;
                }
            }
            return null;
        }



        private static bool IsToday(DateTime date, Day day)
        {
            var now = DateTime.Now;
            return date.Year == now.Year && date.Month == now.Month && day.Value == now.Day && !day.IsRogue;
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
                var prevMonthDate = _calendarDate.AddMonths(-1);
                var prevNoOfDays = DateTime.DaysInMonth(prevMonthDate.Year, prevMonthDate.Month);
                while (leftPadding-- > 0)
                {
                    days.Add(new Day(prevNoOfDays - leftPadding, true, new DateTime(prevMonthDate.Year, prevMonthDate.Month, prevNoOfDays - leftPadding)));
                }
            }

            for (var i = 1; i <= daysInMonth; i++)
            {
                days.Add(new Day(i, false, new DateTime(_calendarDate.Year, _calendarDate.Month, i)));
            }

            if (rightPadding > 0)
            {
                var nextMonthDate = _calendarDate.AddMonths(1);
                while (rightPadding-- > 0)
                {
                    days.Add(new Day(rightPadding + 1, true, new DateTime(nextMonthDate.Year, nextMonthDate.Month, rightPadding + 1)));
                }
            }

            return days;
        }

        private SizeResult MaxDaySize(Graphics g)
        {
            var sizes = new[]
            {
                g.MeasureString("Mon", _dayOfWeekFont),
                g.MeasureString("Tue", _dayOfWeekFont),
                g.MeasureString("Wed", _dayOfWeekFont),
                g.MeasureString("Thu", _dayOfWeekFont),
                g.MeasureString("Fri", _dayOfWeekFont),
                g.MeasureString("Sat", _dayOfWeekFont),
                g.MeasureString("Sun", _dayOfWeekFont),
            };

            return new SizeResult
            {
                Width = (int) sizes.Select(f => f.Width).Max(),
                Height = (int) sizes.Select(f => f.Height).Max()
            };
        }

        private SizeResult MaxDateSize(Graphics g)
        {
            var maxWidth = g.MeasureString("0", _dayOfWeekFont).Width;
            var maxHeight = g.MeasureString("0", _dayOfWeekFont).Height;
            for (var i = 1; i <= 31; i++)
            {
                var text = i.ToString(CultureInfo.InvariantCulture);
                var width = g.MeasureString(text, _dayOfWeekFont).Width;
                var height = g.MeasureString(text, _dayOfWeekFont).Height;
                if (width > maxWidth)
                {
                    maxWidth = width;
                }
                if (height > maxHeight)
                {
                    maxHeight = height;
                }
            }

            return new SizeResult
            {
                Width = (int) maxWidth,
                Height = (int) maxHeight
            };
        }

        private struct SizeResult
        {
            public int Height;
            public int Width;
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
            public Day(int value, bool isRogue, DateTime date)
            {
                Value = value;
                IsRogue = isRogue;
                Date = date;
            }

            public int Value { get; private set; }
            
            public DayOfWeek DayOfWeek { get; set; }
            
            public bool IsRogue { get; set; }
            
            public DateTime Date { get; }

            public override string ToString()
            {
                return Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}