using System;
using System.Drawing;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CustomToolTip : UserControl
    {
        private static bool _firstTime = true;

        private Font _font = DefaultFont;

        private string _text;

        private Margin _margin;

        public Margin ToolTipMargin
        {
            get => _margin;
            set
            {
                _margin = value;
                Refresh();
            }
        }

        public Font ToolTipFont
        {
            get => _font;
            set
            {
                _font = value;
                Refresh();
            }
        }

        public string ToolTipText
        {
            get => _text;
            set
            {
                _text = value;
                Refresh();
            }
        }
        
        public CustomToolTip()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            BackColor = Color.Transparent;
            ToolTipMargin = new Margin(5, 5, 5, 5);
            DoubleBuffered = true;
            Name = "CustomToolTip";
            Paint += CustomEventToolTipPaint;
            ResumeLayout(false);
        }

        private void CustomEventToolTipPaint(object sender, PaintEventArgs e)
        {
            var textSize = CreateGraphics().MeasureString(ToolTipText, ToolTipFont);

            var width = Math.Max(65, (int) textSize.Width + ToolTipMargin.Left + ToolTipMargin.Right);
            var height = Math.Max(48, (int) textSize.Height + ToolTipMargin.Top + ToolTipMargin.Bottom);

            Size = new Size(width, height);

            using (var bmp = new Bitmap(ClientSize.Width, ClientSize.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    using (var pen = new Pen(Color.Black))
                    {
                        g.DrawRectangle(pen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
                        g.FillRectangle(Brushes.Beige, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);

                        g.DrawString(ToolTipText, ToolTipFont, Brushes.Black, (ClientSize.Width - textSize.Width) / 2 - ToolTipMargin.Right, (ClientSize.Height - textSize.Height) / 2 - ToolTipMargin.Bottom);
                    }
                }
                e.Graphics.DrawImage(bmp, 0, 0);
            }

            if (_firstTime)
            {
                // Bit of a hack to fix some other time
                Visible = false;
                _firstTime = false;
            }
        }
    }



    public class Margin
    {
        public Margin(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }
    }
}