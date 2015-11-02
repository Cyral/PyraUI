using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    internal class Border : Control
    {
        public Color Background { get; set; }

        public Border(Manager manager) : base(manager)
        {
            Margin = Padding = 0;
        }

        public override void Draw(float delta)
        {
            Manager.Renderer.DrawRectangle(BorderArea, Background, ParentBounds);
            base.Draw(delta);
        }
    }
}