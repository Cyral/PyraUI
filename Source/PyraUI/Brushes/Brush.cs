using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyratron.UI.Types;

namespace Pyratron.UI.Brushes
{
    /// <summary>
    /// A method of painting a visual.
    /// </summary>
    public class Brush
    {
        /// <summary>
        /// Indicates the transparency of the brush, from 0 to 1.
        /// </summary>
        public double Opacity { get; set; }

        public static implicit operator Brush(Color color) => new ColorBrush(color);

        public static explicit operator Brush(string value)
        {
            return new ColorBrush((Color) value);
        }
    }
}
