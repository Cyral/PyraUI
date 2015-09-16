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

        internal void LoadTextureInternal(string name)
            => Textures.Add(name, LoadTexture(Path.Combine("Textures", name)));

        internal void LoadFontInternal(string name) => Fonts.Add(name, LoadFont(Path.Combine("Fonts", name)));
    }
}