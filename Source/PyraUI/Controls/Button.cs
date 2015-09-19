using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    internal class Button : Control
    {
        public Button(Manager manager) : base(manager)
        {
            Margin = 1;
            Padding = new Thickness(5, 8);
        }
    }
}