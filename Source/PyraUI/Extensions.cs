using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyratron.UI
{
    internal static class InternalExtensions
    {
        /// <summary>
        /// Returns true of the value is in range of the min and max values, inclusive.
        /// </summary>
        public static bool InRange(this float val, float min, float max)
        {
            return val >= min && val <= max;
        }

        /// <summary>
        /// Returns true of the value is in range of the min and max values, inclusive.
        /// </summary>
        public static bool InRange(this int val, int min, int max)
        {
            return val >= min && val <= max;
        }

        /// <summary>
        /// Returns true of the value is in range of the min and max values, inclusive.
        /// </summary>
        public static bool InRange(this byte val, byte min, byte max)
        {
            return val >= min && val <= max;
        }

        public static bool IsValid(this double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
