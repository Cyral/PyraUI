using Pyratron.UI.Effects;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Brushes
{
    /// <summary>
    /// Brush to paint a visual with a solid color.
    /// </summary>
    public class ColorBrush : Brush
    {
        public Color Color
        {
            get { return GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty<Color> ColorProperty =
          DependencyProperty.Register<DropShadowEffect, Color>(nameof(ColorBrush), Color.Black);

        public ColorBrush()
        {

        }

        public ColorBrush(Color color)
        {
            Color = color;
        }

        public static implicit operator ColorBrush(Color color) => new ColorBrush(color);

        public static implicit operator Color(ColorBrush brush) => brush.Color;
    }
}