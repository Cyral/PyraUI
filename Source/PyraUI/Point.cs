using System.ComponentModel;
using System.Globalization;

namespace PyraUI
{
    public struct Point
    {
        public static readonly Point Zero = new Point();

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        [Browsable(false)]
        public bool IsZero => X == 0 && Y == 0;

        public int X { get; }

        public int Y { get; }

        public static Point operator +(Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);

        public static Point operator -(Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y);

        public static bool operator ==(Point left, Point right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Point left, Point right) => !(left == right);

        public static Point operator +(Point point, Thickness thickness) => new Point(point.X + thickness.Left, point.Y + thickness.Top);

        public override bool Equals(object obj)
        {
            if (!(obj is Point)) return false;
            var comp = (Point) obj;
            return comp.X == X && comp.Y == Y;
        }

        public override int GetHashCode() => unchecked(X ^ Y);

        public override string ToString() => "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) + "}";
    }
}