using Pyratron.UI.Brushes;
using Pyratron.UI.Types;

namespace Pyratron.UI
{
    /// <summary>
    /// Renders text, textures, and shapes.
    /// </summary>
    public abstract class Renderer
    {
        private readonly int defaultSize = 10;

        /// <summary>
        /// Instructs the renderer to prepare for a draw pass.
        /// </summary>
        public abstract void BeginDraw();
        
        /// <summary>
        /// Finalizes the draw pass.
        /// </summary>
        public abstract void EndDraw();

        public abstract Size Viewport { get; }

        /// <summary>
        /// Draws a texture loaded by the skin in the specified region.
        /// </summary>
        public void DrawTexture(string name, Rectangle rectangle, Rectangle bounds) => DrawTexture(name, rectangle, Color.White, bounds);

        /// <summary>
        /// Draws a texture loaded by the skin in the specified region with the specified tint.
        /// </summary>
        public abstract void DrawTexture(string name, Rectangle rectangle, ColorBrush color, Rectangle bounds);

        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public void DrawString(string text, Point point, Rectangle bounds, bool ignoreFormatting = false) => DrawString(text, point, Color.Black, defaultSize, bounds, ignoreFormatting);

        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public void DrawString(string text, Point point, ColorBrush color, Rectangle bounds, bool ignoreFormatting = false) => DrawString(text, point, color, defaultSize, bounds, ignoreFormatting);

        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public void DrawString(string text, Point point, int size, Rectangle bounds,bool ignoreFormatting = false) => DrawString(text, point, Color.Black, size, bounds, ignoreFormatting);

        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public void DrawString(string text, Point point, Brush brush, int size, Rectangle bounds, bool ignoreFormatting = false)
            => DrawString(text, point, brush, size, FontStyle.Regular, bounds, ignoreFormatting);

        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public abstract void DrawString(string text, Point point, Brush brush, int size, FontStyle style, Rectangle bounds,bool ignoreFormatting = false);

        /// <summary>
        /// Draws a rectangle within the specified area.
        /// </summary>
        public void DrawRectangle(Rectangle area, Brush brush, Thickness border, Rectangle bounds) => DrawRectangle(area, brush, border, 0, bounds);

        /// <summary>
        /// Draws a rounded rectangle within the specified area.
        /// </summary>
        public abstract void DrawRectangle(Rectangle area, Brush brush, Thickness border, double cornerRadius, Rectangle bounds);

        /// <summary>
        /// Draws a filled rectangle within the specified area.
        /// </summary>
        public void FillRectangle(Rectangle area, Brush brush, Rectangle bounds) => FillRectangle(area, brush, 0, bounds);

        /// <summary>
        /// Draws a rounded filled rectangle within the specified area.
        /// </summary>
        public abstract void FillRectangle(Rectangle area, Brush brush, double cornerRadius, Rectangle bounds);

        /// <summary>
        /// Stretches a rectangle to fill the bounds and create a shadow effect.
        /// </summary>
        public abstract void StretchRectangle(Rectangle area, Color color, Rectangle bounds);

        /// <summary>
        /// Returns the size the text will use when rendered.
        /// </summary>
        public Size MeasureText(string text) => MeasureText(text, defaultSize, FontStyle.Regular);

        /// <summary>
        /// Returns the size the text will use when rendered.
        /// </summary>
        public Size MeasureText(string text, int size) => MeasureText(text, size, FontStyle.Regular);

        /// <summary>
        /// Returns the size the text will use when rendered.
        /// </summary>
        public Size MeasureText(string text, FontStyle style) => MeasureText(text, defaultSize, style);

        /// <summary>
        /// Returns the size the text will use when rendered.
        /// </summary>
        public abstract Size MeasureText(string text, int size, FontStyle style);
    }
}