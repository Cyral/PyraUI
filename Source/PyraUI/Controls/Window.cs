using System;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    internal class Window : Control
    {
        public string Title { get; set; }

        public Window(Manager manager) : base(manager)
        {
            Title = "Window";
        }
    }
}