using Pyratron.UI.Brushes;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    internal class Border : Control
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
    }
}