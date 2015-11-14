﻿using System;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// Arranges child elements in a stacked manner that can be oriented horizontally or vertically.
    /// </summary>
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
                InvalidateMeasure();
                InvalidateArrange();
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

        protected override Size ArrangeOverride(Size finalSize)
        {
            var stretchLength = GetStretchSize(finalSize);

            double stackLength = 0;
            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];
                // Get the size in the stacking direction of the child.
                var childStackLength = GetStackSize(child.DesiredSize);

                var point = CreatePoint(stackLength, 0);

                // Create a rectangle from the point and length of the stretch and stack lengths.
                var rect = Orientation == Orientation.Vertical
                    ? new Rectangle(point, stretchLength - Padding.Width, childStackLength - Padding.Height)
                    : new Rectangle(point, childStackLength - Padding.Width, stretchLength - Padding.Height);

                child.Arrange(rect);

                stackLength += childStackLength;
            }

            return CreateSize(GetStackSize(finalSize), stretchLength);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double stackLength = 0, stretchLength = 0;

            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];

                // Measure each child with the container size in the stack direction and an infinite size in the other.
                child.Measure(CreateSize(double.PositiveInfinity, GetStretchSize(availableSize)));

                // Add to the stack length so far, and use the largest size in the stretch directorion to set the size of the stack panel.
                stackLength += GetStackSize(child.DesiredSize);
                stretchLength = Math.Max(stretchLength, GetStretchSize(child.DesiredSize));
            }

            return CreateSize(stackLength, stretchLength);
        }

        private Point CreatePoint(double mainLength, double crossLength) => Orientation == Orientation.Vertical
            ? new Point(crossLength, mainLength)
            : new Point(mainLength, crossLength);

        private Size CreateSize(double mainLength, double crossLength) => Orientation == Orientation.Vertical
            ? new Size(crossLength, mainLength)
            : new Size(mainLength, crossLength);

        private double GetStackSize(Size size) => Orientation == Orientation.Vertical ? size.Height : size.Width;

        private double GetStretchSize(Size size) => Orientation == Orientation.Vertical ? size.Width : size.Height;
    }
}