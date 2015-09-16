using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        public Dimension Width { get; set; }
        public Dimension Height { get; set; }

        /// <summary>
        /// Element's parent. Null if root.
        /// </summary>
        public virtual Element Parent { get; set; }

        /// <summary>
        /// List of child elements.
        /// </summary>
        public List<Element> Elements { get; set; }

        /// <summary>
        /// Control bounds (width and height), without position or margin/padding.
        /// </summary>
        public Size Size => new Size(Width, Height);

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
                        Size.Width - Padding.Left - Padding.Right, Size.Height - Padding.Bottom - Padding.Top).Offset(
                            Position - Margin - Padding);
            }
        }

        /// <summary>
        /// Rectangular region of the element.
        /// </summary>
        public Rectangle BorderArea
        {
            get { return Size.Combine(Margin).Offset(Position - Margin - Padding); }
        }

        /// <summary>
        /// Rectangular region of the element plus the margin area.
        /// </summary>
        public Rectangle ExtendedArea
        {
            get { return Size.Extend(Margin).Offset(Position - Margin - Padding); }
        }

        public Element(Manager manager) : base(manager)
        {
            Elements = new List<Element>();

            Width = new Dimension(128, 0, int.MaxValue);
            Height = new Dimension(32, 0, int.MaxValue);

            Box = Box.Inline;

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            Margin = 4;
            Padding = 2;
        }

        /// <summary>
        /// Calculate the layout of the element.
        /// </summary>
        /// <param name="down">
        /// True if child controls will be recursively arranged, false if parent controls will be arranged.
        /// (Width must be from first to last, while height must be from last to first)
        /// </param>
        public virtual void Arrange(bool down = true)
        {
            if (Parent != null)
            {
                Width.Value = Parent.CalcChildWidth(this);
                Height.Value = Parent.CalcChildHeight(this);
            }

            if (down)
            {
                // The position is equal to the parent's position, plus the margin and padding, plus the height of the previous child controls.
                if (Parent == null)
                {
                    Position = Margin + Padding;
                }
                else
                {
                    var pos = Parent.Position + Margin + Padding;
                    Position = Parent.AlignChild(pos, this);

                    for (var i = 0; i < Elements.Count; i++)
                    {
                        var child = Elements[i];
                        // Arrange child controls now that the width and position have been calculated.
                        child.Arrange();
                    }
                }
            }
            else
            {
                Parent?.Arrange(false);
            }
        }

        /// <summary>
        /// Change the height of a child element based on the layout of the parent.
        /// </summary>
        internal virtual int CalcChildHeight(Element element)
        {
            // If the height of the parent is too small to contain this control, extend it (if possible).
            if ((element.ExtendedArea.Height + element.Position.Y > ContentArea.Height) || (FindChildHeight() > ContentArea.Height) && Height.Auto)
            {
                Height.Value = Math.Min(Height.Max, FindChildHeight() + Padding.Top + Padding.Bottom);
                // Arrange controls now that the height has been set.
                Arrange(false);
            }
            // If vertical align is stretch, fill the container vertically.
            if ((element.VerticalAlignment == VerticalAlignment.Stretch))
                return ContentArea.Height - element.Margin.Top - element.Margin.Bottom;
            return element.Height;
        }

        /// <summary>
        /// Change the width of a child element based on the layout of the parent.
        /// </summary>
        internal virtual int CalcChildWidth(Element element)
        {
            // If the width of the parent is too small to contain this control, extend it (If possible).
            if ((element.ExtendedArea.Width + element.Position.X > ContentArea.Width) || (FindChildWidth() > ContentArea.Width) && Width.Auto)
            {
                Width.Value = Math.Min(Width.Max, FindChildWidth() + Padding.Right + Padding.Left);
                // Arrange controls now that the width has been set.
                Arrange(false);
            }
            // If horizontal align is stretch, fill the container horizontally.
            if ((element.HorizontalAlignment == HorizontalAlignment.Stretch))
                return ContentArea.Width - element.Margin.Left - element.Margin.Right;
            return element.Width;
        }

        /// <summary>
        /// Change the position of a child element based on the layout of the parent.
        /// </summary>
        /// <param name="pos">Original position (Parent's position + Margin + Padding by default).</param>
        /// <param name="element">The child element.</param>
        /// <returns>New position</returns>
        internal virtual Point AlignChild(Point pos, Element element)
        {
            if (Parent != null)
            {
                var center = new Point((ContentArea.Width / 2) - (element.ExtendedArea.Width / 2),
                    (ContentArea.Height / 2) - (element.ExtendedArea.Height / 2));
                var x = pos.X;
                var y = pos.Y;

                // Apply horizontal alignment.
                switch (element.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        break;
                    case HorizontalAlignment.Center:
                        x += center.X;
                        break;
                    case HorizontalAlignment.Right:
                        x = ContentArea.Width - element.ExtendedArea.Width;
                        break;
                }

                // Apply vertical alignment.
                switch (element.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        break;
                    case VerticalAlignment.Center:
                        y += center.Y;
                        break;
                    case VerticalAlignment.Bottom:
                        y = ContentArea.Height - element.ExtendedArea.Height;
                        break;
                }
                return new Point(x, y);
            }
            return pos;
        }

        /// <summary>
        /// Returns the height of all the child controls.
        /// </summary>
        public virtual int FindChildHeight()
        {
            var height = 0;
            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];
                height += child.ExtendedArea.Height;
            }
            return height;
        }

        /// <summary>
        /// Returns the width of all the child controls.
        /// </summary>
        public virtual int FindChildWidth()
        {
            var width = 0;
            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];
                width += child.ExtendedArea.Width;
            }
            return width;
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
                    Arrange();
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
                    Arrange();
                }
            }
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
    }
}