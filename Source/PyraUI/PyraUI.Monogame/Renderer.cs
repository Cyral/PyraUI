using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pyratron.UI.Types;
using Color = Pyratron.UI.Types.Color;
using Point = Pyratron.UI.Types.Point;
using Rectangle = Pyratron.UI.Types.Rectangle;

namespace Pyratron.UI.Monogame
{
    public class Renderer : UI.Renderer
    {
        private readonly Manager manager;
        private readonly Texture2D pixel;

        public Renderer(Manager manager)
        {
            this.manager = manager;

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

        public override void DrawTexture(string name, Rectangle rectangle, Color color)
        {
            var rect = new Microsoft.Xna.Framework.Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
            var texture = GetTexture(name);
            var col = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
            manager.SpriteBatch.Draw(texture, rect, col * (color.A / 255f));
        }

        public override void DrawString(string text, Point point, Color color, int size, FontStyle style)
        {
            var pos = new Vector2((int)Math.Round(point.X), (int)Math.Round(point.Y) - 4);
            var col = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
            var closest = GetClosestFontSize(size);
            manager.SpriteBatch.DrawString(GetFont(Path.Combine(style.ToString(), closest.ToString())), text, pos, col * (color.A / 255f), 0,
                Vector2.Zero, size / (float)closest, SpriteEffects.None, 0);
        }

        public override void DrawRectangle(Rectangle bounds, Color color)
        {
            var rect = new Microsoft.Xna.Framework.Rectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
            var col = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
            manager.SpriteBatch.Draw(pixel, rect, col);
        }

        public override Size MeasureText(string text, int size, FontStyle style)
        {
            // Remove any extra spaces, possibly created by the space character for invalid characters not included in the spritefont.
            text = text.Trim();
            var closest = GetClosestFontSize(size);
            var scale = size / (float)closest;
            var measure = GetFont(Path.Combine(style.ToString(), closest.ToString())).MeasureString(text);
            return new Size((int)(Math.Round(measure.X * scale)), (int)(Math.Round((measure.Y * scale) - 4))); //TODO: 4 is due to spacing.
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
    }
}