using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    internal class Button : Control
    {
        static Button()
        {
            MinHeightProperty.OverrideMetadata(typeof(Button), 16);
            MinWidthProperty.OverrideMetadata(typeof(Button), 32);
            PaddingProperty.OverrideMetadata(typeof(Button), new Thickness(5, 8));
        }

        public Button(Manager manager) : base(manager)
        {

        }

        public override void AddContent(string content)
        {
            Presenter.Add(new TextBlock(Manager)
            {
                Text = content,
            });
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