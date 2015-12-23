using System;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Brushes
{
    /// <summary>
    /// A method of painting a visual.
    /// </summary>
    public class Brush : DependencyObject
    {
        /// <summary>
        /// Indicates the transparency of the brush, from 0 to 1.
        /// </summary>
        public double Opacity { get; set; }

        private static readonly string[] splitter = {"->"};

        public static explicit operator Brush(string value)
        {
            var gradientStops = value.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            if (gradientStops.Length == 2)
            {
                return new GradientBrush((Color)gradientStops[0], (Color)gradientStops[1]);
            }
            return new ColorBrush((Color) value);
        }

        public static implicit operator Brush(Color color) => new ColorBrush(color);
    }
}