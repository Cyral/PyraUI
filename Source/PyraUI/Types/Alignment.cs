namespace Pyratron.UI.Types
{
    /// <summary>
    /// Overall alignment for an item.
    /// </summary>
    public enum Alignment
    {
        TopLeft,
        TopCenter,
        TopRight,
        LeftCenter,
        Center,
        RightCenter,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    /// <summary>
    /// Vertical alignment of an item.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// Align to the top. (Default).
        /// </summary>
        Top,
        /// <summary>
        /// Align to the center.
        /// </summary>
        Center,
        /// <summary>
        /// Align to the bottom.
        /// </summary>
        Bottom,
        /// <summary>
        /// Stretch to fill the available space.
        /// </summary>
        Stretch
    }

    /// <summary>
    /// Horizontal alignment of an item.
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// Align to the left. (Default).
        /// </summary>
        Left,
        /// <summary>
        /// Align to the center.
        /// </summary>
        Center,
        /// <summary>
        /// Align to the right.
        /// </summary>
        Right,
        /// <summary>
        /// Stretch to fill the available space.
        /// </summary>
        Stretch
    }
}