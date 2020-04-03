using System.Drawing;

namespace PraxGroup.Sorax.CapacityCalendar.Main
{
    internal class NavigateLeftButton : CustomButton
    {
        public NavigateLeftButton()
        {
            Size = new Size(15, 15);
            Name = nameof(NavigateLeftButton);
            Text = @"<";
        }

        public sealed override string Text { get; set; }
    }
}