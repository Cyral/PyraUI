using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pyratron.UI.Types;

namespace Pyratron.UI.Monogame
{
    /// <summary>
    /// MonoGame implementation of PyraUI.
    /// </summary>
    public class Manager : UI.Manager
    {
        public ContentManager Content { get; set; }
        public SpriteBatch SpriteBatch { get; set; }

        internal readonly int[] FontSizes = {6, 7, 8, 9, 10, 12, 14, 16, 20, 64};

        public override void Load()
        {
            Renderer = new Renderer(this);
            Skin = new Skin(this);

            foreach (var size in FontSizes)
                foreach (var style in Enum.GetValues(typeof (FontStyle)))
                    Skin.LoadFontInternal(Path.Combine(style.ToString(), size.ToString()));

            base.Load();
        }

        public override void Update(float delta)
        {
            DrawDebug = !Keyboard.GetState().IsKeyUp(Keys.D);
            base.Update(delta);
        }
    }
}