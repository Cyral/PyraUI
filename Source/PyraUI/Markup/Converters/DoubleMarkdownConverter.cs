using System;

namespace Pyratron.UI.Markup.Converters
{
    internal class DoubleMarkdownConverter : IMarkdownConverter
    {
        private readonly Type self = typeof (double);
        public bool CanConvert(Type type) => type == self;

        public object Convert(Type type, string value)
        {
            // Auto = Infinity
            return value.Equals("Auto", StringComparison.InvariantCultureIgnoreCase)
                ? double.PositiveInfinity
                : double.Parse(value);
        }
    }
}