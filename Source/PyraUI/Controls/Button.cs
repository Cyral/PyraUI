using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    internal class Button : Control
    {
        public Button(Manager manager) : base(manager)
        {
            Padding = new Thickness(5, 8);

      
            Height = 32;
            MinHeight = 20;
            MinWidth = 32;
        }

        public override void Draw(float delta)
        {
            var thickness = 2;
            var brush = Color.Gray;
            var background = Color.DarkGray;
            var radius = 5;
            Manager.Renderer.DrawRectangle(BorderArea, brush, thickness, radius, ParentBounds);
            Manager.Renderer.FillRectangle(BorderArea.RemoveBorder(thickness), background, radius - thickness,
                ParentBounds);

            base.Draw(delta);
        }
    }
}