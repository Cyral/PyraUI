using Pyratron.UI.Types;

namespace Pyratron.UI
{
    public abstract class Renderer
    {
        public abstract void BeginDraw();
        public abstract void EndDraw();
        public abstract void DrawTexture(string name, Rectangle rectangle);
        public abstract void DrawTexture(string name, Rectangle rectangle, Color color);
        public abstract void DrawString(string text, Point point);
        public abstract void DrawString(string text, Point point, Color color);
        public abstract void DrawString(string text, Point point, float pt);
        public abstract void DrawRectangle(Rectangle bounds, Color color);

        public abstract Size MeasureText(string text);
    }
}