using System;
using Pyratron.UI.Brushes;
using Pyratron.UI.Types;

namespace Pyratron.UI.Markup.Converters
{
    internal class BrushMarkdownConverter : IMarkdownConverter
    {
        private readonly Type self = typeof (Brush);
        public bool CanConvert(Type type) => type == self;

        public object Convert(Type type, string value)
        {
            return (Brush)value;
        }
    }
}