using System;
using System.Collections.Generic;
using System.Linq;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// A UI element that has a position and parent element.
    /// </summary>
    public class Element : Component
    {
        public Element(Manager manager) : base(manager)
        {
            Elements = new List<Element>();

            MinWidth = MaxWidth = 0;
            MaxWidth = MaxHeight = double.PositiveInfinity;
            Width = double.PositiveInfinity;
            Height = double.PositiveInfinity;

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            Margin = 10;
            Padding = 10;

            FontSize = 10;
            FontStyle = FontStyle.Regular;
            TextColor = Color.Black;

            textColorSet = fontSizeSet = fontStyleSet = false;
        }

        /// <summary>
        /// Handles inline XML content (such as text between the opening and closing tag).
        /// </summary>
        public virtual void AddContent(string content)
        {
            // By default, create a label inside this element.
            Add(new Label(Manager, content));
        }

        public virtual Size MeasureSelf(Size availableSize)
        {
            
            if (Parent == null) return Size;
            double w = Width, h = Height;

            // If horizontal alignment is stretch and the width is more than the available size, try and fit it.
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                if (Width > availableSize.Width)
                    w = Math.Max(availableSize.Width, MinWidth);
            }
            else // If horizontal alignment is not stretch, find the largest child element and use that as the width.
                w = (IsWidthAuto ? GetMaxChildWidth() : Width) + Padding.Left + Padding.Right;

            // Repeat same process for vertical alignment.
            if (VerticalAlignment == VerticalAlignment.Stretch)
            {
                if (Height > availableSize.Height)
                    h = Math.Max(availableSize.Height, MinHeight);
            }
            else
                h = (IsHeightAuto ? GetMaxChildHeight() : Height) + Padding.Top + Padding.Bottom;
            return new Size(w, h);
        }

        public virtual Point ArrangeChild(Element child)
        {
            // Position the child element in the upper left corner of this element, taking into account for margin and padding.
            return Position + child.Margin + child.Padding;
        }

        public virtual Point AlignChild(Element child)
        {
            var center = new Point((ContentArea.Width / 2) - (child.ExtendedArea.Width / 2),
                (ContentArea.Height / 2) - (child.ExtendedArea.Height / 2));
            double x = 0, y = 0;

            // Apply horizontal alignment.
            if (child.HorizontalAlignment == HorizontalAlignment.Center)
                x += center.X;
            if (child.HorizontalAlignment == HorizontalAlignment.Right)
                x = ContentArea.Width - child.ExtendedArea.Width;

            // Apply vertical alignment.
            if (child.VerticalAlignment == VerticalAlignment.Center)
                y += center.Y;
            if (child.VerticalAlignment == VerticalAlignment.Bottom)
                y = ContentArea.Height - child.ExtendedArea.Height;
            return child.Position + new Point(x, y);
        }
        
        public virtual void Measure()
        {
            // Measure child elements (and their children, and so on).
            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];
                child.Measure();
            }
            // Then set the size of this element.
            ActualSize = MeasureSelf(Parent?.ContentArea.Size.Remove(Margin) ?? Size.Zero);
        }

        public virtual void Arrange()
        {
            if (Parent == null) // Root elements must be positioned manually from the edge of the window.
                Position = Margin + Padding;
            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];
                // Set the child position.
                child.Position = ArrangeChild(child);
                // Align it based on Horizontal and Vertical alignment.
                child.Position = AlignChild(child);
                child.Arrange();
            }
        }

        public virtual void UpdateLayout()
        {
            Measure();
            Arrange();
        }

        /// <summary>
        /// Adds a child element to this element.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public virtual void Add(Element element)
        {
            if (Elements.Count >= MaxChildren)
                throw new InvalidOperationException("This element may not have any more children. (Maximum: " +
                                                    MaxChildren + ") Try placing it inside a panel.");
            if (element != null)
            {
                // If this control doesn't already contain the element.
                if (!Elements.Contains(element))
                {
                    // Remove control from prior parent.
                    element.Parent?.Remove(element);

                    element.Manager = Manager;
                    element.Parent = this;
                    // Add to this element.
                    Elements.Add(element);
                    // Add element to master list.
                    if (!Manager.Elements.Contains(element))
                        Manager.Elements.Add(element);
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Removes a child element from this element.
        /// </summary>
        /// <param name="element">The element to remove.</param>
        /// <param name="dispose">Should the control be disposed and destroyed?</param>
        public virtual void Remove(Element element, bool dispose = true)
        {
            if (element != null)
            {
                if (Elements.Contains(element))
                {
                    Elements.Remove(element);
                    // Element no longer has a parent.
                    element.Parent = null;
                    if (dispose)
                        element.Dispose();
                    UpdateLayout();
                }
            }
        }

        protected internal override void Update(float delta)
        {
            if (LayoutInvalidated)
            {
                Parent?.UpdateLayout();
                LayoutInvalidated = false;
            }
            base.Update(delta);
        }

        /// <summary>
        /// Releases resources used by the element, removes itself from its parent, and disposes of all children.
        /// </summary>
        protected internal virtual void Dispose()
        {
            Parent.Remove(this, false);
            Parent = null;
            Manager.Elements.Remove(this);
            foreach (var child in Elements)
                Remove(child);
            Elements.Clear();
        }

        /// <summary>
        /// Returns the height of all child elements if they were to be stacked on top of each other.
        /// </summary>
        protected internal virtual double GetChildHeight()
        {
            double h = Padding.Height;
            if (Elements.Count == 0)
                return ExtendedArea.Height;
            for (var i = 0; i < Elements.Count; i++)
                h += Elements[i].ExtendedArea.Height;
            return h;
        }

        /// <summary>
        /// Returns the width of all child elements if they were to be stacked beside each other.
        /// </summary>
        protected internal virtual double GetChildWidth()
        {
            double w = Padding.Width;
            if (Elements.Count == 0)
                return ExtendedArea.Width;
            for (var i = 0; i < Elements.Count; i++)
                w += Elements[i].ExtendedArea.Width;
            return w;
        }

        /// <summary>
        /// Returns the extended width of the child element with the greatest width.
        /// </summary>
        protected internal virtual double GetMaxChildWidth()
            => Elements.Count == 0 ? ExtendedArea.Width : Elements.Select(t => t.ExtendedArea.Width).Max();

        /// <summary>
        /// Returns the extended height of the child element with the greatest height.
        /// </summary>
        protected internal virtual double GetMaxChildHeight()
            => Elements.Count == 0 ? ExtendedArea.Height : Elements.Select(t => t.ExtendedArea.Height).Max();

        /// <summary>
        /// Mark this element as invalidated, requiring a Measure and Arrange pass.
        /// </summary>
        internal virtual void InvalidateLayout() => LayoutInvalidated = true;

        #region Properties

        public override int MaxChildren => 1;

        public string Name { get; set; }

        /// <summary>
        /// The area inside the border.
        /// </summary>
        public Thickness Padding
        {
            get { return padding; }
            set
            {
                padding = value;
                InvalidateLayout();
            }
        }

        /// <summary>
        /// The area outside the border.
        /// </summary>
        public Thickness Margin
        {
            get { return margin; }
            set
            {
                margin = value;
                InvalidateLayout();
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set
            {
                horizontalAlignment = value;
                InvalidateLayout();
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set
            {
                verticalAlignment = value;
                InvalidateLayout();
            }
        }

        public bool IsWidthAuto => double.IsInfinity(Width);

        public bool IsHeightAuto => double.IsInfinity(Height);

        public FontStyle FontStyle
        {
            get { return fontStyleSet || Parent == null ? fontStyle : Parent.FontStyle; }
            set
            {
                fontStyle = value;
                fontStyleSet = true;
                InvalidateLayout();
            }
        }

        public int FontSize
        {
            get { return fontSizeSet || Parent == null ? fontSize : Parent.FontSize; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Font size must be more than 0.");
                fontSize = value;
                fontSizeSet = true;
                InvalidateLayout();
            }
        }

        public Color TextColor
        {
            get { return textColorSet || Parent == null ? textColor : Parent.TextColor; }
            set
            {
                textColor = value;
                textColorSet = true;
                InvalidateLayout();
            }
        }

        /// <summary>
        /// The minimum width.
        /// </summary>
        public double MinWidth
        {
            get { return minWidth; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Minimum width must be greater than zero.");
                if (!Equals(value, minWidth))
                {
                    minWidth = value;
                    InvalidateLayout();
                    if (Width < minWidth || IsWidthAuto)
                        Width = minWidth;
                }
            }
        }

        /// <summary>
        /// The maximum width.
        /// </summary>
        public double MaxWidth
        {
            get { return maxWidth; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Maximum width must be greater than zero.");
                if (!Equals(value, maxWidth))
                {
                    maxWidth = value;
                    InvalidateLayout();
                    if (Width > maxWidth)
                        Width = maxWidth;
                }
            }
        }

        /// <summary>
        /// The target width.
        /// </summary>
        public double Width
        {
            get { return width; }
            set
            {
                width = Math.Max(minWidth, Math.Min(value, maxWidth));
                InvalidateLayout();
            }
        }

        /// <summary>
        /// The minimum height.
        /// </summary>
        public double MinHeight
        {
            get { return minHeight; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Minimum height must be greater than zero.");
                if (!Equals(value, minHeight))
                {
                    minHeight = value;
                    InvalidateLayout();
                    if (Height < minHeight || IsWidthAuto)
                        Height = minHeight;
                }
            }
        }

        /// <summary>
        /// The maximum height.
        /// </summary>
        public double MaxHeight
        {
            get { return maxHeight; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Maximum height must be greater than zero.");
                if (!Equals(value, maxHeight))
                {
                    maxHeight = value;
                    InvalidateLayout();
                    if (Height > maxHeight)
                        Height = maxHeight;
                }
            }
        }

        /// <summary>
        /// The target height.
        /// </summary>
        public double Height
        {
            get { return height; }
            set
            {
                height = Math.Max(minHeight, Math.Min(value, maxHeight));
                InvalidateLayout();
            }
        }

        internal bool LayoutInvalidated { get; set; }

        public double ActualWidth
        {
            get { return actualWidth; }
            set
            {
                if ((int) value != (int) ActualWidth)
                {
                    actualWidth = value;
                    // Limit width to parent width.
                    if (Parent != null)
                        actualWidth = Math.Min(Parent.ContentArea.Width, actualWidth);
                }
            }
        }

        public double ActualHeight
        {
            get { return actualHeight; }
            set
            {
                if ((int) value != (int) ActualHeight)
                {
                    actualHeight = value;
                    // Limit height to parent height.
                    if (Parent != null)
                        actualHeight = Math.Min(Parent.ContentArea.Height, actualHeight);
                }
            }
        }

        /// <summary>
        /// Element's parent. Null if root.
        /// </summary>
        public virtual Element Parent { get; set; }

        public Rectangle ParentBounds => Parent?.ContentArea ?? Rectangle.Infinity;

        /// <summary>
        /// List of child elements.
        /// </summary>
        public List<Element> Elements { get; set; }

        public Size Size => new Size((int) Math.Round(Width), (int) Math.Round(Height));

        public Size ChildSize => new Size(GetChildWidth(), GetChildHeight());

        public Size ActualSize
        {
            get { return new Size((int) Math.Round(ActualWidth), (int) Math.Round(ActualHeight)); }
            set
            {
                ActualWidth = value.Width;
                ActualHeight = value.Height;
            }
        }

        /// <summary>
        /// The position of the content area of the element.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Rectangular region of the content area (inside the padding).
        /// </summary>
        public Rectangle ContentArea
        {
            get
            {
                return
                    new Rectangle(Margin.Left + Padding.Left, Margin.Top + Padding.Top,
                        (int) ActualSize.Width - Padding.Left - Padding.Right,
                        (int) ActualSize.Height - Padding.Bottom - Padding.Top).Offset(
                            Position - Margin - Padding);
            }
        }

        /// <summary>
        /// Rectangular region of the element.
        /// </summary>
        public Rectangle BorderArea
        {
            get { return ActualSize.Combine(Margin).Offset(Position - Margin - Padding); }
        }

        /// <summary>
        /// Rectangular region of the element plus the margin area.
        /// </summary>
        public Rectangle ExtendedArea
        {
            get { return ActualSize.Extend(Margin).Offset(Position - Margin - Padding); }
        }

        public Size ActualSizePrevious { get; set; }

        private double actualHeight;
        private double actualWidth;

        private int fontSize;

        // Indicates if the property was set, if it is not, the value will be retrieved from a parent element.
        private bool fontSizeSet, textColorSet, fontStyleSet;

        private double height, minHeight, maxHeight, width, minWidth, maxWidth;
        private HorizontalAlignment horizontalAlignment;
        private Thickness margin;
        private Thickness padding;
        private Color textColor;
        private VerticalAlignment verticalAlignment;
        private FontStyle fontStyle;

        #endregion
    }
}