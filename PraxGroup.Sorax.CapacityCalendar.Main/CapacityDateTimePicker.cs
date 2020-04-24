using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    public class CapacityDateTimePicker : UserControl
    {
        private static readonly Regex TodayRegex = new Regex("Today: ([0-9]{1,2}?)/([0-9]{1,2})/([0-9]{4})");

        private readonly Form _parentForm;
        private ButtonTextBox _btnTxtBox;

        private readonly PopupWindowHelper _windowHelper;

        private bool _isVisible;

        public CapacityDateTimePicker(Form parentForm)
        {
            _parentForm = parentForm;
            InitializeComponents();
            _windowHelper = new PopupWindowHelper();
            _windowHelper.PopupClosed += OnPopupClosed;
            _windowHelper.PopupCancel += OnPopupCancelled;
            _btnTxtBox.ButtonClick += OnButtonClick;
        }

        private void InitializeComponents()
        {
            _btnTxtBox = new ButtonTextBox();
            SuspendLayout();

            // _calendar.MouseClick += OnCalendarMouseClick;
            // _calendar.AddTodayHandler(TodayHandler);

            _btnTxtBox.Size = new Size(210, 20);
            Controls.Add(_btnTxtBox);
            ResumeLayout(false);
        }

        // private void TodayHandler(object sender)
        // {
        //     var text = sender.ToString();
        //     var match = _todayRegex.Match(text);
        //     _btnTxtBox.Text = $@"{match.Groups[1]}/{match.Groups[2]}/{match.Groups[3]}";
        //     if (!string.IsNullOrEmpty(_btnTxtBox.Text))
        //     {
        //         _calendar.Visible = false;
        //         _isVisible = false;
        //     }
        // }

        // private void OnCalendarMouseClick(object sender, MouseEventArgs e)
        // {
        //     if (e.Button == MouseButtons.Left)
        //     {
        //         var date = _calendar.GetDateFromCoordinate(e.X, e.Y);
        //         _btnTxtBox.Text = date.HasValue ? date.Value.ToString("dd/MM/yyyy") : string.Empty;
        //         if (!string.IsNullOrEmpty(_btnTxtBox.Text))
        //         {
        //             _calendar.Visible = false;
        //             _isVisible = false;
        //         }
        //
        //     }
        // }

        protected override void OnHandleCreated(EventArgs e)
        {
            _windowHelper.AssignHandle(_parentForm.Handle);
        }

        private void OnPopupCancelled(object sender, PopupCancelEventArgs e)
        {
            _isVisible = false;
        }

        private void OnPopupClosed(object sender, PopupClosedEventArgs e)
        {
            _isVisible = false;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (_isVisible)
            {
                _windowHelper.ClosePopup();
                _isVisible = false;
            }
            else
            {
                _btnTxtBox.SetActive();
                var calendar = new frmCapacityCalendar();
                // var xPoint = _btnTxtBox.Location.X;
                // var yPoint = _btnTxtBox.Location.Y + _btnTxtBox.Height;

                var xPoint = _btnTxtBox.Left;
                var yPoint = _btnTxtBox.Bottom;

                _windowHelper.ShowPopup(_parentForm, calendar, this.PointToScreen(new Point(xPoint, yPoint)));
                // _calendar.Location = new Point(xPoint, yPoint);
                // _calendar.BackColor = Color.White;
                // _calendar.Size = new Size(_btnTxtBox.Width + 40, 200);
                // _calendar.HighlightCurrentDay = true;
                // _calendar.Visible = true;
                _isVisible = true;
            }
        }
    }
}