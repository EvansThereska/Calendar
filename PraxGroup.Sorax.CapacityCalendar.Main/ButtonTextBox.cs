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

        private CheckBox _checkbox;

        public bool Checked => _checkbox.Checked;

        public ButtonTextBox()
        {
            SetupButton();
            SetupCheckbox();
            SizeChanged += (o, e) => OnResize(e);
        }

        public event EventHandler ButtonClick
        {
            add => _button.Click += value;
            remove => _button.Click -= value;
        }

        private void SetupButton()
        {
            _button = new Button();
            _button.Size = SetButtonSize();
            _button.Location = SetButtonLocation();
            _button.Cursor = DefaultCursor;
            // _button.Image = GetImage();
            Controls.Add(_button);
            PreventTextDisappearUnderButton();
        }

        private void SetupCheckbox()
        {
            _checkbox = new CheckBox();
            _checkbox.Size = SetCheckboxSize();
            _checkbox.Location = SetCheckboxLocation();
            _checkbox.CheckedChanged += OnCheckedChanged;
            Controls.Add(_checkbox);
            PreventTextDisappearUnderCheckbox();
        }

        private static Point SetCheckboxLocation()
        {
            return new Point(1, 0);
        }

        private Size SetCheckboxSize()
        {
            return new Size(ClientSize.Height, ClientSize.Height);
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
            _checkbox.Size = SetCheckboxSize();
            _checkbox.Location = SetCheckboxLocation();
            PreventTextDisappearUnderButton();
            PreventTextDisappearUnderCheckbox();
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (_checkbox.Checked)
            {
                ForeColor = Color.Black;
            }
            else
            {
                ForeColor = Color.Gray;
            }
        }


        public void SetActive()
        {
            _checkbox.Checked = true;
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
            NativeCalls.SendMessage(Handle, 
                NativeCalls.EM_SETMARGINS, 
                (IntPtr) NativeCalls.EM_RIGHTMARGIN, 
                (IntPtr) (_button.Width << 16));
        }

        private void PreventTextDisappearUnderCheckbox()
        {
            NativeCalls.SendMessage(Handle, 
                NativeCalls.EM_SETMARGINS, 
                (IntPtr) NativeCalls.EM_LEFTMARGIN, 
                (IntPtr) (_checkbox.Width));
        }
    }
}