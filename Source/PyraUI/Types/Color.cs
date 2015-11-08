using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Pyratron.UI.Types
{
    /// <summary>
    /// Represents an RGBA color.
    /// </summary>
    [DebuggerDisplay("{R}, {G}, {B}, {A}")]
    public struct Color
    {
        /// <summary>
        /// Collection of defined named colors. (Red, Green, etc.)
        /// </summary>
        public static ReadOnlyDictionary<string, Color> Colors { get; }

        internal long Value { get; }

        public byte A => (byte) (Value >> 24);

        public byte R => (byte) (Value >> 16);

        public byte G => (byte) (Value >> 8);

        public byte B => (byte) Value;

        public Color(float red, float green, float blue) : this(red, green, blue, 1f)
        {
        }

        /// <summary>
        /// Create a color based on RGBA values. (0 to 1)
        /// </summary>
        public Color(float red, float green, float blue, float alpha)
        {
            if (!red.InRange(0, 1))
                throw new ArgumentOutOfRangeException(nameof(red), "Must be between 0 and 1.");
            if (!green.InRange(0, 1))
                throw new ArgumentOutOfRangeException(nameof(green), "Must be between 0 and 1.");
            if (!blue.InRange(0, 1))
                throw new ArgumentOutOfRangeException(nameof(blue), "Must be between 0 and 1.");
            if (!alpha.InRange(0, 1))
                throw new ArgumentOutOfRangeException(nameof(alpha), "Must be between 0 and 1.");

            red *= 255;
            green *= 255;
            blue *= 255;
            alpha *= 255;
            Value = (int) ((uint) alpha << 24) + (((byte) red) << 16) + (((byte) green) << 8) + ((byte) blue);
        }

        public Color(int argb)
        {
            Value = argb;
        }

        /// <summary>
        /// Create a color based on RGBA values. (0 to 255)
        /// </summary>
        public Color(int red, int green, int blue, int alpha = 255)
        {
            if (!red.InRange(0, 255))
                throw new ArgumentOutOfRangeException(nameof(red), "Must be between 0 and 255.");
            if (!green.InRange(0, 255))
                throw new ArgumentOutOfRangeException(nameof(green), "Must be between 0 and 255.");
            if (!blue.InRange(0, 255))
                throw new ArgumentOutOfRangeException(nameof(blue), "Must be between 0 and 255.");
            if (!alpha.InRange(0, 255))
                throw new ArgumentOutOfRangeException(nameof(alpha), "Must be between 0 and 255.");

            Value = (int) ((uint) alpha << 24) + (red << 16) + (green << 8) + blue;
        }

        public Color(int alpha, Color color) : this(alpha, color.R, color.G, color.B)
        {
        }

        static Color()
        {
            // Use reflection to find the name of the predefined colors.
            var colors = new Dictionary<string, Color>();
            var props = typeof (Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in props)
            {
                if (prop.PropertyType != typeof (Color)) continue;
                var color = (Color) prop.GetValue(null, null);
                colors.Add(prop.Name, color);
            }
            Colors = new ReadOnlyDictionary<string, Color>(colors);
        }

        /// <summary>
        /// Multiply a color by a float value to return a more transparent version of the color.
        /// </summary>
        public static Color operator *(Color baseColor, float value)
        {
            return new Color(
                (int) Math.Round(value * baseColor.R),
                (int) Math.Round(value * baseColor.G),
                (int) Math.Round(value * baseColor.B),
                (int) Math.Round(value * baseColor.A));
        }

        #region Named Colors

        public static Color Transparent { get; } = new Color(0, 0, 0, 0);
        public static Color AliceBlue { get; } = new Color(240, 248, 255);
        public static Color AntiqueWhite { get; } = new Color(250, 235, 215);
        public static Color Aqua { get; } = new Color(0, 255, 255);
        public static Color Aquamarine { get; } = new Color(127, 255, 212);
        public static Color Azure { get; } = new Color(240, 255, 255);
        public static Color Beige { get; } = new Color(245, 245, 220);
        public static Color Bisque { get; } = new Color(255, 228, 196);
        public static Color Black { get; } = new Color(0, 0, 0);
        public static Color BlanchedAlmond { get; } = new Color(255, 235, 205);
        public static Color Blue { get; } = new Color(0, 0, 255);
        public static Color BlueViolet { get; } = new Color(138, 43, 226);
        public static Color Brown { get; } = new Color(165, 42, 42);
        public static Color BurlyWood { get; } = new Color(222, 184, 135);
        public static Color CadetBlue { get; } = new Color(95, 158, 160);
        public static Color Chartreuse { get; } = new Color(127, 255, 0);
        public static Color Chocolate { get; } = new Color(210, 105, 30);
        public static Color Coral { get; } = new Color(255, 127, 80);
        public static Color CornflowerBlue { get; } = new Color(100, 149, 237);
        public static Color Cornsilk { get; } = new Color(255, 248, 220);
        public static Color Crimson { get; } = new Color(220, 20, 60);
        public static Color Cyan { get; } = new Color(0, 255, 255);
        public static Color DarkBlue { get; } = new Color(0, 0, 139);
        public static Color DarkCyan { get; } = new Color(0, 139, 139);
        public static Color DarkGoldenrod { get; } = new Color(184, 134, 11);
        public static Color DarkGray { get; } = new Color(169, 169, 169);
        public static Color DarkGreen { get; } = new Color(0, 100, 0);
        public static Color DarkKhaki { get; } = new Color(189, 183, 107);
        public static Color DarkMagenta { get; } = new Color(139, 0, 139);
        public static Color DarkOliveGreen { get; } = new Color(85, 107, 47);
        public static Color DarkOrange { get; } = new Color(255, 140, 0);
        public static Color DarkOrchid { get; } = new Color(153, 50, 204);
        public static Color DarkRed { get; } = new Color(139, 0, 0);
        public static Color DarkSalmon { get; } = new Color(233, 150, 122);
        public static Color DarkSeaGreen { get; } = new Color(143, 188, 139);
        public static Color DarkSlateBlue { get; } = new Color(72, 61, 139);
        public static Color DarkSlateGray { get; } = new Color(47, 79, 79);
        public static Color DarkTurquoise { get; } = new Color(0, 206, 209);
        public static Color DarkViolet { get; } = new Color(148, 0, 211);
        public static Color DeepPink { get; } = new Color(255, 20, 147);
        public static Color DeepSkyBlue { get; } = new Color(0, 191, 255);
        public static Color DimGray { get; } = new Color(105, 105, 105);
        public static Color DodgerBlue { get; } = new Color(30, 144, 255);
        public static Color Firebrick { get; } = new Color(178, 34, 34);
        public static Color FloralWhite { get; } = new Color(255, 250, 240);
        public static Color ForestGreen { get; } = new Color(34, 139, 34);
        public static Color Fuchsia { get; } = new Color(255, 0, 255);
        public static Color Gainsboro { get; } = new Color(220, 220, 220);
        public static Color GhostWhite { get; } = new Color(248, 248, 255);
        public static Color Gold { get; } = new Color(255, 215, 0);
        public static Color Goldenrod { get; } = new Color(218, 165, 32);
        public static Color Gray { get; } = new Color(128, 128, 128);
        public static Color Green { get; } = new Color(0, 128, 0);
        public static Color GreenYellow { get; } = new Color(173, 255, 47);
        public static Color Honeydew { get; } = new Color(240, 255, 240);
        public static Color HotPink { get; } = new Color(255, 105, 180);
        public static Color IndianRed { get; } = new Color(205, 92, 92);
        public static Color Indigo { get; } = new Color(75, 0, 130);
        public static Color Ivory { get; } = new Color(255, 255, 240);
        public static Color Khaki { get; } = new Color(240, 230, 140);
        public static Color Lavender { get; } = new Color(230, 230, 250);
        public static Color LavenderBlush { get; } = new Color(255, 240, 245);
        public static Color LawnGreen { get; } = new Color(124, 252, 0);
        public static Color LemonChiffon { get; } = new Color(255, 250, 205);
        public static Color LightBlue { get; } = new Color(173, 216, 230);
        public static Color LightCoral { get; } = new Color(240, 128, 128);
        public static Color LightCyan { get; } = new Color(224, 255, 255);
        public static Color LightGoldenrodYellow { get; } = new Color(250, 250, 210);
        public static Color LightGray { get; } = new Color(211, 211, 211);
        public static Color LightGreen { get; } = new Color(144, 238, 144);
        public static Color LightPink { get; } = new Color(255, 182, 193);
        public static Color LightSalmon { get; } = new Color(255, 160, 122);
        public static Color LightSeaGreen { get; } = new Color(32, 178, 170);
        public static Color LightSkyBlue { get; } = new Color(135, 206, 250);
        public static Color LightSlateGray { get; } = new Color(119, 136, 153);
        public static Color LightSteelBlue { get; } = new Color(176, 196, 222);
        public static Color LightYellow { get; } = new Color(255, 255, 224);
        public static Color Lime { get; } = new Color(0, 255, 0);
        public static Color LimeGreen { get; } = new Color(50, 205, 50);
        public static Color Linen { get; } = new Color(250, 240, 230);
        public static Color Magenta { get; } = new Color(255, 0, 255);
        public static Color Maroon { get; } = new Color(128, 0, 0);
        public static Color MediumAquamarine { get; } = new Color(102, 205, 170);
        public static Color MediumBlue { get; } = new Color(0, 0, 205);
        public static Color MediumOrchid { get; } = new Color(186, 85, 211);
        public static Color MediumPurple { get; } = new Color(147, 112, 219);
        public static Color MediumSeaGreen { get; } = new Color(60, 179, 113);
        public static Color MediumSlateBlue { get; } = new Color(123, 104, 238);
        public static Color MediumSpringGreen { get; } = new Color(0, 250, 154);
        public static Color MediumTurquoise { get; } = new Color(72, 209, 204);
        public static Color MediumVioletRed { get; } = new Color(199, 21, 133);
        public static Color MidnightBlue { get; } = new Color(25, 25, 112);
        public static Color MintCream { get; } = new Color(245, 255, 250);
        public static Color MistyRose { get; } = new Color(255, 228, 225);
        public static Color Moccasin { get; } = new Color(255, 228, 181);
        public static Color NavajoWhite { get; } = new Color(255, 222, 173);
        public static Color Navy { get; } = new Color(0, 0, 128);
        public static Color OldLace { get; } = new Color(253, 245, 230);
        public static Color Olive { get; } = new Color(128, 128, 0);
        public static Color OliveDrab { get; } = new Color(107, 142, 35);
        public static Color Orange { get; } = new Color(255, 165, 0);
        public static Color OrangeRed { get; } = new Color(255, 69, 0);
        public static Color Orchid { get; } = new Color(218, 112, 214);
        public static Color PaleGoldenrod { get; } = new Color(238, 232, 170);
        public static Color PaleGreen { get; } = new Color(152, 251, 152);
        public static Color PaleTurquoise { get; } = new Color(175, 238, 238);
        public static Color PaleVioletRed { get; } = new Color(219, 112, 147);
        public static Color PapayaWhip { get; } = new Color(255, 239, 213);
        public static Color PeachPuff { get; } = new Color(255, 218, 185);
        public static Color Peru { get; } = new Color(205, 133, 63);
        public static Color Pink { get; } = new Color(255, 192, 203);
        public static Color Plum { get; } = new Color(221, 160, 221);
        public static Color PowderBlue { get; } = new Color(176, 224, 230);
        public static Color Purple { get; } = new Color(128, 0, 128);
        public static Color Red { get; } = new Color(255, 0, 0);
        public static Color RosyBrown { get; } = new Color(188, 143, 143);
        public static Color RoyalBlue { get; } = new Color(65, 105, 225);
        public static Color SaddleBrown { get; } = new Color(139, 69, 19);
        public static Color Salmon { get; } = new Color(250, 128, 114);
        public static Color SandyBrown { get; } = new Color(244, 164, 96);
        public static Color SeaGreen { get; } = new Color(46, 139, 87);
        public static Color SeaShell { get; } = new Color(255, 245, 238);
        public static Color Sienna { get; } = new Color(160, 82, 45);
        public static Color Silver { get; } = new Color(192, 192, 192);
        public static Color SkyBlue { get; } = new Color(135, 206, 235);
        public static Color SlateBlue { get; } = new Color(106, 90, 205);
        public static Color SlateGray { get; } = new Color(112, 128, 144);
        public static Color Snow { get; } = new Color(255, 250, 250);
        public static Color SpringGreen { get; } = new Color(0, 255, 127);
        public static Color SteelBlue { get; } = new Color(70, 130, 180);
        public static Color Tan { get; } = new Color(210, 180, 140);
        public static Color Teal { get; } = new Color(0, 128, 128);
        public static Color Thistle { get; } = new Color(216, 191, 216);
        public static Color Tomato { get; } = new Color(255, 99, 71);
        public static Color TransparentBlack { get; } = new Color(0, 0, 0);
        public static Color TransparentWhite { get; } = new Color(255, 255, 255);
        public static Color Turquoise { get; } = new Color(64, 224, 208);
        public static Color Violet { get; } = new Color(238, 130, 238);
        public static Color Wheat { get; } = new Color(245, 222, 179);
        public static Color White { get; } = new Color(255, 255, 255);
        public static Color WhiteSmoke { get; } = new Color(245, 245, 245);
        public static Color Yellow { get; } = new Color(255, 255, 0);
        public static Color YellowGreen { get; } = new Color(154, 205, 50);

        #endregion

        public override string ToString() => $"R={R}, G={G}, B={B}, A={A}";

        public static bool operator ==(Color left, Color right) => left.Equals(right);

        public static bool operator !=(Color left, Color right) => !Equals(left, right);

        public bool Equals(Color other) => Value == other.Value;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color && Equals((Color)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static explicit operator Color(string value)
        {
            var parts = value.Split(',');
            int r, g, b;
            // Handled named (e.g. "Red") colors and hex codes.
            if (parts.Length == 1)
            {
                if (Colors.ContainsKey(parts[0]))
                    return Colors[parts[0]];
                if (parts[0].StartsWith("#") && (parts[0].Length == 7 || parts[0].Length == 9))
                {
                    var hex = parts[0].Substring(1);
                    int argb;
                    if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out argb))
                    {
                        return new Color(argb);
                    }
                }
#if DEBUG
                throw new InvalidCastException("\"" + parts[0] + "\" is not a valid named color.");
#else
                return Black;
#endif
            }
            // Handle ARGB or RGB colors.
            if (parts.Length == 3)
            {
                if (int.TryParse(parts[0], out r) &&
                    int.TryParse(parts[1], out g) &&
                    int.TryParse(parts[2], out b))
                    return new Color(r, g, b);
            }
            else if (parts.Length == 4)
            {
                int a;
                if (int.TryParse(parts[0], out r) &&
                    int.TryParse(parts[1], out g) &&
                    int.TryParse(parts[2], out b) &&
                    int.TryParse(parts[3], out a))
                    return new Color(r, g, b, a);
            }
#if DEBUG
            throw new InvalidCastException("Not a named color, hex color, or a valid color in R,G,B or R,G,B,A format.");
#else
            return Black;
#endif
        }
    }
}