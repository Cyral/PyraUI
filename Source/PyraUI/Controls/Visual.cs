using System.Collections.Generic;
using System.ComponentModel;
using Pyratron.UI.Effects;
using Pyratron.UI.States;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// A visual UI element.
    /// </summary>
    public class Visual : Element
    {
        private static readonly Color marginColor = ((Color) "#f4ffa2") * .5f;
        private static readonly Color extendedColor = ((Color) "#0088cc") * .75f;
        private static readonly Color contentColor = ((Color) "#039702") * .75f;

        /// <summary>
        /// The opacity of the visual, from 0 to 1.
        /// </summary>
        public float Alpha { get; }

        /// <summary>
        /// The color of the control.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Returns if the element is visible.
        /// </summary>
        [Browsable(false)]
        public bool IsVisible => Visibility == Visibility.Visible;

        public List<Effect> Effects { get; private set; }

        public override Element Parent
        {
            get { return parent; }
            protected internal set
            {
                parent = value;

                // If parent is a visual, set the child visibility to the parent's.
                var visual = parent as Visual;
                if (visual != null)
                    Visibility = visual.Visibility;
            }
        }

        private Element parent;

        /// <summary>
        /// The display state of the element.
        /// </summary>
        public Visibility Visibility;

        public Visual(Manager manager) : base(manager)
        {
            Effects = new List<Effect>();
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Draws the component.
        /// </summary>
        /// <param name="delta">Elapsed seconds since the last frame.</param>
        public virtual void Draw(float delta)
        {
            DrawEffects(delta);
            
            if (Manager.DrawDebug)
            {
                DrawDebug(delta);
            }

            DrawChildren(delta);
        }

        private void DrawEffects(float delta)
        {
            foreach (var effect in Effects)
            {
                effect.Render(ExtendedBounds, Bounds, delta);
            }
        }

        internal virtual void DrawDebug(float delta)
        {
            Manager.Renderer.DrawRectangle(ExtendedBounds, marginColor, Margin, ParentBounds);
            Manager.Renderer.DrawRectangle(ExtendedBounds, extendedColor, 1, ParentBounds);
            Manager.Renderer.DrawRectangle(Bounds, contentColor, 1, ParentBounds);

            Manager.Renderer.DrawString($"*{ToString().Remove(0, "Pyratron.UI.Controls.".Length)}*", Bounds.Point + new Point(2, 0), 8, ParentBounds);
            //Manager.Renderer.DrawString($"_{BorderArea.X},{BorderArea.Y} {BorderArea.Width}x{BorderArea.Height}_", BorderArea.Point + new Point(2, 12), 8, ParentBounds);
            //Manager.Renderer.DrawString("*Content*: " + BorderArea, BorderArea.Point + new Point(2, 12), 8, ParentBounds);
        }

        /// <summary>
        /// Draws all of the children elements.
        /// </summary>
        private void DrawChildren(float delta)
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                var visual = element as Visual;
                visual?.Draw(delta);
            }
        }
    }
}