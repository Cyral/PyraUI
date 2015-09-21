using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Pyratron.UI.Types
{
    [DebuggerDisplay("{X}, {Y}")]
    public struct Point
    {
        public static readonly Point Zero = new Point();

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public static Point operator +(Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);

        public static Point operator -(Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y);

        public static bool operator ==(Point left, Point right) => Equals(left.X, right.X) && Equals(left.Y, right.Y);

        public static bool operator !=(Point left, Point right) => !(left == right);

        public static Point operator +(Point point, Thickness thickness) => new Point(point.X + thickness.Left, point.Y + thickness.Top);

        public override bool Equals(object obj)
        {
            if (!(obj is Point)) return false;
            var comp = (Point) obj;
            return Equals(comp.X, X) && Equals(comp.Y, Y);
        }

        public bool Equals(Point other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
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