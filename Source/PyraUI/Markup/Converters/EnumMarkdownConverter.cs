using System;

namespace Pyratron.UI.Markup.Converters
{
    public class EnumMarkdownConverter : IMarkdownConverter
    {
        public bool CanConvert(Type type) => type.IsEnum;

        public object Convert(Type type, string value)
        {
            return Enum.Parse(type, value);
        }
    }
}