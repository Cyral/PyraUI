using System.ComponentModel;
using System.Globalization;

namespace Pyratron.UI.Types
{
    public struct Rectangle
    {
        public static readonly Rectangle Empty = new Rectangle();

        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(Point location, double width, double height)
        {
            X = location.X;
            Y = location.Y;
            Width = width;
            Height = height;
        }

        public Size Size => new Size(Width, Height);

        public double X { get; set; }
    
        public double Y { get; set; }
         
        public double Width { get; set; }
    
        public double Height { get; set; }

        [Browsable(false)]
        public double Left => X;

        [Browsable(false)]
        public double Top => Y;

        [Browsable(false)]
        public double Right => X + Width;

        [Browsable(false)]
        public double Bottom => Y + Height;

        [Browsable(false)]
        public bool IsEmpty => Height == 0 && Width == 0 && X == 0 && Y == 0;

        [Browsable(false)]
        public Point TopLeft => new Point(Left, Top);


        [Browsable(false)]
        public Point BottomLeft => new Point(Left, Bottom);


        [Browsable(false)]
        public Point TopRight => new Point(Right, Top);


        [Browsable(false)]
        public Point BottomRight => new Point(Right, Bottom);

        public static implicit operator Rectangle(Thickness thickness)
            =>
                new Rectangle(thickness.Left, thickness.Top, thickness.Right - thickness.Left,
                    thickness.Bottom - thickness.Top);

        public bool Contains(double x, double y) => X <= x &&
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