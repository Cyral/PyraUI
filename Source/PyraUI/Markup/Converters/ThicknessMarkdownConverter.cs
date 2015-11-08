using System;
using Pyratron.UI.Types;

namespace Pyratron.UI.Markup.Converters
{
    public class ThicknessMarkdownConverter : IMarkdownConverter
    {
        private readonly Type self = typeof (Thickness);
        public bool CanConvert(Type type) => type == self;

        public object Convert(Type type, string value)
        {
            return (Thickness) value;
        }
    }
}