using System;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    public class Label : Control
    {
        public static readonly DependencyProperty<string> TextProperty =
            DependencyProperty.Register<Element, string>(nameof(Text), "Text", new PropertyMetadata(
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

        private bool textAlignInvalidated = true;
        private Point textAlignOffset;

        static Label()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof (Label), HorizontalAlignment.Center);
            VerticalAlignmentProperty.OverrideMetadata(typeof(Label), VerticalAlignment.Center);
        }

        public Label(Manager manager, string text) : this(manager)
        {
            Text = text;
        }

        public Label(Manager manager) : base(manager)
        {
            manager.Input.KeyPress += key => { Text = manager.Input.AddKeyPress(Text, key); };
        }

        public override void AddContent(string content)
        {
            Text = content;
        }

        public override void Draw(float delta)
        {
            // Get text alignment offset.
            if (textAlignInvalidated)
            {
                var textsize = Manager.Renderer.MeasureText(Text, FontSize, FontStyle);
                textAlignOffset = AlignText(textsize);
                textAlignInvalidated = false;
            }
            Manager.Renderer.DrawString(Text, ContentArea.Point + textAlignOffset, TextColor, FontSize, FontStyle,
                ParentBounds);
            base.Draw(delta);
        }

        protected override void OnPropertyChanged(DependencyProperty property, object newValue, object oldValue)
        {
            // If text or text alignment is changed, text alignment will need to be recalculated.
            if (property == TextAlignmentProperty || property == TextProperty)
                textAlignInvalidated = true;
            base.OnPropertyChanged(property, newValue, oldValue);
        }

        private Point AlignText(Size textsize)
        {
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
            return new Point(x, y);
        }

        protected override Size MeasureCore(Size availableSize)
        {
            return (Manager.Renderer.MeasureText(Text, FontSize, FontStyle) + Padding).Max(Size);
        }
    }
}