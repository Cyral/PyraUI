namespace Pyratron.UI.Controls
{
    /// <summary>
    /// The most basic form of a UI component.
    /// </summary>
    public abstract class Component
    {
        public abstract int MaxChildren { get; }

        public Component(Manager manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Updates the component.
        /// </summary>
        /// <param name="delta">Elapsed seconds since the last frame.</param>
        protected internal virtual void Update(float delta)
        {

        }

        /// <summary>
        /// The UI manager that owns this component.
        /// </summary>
        public Manager Manager { get; internal set; }
    }
}