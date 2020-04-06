using System;
using System.Drawing;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CustomButton : UserControl
    {
        private const int MarginSize = 5;

        private bool _mouseOver;
        private bool _mouseDown;

        public CustomButton()
        {
            InitialiseComponents();
        }

        public delegate void ButtonClickedArgs (object sender);

        public event ButtonClickedArgs ButtonClicked;


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
            MouseDown += OnCustomButtonMouseDown;
            MouseUp += OnCustomButtonMouseUp;
            SizeChanged += OnCustomButtonSizeChanged;
            ResumeLayout(false);
        }

        private void OnCustomButtonSizeChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void OnCustomButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            
            if (_mouseDown)
            {
                ButtonClicked?.Invoke(this);
            }
            _mouseDown = false;
        }

        private void OnCustomButtonMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mouseDown = true;
            }
        }

        private void OnCustomButtonMouseLeave(object sender, EventArgs e)
        {
            _mouseOver = false;
            _mouseDown = false;
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