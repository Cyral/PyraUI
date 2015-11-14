using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;

namespace Pyratron.UI.Types
{
    /// <summary>
    /// The thickness around a Thickness. (Such as margin and padding).
    /// </summary>
    public struct Thickness
    {
        public static readonly Thickness None = new Thickness();

        public Thickness(int left, int top, int right, int bottom)
        { 
            if (left < 0 || top < 0 || right < 0 || bottom < 0)
                throw new ArgumentOutOfRangeException("Values must be greater than 0.");
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Thickness(int veritical, int horizontal)
        {
            if (veritical < 0 || horizontal < 0)
                throw new ArgumentOutOfRangeException("Values must be greater than 0.");
            Top = Bottom = veritical;
            Left = Right = horizontal;
        }

        public Thickness(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Value must be greater than 0.");
            Left = Top = Right = Bottom = value;
        }

        public bool Equals(Thickness other)
            => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left;
                hashCode = (hashCode * 397) ^ Top;
                hashCode = (hashCode * 397) ^ Right;
                hashCode = (hashCode * 397) ^ Bottom;
                return hashCode;
            }
        }

        public int Left { get; }

        public int Top { get; }

        public int Right { get; }

        public int Bottom { get; }

        [Browsable(false)]
        public bool IsEmpty => Bottom == 0 && Right == 0 && Left == 0 && Top == 0;

        public override bool Equals(object obj)
        {
            if (!(obj is Thickness))
                return false;

            var comp = (Thickness) obj;

            return (comp.Left == Left) &&
                   (comp.Top == Top) &&
                   (comp.Right == Right) &&
                   (comp.Bottom == Bottom);
        }

        public static bool operator ==(Thickness left, Thickness right) => left.Equals(right);

        public static bool operator !=(Thickness left, Thickness right) => !(left == right);

        public static Thickness operator +(Thickness left, Thickness right)
            =>
                new Thickness(left.Left + right.Left, left.Top + right.Top,
                    left.Right + right.Right, left.Bottom + right.Bottom);

        /// <summary>
        /// The combination of the top and bottom thickness.
        /// </summary>
        public int Height => Top + Bottom;

        /// <summary>
        /// The combination of the left and right thickness.
        /// </summary>
        public int Width => Left + Right;

        public double Max => Math.Max(Math.Max(Left, Right), Math.Max(Top, Bottom));

        /// <summary>
        /// Add the left and top of the rectangle to the left and top of the thickness.
        /// </summary>
        public Thickness Offset(Thickness rectangle) => new Thickness(rectangle.Left + Left, rectangle.Top + Top, Right, Bottom);

        public static implicit operator Thickness(int value) => new Thickness(value);
        public static implicit operator Point(Thickness thickness) => new Point(thickness.Left, thickness.Top);

        public static Size operator +(Size size, Thickness thickness) => new Size(size.Width + thickness.Left + thickness.Right, size.Height + thickness.Top + thickness.Bottom);

        public override string ToString()
            =>
                "Top=" + Left.ToString(CultureInfo.CurrentCulture) + ",Bottom=" +
                Top.ToString(CultureInfo.CurrentCulture) +
                ",Left=" + Right.ToString(CultureInfo.CurrentCulture) +
                ",Right=" + Bottom.ToString(CultureInfo.CurrentCulture);

        public static explicit operator Thickness(string value)
        {
            var parts = value.Split(',');
            switch (parts.Length)
            {
                case 1:
                    return new Thickness(int.Parse(value));
                case 2:
                    return new Thickness(int.Parse(parts[1]), int.Parse(parts[0]));
                case 4:
                    return new Thickness(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
            }
            throw new ArgumentException();
        }
    }
}