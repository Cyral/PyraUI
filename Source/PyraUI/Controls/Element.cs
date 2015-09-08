using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Pyratron.UI.States;
using PyraUI;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// A UI element that has a position and parent element.
    /// </summary>
    public class Element : Component
    {
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

        /// <summary>
        /// The minimum width.
        /// </summary>
        public int MinWidth
        {
            get { return minWidth; }
            set
            {
                minWidth = value;
                if (Width < minWidth)
                    Width = minWidth;
            }
        }

        /// <summary>
        /// The minimum height.
        /// </summary>
        public int MinHeight
        {
            get { return minHeight; }
            set
            {
                minHeight = value;
                if (Height < minHeight)
                    Height = minHeight;
            }
        }

        /// <summary>
        /// The maximum width.
        /// </summary>
        public int MaxWidth
        {
            get { return maxWidth; }
            set
            {
                maxWidth = value;
                if (Width > maxWidth)
                    Width = maxWidth;
            }
        }

        /// <summary>
        /// The maximum height.
        /// </summary>
        public int MaxHeight
        {
            get { return maxHeight; }
            set
            {
                maxHeight = value;
                if (Height > maxHeight)
                    Height = maxHeight;
            }
        }

        public int Width { get; set; }
        public int Height { get; set; }

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
        public Point Position
        {
            get
            {
                var initial = Parent?.Position + Margin + Padding ?? Margin + Padding;
                return initial;
            }
        }

        /// <summary>
        /// The position of the content area of the parent. (The top left corner of the margin.)
        /// </summary>
        public Point BasePosition
        {
            get
            {
                var initial = Parent?.Position ?? Point.Zero;
                if (Box == Box.Overlap || Parent == null)
                    return initial;
                if (Box == Box.Inline)
                {
                    var x = 0;
                    var max = 0;
                    foreach (var element in Parent.Elements.TakeWhile(element => element != this))
                    {
                        if (element.Box == Box.Inline)
                        {
                            x += element.ExtendedBounds.Width;
                        }
                        else
                           x = 0;
                        max = Math.Max(max, element.ContentBounds.Bottom);
                    }
                    var index = Parent.Elements.IndexOf(this);
                    if (index > 0 && Parent.Elements[index - 1].Box == Box.Block)
                        return new Point(initial.X + x, initial.Y + max);
                    return x + Width > Parent.ContentBounds.Width ?
                        new Point(initial.X, initial.Y + max) : 
                        index <= 0 ? new Point(initial.X + x, initial.Y)
                        : new Point(initial.X + x, initial.Y + Parent.Elements[index - 1].ExtendedBounds.Top - Parent.Elements[index - 1].Parent.Position.Y);
                }
                if (Box == Box.Block)
                {
                    var max = Parent.Elements.TakeWhile(element => element != this)
                        .Select(element => element.ExtendedBounds.Bottom - element.Parent.Position.Y).Max();
                    return new Point(initial.X, initial.Y + max);
                }
                return initial;
            }
        }

        /// <summary>
        /// Rectangular region of the content area (inside the padding).
        /// </summary>
        public Rectangle ContentBounds
        {
            get { return (Padding.Offset(Margin) + Size).Offset(BasePosition); }
        }

        /// <summary>
        /// Rectangular region of the element.
        /// </summary>
        public Rectangle BorderBounds
        {
            get { return Size.Offset(Margin).Offset(BasePosition); }
        }

        /// <summary>
        /// Rectangular region of the element plus the margin area.
        /// </summary>
        public Rectangle ExtendedBounds
        {
            get { return Size.Extend(Margin).Offset(BasePosition); }
        }

        private int minWidth, minHeight, maxWidth, maxHeight;

        public Element(Manager manager) : base(manager)
        {
            Elements = new List<Element>();

            Width = 100;
            Height = 50;

            MaxWidth = 200;
            MaxHeight = 100;

            Box = Box.Inline;
            Margin = 8;
            Padding = 4;
        }

        /// <summary>
        /// Adds a child element to this element.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void Add(Element element)
        {
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
                }
            }
        }

        /// <summary>
        /// Removes a child element from this element.
        /// </summary>
        /// <param name="element">The element to remove.</param>
        /// <param name="dispose">Should the control be disposed and destroyed?</param>
        public void Remove(Element element, bool dispose = true)
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
                }
            }
        }

        /// <summary>
        /// Releases resources used by the element, removes itself from its parent, and disposes of all children.
        /// </summary>
        private void Dispose()
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