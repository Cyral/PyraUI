using Pyratron.UI.Types;
using Pyratron.UI.Types.Properties;

namespace Pyratron.UI.Effects
{
    internal class DropShadowEffect : Effect
    {
        /// <summary>
        /// The amount to blur the shadow.
        /// </summary>
        public int BlurRadius
        {
            get { return GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        public static readonly DependencyProperty<int> BlurRadiusProperty =
          DependencyProperty.Register<DropShadowEffect, int>(nameof(BlurRadius), 10, new PropertyMetadata(MetadataOption.IgnoreInheritance));

        /// <summary>
        /// The color of the drop shadow.
        /// </summary>
        public Color Color
        {
            get { return GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty<Color> ColorProperty =
          DependencyProperty.Register<DropShadowEffect, Color>(nameof(Color), Color.Black * .2f, new PropertyMetadata(MetadataOption.IgnoreInheritance));


        public DropShadowEffect(Manager manager) : base(manager)
        {
        }

        public DropShadowEffect(Manager manager, int blurRadius, Color color) : base(manager)
        {
            BlurRadius = blurRadius;
            Color = color;
        }

        public override void Render(Rectangle extendedArea, Rectangle contentArea, float delta)
        {
            Manager.Renderer.StretchRectangle(contentArea, Color, contentArea.AddBorder(BlurRadius));
        }
    }
}