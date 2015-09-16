using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    public class Panel : Visual //TODO: A visual for now just to show the outline.
    {
        public override int MaxChildren => int.MaxValue; // Panels are designed for containing a number of elements.

        public Panel(Manager manager) : base(manager)
        {
            Padding = Margin = 1;

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}