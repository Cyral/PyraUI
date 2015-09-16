using System.ComponentModel;
using System.Globalization;

namespace Pyratron.UI.Types
{
    public struct Size
    {
        public static readonly Size Zero = new Size();

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        [Browsable(false)]
        public bool IsZero => Width == 0 && Height == 0;

        public int Width { get; }

        public int Height { get; }

        public static bool operator ==(Size left, Size right)
            => left.Width == right.Width && left.Height == right.Height;

        public static bool operator !=(Size left, Size right) => !(left == right);

        public static Rectangle operator +(Thickness thickness, Size size) => ((Rectangle) thickness).Extend(size);

        public override bool Equals(object obj)
        {
            if (!(obj is Size)) return false;
            var comp = (Size) obj;
            return comp.Width == Width && comp.Height == Height;
        }

        public override int GetHashCode() => unchecked(Width ^ Height);

        public override string ToString() => "{Width=" + Width.ToString(CultureInfo.CurrentCulture) + ",Height=" +
                                             Height.ToString(CultureInfo.CurrentCulture) + "}";

        /// <summary>
        /// Combines the left and top of a thickness with the width and height of a size to produce a rectangle.
        /// </summary>
        public Rectangle Combine(Thickness thickness) => new Rectangle(thickness.Left, thickness.Top, Width, Height);

        /// <summary>
        /// Combines the X and Y of a point with the width and height of a size to produce a rectangle.
        /// </summary>
        public Rectangle Combine(Point point) => new Rectangle(point.X, point.Y, Width, Height);

        /// <summary>
        /// Extend the size to include the specified thickness.
        /// </summary>
        public Rectangle Extend(Thickness thickness) => new Rectangle(0, 0, thickness.Right + thickness.Left + Width, thickness.Bottom + thickness.Top + Height);

        /// <summary>
        /// Remove the specified thickness from the size.
        /// </summary>
        public Size Remove(Thickness thickness) => new Size(Width - thickness.Left - thickness.Right, Height - thickness.Top - thickness.Bottom);
    }
}