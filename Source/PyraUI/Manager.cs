using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Pyratron.UI.Controls;
using Pyratron.UI.Markup;
using Pyratron.UI.Types;

namespace Pyratron.UI
{
    /// <summary>
    /// Manages the user interface.
    /// </summary>
    public abstract class Manager
    {
        private static readonly TimeSpan second = TimeSpan.FromSeconds(1);

        /// <summary>
        /// List of root level elements.
        /// </summary>
        public ReadOnlyCollection<Element> Elements { get; private set; }

        /// <summary>
        /// Renderer to render text, textures, and shapes.
        /// </summary>
        public Renderer Renderer { get; set; }

        /// <summary>
        /// UI skin to load and manage assets.
        /// </summary>
        public Skin Skin { get; set; }

        /// <summary>
        /// Parses XAML into a visual tree.
        /// </summary>
        public MarkupParser Markup { get; set; }

        /// <summary>
        /// Manages a queue of elements to arrange and measure.
        /// </summary>
        public LayoutManager Layout { get; set; }

        /// <summary>
        /// Indicates if debugging information should be rendered.
        /// </summary>
        public bool DrawDebug { get; set; }

        /// <summary>
        /// The average frames per second over the past second.
        /// </summary>
        public int FPS { get; private set; }

        private readonly List<Element> elements = new List<Element>();
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int totalFrames;
        private string xml;

        public virtual void Init()
        {
            Elements = new ReadOnlyCollection<Element>(elements);
            Layout = new LayoutManager(this);
            Markup = new MarkupParser(this);

            xml = File.ReadAllText("window.xml");

            // Parse XAML into visual tree.
            var stopwatch = Stopwatch.StartNew();

            Markup.LoadFromXAML(xml, null);
            stopwatch.Stop();

            Console.WriteLine("XAML parsed in " + stopwatch.ElapsedMilliseconds + "ms");

            // Run initial layout pass.
            stopwatch = Stopwatch.StartNew();
            Layout.UpdateLayout();
            stopwatch.Stop();

            Console.WriteLine("Layout measured and arranged in " + stopwatch.ElapsedMilliseconds + "ms");
        }

        /// <summary>
        /// Add a root level element to the UI.
        /// </summary>
        public void AddRootElement(Element element)
        {
            element.Level = 0;
            element.Parent = null;
            elements.Add(element);
        }

        /// <summary>
        /// Draws the UI.
        /// </summary>
        /// <param name="delta">Seconds elapsed since last frame.</param>
        public virtual void Draw(float delta)
        {
            Renderer.BeginDraw();

            // Render all top level elements. (Those with no parent).
            for (var i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                var visual = element as Visual;
                visual?.Draw(delta);
            }

            // Debugging stuff.
            Renderer.DrawString($"FPS: {FPS}\nRendered From XML:\n{xml}",
                new Point(8, Elements[0].ExtendedArea.Height + 8), Color.Black,
                8, Rectangle.Infinity, true);
            var sb = new StringBuilder("Visual Tree:");
            AddToTree(Elements[0], sb);
            var treeStr = sb.ToString();
            Renderer.DrawString(treeStr,
                new Point(Renderer.Viewport.Width - Renderer.MeasureText(treeStr, 8).Width - 8,
                    Elements[0].ExtendedArea.Height + 8), Color.Black, 8, Rectangle.Infinity, true);
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

        public virtual void Load()
        {
            Skin.LoadTextureInternal("button");
        }

        /// <summary>
        /// Remove a root level element from the UI.
        /// </summary>
        public void RemoveRootElement(Element element)
        {
            elements.Remove(element);
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

        private static void AddToTree(Element element, StringBuilder sb)
        {
            sb.Append(Environment.NewLine + new string(' ', element.Level * 3) +
                      element.ToString().Remove(0, "Pyratron.UI.Controls.".Length));
            foreach (var child in element.Elements)
                AddToTree(child, sb);
        }
    }
}