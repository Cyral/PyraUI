using System;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    public class Label : Control
    {
        private string text;

        public string Text
        {
            get { return text; }
            set
            {
                text = value.Trim();
                LayoutInvalidated = true;
            }
        }

        public Alignment TextAlignment { get; set; }

        public Label(Manager manager, string text) : this(manager)
        {
            Text = text;
        }

        public Label(Manager manager) : base(manager)
        {
            Text = "Label Text";

            TextAlignment = Alignment.Center;

            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            Padding = Margin = 0;
        }

        public override void AddContent(string content)
        {
            Text = content;
        }

        public override Size MeasureOverride(Size availableSize)
        {
            return Manager.Renderer.MeasureText(Text, FontSize, FontStyle);
            //return base.MeasureOverride(availableSize);
        }

        public override void Draw(float delta)
        {
            base.Draw(delta);

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
            Manager.Renderer.DrawString(Text, Position + new Point(x, y), TextColor, FontSize, FontStyle);
        }
    }
}