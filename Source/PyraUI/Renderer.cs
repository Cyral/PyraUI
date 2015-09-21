using Pyratron.UI.Types;

namespace Pyratron.UI
{
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

        /// <summary>
        /// Draws a texture loaded by the skin in the specified region.
        /// </summary>
        public void DrawTexture(string name, Rectangle rectangle) => DrawTexture(name, rectangle, Color.White);

        /// <summary>
        /// Draws a texture loaded by the skin in the specified region with the specified tint.
        /// </summary>
        public abstract void DrawTexture(string name, Rectangle rectangle, Color color);


        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public void DrawString(string text, Point point) => DrawString(text, point, Color.Black, defaultSize);

        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public void DrawString(string text, Point point, Color color) => DrawString(text, point, color, defaultSize);

        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public void DrawString(string text, Point point, int size) => DrawString(text, point, Color.Black, size);

        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public void DrawString(string text, Point point, Color color, int size)
            => DrawString(text, point, color, size, FontStyle.Regular);

        /// <summary>
        /// Draws a string at the specified point.
        /// </summary>
        public abstract void DrawString(string text, Point point, Color color, int size, FontStyle style);

        /// <summary>
        /// Draws a rectangle within the specified bounds.
        /// </summary>
        public abstract void DrawRectangle(Rectangle bounds, Color color);

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