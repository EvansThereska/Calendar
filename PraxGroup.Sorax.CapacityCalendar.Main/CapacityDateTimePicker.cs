using System;
using System.Drawing;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CapacityDateTimePicker : UserControl
    {
        private ButtonTextBox _btnTxtBox;
        private CapacityCalendar _calendar;

        private bool _isVisible;

        public CapacityDateTimePicker()
        {
            InitializeComponents();
            _btnTxtBox.ButtonClick += OnButtonClick;
        }

        private void InitializeComponents()
        {
            _btnTxtBox = new ButtonTextBox();
            _calendar = new CapacityCalendar();
            SuspendLayout();

            // this._btnTxtBox.Location = new System.Drawing.Point(505, 27);
            // this._btnTxtBox.Name = "_btnTxtBox";
            // this._btnTxtBox.ReadOnly = true;

            _calendar.Visible = false;
            _btnTxtBox.Size = new System.Drawing.Size(210, 20);
            // this._btnTxtBox.TabIndex = 0;
            // this._btnTxtBox.Visible = true;
            Controls.Add(_btnTxtBox);
            Controls.Add(_calendar);
            ResumeLayout(false);
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (_isVisible)
            {
                _calendar.Visible = false;
                _isVisible = false;
                return;
            }
            
            var xPoint = _btnTxtBox.Location.X;
            var yPoint = _btnTxtBox.Location.Y + _btnTxtBox.Height;
            _calendar.Location = new Point(xPoint, yPoint);
            _calendar.BackColor = Color.White;
            _calendar.Size = new Size(_btnTxtBox.Width + 40, 200);
            _calendar.HighlightCurrentDay = true;
            _calendar.Visible = true;
            _isVisible = true;
        }


        
    }
}