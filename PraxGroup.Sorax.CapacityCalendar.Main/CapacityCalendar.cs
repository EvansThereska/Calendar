using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CapacityCalendar : UserControl
    {
        private readonly ICapacityProvider _capacityProvider;
        private readonly Font _dayOfWeekFont = DefaultFont;
        private readonly Font _dateHeaderFont = DefaultFont;

        private System.Timers.Timer _toolTipTimer;
        private int _xMouse;
        private int _yMouse;

        private readonly string[] _dayNames = {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"};

        private DateTime _calendarDate = DateTime.Now;

        private int[][][] _capacity;

        private const int MarginSize = 5;
        private const int NumberOfDaysAWeek = 7;

        public bool HighlightCurrentDay { get; set; }

        public IDayRenderer DayRenderer { get; set; }

        public bool ShowToolTips { get; set; }

        private CustomToolTip _toolTip;

        private readonly List<CalendarDayPoint> _calendarDayPoints = new List<CalendarDayPoint>();

        private TodayButton _btnToday;
        private NavigateLeftButton _btnLeft;
        private NavigateRightButton _btnRight;

        // These are determined by the rendering process
        private int _gridXLeft;
        private int _gridXRight;
        private int _gridYTop;
        private int _gridYBottom;

        public CapacityCalendar(ICapacityProvider capacityProvider) : this(capacityProvider, new AlternativeDayRenderer(DefaultFont))
        {
        }

        public CapacityCalendar(ICapacityProvider capacityProvider, IDayRenderer dayRenderer)
        {
            _capacityProvider = capacityProvider;
            DayRenderer = dayRenderer;
            InitializeComponents();
        }


        private void InitializeComponents()
        {
            SuspendLayout();

            _btnToday = new TodayButton {Name = "_btnToday"};
            _btnLeft = new NavigateLeftButton {Name = "_btnLeft"};
            _btnRight = new NavigateRightButton {Name = "_btnRight"};
            _toolTip = new CustomToolTip();
            _toolTipTimer = new System.Timers.Timer();
            _toolTipTimer.Elapsed += OnToolTipTimerElapsed;
            _toolTipTimer.AutoReset = false;
            _toolTipTimer.Interval = 1000;

            
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

        private void OnToolTipTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Invoke(new Action(() => _toolTip.Show()));
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

            if (!MouseReallyMoved(e))
            {
                return;
            }
            
            _toolTip.Hide();
            _toolTipTimer.Stop();

            var day = GetDayFromCoordinate(e.X, e.Y);
            if (day == null)
            {
                return;
            }

            foreach (var detail in day.Details)
            {
                if (detail.Contains(e.X, e.Y))
                {
                    var sb = new StringBuilder();
                    sb.Append("Shift: ").Append(detail.ShiftValue).Append("\n")
                        .Append("Total: ").Append(detail.Total).Append("\n")
                        .Append("Used: ").Append(detail.Used).Append("\n");

                    _toolTip.ToolTipText = sb.ToString();

                    _toolTip.Location = DetermineLocation(e);

                    _toolTipTimer.Start();

                    return;
                }
            }
        }

        private Point DetermineLocation(MouseEventArgs e)
        {
            var x = e.X;
            var y = e.Y;
            if (e.X + _toolTip.Size.Width > _gridXRight)
            {
                x = e.X - _toolTip.Size.Width;
            }

            if (e.Y - _toolTip.Size.Height < _gridYTop)
            {
                y = e.Y + _toolTip.Size.Height;
            }

            return new Point(x + 5, y - _toolTip.Size.Height);
        }

        private bool MouseReallyMoved(MouseEventArgs args)
        {
            if (args.X != _xMouse || args.Y != _yMouse)
            {
                _xMouse = args.X;
                _yMouse = args.Y;
                return true;
            }

            return false;
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
                    int dateHeaderSize = heightOfMonthAndYear + 10;
                    int daySpace = MaxDaySize(g).Height;
                    int effectiveWidth = ClientSize.Width - 2 * MarginSize;
                    int effectiveHeight = ClientSize.Height - (daySpace + dateHeaderSize + MarginSize);
                    var monthInfo = CalculateNumberOfWeeks(_calendarDate.Year, _calendarDate.Month);
                    int cellWidth = effectiveWidth / NumberOfDaysAWeek;
                    int cellHeight = (effectiveHeight - 30) / monthInfo.NumberOfWeeks;

                    int xStart = MarginSize;
                    int yStart = MarginSize + dateHeaderSize + daySpace + 5;
                    _gridYTop = yStart;
                    _gridXLeft = xStart;
                    
                    // Draw grid and dates

                    var dayCount = 0;
                    var thisMonthDayOffset = 0;
                    foreach (var day in monthInfo.Days)
                    {
                        DayRenderer.RenderDay(g, day, xStart, cellWidth, yStart, cellHeight);

                        var dayPoint = new CalendarDayPoint(day.Date, new Rectangle(xStart, yStart, cellWidth, cellHeight));
                        
                        _calendarDayPoints.Add(dayPoint);

                        if (HighlightCurrentDay & IsToday(_calendarDate, day))
                        {
                            DayRenderer.HighlightDay(g, xStart, cellWidth, yStart, cellHeight);
                        }

                        // We do the capacity drawing here
                        if (!day.IsRogue && thisMonthDayOffset < monthInfo.DaysInMonth)
                        {
                            var dayCapacity = _capacity[thisMonthDayOffset];

                            dayPoint.AddDetails(DayRenderer.RenderCapacity(g, dayCapacity, xStart, yStart, cellWidth, cellHeight));

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

                    _gridXLeft = MarginSize;
                    _gridXRight = ClientSize.Width - MarginSize;
                    var endOfGrid = yStart + 2;

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

                    // // Draw line
                    // var pen = Pens.LightGray;
                    // yStart = MarginSize + dateHeaderSize + daySpace + 5;
                    // g.DrawLine(pen, xMondayBegin, yStart, xSundayEnd, yStart);

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
    }
}