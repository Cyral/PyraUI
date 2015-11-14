using System.Collections.Generic;
using System.IO;

namespace Pyratron.UI
{
    /// <summary>
    /// Loads and manages assets.
    /// </summary>
    public abstract class Skin
    {
        /// <summary>
        /// Collection of all textures loaded.
        /// </summary>
        public Dictionary<string, object> Textures { get; protected set; } = new Dictionary<string, object>();

        /// <summary>
        /// Collection of all fonts loaded.
        /// </summary>
        public Dictionary<string, object> Fonts { get; protected set; } = new Dictionary<string, object>();

        /// <summary>
        /// Load a texture relative to the skin directory.
        /// </summary>
        public abstract object LoadTexture(string name);


        /// <summary>
        /// Load a font relative to the skin directory.
        /// </summary>
        public abstract object LoadFont(string name);

        internal object LoadTextureInternal(string name)
        {
            var texture = LoadTexture(Path.Combine("Textures", name));
            Textures.Add(name, texture);
            return texture;
        }

        internal object LoadFontInternal(string name)
        {
            var font = LoadFont(Path.Combine("Fonts", name));
            Fonts.Add(name, font);
            return font;
        }
    }
}