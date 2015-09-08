namespace Pyratron.UI.States
{
    /// <summary>
    /// Defines the layout positioning of an element.
    /// </summary>
    public enum Box
    {
        /// <summary>
        /// A block element will appear below a control. It uses all the available width by default.
        /// </summary>
        Block,
        /// <summary>
        /// An inline element will attempt to appear next to a control. It only uses the space it needs.
        /// </summary>
        Inline,
        /// <summary>
        /// The element is positioned relative to the top left corner of the parent control. It only uses the space it needs.
        /// </summary>
        Overlap,
        /// <summary>
        /// The element will inherit the layout type of it's parent.
        /// </summary>
        Inherit
    }
}