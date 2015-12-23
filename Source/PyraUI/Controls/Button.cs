using Pyratron.UI.Brushes;
using Pyratron.UI.States;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Input;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    internal class Button : Control
    {
        public static readonly DependencyProperty<Brush> BackgroundProperty =
            DependencyProperty.Register<Window, Brush>(nameof(Background),
                new GradientBrush((Color) "#4fcfe6", (Color) "#14b9d7"),
                new PropertyMetadata(MetadataOption.IgnoreInheritance));

        public Brush Background
        {
            get { return GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        private readonly Border border;

        static Button()
        {
            MinHeightProperty.OverrideMetadata(typeof (Button), 16);
            MinWidthProperty.OverrideMetadata(typeof (Button), 32);
            PaddingProperty.OverrideMetadata(typeof (Button), new Thickness(5, 8));
            FontStyleProperty.OverrideMetadata(typeof (Button), FontStyle.Bold);
            TextColorProperty.OverrideMetadata(typeof (Button), Color.White);
        }

        public Button(Manager manager) : base(manager)
        {
            // Todo: Use content template
            border = new Border(manager)
            {
                BorderThickness = new Thickness(0, 0, 0, 2),
                Background = Background,
                BorderBrush = (Color) "#3198ab",
                CornerRadius = 5,
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

        protected override void OnPropertyChanged(DependencyProperty property, object newValue, object oldValue)
        {
            if (property == BackgroundProperty)
            {
                border.Background = (Brush)newValue;
            }
            base.OnPropertyChanged(property, newValue, oldValue);
        }
    }
}