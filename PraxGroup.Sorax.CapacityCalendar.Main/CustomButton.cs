using System;
using System.Drawing;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CustomButton : Button
    {
        private const int MarginSize = 5;

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    
                    using (var p = new Pen(Color.White))
                    {
                        g.FillRectangle(p.Brush, ClientRectangle);
                    }
                    var measure = g.MeasureString(Text, DefaultFont);
                    Size = new Size((int) measure.Width + MarginSize * 2, (int) measure.Height + MarginSize * 2);
                    g.DrawString(Text, DefaultFont, Brushes.Black, (ClientSize.Width - measure.Width) / 2, (ClientSize.Height - measure.Height) / 2);
                }
                e.Graphics.DrawImage(bitmap, 0, 0, Size.Width, Size.Height);
            }

            Show();
            
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // Get rid of the annoying colour change on mouse enter
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            // base.OnMouseClick(e);
        }
    }
}