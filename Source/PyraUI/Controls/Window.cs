using Pyratron.UI.Brushes;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    internal class Window : Control
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

        private readonly TextBlock titleLabel;
        private readonly Border windowBorder, titleBorder;
        private readonly StackPanel containerPanel;

        public Window(Manager manager) : base(manager)
        {
            // TODO: Replace with control template.
            containerPanel = new StackPanel(manager);
            windowBorder =new Border(manager)
            {
                BorderThickness = 1,
                Background = this.Background,
                BorderBrush = Color.DarkGray,
            };
            base.Add(windowBorder);
            titleBorder = new Border(manager)
            {
                Background = Color.DarkGray,
                Padding = new Thickness(8, 12),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
            };
            titleLabel = new TextBlock(manager, Title)
            {
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
        }

        public override void Add(Element element)
        {
            contentArea.Add(element);
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