using System.Linq;
using Pyratron.UI.Types;

namespace Pyratron.UI.Controls
{
    public class StackPanel : Panel
    {
        /// <summary>
        /// The direction controls will stack.
        /// </summary>
        public Orientation Orientation { get; set; }

        public StackPanel(Manager manager, Orientation orientation) : base(manager)
        {
            Orientation = orientation;
        }

        // ReSharper disable once IntroduceOptionalParameters.Global
        public StackPanel(Manager manager) : this(manager, Orientation.Vertical)
        {
            
        }

        public override int FindChildHeight()
        {
            // For horizontal layout, use the maximum height of a child element.
            if (Orientation == Orientation.Horizontal)
                return Elements.Select(child => child.ExtendedArea.Height).Max();
            return base.FindChildHeight();
        }

        internal override int CalcChildHeight(Element element)
        {
            if (Orientation == Orientation.Horizontal)
            {
                if ((FindChildHeight() > ContentArea.Height) && Height.Auto)
                {
                    Height.Value = FindChildHeight() + Padding.Top + Padding.Bottom;
                    // Force all child elements to fill the area.
                    foreach (var child in Elements)
                    {
                        child.Height.Value = FindChildHeight() - Padding.Top - Padding.Bottom;
                        child.Arrange(); // TODO: rearrange automatically.
                    }
                    Arrange(false);
                }
                else if (element.Height.Auto || element.VerticalAlignment == VerticalAlignment.Stretch)
                {
                    var val = ContentArea.Height - element.Margin.Top - element.Margin.Bottom;
                    if (val > Height.Min)
                        return val;
                }
            }
            else
            {
                // If the height of the parent is too small to contain this control, extend it.
                if ((element.ExtendedArea.Height + element.Position.Y > ContentArea.Height) ||
                    (FindChildHeight() > ContentArea.Height) && Height.Auto)
                {
                    Height.Value = FindChildHeight() + Padding.Top + Padding.Bottom;
                    // Arrange controls now that the height has been set.
                    Arrange(false);
                }
            }

            return element.Height;
        }

        internal override int CalcChildWidth(Element element)
        {
            if (Orientation == Orientation.Vertical)
            {
                // Stretch child controls to fill width.
                if (element.Width.Auto || element.HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    var val = ContentArea.Width - element.Margin.Left - element.Margin.Right;
                    if (val > Width.Min)
                        return val;
                }
                // If the width of the parent is too small to contain this control, extend it.
                if ((FindChildWidth() > ContentArea.Width) && Width.Auto)
                {
                    Width.Value = FindChildWidth() + Padding.Left + Padding.Right;
                    Arrange(false);
                }
            }
            else
            {
                // If the width of the parent is too small to contain this control, extend it.
                if ((element.ExtendedArea.Width + element.Position.X > ContentArea.Width) ||
                    (FindChildWidth() > ContentArea.Width) && Width.Auto)
                {
                    Width.Value = FindChildWidth() + Padding.Left + Padding.Right;
                    // Arrange controls now that the width has been set.
                    Arrange(false);
                }
            }
            return element.Width;
        }

        internal override Point AlignChild(Point pos, Element element)
        {
            if (Orientation == Orientation.Vertical)
            {
                var extra = 0;
                if (Parent != null)
                {
                    for (var i = 0; i < Elements.Count; i++)
                    {
                        // Until we hit this control, add the total height of child controls before this one.
                        if (Elements[i] == element) break;
                        extra += Elements[i].ExtendedArea.Height;
                    }
                }
                return base.AlignChild(new Point(pos.X, pos.Y + extra), element);
            }
            else
            {
                var extra = 0;
                if (Parent != null)
                {
                    for (var i = 0; i < Elements.Count; i++)
                    {
                        // Until we hit this control, add the total width of child controls before this one.
                        if (Elements[i] == element) break;
                        extra += Elements[i].ExtendedArea.Width;
                    }
                }
                return base.AlignChild(new Point(pos.X + extra, pos.Y), element);
            }
        }
    }
}