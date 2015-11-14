namespace Pyratron.UI.Controls
{
    /// <summary>
    /// Provides a base for elements that render an effect around a child element.
    /// </summary>
    public abstract class Decorator : Visual
    {
        public override int MaxChildren => 1;

        public Element Child => Elements.Count == 0 ? null : Elements[0];

        public Decorator(Manager manager) : base(manager)
        {
        }
    }
}