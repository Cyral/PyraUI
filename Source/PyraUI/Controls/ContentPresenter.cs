using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    public class ContentPresenter : Visual
    {
        // TODO: Use content presenter for control templates
        // For now it is just for padding.
        private static readonly Color paddingColor = ((Color) "#a69ce1") * .5f;
        public override int MaxChildren => int.MaxValue; // Panels are designed for containing a number of elements.

        public ContentPresenter(Manager manager) : base(manager)
        {
        }

        internal override void DrawDebug(float delta)
        {
            Manager.Renderer.DrawRectangle(ExtendedBounds, paddingColor, Margin, ParentBounds);
        }
    }
}