using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    internal class Button : Control
    {
        public Button(Manager manager) : base(manager)
        {
            Margin = 1;
            Padding = new Thickness(5, 8);

            Height.Min = 8;
            Height.Max = 64;
            Height.Value = 20;

            Width.Min = 32;
            Width.Max = 512;
            Width.Value = 96;
        }
    }
}