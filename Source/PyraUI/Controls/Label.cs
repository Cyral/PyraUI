using System;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    public class Label : Control
    {
        public string Text { get; set; }

        public Alignment TextAlignment { get; set; }

        public Label(Manager manager) : base(manager)
        {
            Text = "Label Text";

            TextAlignment = Alignment.Center;

            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            Padding = Margin = 0;

            Height.Min = 6;
            Height.Max = 64;
            Height.Value = 12;

            Width.Value = 96;
        }

        public override void Arrange(bool down = true)
        {
            var textsize = Manager.Renderer.MeasureText(Text);
            Width.Value = textsize.Width;
            Height = textsize.Height;

            base.Arrange(down);
        }

        internal override Point AlignChild(Point pos, Element element)
        {
            Console.WriteLine(element.Width);
            return base.AlignChild(pos, element);
        }

        public override void Draw(float delta)
        {
            base.Draw(delta);

            // Get size of text.
            var textsize = Manager.Renderer.MeasureText(Text);
            // Calculate center for each axis.
            var center = new Point((ContentArea.Width / 2) - (textsize.Width / 2),
                (ContentArea.Height / 2) - (textsize.Height / 2));
            var x = ContentArea.Left;
            var y = ContentArea.Top;

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
            Manager.Renderer.DrawString(Text, Position + new Point(x, y));
        }
    }
}