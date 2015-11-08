using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Pyratron.UI.Types
{
    /// <summary>
    /// Represents a point in 2D space.
    /// </summary>
    [DebuggerDisplay("{X}, {Y}")]
    public struct Point
    {
        /// <summary>
        /// 0,0
        /// </summary>
        public static readonly Point Zero = new Point();

        public Point(double x, double y)
        {
            if (x.IsValid())
                throw new ArgumentException(nameof(x), "Value must be a valid number.");
            if (y.IsValid())
                throw new ArgumentException(nameof(y), "Value must be a valid number.");

            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public static Point operator -(Point point) => new Point(-point.X, -point.Y);

        public static Point operator +(Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);

        public static Point operator -(Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y);

        public static bool operator ==(Point left, Point right) => left.Equals(right);

        public static bool operator !=(Point left, Point right) => !(left == right);

        public static Point operator +(Point point, Thickness thickness) => new Point(point.X + thickness.Left, point.Y + thickness.Top);

        public static Point operator *(Point point, double factor) => new Point(point.X * factor, point.Y * factor);

        public static Point operator /(Point point, double factor) => new Point(point.X / factor, point.Y / factor);

        public bool Equals(Point other) => X.Equals(other.X) && Y.Equals(other.Y);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString() => "X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture);
    }
}