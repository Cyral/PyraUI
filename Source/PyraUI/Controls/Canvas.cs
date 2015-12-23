using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// Arranges elements by explicit positions using coordinates.
    /// </summary>
    public class Canvas : Panel
    {
        public static readonly DependencyProperty<double> LeftProperty =
            DependencyProperty.RegisterAttached<Canvas, double>("Left", double.NaN,
                new PropertyMetadata(MetadataOption.AffectsArrange | MetadataOption.IgnoreInheritance));

        public static readonly DependencyProperty<double> TopProperty =
            DependencyProperty.RegisterAttached<Canvas, double>("Top", double.NaN,
                new PropertyMetadata(MetadataOption.AffectsArrange | MetadataOption.IgnoreInheritance));

        public static readonly DependencyProperty<double> RightProperty =
            DependencyProperty.RegisterAttached<Canvas, double>("Right", double.NaN,
                new PropertyMetadata(MetadataOption.AffectsArrange | MetadataOption.IgnoreInheritance));

        public static readonly DependencyProperty<double> BottomProperty =
            DependencyProperty.RegisterAttached<Canvas, double>("Bottom", double.NaN,
                new PropertyMetadata(MetadataOption.AffectsArrange | MetadataOption.IgnoreInheritance));

        public Canvas(Manager manager) : base(manager)
        {
        }

        public static double GetBottom(DependencyObject obj)
        {
            return obj.GetValue(BottomProperty);
        }

        public static double GetLeft(DependencyObject obj)
        {
            return obj.GetValue(LeftProperty);
        }

        public static double GetRight(DependencyObject obj)
        {
            return obj.GetValue(RightProperty);
        }

        public static double GetTop(DependencyObject obj)
        {
            return obj.GetValue(TopProperty);
        }

        public static void SetBottom(DependencyObject obj, double value)
        {
            obj.SetValue(BottomProperty, value);
        }

        public static void SetLeft(DependencyObject obj, double value)
        {
            obj.SetValue(LeftProperty, value);
        }

        public static void SetRight(DependencyObject obj, double value)
        {
            obj.SetValue(RightProperty, value);
        }

        public static void SetTop(DependencyObject obj, double value)
        {
            obj.SetValue(TopProperty, value);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Elements)
            {
                // By default, position them in the top left corner.
                var x = 0d;
                var y = 0d;
                var left = GetLeft(child);
                var top = GetTop(child);

                // X axis
                if (!double.IsNaN(left))
                    x = left; // If left is defined, use that for the x position.
                else // If it is not, use the right position.
                {
                    // Arrange with right.
                    var right = GetRight(child);
                    if (!double.IsNaN(right))
                        x = finalSize.Width - child.DesiredSize.Width - right;
                }

                // Y axis
                if (!double.IsNaN(top))
                    y = top;
                else
                {
                    var elementBottom = GetBottom(child);
                    if (!double.IsNaN(elementBottom))
                        y = finalSize.Height - child.DesiredSize.Height - elementBottom;
                }

                child.Arrange(new Rectangle(new Point(x, y), child.DesiredSize));
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Give each child infinite size.
            foreach (var child in Elements)
            {
                // Idea: Give each child the available size of the container, minus their position?
                //child.Measure(availableSize - new Size(child.Position.X, child.Position.Y));
                child.Measure(Size.Infinity);
            }

            return Size.Zero;
        }
    }
}