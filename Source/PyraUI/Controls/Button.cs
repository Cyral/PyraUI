using Pyratron.UI.Effects;
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
            FontStyleProperty.OverrideMetadata(typeof(Button), FontStyle.Bold);
            TextColorProperty.OverrideMetadata(typeof(Button), Color.White);
        }

        private Border border;

        public Button(Manager manager) : base(manager)
        {
            // Todo: Use content template
            border = new Border(manager)
            {
                BorderThickness = new Thickness(0, 0, 0, 2),
                Background = (Color)"#14b9d7",
                BorderBrush =  (Color)"#3198ab",
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