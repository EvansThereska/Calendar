using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class AlternativeDayRenderer : IDayRenderer
    {
        private float _margin = 0.6f;

        private readonly Font _font;

        public AlternativeDayRenderer(Font font)
        {
            _font = font;
        }

        public void RenderDay(Graphics g, Day day, int xStart, int cellWidth, int yStart, int cellHeight)
        {
            var gray = new SolidBrush(Color.FromArgb(170, 170, 170));
            var black = Brushes.Black;

            var brush = day.IsRogue ? gray : black;
            g.DrawString(day.ToString(), _font, brush,
                xStart + (_margin * cellWidth -  g.MeasureString(day.ToString(), _font).Width) / 2,
                yStart + (cellHeight - g.MeasureString(day.ToString(), _font).Height) / 2 - 1);

            using (var pen = new Pen(black, 1f))
            {
                g.DrawRectangle(pen, xStart, yStart, cellWidth, cellHeight);
            }
        }

        public void HighlightDay(Graphics g, int xStart, int cellWidth, int yStart, int cellHeight)
        {
            g.CompositingQuality = CompositingQuality.GammaCorrected;
            var dashed = new Pen(Color.Black, 0.5f) {DashStyle = DashStyle.Dot};
            g.DrawRectangle(dashed, xStart + 1f, yStart + 1f, cellWidth - 2f, cellHeight - 2f);
            g.FillRectangle(new SolidBrush(Color.FromArgb(128, 204, 229, 255)), xStart, yStart, cellWidth, cellHeight);
        }

        public List<CapacityDetail> RenderCapacity(Graphics g, int[][] capacity, int xStart, int yStart, int cellWidth, int cellHeight)
        {
            List<CapacityDetail> result = new List<CapacityDetail>();
            // 0...am,  1...pm,  0.. total ... 1.. available
            var amRow = capacity[0];
            var pmRow = capacity[1];

            var pen = new Pen(Color.Black, 1.0f);

            if (amRow[0] > 0) // AM Total
            {
                var x = xStart + _margin * cellWidth;
                var y = yStart;
                var width = (1 - _margin) * cellWidth;
                var height = cellHeight / 2;

                var fractionUsed = (decimal) amRow[1] / amRow[0];
                g.DrawRectangle(pen, x, y, width, height);
                g.FillRectangle(new SolidBrush(Color.FromArgb(128, 91, 182, 146)), x, y, (int) ((decimal) width * fractionUsed), height);
                result.Add(new CapacityDetail(Shift.Am, amRow[0], amRow[1]) {DrawArea = new Rectangle((int) x, y, (int) width, height)});
            }

            if (pmRow[0] > 0) // PM Total
            {
                var x = xStart + _margin * cellWidth;
                var y = yStart + cellHeight / 2;
                var width = (1 - _margin) * cellWidth;
                var height = cellHeight / 2 + 1;

                var fractionUsed = (decimal) pmRow[1] / pmRow[0];
                
                g.DrawRectangle(pen, x, y, width, height);
                g.FillRectangle(new SolidBrush(Color.FromArgb(128, 72, 37, 152)), x, y, (int) ((decimal) width * fractionUsed), height);

                result.Add(new CapacityDetail(Shift.Pm, pmRow[0], pmRow[1]) { DrawArea = new Rectangle((int) x, y, (int) width, height)});
            }

            return result;
        }
    }
}