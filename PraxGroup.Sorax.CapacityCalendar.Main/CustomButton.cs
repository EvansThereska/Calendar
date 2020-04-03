using System;
using System.Drawing;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CustomButton : UserControl
    {
        private bool _mouseOver;
        private const int MarginSize = 5;

        public CustomButton()
        {
            InitialiseComponents();
        }

        private void InitialiseComponents()
        {
            SuspendLayout();
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Size = new Size(100, 40);
            MouseEnter += OnCustomButtonMouseEnter;
            MouseLeave += OnCustomButtonMouseLeave;
            Paint += OnCustomButtonPaint;
            LostFocus += OnFocus;
            GotFocus += OnFocus;
            // this.MouseLeave += new System.EventHandler(this.CoolButtonMouseLeave);
            // this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CoolButtonMouseUp);
            // this.SizeChanged += new System.EventHandler(this.CoolButtonSizeChanged);
            ResumeLayout(false);
        }

        private void OnCustomButtonMouseLeave(object sender, EventArgs e)
        {
            _mouseOver = false;
            Refresh();
        }

        private void OnCustomButtonMouseEnter(object sender, EventArgs e)
        {
            _mouseOver = true;
            Refresh();
        }

        private void OnFocus(object sender, EventArgs e)
        {
            Refresh();
        }

        private void OnCustomButtonPaint(object sender, PaintEventArgs e)
        {
            using (var bmp = new Bitmap(ClientSize.Width, ClientSize.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    using (var p = new Pen(Color.White))
                    {
                        g.FillRectangle(p.Brush, ClientRectangle);
                    }
                    var measure = e.Graphics.MeasureString(Text, DefaultFont);
                    Size = new Size((int) measure.Width + MarginSize * 2, (int) measure.Height + MarginSize * 2);
                    var brush = _mouseOver ? Brushes.DodgerBlue : Brushes.Black;
                    g.DrawString(Text, DefaultFont, brush, (ClientSize.Width - measure.Width) / 2, (ClientSize.Height - measure.Height) / 2);
                }

                e.Graphics.DrawImage(bmp, 0, 0, ClientSize.Width, ClientSize.Height);
            }
        }
    }
}