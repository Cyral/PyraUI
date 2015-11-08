using System;

namespace Pyratron.UI.Markup.Converters
{
    /// <summary>
    /// Convert a string (from XAML) to a type.
    /// </summary>
    public interface IMarkdownConverter
    {
        bool CanConvert(Type type);

        object Convert(Type type, string value);
    }
}