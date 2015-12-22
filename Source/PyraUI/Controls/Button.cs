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

        private Border border;

        public Button(Manager manager) : base(manager)
        {
            // Todo: Use content template
            border = new Border(manager)
            {
                BorderThickness = new Thickness(0, 0, 0, 2),
                Background = Color.DarkGray,
                BorderBrush =  Color.Gray,
                CornerRadius =  5,
            };
            RemoveDirect(Presenter);
            AddDirect(border);
            border.Add(Presenter);
        }

        public override void AddContent(string content)
        {
            Presenter.Add(new TextBlock(Manager)
            {
                Text = content,
            });
        }
    }
}