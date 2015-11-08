using System;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    public class StackPanel : Panel
    {
        /// <summary>
        /// The direction controls will stack.
        /// </summary>
        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                InvalidateLayout();
            }
        }

        private Orientation orientation;

        public StackPanel(Manager manager, Orientation orientation) : base(manager)
        {
            Orientation = orientation;
        }

        // ReSharper disable once IntroduceOptionalParameters.Global
        public StackPanel(Manager manager) : this(manager, Orientation.Vertical)
        {
        }

        public override Point ArrangeChild(Element child)
        {
            double x = 0, y = 0;
            for (var i = 0; i < Elements.Count; i++)
            {
                if (Orientation == Orientation.Vertical)
                {
                    if (Elements[i] == child)
                    {
                        var pos = base.ArrangeChild(child);
                        return new Point(pos.X, pos.Y + y);
                    }
                    y += Elements[i].ExtendedArea.Height;
                }
                if (Orientation == Orientation.Horizontal)
                {
                    if (Elements[i] == child)
                    {
                        var pos = base.ArrangeChild(child);
                        return new Point(pos.X + x, pos.Y);
                    }
                    x += Elements[i].ExtendedArea.Width;
                }
            }
            return base.ArrangeChild(child);
        }

        public override Point AlignChild(Element child)
        {
            if (!ContentArea.IsInfinity)
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
                    if (child.HorizontalAlignment == HorizontalAlignment.Center
                        || (child.HorizontalAlignment == HorizontalAlignment.Stretch
                            && !child.IsWidthAuto))
                        x += center.X;
                }

                // Apply vertical alignment.
                if (Orientation == Orientation.Horizontal)
                {
                    if (child.VerticalAlignment == VerticalAlignment.Bottom)
                        y = ContentArea.Height - child.ExtendedArea.Height;
                    // If vertical alignment is stretch and the height is specified (not auto), make it center.
                    if (child.VerticalAlignment == VerticalAlignment.Center
                        || (child.VerticalAlignment == VerticalAlignment.Stretch
                            && !child.IsHeightAuto))
                        y += center.Y;
                }
                return child.Position + new Point(x, y);
            }
            else
                return child.Position;
        }

        public override Size MeasureSelf(Size availableSize)
        {
         
            double w = Width, h = Height;
            if (Orientation == Orientation.Vertical)
            {
                if (Parent == null) return Size;
            

                // If horizontal alignment is stretch and the width is more than the available size, try and fit it.
                if ((!(Parent is StackPanel)) && HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    if (Width > availableSize.Width)
                        w = Math.Min(availableSize.Width, Width);
                }
                else
                // If horizontal alignment is not stretch, find the largest child element and use that as the width.
                    w = (IsWidthAuto ? GetMaxChildWidth() : Width) + Padding.Left + Padding.Right;

                // Repeat same process for vertical alignment.
                if (Height > availableSize.Height)
                    h = Math.Max(IsHeightAuto ? GetChildHeight() : Height, MinHeight);
            }
            if (Orientation == Orientation.Horizontal)
            {
                if (Parent == null) return Size;

                // If horizontal alignment is stretch and the width is more than the available size, try and fit it.
                if (VerticalAlignment == VerticalAlignment.Stretch)
                {
                    if (Height > availableSize.Height)
                        h = Math.Min(availableSize.Height, Padding.Height + GetMaxChildHeight());
                }
                else
                    // If horizontal alignment is not stretch, find the largest child element and use that as the width.
                    h = (IsHeightAuto ? GetMaxChildHeight() : Height) + Padding.Top + Padding.Bottom;

                // Repeat same process for vertical alignment.
                if (Width> availableSize.Width)
                    w = Math.Max(GetChildWidth(), MinWidth);
            }
            if (Orientation == Orientation.Vertical)
            {
                for (var i = 0; i < Elements.Count; i++)
                {
                    if (Elements[i].IsWidthAuto)
                        Elements[i].ActualWidth = ContentArea.Width - Elements[i].Margin.Width;
                    else
                        Elements[i].ActualWidth = Math.Min(Elements[i].Width, ContentArea.Width - Elements[i].Margin.Width);
                    Elements[i].Arrange();
                }
            }

            if (Orientation == Orientation.Horizontal)
            {
                for (var i = 0; i < Elements.Count; i++)
                {
                    Elements[i].ActualHeight = Math.Min(Elements[i].Height,
                        ContentArea.Height - Elements[i].Margin.Height);
                    Elements[i].Arrange();
                }
            }
            return new Size(w, h);
        }
    }
}