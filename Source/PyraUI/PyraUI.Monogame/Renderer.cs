using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pyratron.UI.Brushes;
using Pyratron.UI.Types;
using Color = Pyratron.UI.Types.Color;
using Point = Pyratron.UI.Types.Point;
using Rectangle = Pyratron.UI.Types.Rectangle;

namespace Pyratron.UI.Monogame
{
    public class Renderer : UI.Renderer
    {
        private readonly Primitives primitives;
        private readonly Manager manager;
        private readonly Texture2D pixel;

        public Renderer(Manager manager)
        {
            this.manager = manager;

            primitives = new Primitives(manager.SpriteBatch.GraphicsDevice);

            // Create 1x1 texture for rendering rectangles.
            pixel = new Texture2D(manager.SpriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] {Color.White});
        }

        public override void BeginDraw()
        {
          manager.SpriteBatch.Begin();
        }

        public override void EndDraw()
        {
          manager.SpriteBatch.End();
        }

        public override void DrawTexture(string name, Rectangle rectangle, ColorBrush brush, Rectangle bounds)
        {
            
            var rect = new Microsoft.Xna.Framework.Rectangle((int) rectangle.X, (int) rectangle.Y, (int) rectangle.Width,
                (int) rectangle.Height);
            var texture = GetTexture(name);
            var col = new Microsoft.Xna.Framework.Color(brush.Color.R, brush.Color.G, brush.Color.B, brush.Color.A);
            manager.SpriteBatch.Draw(texture, rect, col * (brush.Color.A / 255f));
        }

        public override void DrawString(string text, Point point, ColorBrush brush, int size, FontStyle style, Rectangle bounds,
            bool ignoreFormatting = false)
        {

       
            var pos = new Vector2((int) Math.Round(point.X), (int) Math.Round(point.Y));
            var closest = GetClosestFontSize(size);

            var parts = ParseFormattedText(text, brush.Color, style);

            if (!ignoreFormatting)
            {
                foreach (var part in parts)
                {
                    var font = GetFont(Path.Combine(part.Style.ToString(), closest.ToString()));
                    var measure = MeasureTextNoTrim(part.Text, size, part.Style);
                    var col = new Microsoft.Xna.Framework.Color(part.Color.R, part.Color.G, part.Color.B, brush.Color.A);
                    manager.SpriteBatch.DrawString(font, part.Text, pos,
                        col * (brush.Color.A / 255f), 0,
                        Vector2.Zero, size / (float) closest, SpriteEffects.None, 0);
                    pos = new Vector2(pos.X + (float) measure.Width, pos.Y);
                }
            }
            else
            {
                var font = GetFont(Path.Combine(style.ToString(), closest.ToString()));
                var col = new Microsoft.Xna.Framework.Color(brush.Color.R, brush.Color.G, brush.Color.B, brush.Color.A);
                manager.SpriteBatch.DrawString(font, text, pos,
                    col * (brush.Color.A / 255f), 0,
                    Vector2.Zero, size / (float) closest, SpriteEffects.None, 0);
            }
        }

        public override void DrawRectangle(Rectangle area, Brush brush, double radius, Rectangle bounds)
        {
            area = area.FitToBounds(bounds);
            if (area != Rectangle.Empty)
            {
                var rect = new Microsoft.Xna.Framework.Rectangle((int) area.X, (int) area.Y, (int) area.Width,
                    (int) area.Height);
                var color = brush as ColorBrush;
                if (color != null)
                {
                    var col = new Microsoft.Xna.Framework.Color(color.Color.R, color.Color.G, color.Color.B, color.Color.A);
               
                    primitives.DrawRectangle(rect, col, true, (float)radius);
                }
            }
        }

        public override void FillRectangle(Rectangle area, Brush brush, double radius, Rectangle bounds)
        {
            area = area.FitToBounds(bounds);
            if (area != Rectangle.Empty)
            {
                var rect = new Microsoft.Xna.Framework.Rectangle((int)area.X, (int)area.Y, (int)area.Width,
                    (int)area.Height);
                var color = brush as ColorBrush;
                if (color != null)
                {
                    var col = new Microsoft.Xna.Framework.Color(color.Color.R, color.Color.G, color.Color.B, color.Color.A);
                    primitives.DrawRectangle(rect, col, true, (float)radius);
                }
            }
        }

      

        public override Size MeasureText(string text, int size, FontStyle style)
        {
            // Remove any extra spaces, possibly created by the space character for invalid characters not included in the spritefont.
            text = text.Trim();
            var parts = ParseFormattedText(text, Color.Transparent, style);

            double w = 0, h = 0;
            foreach (var part in parts)
            {
                var measure = MeasureTextNoTrim(part.Text, size, part.Style);
                w += measure.Width;
                h = Math.Max(h, measure.Height);
            }
            return new Size(w, h);
        }

        /// <summary>
        /// Parse formatted text into separate parts.
        /// </summary>
        /// <remarks>
        /// PyraUI (Monogame) supports bold and italic formatting:
        /// Italic Text using _Example_
        /// Bold Text using *Example*
        /// Nested formatting is not supported as it would increase the file size (more fonts).
        /// Colors can be done using [ColorName or #Hex]Text[]
        /// A color name (e.g. "Red") or hex code is put into brackets to signify the start of a new color.
        /// Empty brackets show the current color should be removed, and the last used color will be applied.
        /// (Like a stack)
        /// </remarks>
        private static IEnumerable<TextRenderProperties> ParseFormattedText(string text, Color color, FontStyle style)
        {
            var parts = new List<TextRenderProperties>();
            var sb = new StringBuilder();
            bool inBold = false, inItalic = false;
            var inColor = false;
            var colors = new Stack<Color>();
            colors.Push(color);
            var ignoreColor = color == Color.Transparent; // Transparent signals not to parse colors.
            for (var i = 0; i < text.Length; i++)
            {
                var character = text[i];
                // If we are in a color block, append the characters to the color name, or try and parse the name/color.
                if (inColor)
                {
                    if (character == ']')
                    {
                        if (!ignoreColor)
                        {
                            var colorStr = sb.ToString();
                            // Remove current color if the brackets were empty.
                            if (string.IsNullOrEmpty(colorStr))
                                colors.Pop();
                            else
                            {
                                var newColor = (Color) colorStr;
                                colors.Push(newColor);
                            }
                        }
                                                    inColor = false;
                        sb.Clear();
                    }
                    else
                        sb.Append(character);
                }
                else
                {
                    // Start/end bold text.
                    if (character == '*')
                    {
                        parts.Add(new TextRenderProperties(sb.ToString(), colors.Peek(), inBold ? FontStyle.Bold : style));
                        inBold = !inBold;
                        sb.Clear();
                    }
                    // Start/end italic text.
                    else if (character == '_')
                    {
                        parts.Add(new TextRenderProperties(sb.ToString(), colors.Peek(),
                            inItalic ? FontStyle.Italic : style));
                        inItalic = !inItalic;
                        sb.Clear();
                    }
                    // Start a new color block.
                    else if (character == '[')
                    {
                        parts.Add(new TextRenderProperties(sb.ToString(), colors.Peek(),
                            inBold ? FontStyle.Bold : inItalic ? FontStyle.Italic : style));
                        inColor = true;
                        sb.Clear();
                    }
                    else
                        sb.Append(character);
                }
            }
            parts.Add(new TextRenderProperties(sb.ToString(), colors.Peek(), inBold ? FontStyle.Bold : style));
            return parts;
        }

        private Size MeasureTextNoTrim(string text, int size, FontStyle style)
        {
            var closest = GetClosestFontSize(size);
            var scale = size / (float) closest;
            var measure = GetFont(Path.Combine(style.ToString(), closest.ToString())).MeasureString(text);
            return new Size((int) (Math.Round(measure.X * scale)), (int) (Math.Round(measure.Y * scale)));
        }

        /// <summary>
        /// Find the font size that is closest to the specified value.
        /// </summary>
        /// <remarks>
        /// MonoGame does not support vector fonts, so many common font sizes are included with PyraUI.
        /// If a size is not included, the next highest size will be chosen and scaled down.
        /// </remarks>
        private int GetClosestFontSize(int fontsize)
        {
            var ret = manager.FontSizes[manager.FontSizes.Length - 1];
            foreach (var size in manager.FontSizes.Where(size => size >= fontsize))
            {
                ret = size;
                break;
            }
            return ret;
        }

        private Texture2D GetTexture(string name) => manager.Skin.Textures[name] as Texture2D;
        private SpriteFont GetFont(string name) => manager.Skin.Fonts[name] as SpriteFont;

        private class TextRenderProperties
        {
            public string Text { get; }
            public Color Color { get; }
            public FontStyle Style { get; }

            public TextRenderProperties(string text, Color color, FontStyle style)
            {
                Text = text;
                Color = color;
                Style = style;
            }
        }
    }
}