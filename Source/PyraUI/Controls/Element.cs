using System;
using System.Collections.Generic;
using Pyratron.UI.States;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// A UI element that has a position and parent element.
    /// </summary>
    public class Element : Component
    {
        public override int MaxChildren => 1;

        public string Name { get; set; }

        /// <summary>
        /// The area inside the border.
        /// </summary>
        public Thickness Padding { get; set; }

        /// <summary>
        /// The area outside the border.
        /// </summary>
        public Thickness Margin { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// The type of display box.
        /// </summary>
        public Box Box { get; set; }

        public bool IsWidthAuto => double.IsPositiveInfinity(Width);

        public bool IsHeightAuto => double.IsPositiveInfinity(Height);

        public int FontSize
        {
            get { return fontSizeSet || Parent == null ? fontSize : Parent.FontSize; }
            set
            {
                fontSize = value;
                fontSizeSet = true;
            }
        }

        public Color TextColor
        {
            get { return textColorSet || Parent == null ? textColor : Parent.TextColor; }
            set
            {
                textColor = value;
                textColorSet = true;
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
                    LayoutInvalidated = true;
                    if (Width < minWidth)
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
                    LayoutInvalidated = true;
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
                LayoutInvalidated = true;
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
                    LayoutInvalidated = true;
                    if (Height < minHeight)
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
                    LayoutInvalidated = true;
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
                LayoutInvalidated = true;
            }
        }

        public bool LayoutInvalidated { get; set; }

        public double ActualWidth { get; set; }

        public double ActualHeight { get; set; }

        /// <summary>
        /// Element's parent. Null if root.
        /// </summary>
        public virtual Element Parent { get; set; }

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
                if (!Equals(value.Width, ActualWidth))
                {
                    ActualWidth = value.Width;
                    LayoutInvalidated = true;
                }
                if (!Equals(value.Height, ActualHeight))
                {
                    ActualHeight = value.Height;
                    LayoutInvalidated = true;
                }
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

        private int fontSize;

        // Indicates if the property was set, if it is not, the value will be retrieved from a parent element.
        private bool fontSizeSet, textColorSet;

        private double height, minHeight, maxHeight, width, minWidth, maxWidth;
        private Color textColor;

        public Element(Manager manager) : base(manager)
        {
            Elements = new List<Element>();

            MinWidth = MaxWidth = 0;
            MaxWidth = MaxHeight = double.PositiveInfinity;
            Width = double.PositiveInfinity;
            Height = double.PositiveInfinity;

            Box = Box.Inline;

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            Margin = 10;
            Padding = 10;

            FontSize = 10;
            TextColor = Color.Black;
            textColorSet = fontSizeSet = false;
        }

        /// <summary>
        /// Handle inline XML content (such as text between the opening and closing tag)
        /// </summary>
        public virtual void AddContent(string content)
        {
            Add(new Label(Manager, content));
        }

        public virtual Size MeasureOverride(Size availableSize)
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
                w = GetMaxChildWidth() + Padding.Left + Padding.Right;

            // Repeat same process for vertical alignment.
            if (VerticalAlignment == VerticalAlignment.Stretch)
            {
                if (Height > availableSize.Height)
                    h = Math.Max(availableSize.Height, MinHeight);
            }
            else
                h = GetMaxChildHeight() + Padding.Top + Padding.Bottom;
            return new Size(w, h);
        }

        public virtual Point ArrangeOverride()
        {
            if (Parent != null)
                return Parent.AlignChild(this);
            return Margin + Padding;
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
            return Position + new Point(x, y) + child.Margin + child.Padding;
        }

        public virtual void Measure()
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];
                child.Measure();
            }
            ActualSize = MeasureOverride(Parent?.ContentArea.Size.Remove(Margin) ?? Size.Zero);
        }

        public virtual void Arrange()
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];
                child.Arrange();
            }
            Position = ArrangeOverride();
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
        protected virtual void Dispose()
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
        protected virtual double GetChildHeight()
        {
            var h = 0d;
            if (Elements.Count == 0)
                return BorderArea.Height;
            for (var i = 0; i < Elements.Count; i++)
                h += Elements[i].GetChildHeight();
            return h;
        }

        /// <summary>
        /// Returns the width of all child elements if they were to be stacked beside each other.
        /// </summary>
        protected virtual double GetChildWidth()
        {
            var w = 0d;
            if (Elements.Count == 0)
                return BorderArea.Width;
            for (var i = 0; i < Elements.Count; i++)
                w += Elements[i].GetChildWidth();
            return w;
        }

        /// <summary>
        /// Returns the extended width of the child element with the greatest width.
        /// </summary>
        protected virtual double GetMaxChildWidth()
        {
            var w = 0d;
            if (Elements.Count == 0)
                return BorderArea.Width;
            for (var i = 0; i < Elements.Count; i++)
                w = Math.Max(w,
                    (double.IsInfinity(Elements[i].Width) ? Elements[i].GetChildWidth() : Elements[i].Width) + Elements[i].Padding.Left + Elements[i].Padding.Right +
                    Elements[i].Margin.Left + Elements[i].Margin.Right);
            return w;
        }

        /// <summary>
        /// Returns the extended height of the child element with the greatest height.
        /// </summary>
        protected virtual double GetMaxChildHeight()
        {
            var h = 0d;
            for (var i = 0; i < Elements.Count; i++)
                h = Math.Max(h,
                    (double.IsInfinity(Elements[i].Height) ? Elements[i].GetChildHeight() : Elements[i].Height) + Elements[i].Padding.Top + Elements[i].Padding.Bottom +
                    Elements[i].Margin.Top + Elements[i].Margin.Bottom);
            return h;
        }
    }
}