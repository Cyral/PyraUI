using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    public abstract class Panel : Visual //TODO: A visual for now just to show the outline.
    {
        public override int MaxChildren => int.MaxValue; // Panels are designed for containing a number of elements.

        public Panel(Manager manager) : base(manager)
        {
            Padding = Margin = 0;

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
        }
        
        // Force derived panels to implement their own layout logic.
        protected abstract override Size ArrangeOverride(Size finalSize);

        protected abstract override Size MeasureOverride(Size availableSize);
    }
}