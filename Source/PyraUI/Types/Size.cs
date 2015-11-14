using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Pyratron.UI.Types
{
    /// <summary>
    /// Represents a size of a 2D rectangle.
    /// </summary>
    [DebuggerDisplay("{Width} x {Height}")]
    public struct Size
    {
        public static readonly Size Zero = new Size();

        public static readonly Size Infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);

        public static readonly Size NaN = new Size(double.NaN, double.NaN);

        public Size(double width, double height)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Size must be greater than or equal to 0.");
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Size must be greater than or equal to 0.");
            Width = width;
            Height = height;
        }

        public double Width { get; set; }

        public double Height { get; set; }

        [Browsable(false)]
        public bool IsEmpty => IsWidthEmpty && IsHeightEmpty;

        [Browsable(false)]
        public bool IsWidthEmpty => Width.IsClose(0);

        [Browsable(false)]
        public bool IsHeightEmpty => Height.IsClose(0);

        public static bool operator ==(Size left, Size right)
            => Equals(left, right);

        public static bool operator !=(Size left, Size right) => !(left == right);

        public static Rectangle operator +(Thickness thickness, Size size) => ((Rectangle) thickness).Extend(size);

        public static Size operator +(Size a, Size b) => new Size(a.Width + b.Width, a.Height + b.Height);

        public static Size operator -(Size a, Size b) => new Size(a.Width - b.Width, a.Height - b.Height);

        public static Size operator *(Size size, double factor) => new Size(size.Width * factor, size.Width * factor);

        public static Size operator /(Size size, double factor) => new Size(size.Height / factor, size.Height / factor);

        public bool Equals(Size other) => Width.Equals(other.Width) && Height.Equals(other.Height);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Size && Equals((Size) obj);
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
        /// Determines if the size is equal to another size within margin of error (because of floating point precision).
        /// </summary>
        public bool IsClose(Size size) => Width.IsClose(size.Width) && Height.IsClose(size.Height);


        /// <summary>
        /// If the width or height is NaN or 0, fall back to another value.
        /// </summary>
        public Size Fallback(Size fallback)
        {
            return new Size(
                double.IsNaN(Width) || Width.IsClose(0) ? fallback.Width : Width,
                double.IsNaN(Height) || Height.IsClose(0) ? fallback.Height : Height);
        }

        /// <summary>
        /// Combines the X and Y of a point with the width and height of a size to produce a rectangle.
        /// </summary>
        public Rectangle Combine(Point point) => new Rectangle(point.X, point.Y, Width, Height);

        /// <summary>
        /// Extend the rectangle to include the specified thickness.
        /// </summary>
        public Rectangle Extend(Thickness thickness)
            =>
                new Rectangle(0, 0, thickness.Right + thickness.Left + Width,
                    thickness.Bottom + thickness.Top + Height);

        /// <summary>
        /// Clamp the size between a lower and upper bound.
        /// </summary>
        public Size Clamp(Size min, Size max)
        {
            if (min.Width > max.Width || min.Height > max.Height)
                throw new ArgumentException("Minimum must be less than maximum.");

            return Max(min).Min(max);
        }

        /// <summary>
        /// Return the smaller size of this size and the specified size.
        /// </summary>
        public Size Min(Size size)
        {
            double width, height;
            if (IsWidthEmpty)
                width = size.Width;
            else
                width = size.IsWidthEmpty ? Width : Math.Min(Width, size.Width);

            if (IsHeightEmpty)
                height = size.Height;
            else
                height = size.IsHeightEmpty ? Height : Math.Min(Height, size.Height);

            return new Size(width, height);
        }

        /// <summary>
        /// Return the larger size of this size and the specified size.
        /// </summary>
        public Size Max(Size size)
        {
            double width, height;
            if (IsWidthEmpty)
                width = size.Width;
            else
                width = size.IsWidthEmpty ? Width : Math.Max(Width, size.Width);

            if (IsHeightEmpty)
                height = size.Height;
            else
                height = size.IsHeightEmpty ? Height : Math.Max(Height, size.Height);

            return new Size(width, height);
        }


        /// <summary>
        /// Remove the specified thickness from the size.
        /// </summary>
        public Size Remove(Thickness thickness)
            =>
                new Size(Math.Max(0, Width - thickness.Left - thickness.Right),
                    Math.Max(0, Height - thickness.Top - thickness.Bottom));

        /// <summary>
        /// Add the specified thickness to the size.
        /// </summary>
        public Size Add(Thickness thickness)
            =>
                new Size(Math.Max(0, Width + thickness.Left + thickness.Right),
                    Math.Max(0, Height + thickness.Top + thickness.Bottom));
    }
}