using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    public class Label : Control
    {
        public static readonly DependencyProperty<string> TextProperty =
            DependencyProperty.Register<Element, string>(nameof(Text), "Label Text", new PropertyMetadata(
                MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange));

        public static readonly DependencyProperty<Alignment> TextAlignmentProperty =
            DependencyProperty.Register<Element, Alignment>(nameof(TextAlignment), Alignment.Center,
                new PropertyMetadata(
                    MetadataOption.IgnoreInheritance));

        /// <summary>
        /// The label's text.
        /// </summary>
        public string Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Alignment of label text.
        /// </summary>
        public Alignment TextAlignment
        {
            get { return GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public Label(Manager manager, string text) : this(manager)
        {
            Text = text;
        }

        public Label(Manager manager) : base(manager)
        {
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            MinHeight = 20;

            Padding = Margin = 0;

            manager.Input.KeyPress += key => { Text = manager.Input.AddKeyPress(Text, key); };
        }

        public override void AddContent(string content)
        {
            Text = content;
        }

        public override void Draw(float delta)
        {
            // Get size of text.
            var textsize = Manager.Renderer.MeasureText(Text, FontSize, FontStyle);
            // Calculate center for each axis.
            var center = new Point((ContentArea.Width / 2) - (textsize.Width / 2),
                (ContentArea.Height / 2) - (textsize.Height / 2));
            var x = 0d;
            var y = 0d;

            switch (TextAlignment)
            {
                // Top.
                case Alignment.TopLeft:
                    break;
                case Alignment.TopRight:
                    x = ContentArea.Width - textsize.Width;
                    break;
                // Bottom.
                case Alignment.BottomLeft:
                    y = ContentArea.Height - textsize.Height;
                    break;
                case Alignment.BottomRight:
                    y = ContentArea.Height - textsize.Height;
                    x = ContentArea.Width - textsize.Width;
                    break;
                // Center.
                case Alignment.TopCenter:
                    x = center.X;
                    break;
                case Alignment.BottomCenter:
                    y = ContentArea.Height - textsize.Height;
                    x = center.X;
                    break;
                case Alignment.LeftCenter:
                    y = center.Y;
                    break;
                case Alignment.RightCenter:
                    y = center.Y;
                    x = ContentArea.Width - textsize.Width;
                    break;
                case Alignment.Center:
                    y = center.Y;
                    x = center.X;
                    break;
            }
            Manager.Renderer.DrawString(Text, ContentArea.Point + new Point(x, y), TextColor, FontSize, FontStyle,
                ParentBounds);
            base.Draw(delta);
        }

        protected override Size MeasureCore(Size availableSize)
        {
            return (Manager.Renderer.MeasureText(Text, FontSize, FontStyle) + Padding).Max(Size);
            //return base.MeasureOverride(availableSize);
        }
    }
}