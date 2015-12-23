using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Input;

namespace Pyratron.UI.Monogame
{
    /// <summary>
    /// MonoGame implementation of PyraUI.
    /// </summary>
    public class Manager : UI.Manager
    {
        public ContentManager Content { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        internal readonly int[] CircleSizes = {16, 64};

        internal readonly int[] FontSizes = {6, 7, 8, 9, 10, 12, 14, 16, 20, 64};

        public override void Load()
        {
            SpriteBatch.GraphicsDevice.RasterizerState = new RasterizerState() { ScissorTestEnable = true, MultiSampleAntiAlias = true };
            Skin = new Skin(this);
            Renderer = new Renderer(this);
            Input = new InputHandler(this);

            foreach (var size in FontSizes)
                foreach (var style in Enum.GetValues(typeof (FontStyle)))
                    Skin.LoadFontInternal(Path.Combine(style.ToString(), size.ToString()));

            // Load circles used for rendering shapes.
            foreach (var size in CircleSizes)
                Skin.LoadTextureInternal(Path.Combine("Shapes", "circle" + size));

            Skin.LoadTextureInternal("shadow");

            base.Load();
        }

        public override void Update(float delta, float total)
        {
            base.Update(delta, total);
        }
    }
}