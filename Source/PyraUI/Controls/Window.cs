using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    internal class Window : Visual
    {
        private static readonly Color background = new Color(240, 240, 240);

        public Color Background { get; set; } = background;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                titleLabel.Text = title;
            }
        }

        private readonly Label titleLabel;
        private string title;

        public Window(Manager manager) : base(manager)
        {
            title = "Window";
           // titleLabel = new Label(manager, Title) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Top, TextAlignment = Alignment.TopLeft};
            //Add(titleLabel);

            Padding = 8;
            Margin = 16;
        }

        public override void Draw(float delta)
        {
            Manager.Renderer.FillRectangle(BorderArea, Background, ParentBounds);
            //Manager.Renderer.FillRectangle(titleLabel.ExtendedArea, Color.DarkGray, ParentBounds);
            base.Draw(delta);
        }
    }
}