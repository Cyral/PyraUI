using System.Collections.Generic;
using System.ComponentModel;
using Pyratron.UI.Effects;
using Pyratron.UI.States;
using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Controls
{
    /// <summary>
    /// A visual UI element.
    /// </summary>
    public class Visual : Element
    {
        public static readonly DependencyProperty<Visibility> VisibilityProperty =
            DependencyProperty.Register<Visual, Visibility>(nameof(Visibility), Visibility.Visible,
                new PropertyMetadata(MetadataOption.AffectsMeasure | MetadataOption.AffectsArrange));

        private static readonly Color marginColor = ((Color) "#f4ffa2") * .5f;
        private static readonly Color extendedColor = ((Color) "#0088cc") * .75f;
        private static readonly Color contentColor = ((Color) "#039702") * .75f;

        /// <summary>
        /// The display state of the element.
        /// </summary>
        public Visibility Visibility
        {
            get { return GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        /// <summary>
        /// Returns if the element is visible.
        /// </summary>
        [Browsable(false)]
        public bool IsVisible => Visibility == Visibility.Visible;

        public List<Effect> Effects { get; }

        public override Element Parent { get; protected internal set; }

        public Visual(Manager manager) : base(manager)
        {
            Effects = new List<Effect>();
        }

        /// <summary>
        /// Draws the component.
        /// </summary>
        /// <param name="delta">Elapsed seconds since the last frame.</param>
        public virtual void Draw(float delta)
        {
            if (Visibility == Visibility.Visible)
            {
                DrawEffects(delta);

                if (Manager.DrawDebug)
                {
                    DrawDebug(delta);
                }

                DrawChildren(delta);
            }
        }

        public override void Measure(Size availableSize)
        {
            // If visibility is collapsed, do not account for the element's size.
            if (Visibility == Visibility.Collapsed)
            {
                Manager.Layout.RemoveMeasure(this);
                DesiredSize = Size.Zero;
                return;
            }

            base.Measure(availableSize);
        }

        internal virtual void DrawDebug(float delta)
        {
            Manager.Renderer.DrawRectangle(ExtendedBounds, marginColor, Margin, ParentBounds);
            Manager.Renderer.DrawRectangle(ExtendedBounds, extendedColor, 1, ParentBounds);
            Manager.Renderer.DrawRectangle(Bounds, contentColor, 1, ParentBounds);

            Manager.Renderer.DrawString($"*{ToString().Remove(0, "Pyratron.UI.Controls.".Length)}*",
                Bounds.Point + new Point(2, 0), 8, ParentBounds);
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

        private void DrawEffects(float delta)
        {
            foreach (var effect in Effects)
            {
                effect.Render(ExtendedBounds, Bounds, delta);
            }
        }
    }
}