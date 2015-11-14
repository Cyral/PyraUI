using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Pyratron.UI.Brushes;

namespace Pyratron.UI.Monogame
{
    /// <summary>
    /// Converts PyraUI types to XNA types and vice-versa.
    /// </summary>
    public static class Convert
    {
        public static Rectangle ToXNA(this Types.Rectangle rect)
        {
            return new Rectangle((int)Math.Round(rect.X), (int)Math.Round(rect.Y), (int)Math.Round(rect.Width), (int)Math.Round(rect.Height));
        }

        public static Color ToXNA(this Types.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public static Color ToXNA(this Brush brush)
        {
            var color = brush as ColorBrush;
            if (color != null)
                return new Color(color.Color.R, color.Color.G, color.Color.B, color.Color.A);
            return Color.Black;
        }

        public static Types.Rectangle ToOriginal(this Rectangle rect)
        {
            return new Types.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Types.Color ToOriginal(this Color color)
        {
            return new Types.Color(color.R, color.G, color.B, color.A);
        }

        public static Types.Point ToOriginal(this Point point)
        {
            return new Types.Point(point.X, point.Y);
        }
    }
}
