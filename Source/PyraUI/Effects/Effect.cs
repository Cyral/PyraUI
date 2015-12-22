using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Effects
{
    /// <summary>
    /// A method of painting a visual.
    /// </summary>
    public abstract class Effect : DependencyObject
    {
        protected Manager Manager { get; private set; }

        public Effect(Manager manager)
        {
            Manager = manager;
        }

        public abstract void Render(Rectangle extendedArea, Rectangle contentArea, float delta);
    }
}
