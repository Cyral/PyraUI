namespace Pyratron.UI.Controls
{
    internal class Window : Control
    {
        public string Title { get; set; }

        public Window(Manager manager) : base(manager)
        {
            Width = 512;
            Height = 256;

            Title = "Window";
        }
    }
}