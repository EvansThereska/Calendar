using System.Drawing;

namespace PraxGroup.Sorax.CapacityCalendar.Demo
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";

            this._btnTxtBox = new PraxGroup.Sorax.CapacityCalendar.Main.ButtonTextBox();
            this._btnTxtBox.Location = new System.Drawing.Point(505, 27);
            this._btnTxtBox.Name = "_btnTxtBox";
            this._btnTxtBox.ReadOnly = true;
            this._btnTxtBox.Size = new System.Drawing.Size(210, 20);
            this._btnTxtBox.TabIndex = 0;

            this._calendar = new PraxGroup.Sorax.CapacityCalendar.Main.CapacityCalendar();
            this._calendar.Location = new System.Drawing.Point(12, 12);
            this._calendar.Size = new System.Drawing.Size(300, 200);
            this._calendar.Name = "_calendar";
            this._calendar.BackColor = Color.White;
            this._calendar.HighlightCurrentDay = true;
            this._calendar.CapacityProvider = new RandomCapacityProvider();

            this.Controls.Add(this._btnTxtBox);
            this.Controls.Add(this._calendar);
        }

        private PraxGroup.Sorax.CapacityCalendar.Main.ButtonTextBox _btnTxtBox;
        private PraxGroup.Sorax.CapacityCalendar.Main.CapacityCalendar _calendar;

        #endregion
    }
}

