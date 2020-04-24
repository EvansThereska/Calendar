﻿using System.Drawing;
using System.Windows.Forms;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    // ReSharper disable once InconsistentNaming (naming convention for forms)
    public class frmCapacityCalendar : Form
    {
        private System.ComponentModel.IContainer components = null;

        public frmCapacityCalendar()
        {
            InitializeComponent();

            Size = _calendar.Size;
        }

        private void InitializeComponent()
        {
            this._calendar = new CapacityCalendar(new DummyCapacityProvider());
            this.SuspendLayout();
            // 
            // _calendar
            // 
            this._calendar.HighlightCurrentDay = false;
            this._calendar.Location = new System.Drawing.Point(0, 0);
            this._calendar.Name = "_calendar";
            this._calendar.ShowToolTips = true;
            this._calendar.Size = new System.Drawing.Size(250, 200);
            this._calendar.TabIndex = 0;
            // 
            // frmCapacityCalendar
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(250, 198);
            this.ControlBox = false;
            this.Controls.Add(this._calendar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCapacityCalendar";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private PraxGroup.Sorax.CapacityCalendar.Main.CapacityCalendar _calendar;
    }
}