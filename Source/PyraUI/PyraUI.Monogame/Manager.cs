using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pyratron.UI.Monogame
{
    /// <summary>
    /// MonoGame implementation of PyraUI.
    /// </summary>
    public class Manager : UI.Manager
    {
        public ContentManager Content { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public SpriteFont Font { get; set; }

        public override void Load()
        {
            Renderer = new Renderer(this);
            Skin = new Skin(this);
            base.Load();
        }

        public override void Update(float delta)
        {
            DrawDebug = Keyboard.GetState().IsKeyUp(Keys.D);
            base.Update(delta);
        }
    }
}