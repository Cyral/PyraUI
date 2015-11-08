using System;
using Pyratron.UI.Types;

namespace Pyratron.UI.Markup.Converters
{
    internal class ColorMarkdownConverter : IMarkdownConverter
    {
        private readonly Type self = typeof(Color);
        public bool CanConvert(Type type) => type == self;

        public object Convert(Type type, string value)
        {
            return (Color) value;
        }
    }
}