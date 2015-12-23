using System;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// Dock directions.
    /// </summary>
    public enum Dock
    {
        Left,
        Top,
        Right,
        Bottom
    }

    /// <summary>
    /// Arranges controls to the left, top,right, or bottom of the panel.
    /// </summary>
    public class DockPanel : Panel
    {
        public static readonly DependencyProperty<Dock> DockProperty =
            DependencyProperty.RegisterAttached<DockPanel, Dock>("Dock",
                new PropertyMetadata(MetadataOption.AffectsArrange | MetadataOption.AffectsMeasure |
                                     MetadataOption.AffectsParentArrange |
                                     MetadataOption.IgnoreInheritance));

        public static readonly DependencyProperty<bool> LastChildFillProperty =
            DependencyProperty.Register<DockPanel, bool>(nameof(LastChildFill), true,
                new PropertyMetadata(MetadataOption.IgnoreInheritance |
                                     MetadataOption.AffectsArrange));

        /// <summary>
        /// Indicates if the last element in the panel will stretch to fill the available space.
        /// </summary>
        public bool LastChildFill
        {
            get { return GetValue(LastChildFillProperty); }
            set { SetValue(LastChildFillProperty, value); }
        }

        public DockPanel(Manager manager) : base(manager)
        {
        }

        public static Dock GetDock(DependencyObject obj)
        {
            return obj.GetValue(DockProperty);
        }

        public static void SetDock(DependencyObject obj, Dock value)
        {
            obj.SetValue(DockProperty, value);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var remainingWidth = finalSize.Width;
            var remainingHeight = finalSize.Height;

            double left = 0, top = 0;

            for (var i = 0; i < Elements.Count; i++)
            {
                var child = Elements[i];
                var dock = GetDock(child);
                var orientation = GetOrientation(dock);

                // If this is the last child and LastChildFill is true, fill the child to the available size.
                var fill = LastChildFill && i == Elements.Count - 1;

                // If the orientation is vertical, fill the width, and if it is horizontal, will the height.
                var cellWidth = orientation == Orientation.Vertical || fill
                    ? remainingWidth
                    : child.DesiredSize.Width;
                var cellHeight = orientation == Orientation.Horizontal || fill
                    ? remainingHeight
                    : child.DesiredSize.Height;

                // If to the right or bottom, use the remaining size - the cell size.
                double cellLeft = 0, cellTop = 0;
                if (dock == Dock.Right)
                    cellLeft = remainingWidth - cellWidth;
                else if (dock == Dock.Bottom)
                    cellTop = remainingHeight - cellHeight;

                child.Arrange(new Rectangle(left + cellLeft, top + cellTop, cellWidth, cellHeight));

                // Add to the left or top if from the left or top.
                if (orientation == Orientation.Horizontal)
                {
                    remainingWidth -= cellWidth;
                    if (dock == Dock.Left)
                        left += cellWidth;
                }
                else
                {
                    remainingHeight -= cellHeight;
                    if (dock == Dock.Top)
                        top += cellHeight;
                }
            }

            return finalSize;
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            var remainingWidth = availableSize.Width;
            var remainingHeight = availableSize.Height;

            foreach (var child in Elements)
            {
                // Measure the child with the remaining area.
                child.Measure(new Size(remainingWidth, remainingHeight));

                // Subtract from the remaining area depending on the orientation.
                if (GetOrientation(GetDock(child)) == Orientation.Horizontal)
                    remainingWidth -= child.DesiredSize.Width;
                else
                    remainingHeight -= child.DesiredSize.Height;
            }

            double totalWidth = 0;
            double totalHeight = 0;

            foreach (var child in Elements)
            {
                if (GetOrientation(GetDock(child)) == Orientation.Horizontal)
                {
                    totalWidth += child.DesiredSize.Width;
                    totalHeight = Math.Max(totalHeight, child.DesiredSize.Height);
                }
                else
                {
                    totalHeight += child.DesiredSize.Height;
                    totalWidth = Math.Max(totalWidth, child.DesiredSize.Width);
                }
            }

            return new Size(totalWidth, totalHeight);
        }

        private static Orientation GetOrientation(Dock dock)
            => dock == Dock.Left || dock == Dock.Right ? Orientation.Horizontal : Orientation.Vertical;
    }
}