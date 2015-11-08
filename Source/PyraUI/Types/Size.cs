using System;
using System.Diagnostics;
using System.Globalization;

namespace Pyratron.UI.Types
{
    /// <summary>
    /// Represents a size of a 2D rectangle.
    /// </summary>
    [DebuggerDisplay("{Width}x{Height}")]
    public struct Size
    {
        public static readonly Size Zero = new Size();

        public static readonly Size Infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);

        public Size(double width, double height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Size must be greater than 0.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Size must be greater than 0.");
            Width = width;
            Height = height;
        }

        public double Width { get; set; }

        public double Height { get; set; }

        public static bool operator ==(Size left, Size right)
            => Equals(left, right);

        public static bool operator !=(Size left, Size right) => !(left == right);

        public static Rectangle operator +(Thickness thickness, Size size) => ((Rectangle) thickness).Extend(size);

        public static Size operator +(Size a, Size b) => new Size(a.Width + b.Width, a.Height = b.Height);

        public static Size operator*(Size size, double factor) => new Size(size.Width * factor, size.Width * factor);

        public static Size operator /(Size size, double factor) => new Size(size.Height / factor, size.Height / factor);

        public bool Equals(Size other) => Width.Equals(other.Width) && Height.Equals(other.Height);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Size && Equals((Size)obj);
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