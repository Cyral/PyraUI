using Pyratron.UI.Brushes;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    internal class Window : StackPanel
    {
        public static readonly DependencyProperty<string> TitleProperty =
            DependencyProperty.Register<Window, string>(nameof(Title), "Window",
                new PropertyMetadata(MetadataOption.IgnoreInheritance));

        public static readonly DependencyProperty<Brush> BackgroundColorProperty =
            DependencyProperty.Register<Window, Brush>(nameof(Background), new Color(240, 240, 240),
                new PropertyMetadata(MetadataOption.IgnoreInheritance));

        /// <summary>
        /// The title of the window.
        /// </summary>
        public string Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// The background of the window.
        /// </summary>
        public Brush Background
        {
            get { return GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        private readonly Element contentArea;

        private readonly Label titleLabel;

        public Window(Manager manager) : base(manager)
        {
            titleLabel = new Label(manager, Title)
            {
                Padding = new Thickness(8, 12),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlignment = Alignment.TopLeft
            };
            base.Add(titleLabel);
            contentArea = new StackPanel(manager)
            {
                Margin = 20,
            };
            base.Add(contentArea);
            Margin = 16;
        }

        public override void Add(Element element)
        {
            contentArea.Add(element);
        }

        public override void Draw(float delta)
        {
            Manager.Renderer.FillRectangle(BorderArea, Background, ParentBounds);
            Manager.Renderer.DrawRectangle(BorderArea, Color.DarkGray, 1, ParentBounds);
            Manager.Renderer.FillRectangle(titleLabel.BorderArea, Color.DarkGray, ParentBounds);
            base.Draw(delta);
        }

        protected override void OnPropertyChanged(DependencyProperty property, object newValue, object oldValue)
        {
            if (property == TitleProperty)
            {
                titleLabel.Text = (string) newValue;
            }
            base.OnPropertyChanged(property, newValue, oldValue);
        }
    }
}