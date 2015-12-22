using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    public class TextBlock : Visual
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

        private bool textAlignInvalidated = true, textSizeInvalidated = true;
        private Point textAlignOffset;
        private Size textSize;
        private Rectangle lastBorderSize;

        public TextBlock(Manager manager, string text) : this(manager)
        {
            Text = text;
        }

        public TextBlock(Manager manager) : base(manager)
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(TextBlock), HorizontalAlignment.Center);
            VerticalAlignmentProperty.OverrideMetadata(typeof(TextBlock), VerticalAlignment.Center);
            manager.Input.KeyPress += key => { Text = manager.Input.AddKeyPress(Text, key); };
        }

        public override void AddContent(string content)
        {
            Text = content;
        }

        public override void Draw(float delta)
        {
            // Get text alignment offset.
            if (textAlignInvalidated || textSizeInvalidated || !lastBorderSize.IsClose(Bounds))
            {
                textAlignOffset = AlignText(textSize);
                textAlignInvalidated = false;
                textSizeInvalidated = false;
            }
            Manager.Renderer.DrawString(Text, Bounds.Point + textAlignOffset, TextColor, FontSize, FontStyle,
                ParentBounds);
            lastBorderSize = Bounds;
            base.Draw(delta);
        }

        protected override void OnPropertyChanged(DependencyProperty property, object newValue, object oldValue)
        {
            // If text or text alignment is changed, text alignment will need to be recalculated.
            base.OnPropertyChanged(property, newValue, oldValue);
            if (property == TextAlignmentProperty || property == TextProperty)
                textAlignInvalidated = true;
            if (property == TextProperty)
                textSizeInvalidated = true;
        }

        private Point AlignText(Size textsize)
        {
            // Calculate center for each axis.
            var center = new Point((Bounds.Width / 2) - (textsize.Width / 2),
                (Bounds.Height / 2) - (textsize.Height / 2));
            var x = 0d;
            var y = 0d;
            switch (TextAlignment)
            {
                // Top.
                case Alignment.TopLeft:
                    break;
                case Alignment.TopRight:
                    x = Bounds.Width - textsize.Width;
                    break;
                // Bottom.
                case Alignment.BottomLeft:
                    y = Bounds.Height - textsize.Height;
                    break;
                case Alignment.BottomRight:
                    y = Bounds.Height - textsize.Height;
                    x = Bounds.Width - textsize.Width;
                    break;
                // Center.
                case Alignment.TopCenter:
                    x = center.X;
                    break;
                case Alignment.BottomCenter:
                    y = Bounds.Height - textsize.Height;
                    x = center.X;
                    break;
                case Alignment.LeftCenter:
                    y = center.Y;
                    break;
                case Alignment.RightCenter:
                    y = center.Y;
                    x = Bounds.Width - textsize.Width;
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
            if (textSizeInvalidated)
            {
                textSize = Manager.Renderer.MeasureText(Text, FontSize, FontStyle);
                textSizeInvalidated = false;
            }
            return textSize.Max(Size);
        }
    }
}
