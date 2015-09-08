namespace Pyratron.UI.Controls
{
    internal class Window : Control
    {
        public Window(Manager manager) : base(manager)
        {
            Width = 512;
            Height = 256;

            MaxWidth = 2048;
            MaxHeight = 2048;

            MinWidth = 128;
            MinHeight = 128;
        }
    }
}