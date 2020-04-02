using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class ButtonTextBox : TextBox
    {
        private const string ImageName = "moo.png";

        private const int ButtonWidth = 25;

        private Button _button;

        public ButtonTextBox()
        {
            InitButton();
            SizeChanged += (o, e) => OnResize(e);
        }

        public event EventHandler ButtonClick
        {
            add => _button.Click += value;
            remove => _button.Click -= value;
        }

        private void InitButton()
        {
            _button = new Button();
            _button.Size = SetButtonSize();
            _button.Location = SetButtonLocation();
            _button.Cursor = DefaultCursor;
            // _button.Image = GetImage();
            Controls.Add(_button);
            PreventTextDisappearUnderButton();
        }

        private Point SetButtonLocation()
        {
            return new Point(ClientSize.Width - _button.Width + 1, -1);
        }

        private Size SetButtonSize()
        {
            return new Size(ButtonWidth, ClientSize.Height + 2);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _button.Size = SetButtonSize();
            _button.Location = SetButtonLocation();
            PreventTextDisappearUnderButton();
        }

        private Image GetImage()
        {
            
            var name = nameof(Main);
            var assembly = Assembly.GetExecutingAssembly();
            var myStream = assembly.GetManifestResourceStream($"{name}.{ImageName}");
            Debug.Assert(myStream != null, nameof(myStream) + " != null");
            return new Bitmap(myStream);
        }

        private void PreventTextDisappearUnderButton()
        {
            NativeCalls.SendMessage(Handle, 0xd3, (IntPtr) 2, (IntPtr) (_button.Width << 16));
        }
    }
}