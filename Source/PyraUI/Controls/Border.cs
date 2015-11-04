using Pyratron.UI.Brushes;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    internal class Border : Control
    {
        public Brush Background { get; set; }

        public double CornerRadius { get; set; }

        public bool Filled { get; set; } = true;

        public Border(Manager manager) : base(manager)
        {
            Margin = Padding = 0;
        }

        public override void Draw(float delta)
        {
             if (Filled)
                Manager.Renderer.FillRectangle(BorderArea, Background, CornerRadius, ParentBounds);
             else
                 Manager.Renderer.DrawRectangle(BorderArea, Background, CornerRadius, ParentBounds);

            base.Draw(delta);
        }
    }
}