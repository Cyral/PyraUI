using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Brushes
{
    public class GradientBrush : Brush
    {
        public static readonly DependencyProperty<Color> StartProperty =
            DependencyProperty.Register<GradientBrush, Color>(nameof(ColorBrush), Color.Black);

        public static readonly DependencyProperty<Color> EndProperty =
            DependencyProperty.Register<GradientBrush, Color>(nameof(ColorBrush), Color.White);

        public Color Start
        {
            get { return GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        public Color End
        {
            get { return GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        public GradientBrush(Color start, Color end)
        {
            Start = start;
            End = end;
        }
    }
}