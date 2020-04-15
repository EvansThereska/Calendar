using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CapacityDateTimePicker : UserControl
    {
        private readonly Regex _todayDate = new Regex("Today: ([0-9]{1,2}?)/([0-9]{1,2})/([0-9]{4})");

        private ButtonTextBox _btnTxtBox;
        private CapacityCalendar _calendar;

        private readonly ICapacityProvider _capacityProvider;

        private bool _isVisible;

        public CapacityDateTimePicker(ICapacityProvider capacityProvider)
        {
            _capacityProvider = capacityProvider;
            InitializeComponents();
            _btnTxtBox.ButtonClick += OnButtonClick;
        }

        private void InitializeComponents()
        {
            _btnTxtBox = new ButtonTextBox();
            _calendar = new CapacityCalendar(_capacityProvider);
            SuspendLayout();

            // this._btnTxtBox.Location = new System.Drawing.Point(505, 27);
            // this._btnTxtBox.Name = "_btnTxtBox";
            // this._btnTxtBox.ReadOnly = true;

            _calendar.Visible = false;
            _calendar.ShowToolTips = true;

            _calendar.MouseClick += OnCalendarMouseClick;
            _calendar.AddTodayHandler(TodayHandler);

            _btnTxtBox.Size = new Size(210, 20);
            // this._btnTxtBox.TabIndex = 0;
            // this._btnTxtBox.Visible = true;
            Controls.Add(_btnTxtBox);
            Controls.Add(_calendar);
            ResumeLayout(false);
        }

        private void TodayHandler(object sender)
        {
            var text = sender.ToString();
            var match = _todayDate.Match(text);
            _btnTxtBox.Text = $@"{match.Groups[1]}/{match.Groups[2]}/{match.Groups[3]}";
            if (!string.IsNullOrEmpty(_btnTxtBox.Text))
            {
                _calendar.Visible = false;
                _isVisible = false;
            }
        }

        private void OnCalendarMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var date = _calendar.GetDateFromCoordinate(e.X, e.Y);
                _btnTxtBox.Text = date.HasValue ? date.Value.ToString("dd/MM/yyyy") : string.Empty;
                if (!string.IsNullOrEmpty(_btnTxtBox.Text))
                {
                    _calendar.Visible = false;
                    _isVisible = false;
                }

            }
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