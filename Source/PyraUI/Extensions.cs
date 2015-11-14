using System;
using System.Windows.Markup;
using Pyratron.UI.Types;

namespace Pyratron.UI
{
    internal static class InternalExtensions
    {
        /// <summary>
        /// Returns true of the value is in range of the min and max values, inclusive.
        /// </summary>
        public static bool InRange(this float val, float min, float max) => val >= min && val <= max;

        /// <summary>
        /// Returns true of the value is in range of the min and max values, inclusive.
        /// </summary>
        public static bool InRange(this int val, int min, int max) => val >= min && val <= max;

        /// <summary>
        /// Returns true of the value is in range of the min and max values, inclusive.
        /// </summary>
        public static bool InRange(this byte val, byte min, byte max) => val >= min && val <= max;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        /// <summary>
        /// Returns true if each double value are equal within margin of error (possibly due to floating point precision).
        /// </summary>
        public static bool IsClose(this double self, double value) => self == value || (Math.Abs(self - value) < 1e-5);
    }
}