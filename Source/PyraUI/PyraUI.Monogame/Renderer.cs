using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = PyraUI.Point;
using Rectangle = PyraUI.Rectangle;

namespace Pyratron.UI.Monogame
{
    public class Renderer : PyraUI.Renderer
    {
        private readonly Texture2D pixel;
        private readonly Manager manager;

        public Renderer(Manager manager)
        {
            this.manager = manager;

            // Create 1x1 texture for rendering rectangles.
            pixel = new Texture2D(manager.SpriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public override void BeginDraw()
        {
            manager.SpriteBatch.Begin();
        }

        public override void EndDraw()
        {
            manager.SpriteBatch.End();
        }

        public override void DrawTexture(string name, Rectangle rectangle)
        {
            DrawTexture(name, rectangle, Color.White);
        }

        public override void DrawTexture(string name, Rectangle rectangle, Color color)
        {
            var rect = new Microsoft.Xna.Framework.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            var texture = GetTexture(name);
            var col = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
            manager.SpriteBatch.Draw(texture, rect, col * (color.A / 255f));
        }

        public override void DrawString(string text, Point point)
        {
            DrawString(text, point, Color.Red);
        }

        public override void DrawString(string text, Point point, Color color)
        {
            var pos = new Vector2(point.X, point.Y);
            var col = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
            manager.SpriteBatch.DrawString(GetFont("default"), text, pos, col * (color.A / 255f), 0,
                Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }

        public override void DrawString(string text, Point point, float pt)
        {
            // TODO: Use multiple spritefonts.
            var pos = new Vector2(point.X, point.Y);
            var scale = Vector2.One * (pt / 10);
            manager.SpriteBatch.DrawString(GetFont("default"), text, pos, Microsoft.Xna.Framework.Color.Black, 0,
                Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public override void DrawRectangle(Rectangle bounds, Color color)
        {
            var rect = new Microsoft.Xna.Framework.Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            var col = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
            manager.SpriteBatch.Draw(pixel, rect, col);
        }

        private Texture2D GetTexture(string name) => manager.Skin.Textures[name] as Texture2D;
        private SpriteFont GetFont(string name) => manager.Skin.Fonts[name] as SpriteFont;
    }
}