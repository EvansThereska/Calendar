using System.Collections.Generic;
using System.Drawing;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public interface IDayRenderer
    {
        void RenderDay(Graphics g, Day day, int xStart, int cellWidth, int yStart, int cellHeight);

        void HighlightDay(Graphics g, int xStart, int cellWidth, int yStart, int cellHeight);

        List<CapacityDetail> RenderCapacity(Graphics g, int[][] capacity, int xStart, int yStart, int cellWidth, int cellHeight);
    }
}