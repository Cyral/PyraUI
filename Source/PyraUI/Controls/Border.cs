using Pyratron.UI.Brushes;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// Draws a border and/or background around an element.
    /// </summary>
    public class Border : Decorator
    {
        public Brush Background { get; set; } = Color.Transparent;

        public int CornerRadius { get; set; }

        public Thickness BorderThickness { get; set; }

        public Brush BorderBrush { get; set; }

        public Border(Manager manager) : base(manager)
        {
            Margin = Padding = 0;
        }

        public override void Draw(float delta)
        {
            if (!BorderThickness.IsEmpty)
            {
                Manager.Renderer.DrawRectangle(BorderArea, BorderBrush, BorderThickness, CornerRadius, ParentBounds);
                Manager.Renderer.FillRectangle(BorderArea.RemoveBorder(BorderThickness), Background, CornerRadius - BorderThickness.Max,
                    ParentBounds);
            }
            else
                Manager.Renderer.FillRectangle(BorderArea.RemoveBorder(BorderThickness), Background, CornerRadius,
                    ParentBounds);

            base.Draw(delta);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Remove the border and padding, measure the element, and then add them back, as they must be ignored.
            availableSize = availableSize.Remove(BorderThickness).Remove(Padding);
            var size = base.MeasureOverride(availableSize) + BorderThickness + Padding;
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Child?.Arrange(new Rectangle(BorderThickness, finalSize.Remove(BorderThickness).Remove(Padding)));

            return finalSize;
        }
    }
}