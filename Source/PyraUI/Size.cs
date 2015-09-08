using System.ComponentModel;
using System.Globalization;

namespace PyraUI
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

        public Rectangle Offset(Thickness margin) => new Rectangle(margin.Left, margin.Top, Width, Height);

        public Rectangle Offset(Point point) => new Rectangle(point.X, point.Y, Width, Height);

        public Rectangle Extend(Thickness margin) => new Rectangle(0, 0, margin.Right + margin.Left + Width, margin.Bottom + margin.Top + Height);
    }
}