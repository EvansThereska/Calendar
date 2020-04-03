using System;
using System.Drawing;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    internal class TodayButton : CustomButton
    {
        public TodayButton()
        {
            Size = new Size(100, 40);
            Name = nameof(TodayButton);
            Text = @"Today: " + DateTime.Now.ToString("dd/MM/yyyy");
        }

        public sealed override string Text { get; set; }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TodayButton
            // 
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.Name = "TodayButton";
            this.Size = new System.Drawing.Size(10, 10);
            this.ResumeLayout(false);

        }
    }
}