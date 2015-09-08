using Microsoft.Xna.Framework.Graphics;

namespace Pyratron.UI.Monogame
{
    internal class Skin : UI.Skin
    {
        private readonly Manager manager;

        public Skin(Manager manager)
        {
            this.manager = manager;
        }

        public override object LoadTexture(string name)
        {
            return manager.Content.Load<Texture2D>(name);
        }

        public override object LoadFont(string name)
        {
            return manager.Content.Load<SpriteFont>(name);
        }
    }
}