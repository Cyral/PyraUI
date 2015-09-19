using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    public class StackPanel : Panel
    {
        /// <summary>
        /// The direction controls will stack.
        /// </summary>
        public Orientation Orientation { get; set; }

        public StackPanel(Manager manager, Orientation orientation) : base(manager)
        {
            Orientation = orientation;
        }

        // ReSharper disable once IntroduceOptionalParameters.Global
        public StackPanel(Manager manager) : this(manager, Orientation.Vertical)
        {
        }

        public override Point AlignChild(Element child)
        {
            var center = new Point((ContentArea.Width / 2) - (child.ExtendedArea.Width / 2),
                (ContentArea.Height / 2) - (child.ExtendedArea.Height / 2));
            double x = 0, y = 0;

            // Apply horizontal alignment.
            if (Orientation == Orientation.Vertical)
            {
                if (child.HorizontalAlignment == HorizontalAlignment.Right)
                    x = ContentArea.Width - child.ExtendedArea.Width;
                // If Horizontal alignment is stretch and the Width is specified (not auto), make it center.
                else if (child.HorizontalAlignment == HorizontalAlignment.Center
                    || (child.HorizontalAlignment == HorizontalAlignment.Stretch
                    && !double.IsPositiveInfinity(child.Width)))
                    x += center.X;
            }

            // Apply vertical alignment.
            if (Orientation == Orientation.Horizontal)
            {
                if (child.VerticalAlignment == VerticalAlignment.Bottom)
                    y = ContentArea.Height - child.ExtendedArea.Height;
                // If vertical alignment is stretch and the height is specified (not auto), make it center.
                else if (child.VerticalAlignment == VerticalAlignment.Center 
                    || (child.VerticalAlignment == VerticalAlignment.Stretch
                    && !double.IsPositiveInfinity(child.Height)))
                    y += center.Y;
            }
            return Position + new Point(x, y) + child.Margin + child.Padding;
        }

        public override Size MeasureOverride(Size availableSize)
        {
            if (Orientation == Orientation.Vertical)
            {
                for (var i = 0; i < Elements.Count; i++)
                {   // For a vertical stack panel, stretch the child control horizontally if its
                    // width equals Auto or its HorizontalAlignment equals Stretch.
                    // If the width is specified, use that and align it based on the HorizontalAlignment.
                    // If the width is not specified, but the HorizontalAlignment is not Stretch, then use the
                    // width of the child's children to determine the width of the child, which will then be
                    // aligned with the standard behavior in AlignChild.

                    double width, height;
                    // If height is auto, use height of children.
                    if (double.IsPositiveInfinity(Elements[i].Height))
                        height = Elements[i].ChildSize.Height;
                    // If it is specified, use the specified height.
                    else
                        height = Elements[i].Height;
                    height += Elements[i].Padding.Top + Elements[i].Padding.Bottom; // Add padding.

                    // If horizontal alignment is Stretch and height is not specified, fill the panel.
                    if (Elements[i].HorizontalAlignment == HorizontalAlignment.Stretch && double.IsPositiveInfinity(Elements[i].Width))
                        width = ContentArea.Width - Elements[i].Margin.Left - Elements[i].Margin.Right;
                    else
                    {
                        // If horizontal alignment is not stretch, find the width (either automatically or specified).
                        if (double.IsPositiveInfinity(Elements[i].Width))
                            width = Elements[i].ChildSize.Width;
                        else
                            width = Elements[i].Width;
                        width += Elements[i].Padding.Left + Elements[i].Padding.Right;  // Add padding.
                    }
                    Elements[i].ActualSize = new Size(width, height);
                }
            }
            else if (Orientation == Orientation.Horizontal)
            {
                for (var i = 0; i < Elements.Count; i++)
                {
                    // For a horizontal stack panel, stretch the child control vertically if its
                    // height equals Auto or its VerticalAlignment equals Stretch.
                    // If the height is specified, use that and align it based on the VerticalAlignment.
                    // If the height is not specified, but the VerticalAlignment is not Stretch, then use the
                    // height of the child's children to determine the height of the child, which will then be
                    // aligned with the standard behavior in AlignChild.

                    double width, height;
                    // If width is auto, use width of children.
                    if (double.IsPositiveInfinity(Elements[i].Width))
                        width = Elements[i].ChildSize.Width;
                    // If it is specified, use the specified width.
                    else
                        width = Elements[i].Width;
                    width += Elements[i].Padding.Left + Elements[i].Padding.Right; // Add padding.

                    // If vertical alignment is Stretch and height is not specified, fill the panel.
                    if (Elements[i].VerticalAlignment == VerticalAlignment.Stretch && double.IsPositiveInfinity(Elements[i].Height))
                        height = ContentArea.Height - Elements[i].Margin.Top - Elements[i].Margin.Bottom;
                    else
                    {
                        // If vertical alignment is not stretch, find the height (either automatically or specified).
                        if (double.IsPositiveInfinity(Elements[i].Height))
                            height = Elements[i].ChildSize.Height;
                        else
                            height = Elements[i].Height;
                        height += Elements[i].Padding.Top + Elements[i].Padding.Bottom;  // Add padding.
                    }
                    Elements[i].ActualSize = new Size(width, height);
                }
            }
            return base.MeasureOverride(availableSize);
        }

        // Because of the stacking behavior, the two below methods must be overridden.
        protected override double GetMaxChildWidth()
        {
            if (Orientation == Orientation.Horizontal)
            {
                var w = 0d;
                for (var i = 0; i < Elements.Count; i++)
                    w += Elements[i].ActualWidth + Elements[i].Margin.Left + Elements[i].Margin.Right;
                return w;
            }
            return base.GetMaxChildWidth();
        }

        protected override double GetMaxChildHeight()
        {
            if (Orientation == Orientation.Vertical)
            {
                var h = 0d;
                for (var i = 0; i < Elements.Count; i++)
                    h += Elements[i].ActualHeight + Elements[i].Margin.Top + Elements[i].Margin.Bottom;
                return h;
            }
            return base.GetMaxChildHeight();
        }

        public override Point ArrangeOverride()
        {
            if (Orientation == Orientation.Vertical)
            {
                double y = 0;
                for (var i = 0; i < Elements.Count; i++)
                {
                    Elements[i].Position = Elements[i].ArrangeOverride() + new Point(0, y);
                    y += Elements[i].ExtendedArea.Height;
                }
            }
            else if (Orientation == Orientation.Horizontal)
            {
                double x = 0;
                for (var i = 0; i < Elements.Count; i++)
                {
                    Elements[i].Position = Elements[i].ArrangeOverride() + new Point(x, 0);
                    x += Elements[i].ExtendedArea.Width;
                }
            }
            return base.ArrangeOverride();
        }
    }
}