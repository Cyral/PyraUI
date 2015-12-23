using Pyratron.UI.Brushes;
using Pyratron.UI.Effects;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    internal class Window : Control
    {
        public static readonly DependencyProperty<string> TitleProperty =
            DependencyProperty.Register<Window, string>(nameof(Title), "Window",
                new PropertyMetadata(MetadataOption.IgnoreInheritance));

        public static readonly DependencyProperty<Brush> BackgroundProperty =
            DependencyProperty.Register<Window, Brush>(nameof(Background), (ColorBrush)"#ebedf0",
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
            get { return GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        private readonly Element contentArea;

        private readonly TextBlock titleLabel;
        private readonly Border windowBorder, titleBorder;
        private readonly StackPanel containerPanel;

        static Window()
        {
        }
        public Window(Manager manager) : base(manager)
        { 
            // TODO: Replace with control template.
            containerPanel = new StackPanel(manager);
            var background = (Color)"#323741";
            windowBorder =new Border(manager)
            {
                CornerRadius = 5,
                BorderThickness = 1,
                Background = Background,
                BorderBrush = background,
            };
            base.Add(windowBorder);
            titleBorder = new Border(manager)
            {
                Background = background,
                Padding = new Thickness(8, 12),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
            };
            titleLabel = new TextBlock(manager, Title)
            {
                FontStyle = FontStyle.Bold,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextAlignment = Alignment.TopLeft
            };
            windowBorder.Add(containerPanel);
            containerPanel.Add(titleBorder);
            titleBorder.Add(titleLabel);
            contentArea = new StackPanel(manager)
            {
                Margin = 8,
            };
            containerPanel.Add(contentArea);
            Margin = 16;

            Effects.Add(new DropShadowEffect(manager, 12, Color.Black * .2f));
        }

        public override void Add(Element element)
        {
            contentArea.Add(element);
        }

        public override void AddContent(string content)
        {
            contentArea.AddContent(content);
        }

        protected override void OnPropertyChanged(DependencyProperty property, object newValue, object oldValue)
        {
            if (property == TitleProperty)
            {
                titleLabel.Text = (string) newValue;
            }
            if (property == BackgroundProperty)
            {
                windowBorder.Background = (Brush) newValue;
            }
            base.OnPropertyChanged(property, newValue, oldValue);
        }
    }
}