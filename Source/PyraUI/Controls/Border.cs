using Pyratron.UI.Brushes;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// Draws a border and/or background around an element.
    /// </summary>
    public class Border : Decorator
    {
        public static readonly DependencyProperty<Color> BackgroundProperty =
            DependencyProperty.Register<Border, Color>(nameof(Background), Color.Transparent);

        public static readonly DependencyProperty<int> CornderRadiusProperty =
            DependencyProperty.Register<Border, int>(nameof(CornerRadius));

        public static readonly DependencyProperty<Thickness> BorderThicknessProperty =
            DependencyProperty.Register<Border, Thickness>(nameof(BorderThickness));

        public static readonly DependencyProperty<Brush> BorderBrushProperty =
            DependencyProperty.Register<Border, Brush>(nameof(BorderBrush));

        /// <summary>
        /// The area inside the border.
        /// </summary>
        public Thickness Padding
        {
            get { return GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty<Thickness> PaddingProperty =
            DependencyProperty.Register<Element, Thickness>(nameof(Padding),
                MetadataOption.IgnoreInheritance | MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange);


        /// <summary>
        /// Background color of the border.
        /// </summary>
        public Color Background
        {
            get { return GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Border radius.
        /// </summary>
        public int CornerRadius
        {
            get { return GetValue(CornderRadiusProperty); }
            set { SetValue(CornderRadiusProperty, value); }
        }


        public Thickness BorderThickness
        {
            get { return GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Brush to render the border with.
        /// </summary>
        public Brush BorderBrush
        {
            get { return GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }


        public Border(Manager manager) : base(manager)
        {
            
        }

        public override void Draw(float delta)
        {
            if (!BorderThickness.IsEmpty)
            {
                Manager.Renderer.DrawRectangle(BorderArea, BorderBrush, BorderThickness, CornerRadius, ParentBounds);
                Manager.Renderer.FillRectangle(BorderArea.RemoveBorder(BorderThickness), Background,
                    CornerRadius - BorderThickness.Max,
                    ParentBounds);
            }
            else
                Manager.Renderer.FillRectangle(BorderArea.RemoveBorder(BorderThickness), Background, CornerRadius,
                    ParentBounds);

            base.Draw(delta);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Child?.Arrange(new Rectangle(BorderThickness + Padding, finalSize.Remove(BorderThickness).Remove(Padding)));

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Remove the border and padding, measure the element, and then add them back, as they must be ignored.
            availableSize = availableSize.Remove(BorderThickness).Remove(Padding);
            var size = base.MeasureOverride(availableSize) + BorderThickness + Padding;
            return size;
        }
    }
}