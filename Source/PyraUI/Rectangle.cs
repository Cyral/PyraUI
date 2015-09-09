using System.ComponentModel;
using System.Globalization;

namespace PyraUI
{
    public struct Rectangle
    {
        public static readonly Rectangle Empty = new Rectangle();

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(Point location, int width, int height)
        {
            X = location.X;
            Y = location.Y;
            Width = width;
            Height = height;
        }

        public Size Size => new Size(Width, Height);

        public bool Equals(Rectangle other)
            => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                return hashCode;
            }
        }

        public int X { get; }

        public int Y { get; }

        public int Width { get; }

        public int Height { get; }

        [Browsable(false)]
        public int Left => X;

        [Browsable(false)]
        public int Top => Y;

        [Browsable(false)]
        public int Right => X + Width;

        [Browsable(false)]
        public int Bottom => Y + Height;

        [Browsable(false)]
        public bool IsEmpty => Height == 0 && Width == 0 && X == 0 && Y == 0;

        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
                return false;

            var comp = (Rectangle) obj;

            return (comp.X == X) &&
                   (comp.Y == Y) &&
                   (comp.Width == Width) &&
                   (comp.Height == Height);
        }

        public static bool operator ==(Rectangle left, Rectangle right) => (left.X == right.X
                                                                            && left.Y == right.Y
                                                                            && left.Width == right.Width
                                                                            && left.Height == right.Height);

        public static bool operator !=(Rectangle left, Rectangle right) => !(left == right);

        public static implicit operator Rectangle(Thickness thickness)
            =>
                new Rectangle(thickness.Left, thickness.Top, thickness.Right - thickness.Left,
                    thickness.Bottom - thickness.Top);

        public bool Contains(int x, int y) => X <= x &&
                                              x < X + Width &&
                                              Y <= y &&
                                              y < Y + Height;

        public bool Contains(Point pt) => Contains(pt.X, pt.Y);

        public bool Contains(Rectangle rect) => (X <= rect.X) &&
                                                ((rect.X + rect.Width) <= (X + Width)) &&
                                                (Y <= rect.Y) &&
                                                ((rect.Y + rect.Height) <= (Y + Height));

        public bool Intersects(Rectangle rect) => (rect.X < X + Width) &&
                                                  (X < (rect.X + rect.Width)) &&
                                                  (rect.Y < Y + Height) &&
                                                  (Y < rect.Y + rect.Height);

        public override string ToString()
            => "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) +
               ",Width=" + Width.ToString(CultureInfo.CurrentCulture) +
               ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}";

        /// <summary>
        /// Extend the rectangle dimensions by the specified size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Rectangle Extend(Size size) => new Rectangle(X, Y, Width + size.Width, Height + size.Height);

        /// <summary>
        /// Offset the rectangle by the specified point. (Add to the X and Y)
        /// </summary>
        public Rectangle Offset(Point position) => new Rectangle(X + position.X, Y + position.Y, Width, Height);

        /// <summary>
        /// Extend the size to include the specified thickness.
        /// </summary>
        public Rectangle Extend(Thickness thickness)
            => new Rectangle(0, 0, thickness.Right + thickness.Left + Width, thickness.Bottom + thickness.Top + Height);
    }
}