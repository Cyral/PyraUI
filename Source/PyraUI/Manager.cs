using System.Collections.Generic;
using System.Linq;
using Pyratron.UI.Controls;
using Pyratron.UI.States;
using PyraUI;

namespace Pyratron.UI
{
    public abstract class Manager
    {
        /// <summary>
        /// Master list of all elements.
        /// </summary>
        public List<Element> Elements { get; private set; } = new List<Element>();

        public Renderer Renderer { get; set; }

        public Skin Skin { get; set; }

        public virtual void Init()
        {
            //Add an initial window.
            var window = new Window(this) { Box = Box.Overlap, Height = 512};
            var control = new Control(this) { Width = 200};
            var control2 = new Control(this) { Width = 100, Height = 32 };
            var control3 = new Control(this) { Width = 110, Height = 32 };
            var control4 = new Control(this) { Width = 100, Height = 32 };
            var control5 = new Control(this) { Width = 120, Box = Box.Block };
            var control6 = new Control(this) { Width = 16, Height = 16, Margin = 1, Padding = 1};
            var control7 = new Control(this) { Width = 24, Height =32, Margin = 1, Padding = 1 };
            var control8 = new Control(this) { Width = 128, Height = 8, Margin = 1, Padding = 1, Box = Box.Block };
            var control9 = new Control(this) { Width = 400, Height = 32 };
            var control10 = new Control(this) { Width = 400, Height = 64 };
            var control11 = new Control(this) { Width = 120, Box = Box.Block };
            var control12 = new Control(this) { Width = 400, Height = 32 };
            window.Add(control);
            window.Add(control2);
            window.Add(control3);
            window.Add(control4);
            window.Add(control5);
            window.Add(control6);
            window.Add(control7);
            window.Add(control8);
            window.Add(control9);
            window.Add(control10);
            window.Add(control11);
            window.Add(control12);
            Elements.Add(window);
        }

        public virtual void Load()
        {
            Skin.LoadTextureInternal("button");
            Skin.LoadFontInternal("default");
        }

        /// <summary>
        /// Draws the UI.
        /// </summary>
        /// <param name="delta">Seconds elapsed since last frame.</param>
        public virtual void Draw(float delta)
        {
            Renderer.BeginDraw();
            //Renderer.DrawTexture("button", new Rectangle(50, 50, 150, 150));
            Renderer.DrawString("Hello! Welcome to PyraUI.", new Point(400, 400));

            // Render all top level elements. (Those with no parent).
            for (var i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                var visual = element as Visual;
                // If the element is a visual, render it.
                if (element.Parent == null && visual != null)
                {
                    visual.Draw(delta);
                }
            }
            Renderer.EndDraw();
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="delta">Seconds elapsed since last frame.</param>
        public virtual void Update(float delta)
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                element.Update(delta);
            }
        }
    }
}