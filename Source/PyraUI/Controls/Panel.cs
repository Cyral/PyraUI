using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    public abstract class Panel : Visual
    {
        public override int MaxChildren => int.MaxValue; // Panels are designed for containing a number of elements.

        public Panel(Manager manager) : base(manager)
        {

        }
        
        // Force derived panels to implement their own layout logic.
        protected abstract override Size ArrangeOverride(Size finalSize);

        protected abstract override Size MeasureOverride(Size availableSize);
    }
}