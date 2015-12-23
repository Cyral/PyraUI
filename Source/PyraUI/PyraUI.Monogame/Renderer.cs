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
using ColorXNA = Microsoft.Xna.Framework.Color;
using Point = Pyratron.UI.Types.Point;
using Rectangle = Pyratron.UI.Types.Rectangle;
using RectangleXNA = Microsoft.Xna.Framework.Rectangle;

namespace Pyratron.UI.Monogame
{
    public class Renderer : UI.Renderer
    {
        public override Size Viewport => new Size(graphics.Viewport.Width, graphics.Viewport.Height);
        private readonly AlphaTestEffect alphaTextEffect;
        private readonly Dictionary<GradientBrush, Texture2D> gradientCache;
        private readonly GraphicsDevice graphics;
        private readonly Manager manager;
        private readonly List<Rectangle> stencilAreas; 
        private readonly DepthStencilState maskStencil, renderStencil;
        private readonly Texture2D pixel;

        public Renderer(Manager manager)
        {
            // Cache of gradients.
            gradientCache = new Dictionary<GradientBrush, Texture2D>();
            stencilAreas = new List<Rectangle>();
            this.manager = manager;
            graphics = manager.SpriteBatch.GraphicsDevice;

            // Create stencils for drawing masked sprites.
            var matrix = Matrix.CreateOrthographicOffCenter(0,
                graphics.PresentationParameters.BackBufferWidth,
                graphics.PresentationParameters.BackBufferHeight,
                0, 0, 1
                );
            alphaTextEffect = new AlphaTestEffect(graphics)
            {
                Projection = matrix
            };
            maskStencil = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };
            renderStencil = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.LessEqual,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };

            // Create 1x1 texture for rendering rectangles.
            pixel = new Texture2D(manager.SpriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] {ColorXNA.White});
        }

        public override void BeginDraw()
        {
            manager.SpriteBatch.Begin(SpriteSortMode.Immediate,
                rasterizerState: manager.SpriteBatch.GraphicsDevice.RasterizerState);
        }

        public override void DrawRectangle(Rectangle area, Brush brush, Thickness thickness, double radius,
            Rectangle bounds)
        {
            SetClipArea(bounds);
            if (area != Rectangle.Empty)
            {
                if (radius > 0)
                {
                    FillRectangle(area, brush, radius, bounds);
                }
                else
                {
                    var areaXNA = area.ToXNA();
                    FillRect(new RectangleXNA(areaXNA.X,
                        areaXNA.Top, areaXNA.Width, thickness.Top), brush);
                    FillRect(new RectangleXNA(areaXNA.X,
                        areaXNA.Top + thickness.Top, thickness.Left, areaXNA.Height - thickness.Top - thickness.Bottom),
                        brush);
                    FillRect(new RectangleXNA(areaXNA.Right - thickness.Right,
                        areaXNA.Top + thickness.Top, thickness.Right, areaXNA.Height - thickness.Top - thickness.Bottom),
                        brush);
                    FillRect(new RectangleXNA(areaXNA.X,
                        areaXNA.Bottom - thickness.Bottom, areaXNA.Width, thickness.Bottom), brush);
                }
            }
            ResetClipArea();
        }

        public override void DrawString(string text, Point point, Brush brush, int size, FontStyle style,
            Rectangle bounds,
            bool ignoreFormatting = false)
        {
            SetClipArea(bounds);
            var pos = new Vector2((int) Math.Round(point.X), (int) Math.Round(point.Y));
            var closest = GetClosestFontSize(size);

            var colorBrush = brush as ColorBrush;
            var gradientBrush = brush as GradientBrush;
            var defaultColor = Color.Black;
            if (colorBrush != null)
                defaultColor = colorBrush.Color;

            if (gradientBrush != null)
            {
                UseMaskStencil(bounds);
            }

            if (!ignoreFormatting) // If formatting is enabled, parse it and render each part.
            {
                var parts = ParseFormattedText(text, defaultColor, style);
                foreach (var part in parts)
                {
                    var font = GetFont(Path.Combine(part.Style.ToString(), closest.ToString()));
                    var measure = MeasureTextNoTrim(part.Text, size, part.Style);
                    var col = new ColorXNA(part.Color.R, part.Color.G, part.Color.B, defaultColor.A);

                    manager.SpriteBatch.DrawString(font, part.Text, pos,
                        col * (defaultColor.A / 255f), 0,
                        Vector2.Zero, size / (float) closest, SpriteEffects.None, 0);
                    pos = new Vector2(pos.X + (float) measure.Width, pos.Y);
                }
            }
            else // Draw plain string
            {
                var font = GetFont(Path.Combine(style.ToString(), closest.ToString()));
                var col = new ColorXNA(defaultColor.R, defaultColor.G, defaultColor.B, defaultColor.A);
                manager.SpriteBatch.DrawString(font, text, pos,
                    col * (defaultColor.A / 255f), 0,
                    Vector2.Zero, size / (float) closest, SpriteEffects.None, 0);
            }

            // If using a gradient brush, draw a gradient over the mask.
            if (gradientBrush != null)
            {
                UseRenderStencil();
                DrawGradient(bounds, gradientBrush);
                EndStencil();
            }

            ResetClipArea();
        }

        public override void DrawTexture(string name, Rectangle rectangle, ColorBrush brush, Rectangle bounds)
        {
            var rect = new RectangleXNA((int) rectangle.X, (int) rectangle.Y, (int) rectangle.Width,
                (int) rectangle.Height);
            var texture = GetTexture(name);
            var col = new ColorXNA(brush.Color.R, brush.Color.G, brush.Color.B, brush.Color.A);
            manager.SpriteBatch.Draw(texture, rect, col);
        }

        public override void EndDraw()
        {
            manager.SpriteBatch.End();
        }

        public override void FillRectangle(Rectangle area, Brush brush, double radius, Rectangle bounds)
        {
            SetClipArea(bounds);
            if (area != Rectangle.Empty)
            {
                if (radius > 0)
                {
                    var corner = (int) Math.Round(radius);
                    var corner2 = corner * 2;
                    var areaXNA = area.ToXNA();

                    var gradientBrush = brush as GradientBrush;
                    if (gradientBrush != null)
                    {
                        UseMaskStencil(area);
                    }

                    // Draw 5 rectangles (top, bottom, left, right, center)
                    FillRect(new RectangleXNA(areaXNA.X + corner,
                        areaXNA.Top, areaXNA.Width - corner2, corner), brush);
                    FillRect(new RectangleXNA(areaXNA.X + corner,
                        areaXNA.Bottom - corner, areaXNA.Width - corner2, corner), brush);
                    FillRect(new RectangleXNA(areaXNA.X,
                        areaXNA.Top + corner, corner, areaXNA.Height - corner2), brush);
                    FillRect(new RectangleXNA(areaXNA.Right - corner,
                        areaXNA.Y + corner, corner, areaXNA.Height - corner2), brush);
                    FillRect(new RectangleXNA(areaXNA.X + corner,
                        areaXNA.Y + corner, areaXNA.Width - corner2, areaXNA.Height - corner2), brush);

                    // Draw edge corners.
                    var circle = GetCircleTexture(radius);
                    FillCorner(new RectangleXNA(areaXNA.X, areaXNA.Y, corner, corner), circle, Corner.TopLeft,
                        brush);
                    FillCorner(new RectangleXNA(areaXNA.Right - corner, areaXNA.Y, corner, corner), circle,
                        Corner.TopRight, brush);
                    FillCorner(new RectangleXNA(areaXNA.X, areaXNA.Bottom - corner, corner, corner), circle,
                        Corner.BottomLeft, brush);
                    FillCorner(new RectangleXNA(areaXNA.Right - corner, areaXNA.Bottom - corner, corner, corner),
                        circle,
                        Corner.BottomRight, brush);

                    // If using a gradient brush, draw a gradient over the mask.
                    if (gradientBrush != null)
                    {
                        UseRenderStencil();
                        DrawGradient(area, gradientBrush);
                        EndStencil();
                    }
                }
                else
                {
                    var colorBrush = brush as ColorBrush;
                    var gradientBrush = brush as GradientBrush;
                    if (colorBrush != null)
                    {
                        manager.SpriteBatch.Draw(pixel, area.ToXNA(), null, colorBrush.ToXNA(), 0, Vector2.Zero,
                            SpriteEffects.None, 0);
                    }
                    if (gradientBrush != null)
                    {
                        DrawGradient(area, gradientBrush);
                    }
                }
            }
            ResetClipArea();
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

        public override void StretchRectangle(Rectangle area, Color color, Rectangle bounds)
        {
            var shadow =
                (Texture2D) manager.Skin.Textures["shadow"];
            var a = area.ToXNA();
            var b = bounds.ToXNA();
            var half = shadow.Width / 2;
            var col = color.ToXNA();

            FillRectangle(area, color, area);
            manager.SpriteBatch.Draw(shadow, new RectangleXNA(b.X, b.Y, a.X - b.X, a.Y - b.Y),
                new RectangleXNA(0, 0, half, half), col);
            manager.SpriteBatch.Draw(shadow, new RectangleXNA(b.X, a.Y + a.Height, a.X - b.X, a.Y - b.Y),
                new RectangleXNA(0, half, half, half), col);

            manager.SpriteBatch.Draw(shadow, new RectangleXNA(a.X + a.Width, b.Y, a.X - b.X, a.Y - b.Y),
                new RectangleXNA(half, 0, half, half), col);
            manager.SpriteBatch.Draw(shadow, new RectangleXNA(a.X + a.Width, a.Y + a.Height, a.X - b.X, a.Y - b.Y),
                new RectangleXNA(half, half, half, half), col);

            manager.SpriteBatch.Draw(shadow, new RectangleXNA(a.X, b.Y, a.Width, a.Y - b.Y),
                new RectangleXNA(half, 0, 1, half), col);
            manager.SpriteBatch.Draw(shadow, new RectangleXNA(a.X, a.Y + a.Height, a.Width, a.Y - b.Y),
                new RectangleXNA(half, half, 1, half), col);

            manager.SpriteBatch.Draw(shadow, new RectangleXNA(b.X, a.Y, a.X - b.X, a.Height),
                new RectangleXNA(0, half, half, 1), col);
            manager.SpriteBatch.Draw(shadow, new RectangleXNA(a.X + a.Width, a.Y, a.X - b.X, a.Height),
                new RectangleXNA(half, half, half, 1), col);
        }

        /// <summary>
        /// Draw a gradient over the specified area. To draw in a custom area (such as rounded rectangles), use a mask.
        /// </summary>
        private void DrawGradient(Rectangle area, GradientBrush gradientBrush)
        {
            Texture2D gradientTexture;
            // Create new gradient texture if it is not cached.
            if (!gradientCache.TryGetValue(gradientBrush, out gradientTexture))
            {
                var total = 15;
                gradientTexture = new Texture2D(manager.SpriteBatch.GraphicsDevice, 1, total);
                var start = gradientBrush.Start.ToXNA();
                var end = gradientBrush.End.ToXNA();
                var colors = new ColorXNA[total];
                // Add the start and end colors to the start and end, then add colors in betweeen.
                // The default way XNA/Monogame handles just two colors will only make 1/3rd of the result
                // a gradient, so we need to add some colors in between for better interpolation.
                colors[0] = start;
                for (var i = 1; i < total - 1; i++)
                {
                    colors[i] = ColorXNA.Lerp(start, end, (float) i / (total - 2));
                }
                colors[total - 1] = end;
                gradientTexture.SetData(colors);
                gradientCache.Add(gradientBrush, gradientTexture);
            }
            manager.SpriteBatch.Draw(gradientTexture, area.ToXNA(), null, ColorXNA.White, 0, Vector2.Zero,
                SpriteEffects.None, 0);
        }

        private void EndStencil()
        {
            EndDraw();
            BeginDraw();
        }

        private void FillCorner(RectangleXNA rect, Texture2D circle, Corner corner, Brush brush)
        {
            var colorBrush = brush as ColorBrush;
            var color = ColorXNA.Black;
            if (colorBrush != null)
                color = colorBrush.Color.ToXNA();

            var source = new RectangleXNA(0, 0, circle.Width / 2, circle.Height / 2);
            switch (corner)
            {
                case Corner.TopRight:
                    source.X = circle.Width / 2;
                    break;
                case Corner.BottomLeft:
                    source.Y = circle.Height / 2;
                    break;
                case Corner.BottomRight:
                    source.Y = circle.Height / 2;
                    source.X = circle.Width / 2;
                    break;
            }
            manager.SpriteBatch.Draw(circle, rect, source, color, 0, Vector2.Zero,
                SpriteEffects.None, 0);
        }

        private void FillRect(RectangleXNA rect, Brush brush)
        {
            var colorBrush = brush as ColorBrush;
            var color = ColorXNA.Black;
            if (colorBrush != null)

                color = colorBrush.Color.ToXNA();
            manager.SpriteBatch.Draw(pixel, rect, null, color, 0, Vector2.Zero,
                SpriteEffects.None, 0);
        }

        private Texture2D GetCircleTexture(double radius)
        {
            var ret = manager.CircleSizes[manager.CircleSizes.Length - 1];
            foreach (var size in manager.CircleSizes.Where(size => size >= radius * 2))
            {
                ret = size;
                break;
            }
            return (Texture2D) manager.Skin.Textures["Shapes\\circle" + ret];
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

        private SpriteFont GetFont(string name) => manager.Skin.Fonts[name] as SpriteFont;

        private Texture2D GetTexture(string name) => manager.Skin.Textures[name] as Texture2D;

        private Size MeasureTextNoTrim(string text, int size, FontStyle style)
        {
            var closest = GetClosestFontSize(size);
            var scale = size / (float) closest;
            var measure = GetFont(Path.Combine(style.ToString(), closest.ToString())).MeasureString(text);
            return new Size((int) (Math.Round(measure.X * scale)), (int) (Math.Round(measure.Y * scale)));
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
        private static IEnumerable<TextRenderProperties> ParseFormattedText(string text, Color color,
            FontStyle style)
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
            if (colors.Count > 0)
                parts.Add(new TextRenderProperties(sb.ToString(), colors.Peek(), inBold ? FontStyle.Bold : style));
            return parts;
        }

        private void ResetClipArea()
        {
            manager.SpriteBatch.GraphicsDevice.ScissorRectangle = new RectangleXNA(0, 0, (int) Viewport.Width,
                (int) Viewport.Height);
        }

        private void SetClipArea(Rectangle bounds)
        {
            manager.SpriteBatch.GraphicsDevice.ScissorRectangle = bounds.ToXNA();
        }

        private void UseMaskStencil(Rectangle area)
        {
            // If the mask area doesn't overlap any previous areas, it is safe to reuse the area without clearing the buffer.
            if (stencilAreas.Any(x => x.Intersects(area)))
            {
                graphics.Clear(ClearOptions.Stencil, ColorXNA.White, 0, 0);
                stencilAreas.Clear();
                stencilAreas.Add(area);
            }
            else if (!stencilAreas.Any(x => x.Equals(area)))
                stencilAreas.Add(area);
            EndDraw();
            manager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, maskStencil,
                manager.SpriteBatch.GraphicsDevice.RasterizerState, alphaTextEffect);
        }

        private void UseRenderStencil()
        {
            EndDraw();
            manager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, renderStencil,
                manager.SpriteBatch.GraphicsDevice.RasterizerState, alphaTextEffect);
        }

        private enum Corner
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

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