using System;
using System.Collections.Generic;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// A UI element that has a position, size, and parent element.
    /// </summary>
    public class Element : Component
    {
        public Element(Manager manager) : base(manager)
        {
            Elements = new List<Element>();

            ActualSize = Size.Zero;

            PreviousFinalRect = Rectangle.Empty;
            PreviousAvailableSize = Size.NaN;
            PreviousDesiredSize = Size.NaN;
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
                    // Reset the level depth.
                    element.Level = -1;

                    element.Manager = Manager;
                    element.Parent = this;

                    // Add to this element.
                    Elements.Add(element);
                    element.DependencyParent = this;

                    // Add to layout queue.
                    Manager.Layout.AddMeasure(element);
                    Manager.Layout.AddArrange(element);
                }
            }
        }

        /// <summary>
        /// Handles inline XML content (such as text between the opening and closing tag).
        /// </summary>
        public virtual void AddContent(string content)
        {
            // By default, create a label inside this element.
            Add(new Label(Manager, content));
        }

        /// <summary>
        /// Arrange an element and all child elements.
        /// </summary>
        /// <param name="finalRect">The final size that the parent computes for the child element.</param>
        public void Arrange(Rectangle finalRect)
        {
            // If the arrangement is valid and the rectangle is unchanged, return.
            if (isArrangeValid && finalRect.IsClose(PreviousFinalRect))
            {
                Manager.Layout.RemoveArrange(this);
                return;
            }

            // If the measure is invalid and the rectangle size is different, measure before continuing.
            if (!isMeasureValid || !PreviousAvailableSize.IsClose(finalRect.Size))
                Measure(finalRect.Size);

            // Move to the arrange process.
            ArrangeCore(finalRect);

            // Keep a record of the last rectangle and remove this element from the layout queue.
            PreviousFinalRect = finalRect;
            isArrangeValid = true;
            Manager.Layout.RemoveArrange(this);
        }

        /// <summary>
        /// Invalidates the arrangement of the element, adding it to the arrange queue.
        /// </summary>
        public void InvalidateArrange()
        {
            isArrangeValid = false;
            Manager.Layout.AddArrange(this);
        }

        /// <summary>
        /// Invalidates the measurement of the element, adding it to the measure queue.
        /// </summary>
        public void InvalidateMeasure()
        {
            if (!isMeasureValid && !PreviousAvailableSize.IsEmpty)
                return;

            isMeasureValid = false;
            Manager.Layout.AddMeasure(this);
        }

        /// <summary>
        /// Computes a desired size of the element.
        /// </summary>
        /// <param name="availableSize">
        /// The available space that the parent element has allocated to the child.
        /// </param>
        public virtual void Measure(Size availableSize)
        {
            // If the measure is valid and the previous size is the same, return the previous size.
            if (isMeasureValid && PreviousAvailableSize.IsClose(availableSize))
            {
                Manager.Layout.RemoveMeasure(this);
                DesiredSize = PreviousDesiredSize;
                return;
            }

            // The element will have to be arranged due to the size being changed.
            InvalidateArrange();

            // Move on to the measure process.
            DesiredSize = MeasureCore(availableSize).Min(availableSize);

            // Set the measure as valid and update the previous sizes.
            isMeasureValid = true;
            PreviousAvailableSize = availableSize;
            PreviousDesiredSize = DesiredSize;
            Manager.Layout.RemoveMeasure(this);
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
                    element.DependencyParent = null;
                    if (dispose)
                        element.Dispose();
                }
            }
        }

        protected internal override void Update(float delta)
        {
            UpdateChildren(delta);
        }

        /// <summary>
        /// Arranges the element.
        /// </summary>
        protected virtual void ArrangeCore(Rectangle finalRect)
        {
            // The final size will be equal to the desired size, unless horizontal or vertical alignment is stretch, in which case it will fill the container.
            var finalWidth = HorizontalAlignment != HorizontalAlignment.Stretch ? DesiredSize.Width : finalRect.Width;
            var finalHeight = VerticalAlignment != VerticalAlignment.Stretch ? DesiredSize.Height : finalRect.Height;
            var finalSize = new Size(finalWidth, finalHeight);

            finalSize = finalSize.Remove(Margin); // Remove the margin as it needs to be ignored.

            // If the elements width or height is 0 or NaN, fall back to the measured size.
            finalSize = Size.Fallback(finalSize).Clamp(MinSize, MaxSize);

            // Arrange all child elements and return a final size.
            var arrangedSize = ArrangeOverride(finalSize);

            // Get the offset for the element (due to alignment)
            var alignedOffset = Align(finalRect, arrangedSize.Add(Margin));

            Position = alignedOffset + Margin;

            ActualSize = arrangedSize;
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// By default, makes each child fill the entire area.
        /// </summary>
        protected virtual Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Elements)
                child.Arrange(new Rectangle(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Releases resources used by the element, removes itself from its parent, and disposes of all children.
        /// </summary>
        protected virtual void Dispose()
        {
            Parent.Remove(this, false);
            Parent = null;
            Manager.RemoveRootElement(this);
            foreach (var child in Elements)
                Remove(child);
            Elements.Clear();
        }

        /// <summary>
        /// Measures the element.
        /// </summary>
        protected virtual Size MeasureCore(Size availableSize)
        {
            // Remove the margin from the size, as it should be ignored for now.
            availableSize = availableSize.Remove(Margin);

            // If the width or height is NaN or 0, fall back to the available size, and clamp it within the min and max sizes.
            availableSize = Size.Fallback(availableSize).Clamp(MinSize, MaxSize);

            // Measure the size of child elements.
            var measuredSize = MeasureOverride(availableSize);

            measuredSize = Size.Fallback(measuredSize).Clamp(MinSize, MaxSize);

            measuredSize = measuredSize.Add(Margin);

            return measuredSize;
        }

        /// <summary>
        /// Measures the size of all child elements in a specific layout pattern.
        /// By default, returns the size of the largest child element.
        /// </summary>
        protected virtual Size MeasureOverride(Size availableSize)
        {
            // By default, return the size of the largest child element.
            var desired = Size.Zero;

            foreach (var child in Elements)
            {
                child.Measure(availableSize);
                desired = desired.Max(child.DesiredSize);
            }

            return desired;
        }

        protected override void OnPropertyChanged(DependencyProperty property, object newValue, object oldValue)
        {
            var type = GetType();
            var metadata = property.GetMetadata(type);
            // Handle metadata properties when property value is changed.
            if (metadata.AffectsMeasure)
                InvalidateMeasure();
            if (metadata.AffectsArrange)
                InvalidateArrange();
        }

        /// <summary>
        /// Retuns an offset to the elements position according to its size, container size, and alignment.
        /// </summary>
        private Point Align(Rectangle container, Size size)
        {
            if (!double.IsInfinity(size.Width)) // Infinite sizes will be handled elsewhere.
            {
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Stretch:
                    case HorizontalAlignment.Center:
                        container.X = container.Left + (container.Width - size.Width) / 2;
                        break;
                    case HorizontalAlignment.Right:
                        container.X = container.Left + container.Width - size.Width;
                        break;
                }
            }

            if (!double.IsInfinity(size.Height))
            {
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Stretch:
                    case VerticalAlignment.Center:
                        container.Y = container.Top + (container.Height - size.Height) / 2;
                        break;
                    case VerticalAlignment.Bottom:
                        container.Y = container.Top + container.Height - size.Height;
                        break;
                }
            }

            return container.Point;
        }

        /// <summary>
        /// Update all children elements.
        /// </summary>
        private void UpdateChildren(float delta)
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                Elements[i].Update(delta);
            }
        }

        #region Properties

        public override int MaxChildren => 1;

        public string Name { get; set; }

        /// <summary>
        /// The area outside the border.
        /// </summary>
        public Thickness Margin
        {
            get { return GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        public static readonly DependencyProperty<Thickness> MarginProperty =
            DependencyProperty.Register<Element, Thickness>(nameof(Margin),
                MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange);


        /// <summary>
        /// The position of the element relative to its parent.
        /// </summary>
        internal Point Position { get; set; }

        /// <summary>
        /// The absolute position of the element.
        /// </summary>
        internal Point AbsolutePosition
        {
            get
            {
                if (Parent == null)
                    return Position;
                return Parent.BorderArea.Point + Position;
            }
        }

        /// <summary>
        /// The horizontal alignment of the element.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty<HorizontalAlignment> HorizontalAlignmentProperty =
            DependencyProperty.Register<Element, HorizontalAlignment>(nameof(HorizontalAlignment),
                HorizontalAlignment.Stretch,
                MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange);

        /// <summary>
        /// The vertical alignment of the element.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty<VerticalAlignment> VerticalAlignmentProperty =
            DependencyProperty.Register<Element, VerticalAlignment>(nameof(VerticalAlignment), VerticalAlignment.Stretch,
                MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange);


        /// <summary>
        /// The font size.
        /// </summary>
        public int FontSize
        {
            get { return GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty<int> FontSizeProperty =
            DependencyProperty.Register<Element, int>(nameof(FontSize), 10);


        /// <summary>
        /// The font style.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public static readonly DependencyProperty<FontStyle> FontStyleProperty =
            DependencyProperty.Register<Element, FontStyle>(nameof(FontStyle));


        /// <summary>
        /// The text color.
        /// </summary>
        public Color TextColor
        {
            get { return GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly DependencyProperty<Color> TextColorProperty =
            DependencyProperty.Register<Element, Color>(nameof(TextColor), Color.Black);

        /// <summary>
        /// Make sure Width/Height values are above 0.
        /// </summary>
        private static object CoerceDiminsion(object value)
        {
            return Math.Max(0, (double) value);
        }

        /// <summary>
        /// Make sure Width/Height values are above 0.
        /// </summary>
        private static bool ValidateDiminsion(object value)
        {
            var val = (double) value;
            return !double.IsInfinity(val) && !double.IsNaN(val);
        }

        /// <summary>
        /// The minimum width.
        /// </summary>
        public double MinWidth
        {
            get { return GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        public static readonly DependencyProperty<double> MinWidthProperty =
            DependencyProperty.Register<Element, double>(nameof(MinWidth), 0,
                new PropertyMetadata(
                    MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange,
                    CoerceDiminsion), ValidateDiminsion);

        /// <summary>
        /// The maximum width.
        /// </summary>
        public double MaxWidth
        {
            get { return GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        public static readonly DependencyProperty<double> MaxWidthProperty =
            DependencyProperty.Register<Element, double>(nameof(MaxWidth), double.PositiveInfinity,
                new PropertyMetadata(
                    MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange,
                    CoerceDiminsion));

        /// <summary>
        /// The target width.
        /// </summary>
        public double Width
        {
            get { return GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty<double> WidthProperty =
            DependencyProperty.Register<Element, double>(nameof(Width), 0,
                new PropertyMetadata(
                    MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange,
                    CoerceDiminsion), ValidateDiminsion);

        /// <summary>
        /// The minimum height.
        /// </summary>
        public double MinHeight
        {
            get { return GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        public static readonly DependencyProperty<double> MinHeightProperty =
            DependencyProperty.Register<Element, double>(nameof(MinHeight), 0,
                new PropertyMetadata(
                    MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange,
                    CoerceDiminsion), ValidateDiminsion);

        /// <summary>
        /// The maximum height.
        /// </summary>
        public double MaxHeight
        {
            get { return GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        public static readonly DependencyProperty<double> MaxHeightProperty =
            DependencyProperty.Register<Element, double>(nameof(MaxHeight), double.PositiveInfinity,
                new PropertyMetadata(
                    MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange,
                    CoerceDiminsion));

        /// <summary>
        /// The target height.
        /// </summary>
        public double Height
        {
            get { return GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty<double> HeightProperty =
            DependencyProperty.Register<Element, double>(nameof(Height), 0,
                new PropertyMetadata(
                    MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange,
                    CoerceDiminsion), ValidateDiminsion);

        /// <summary>
        /// Element's parent. Null if root.
        /// </summary>
        public virtual Element Parent { get; protected internal set; }

        public Rectangle ParentBounds => Parent?.BorderArea ?? Rectangle.Empty;

        /// <summary>
        /// List of child elements.
        /// </summary>
        public List<Element> Elements { get; }

        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Size MinSize => new Size(MinWidth, MinHeight);

        public Size MaxSize => new Size(MaxWidth, MaxHeight);

        /// <summary>
        /// The actual size of the element.
        /// </summary>
        public Size ActualSize { get; protected set; }

        /// <summary>
        /// The desired size of the element if the size is available.
        /// </summary>
        public Size DesiredSize
        {
            get { return desiredSize; }
            protected set
            {
                if (!value.IsClose(desiredSize))
                {
                    desiredSize = value;

                    // Since the size of this element has changed, the parent must be re-measured.
                    Parent?.InvalidateMeasure();
                }
            }
        }

        internal Size PreviousAvailableSize { get; private set; }
        internal Rectangle PreviousFinalRect { get; private set; }
        private Size PreviousDesiredSize { get; set; }

        private int level = -1;

        /// <summary>
        /// The level the element is in the visual tree.
        /// </summary>
        public int Level
        {
            get
            {
                if (level == -1)
                    level = Parent?.Level + 1 ?? 0;
                return level;
            }
            internal set { level = value; }
        }

        /// <summary>
        /// Indicates if the element is a root level element (having no parent).
        /// </summary>
        public bool IsRoot => Parent == null;

        /// <summary>
        /// Rectangular region of the element.
        /// </summary>
        public Rectangle BorderArea => new Rectangle(AbsolutePosition, ActualSize);

        /// <summary>
        /// Rectangular region of the element plus the margin area.
        /// </summary>
        public Rectangle ExtendedArea => new Rectangle(AbsolutePosition - Margin, ActualSize.Add(Margin));

        /// <summary>
        /// Indicates if the element has been measured. If false, the element should be re-measured.
        /// </summary>
        private bool isMeasureValid;

        /// <summary>
        /// Indicates if the element has been arranged. If false, the element should be re-arranged.
        /// </summary>
        private bool isArrangeValid;

        private Size desiredSize;

        #endregion
    }
}