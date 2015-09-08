using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pyratron.UI.Monogame
{
    /// <summary>
    /// MonoGame implementation of PyraUI.
    /// </summary>
    public class Manager : UI.Manager
    {
        public override void Init()
        {
            Renderer = new Renderer(this);
            Skin = new Skin(this);
            base.Init();
        }

        public ContentManager Content { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public SpriteFont Font { get; set; }
    }
}