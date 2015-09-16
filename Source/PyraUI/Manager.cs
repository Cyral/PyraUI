using System.Collections.Generic;
using Pyratron.UI.Controls;
using Pyratron.UI.States;
using Pyratron.UI.Types;

namespace Pyratron.UI
{
    public abstract class Manager
    {
        /// <summary>
        /// Master list of all elements.
        /// </summary>
        public List<Element> Elements { get; } = new List<Element>();

        public Renderer Renderer { get; set; }

        public Skin Skin { get; set; }

        public bool DrawDebug { get; set; }

        public virtual void Init()
        {
            //Add an initial window.
            var window = new Window(this) {Box = Box.Overlap, Height = 700, Width = 900};
            Elements.Add(window);
            var panelMain = new StackPanel(this) { Orientation = Orientation.Vertical};
            window.Add(panelMain);
     
            var panel = new StackPanel(this) { Orientation = Orientation.Vertical};
            panelMain.Add(panel);
            var panel2 = new StackPanel(this) { Orientation = Orientation.Horizontal};
            panelMain.Add(panel2);
  
            var button = new Button(this);
            panel.Add(button);
            button.Add(new Label(this));

            var button2 = new Button(this);
            panel.Add(button2);
            button2.Add(new Label(this));

            var button3 = new Button(this);
            panel2.Add(button3);
            button3.Add(new Label(this));

            var panel3 = new StackPanel(this) { Orientation = Orientation.Vertical };
            panel2.Add(panel3);
            panel3.Width.Value = 500;
            panel3.Width.Auto = false;

            var button4 = new Button(this);
            panel3.Add(button4);
            button4.Add(new Label(this) { Text  = "PyraUI"});

            var button5 = new Button(this);
            panel3.Add(button5);
            button5.Add(new Label(this));

            var button6 = new Button(this);
            panelMain.Add(button6);
            var button7 = new Button(this);
            panelMain.Add(button7);
            var panel5 = new StackPanel(this) {Orientation = Orientation.Horizontal };
            panel5.Width.Max = 200;
            var label = new Label(this) {Text = "Testing", VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
            panelMain.Add(panel5);
            panel5.Add(label);
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
            Elements[0].Arrange();
            Renderer.BeginDraw();
            //Renderer.DrawTexture("button", new Rectangle(50, 50, 150, 150));
            //Renderer.DrawString("Hello! Welcome to PyraUI.", new Point(400, 400));

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