using Pyratron.UI.Types;

namespace Pyratron.UI.Brushes
{
    /// <summary>
    /// Brush to paint a visual with a solid color.
    /// </summary>
    public class ColorBrush : Brush
    {
        public Color Color { get; set; }

        public ColorBrush()
        {
            Color = Color.Black;
        }

        public ColorBrush(Color color)
        {
            Color = color;
        }

        public static implicit operator ColorBrush(Color color) => new ColorBrush(color);

        public static implicit operator Color(ColorBrush brush) => brush.Color;
    }
}