using System.Collections.Generic;
using System.IO;

namespace Pyratron.UI
{
    public abstract class Skin
    {
        public Dictionary<string, object> Textures { get; protected set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Fonts { get; protected set; } = new Dictionary<string, object>();

        public abstract object LoadTexture(string name);
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