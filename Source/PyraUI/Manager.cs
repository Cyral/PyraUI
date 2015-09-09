using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            var panel = new StackPanel(this) { Height =100};
            window.Add(panel);
            panel.Add(new Control(this) { Height = 100 });
            panel.Add(new Control(this) { Height = 75 });
            panel.Add(new Control(this) { Height = 24,});
            panel.Add(new Control(this) { Height = 75, });
            panel.Add(new Control(this) {Height = 100, Margin = 24});
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