using System;
using System.Collections.Generic;
using System.Linq;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// Arranges child elements in a stacked manner that can be oriented horizontally or vertically, breaking the content in
    /// order to wrap to a new line.
    /// </summary>
    public class WrapPanel : Panel
    {
        public static readonly DependencyProperty<Orientation> OrientationProperty =
            DependencyProperty.Register<StackPanel, Orientation>(nameof(Orientation), Orientation.Vertical,
                new PropertyMetadata(MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure |
                                     MetadataOption.AffectsArrange));

        public Orientation Orientation
        {
            get { return GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        private List<WrapGroup> groupCache;
        private bool groupsInvalidated = true;

        public WrapPanel(Manager manager) : base(manager)
        {
            groupCache = new List<WrapGroup>();
        }

        public override void Add(Element element)
        {
            groupsInvalidated = true;
            base.Add(element);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double maxStackLength = 0;
            double stretchLength = 0;
            var groups = GetGroups(finalSize);

            foreach (var group in groups)
            {
                // Find max size of child to be the max width/height of the row/column.
                var groupMaxLength = group.Elements.Select(child => GetStretchLength(child.DesiredSize)).Max();
                double groupLength = 0;

                foreach (var child in group.Elements)
                {
                    var childStackLength = GetStackLength(child.DesiredSize);

                    var rect = Orientation == Orientation.Horizontal
                        ? new Rectangle(groupLength, stretchLength, childStackLength, groupMaxLength)
                        : new Rectangle(stretchLength, groupLength, groupMaxLength, childStackLength);
                    child.Arrange(rect);

                    groupLength += childStackLength; //The next element will be position below/to the right of this one.
                }

                maxStackLength = Math.Max(maxStackLength, groupLength);
                // Move the x/y over by the max length to start a new row or column.
                stretchLength += groupMaxLength;
            }

            return finalSize;
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            // Measure each child with the same available size as the parent, in the direction of the panel.
            foreach (var element in Elements)
                element.Measure(CreateSize(GetStackLength(availableSize), GetStretchLength(availableSize)));

            var groups = GetGroups(availableSize);

            // Find the tallest and longest group lengths.
            double stackLength = 0, stretchLength = 0;
            foreach (var group in groups)
            {
                double groupStackLength = 0, groupStretchLength = 0;
                foreach (var child in group.Elements)
                {
                    groupStackLength += GetStackLength(child.DesiredSize);
                    groupStretchLength = Math.Max(groupStretchLength, GetStretchLength(child.DesiredSize));
                    // Group with largest child.
                }
                stackLength = Math.Max(stackLength, groupStackLength); // Largest group based on total size.
                stretchLength += groupStretchLength;
            }

            return CreateSize(stackLength, stretchLength);
        }


        private Size CreateSize(double stackLength, double stretchLength) => Orientation == Orientation.Vertical
            ? new Size(stretchLength, stackLength)
            : new Size(stackLength, stretchLength);

        /// <summary>
        /// Create or gets the groups of elements that will form rows/colums.
        /// </summary>
        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private List<WrapGroup> GetGroups(Size size)
        {
            if (!groupsInvalidated)
                return groupCache;
            var maxLength = GetStackLength(size); // Size of one row/column

            var groups = new List<WrapGroup>();
            var group = new WrapGroup();

            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];
                var childSize = GetStackLength(child.DesiredSize);

                // If the group length + this child is more than the column/row length, start a new group.
                if (group.Length + childSize > maxLength)
                {
                    groups.Add(group);
                    group = new WrapGroup();
                }

                group.AddElement(child);
                group.Length += childSize;
            }

            groups.Add(group);
            groupsInvalidated = false;
            groupCache = groups;
            return groups;
        }

        private double GetStackLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Width : size.Height;
        }

        private double GetStretchLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Height : size.Width;
        }

        /// <summary>
        /// A row/column of elements.
        /// </summary>
        private class WrapGroup
        {
            public List<Element> Elements { get; }

            /// <summary>
            /// Row/column length.
            /// </summary>
            public double Length { get; set; }

            public WrapGroup()
            {
                Elements = new List<Element>();
            }

            public void AddElement(Element element)
            {
                Elements.Add(element);
            }
        }
    }
}