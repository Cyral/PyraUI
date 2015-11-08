using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using Pyratron.UI.Brushes;
using Pyratron.UI.Controls;
using Pyratron.UI.Markup;
using Pyratron.UI.Types;

namespace Pyratron.UI
{
    public abstract class Manager
    {
        private static readonly TimeSpan second = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Master list of all elements.
        /// </summary>
        public List<Element> Elements { get; } = new List<Element>();

        public Renderer Renderer { get; set; }

        public Skin Skin { get; set; }

        public MarkupLoader Markup { get; set; }

        public bool DrawDebug { get; set; }

        /// <summary>
        /// The current FPS.
        /// </summary>
        public int FPS { get; private set; }

        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int totalFrames;

        private string xml;
     
        public virtual void Init()
        {
            Markup = new MarkupLoader(this);
            xml = Markup.LoadFromXAML(File.ReadAllText("window.xml"), null);
            foreach (var element in Elements)
                element.UpdateLayout();
        }

        public virtual void Load()
        {
            Skin.LoadTextureInternal("button");
        }

        /// <summary>
        /// Draws the UI.
        /// </summary>
        /// <param name="delta">Seconds elapsed since last frame.</param>
        public virtual void Draw(float delta)
        {
            //Elements[0].UpdateLayout();
            Renderer.BeginDraw();
            //Renderer.DrawTexture("button", new Rectangle(50, 50, 150, 150));
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
                element.ActualSizePrevious = element.ActualSize;
            }
            Renderer.DrawString($"FPS: {FPS}\nRendered From XML:\n{xml}", new Point(8, Elements[1].ExtendedArea.Height + 8), Color.Black,
                8, Rectangle.Infinity, true);
            Renderer.EndDraw();

            // Calculate FPS
            elapsedTime += TimeSpan.FromSeconds(delta);
            if (elapsedTime > second)
            {
                elapsedTime -= second;
                FPS = totalFrames;
                totalFrames = 0;
            }
            totalFrames++;
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
                if (element.ActualSize != element.ActualSizePrevious)
                    element.InvalidateLayout();
                element.Update(delta);
            }
        }
    }
}