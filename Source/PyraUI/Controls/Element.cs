using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Pyratron.UI.States;
using PyraUI;

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
                        Size.Width - Padding.Left - Padding.Bottom, Size.Height - Padding.Right - Padding.Left).Offset(
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

            Width = new Dimension(64, 96, 1024);
            Height = new Dimension(64, 96, 1024);

            Box = Box.Inline;
            Margin = 8;
            Padding = 4;
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
            // If width is set to auto, fill the container horizontally.
            if (Width.Auto && Parent != null)
                Width.Value = Parent.ContentArea.Width - Margin.Left - Margin.Right;
            // If the height of the parent is too small to contain this control, extend it.
            if (Parent != null && ExtendedArea.Height + Position.Y > Parent.ContentArea.Height && Parent.Height.Auto)
            {
                Parent.Height.Value = Parent.FindChildHeight() + Parent.Padding.Top + Parent.Padding.Bottom;
                // Arrange parent controls now that the height has been set.
                Parent.Arrange(false);
            }

            if (down)
            {
                // The position is equal to the parent's position, plus the margin and padding, plus the height of the previous child controls.
                var pos = Parent?.Position + Margin + Padding ?? Margin + Padding;
                var extra = 0;
                if (Parent != null)
                {
                    for (var i = 0; i < Parent.Elements.Count; i++)
                    {
                        // Until we hit this control, add the total height of child controls before this one.
                        if (Parent.Elements[i] == this) break;
                        extra += Parent.Elements[i].ExtendedArea.Height;
                    }
                }
                Position = new Point(pos.X, pos.Y + extra);
                for (var i = 0; i < Elements.Count; i++)
                {
                    var child = Elements[i];
                    // Arrange child controls now that the width and position have been calculated.
                    child.Arrange();
                }
            }
            else
            {
                Parent?.Arrange(false);
            }
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