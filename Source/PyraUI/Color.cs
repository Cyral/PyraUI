using System;
using System.ComponentModel;

namespace Pyratron.UI
{
    public struct Color
    {
        public bool Equals(Color other)
        {
            return A == other.A && R == other.R && G == other.G && B == other.B;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = A.GetHashCode();
                hashCode = (hashCode * 397) ^ R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                return hashCode;
            }
        }

        public Color(float red, float green, float blue)
        {
            red *= 255;
            green *= 255;
            blue *= 255;
            A = 255;
            R = (byte)red;
            G = (byte)green;
            B = (byte)blue;
        }

        public Color(float red, float green, float blue, float alpha)
        {
            red *= 255;
            green *= 255;
            blue *= 255;
            alpha *= 255;
            A = (byte)alpha;
            R = (byte)red;
            G = (byte)green;
            B = (byte)blue;
        }

        public Color(int red, int green, int blue, int alpha = 255)
        {
            A = (byte)alpha;
            R = (byte)red;
            G = (byte)green;
            B = (byte)blue;
        }

        public Color(int alpha, Color color) : this(alpha, color.R, color.G, color.B)
        {
        }

        public static readonly Color Empty = new Color();

        public static bool operator ==(Color left, Color right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !Equals(left, right);
        }

        public static Color operator *(Color baseColor, float value)
        {
            return new Color(
                (int) Math.Round(value * baseColor.R),
                (int) Math.Round(value * baseColor.G),
                (int) Math.Round(value * baseColor.B),
                (int) Math.Round(value * baseColor.A));
        }

        public byte A { get; }
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Color))
                return false;
            var c = (Color) obj;
            return this == c;
        }

        public override string ToString()
        {
            return $"R={R}, G={G}, B={B}, A={A}";
        }

        public static Color Transparent { get; } = new Color(0, 0, 0, 0);

        public static Color White { get; } = new Color(255, 255, 255, 255);

        public static Color Black { get; } = new Color(0, 0, 0, 255);

        public static Color Red { get; } = new Color(255, 0, 0, 255);

        public static Color Green { get; } = new Color(0, 255, 0, 255);

        public static Color Blue { get; } = new Color(0, 0, 255, 255);
    }
}