using System.Drawing;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    internal class NavigateRightButton : CustomButton
    {
        public NavigateRightButton()
        {
            Size = new Size(15, 15);
            Name = nameof(NavigateRightButton);
            Text = @">";
        }

        public sealed override string Text { get; set; }
    }
}