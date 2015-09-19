using System.Globalization;

namespace Pyratron.UI.Types
{
    public struct Size
    {
        public static readonly Size Zero = new Size();

        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double Width { get; set; }

        public double Height { get; set; }

        public static bool operator ==(Size left, Size right)
            => Equals(left.Width, right.Width) && Equals(left.Height, right.Height);

        public static bool operator !=(Size left, Size right) => !(left == right);

        public static Rectangle operator +(Thickness thickness, Size size) => ((Rectangle) thickness).Extend(size);

        public static Size operator +(Size a, Size b) => new Size(a.Width + b.Width, a.Height = b.Height);

        public override bool Equals(object obj)
        {
            if (!(obj is Size)) return false;
            var comp = (Size) obj;
            return Equals(comp.Width, Width) && Equals(comp.Height, Height);
        }

        public bool Equals(Size other)
        {
            return Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
            }
        }

        public override string ToString() => "{Width=" + Width.ToString(CultureInfo.CurrentCulture) + ",Height=" +
                                             Height.ToString(CultureInfo.CurrentCulture) + "}";

        /// <summary>
        /// Combines the left and top of a thickness with the width and height of a size to produce a rectangle.
        /// </summary>
        public Rectangle Combine(Thickness thickness)
            => new Rectangle(thickness.Left, thickness.Top, (int) Width, (int) Height);

        /// <summary>
        /// Combines the X and Y of a point with the width and height of a size to produce a rectangle.
        /// </summary>
        public Rectangle Combine(Point point) => new Rectangle(point.X, point.Y, (int) Width, (int) Height);

        /// <summary>
        /// Extend the size to include the specified thickness.
        /// </summary>
        public Rectangle Extend(Thickness thickness)
            =>
                new Rectangle(0, 0, thickness.Right + thickness.Left + (int) Width,
                    thickness.Bottom + thickness.Top + (int) Height);

        /// <summary>
        /// Remove the specified thickness from the size.
        /// </summary>
        public Size Remove(Thickness thickness)
            => new Size(Width - thickness.Left - thickness.Right, Height - thickness.Top - thickness.Bottom);
    }
}