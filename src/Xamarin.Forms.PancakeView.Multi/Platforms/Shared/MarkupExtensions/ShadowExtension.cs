using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.PancakeView
{
    [AcceptEmptyServiceProvider]
    public class ShadowExtension : IMarkupExtension<DropShadow>
    {
        public float BlurRadius { set; get; } = (float)DropShadow.BlurRadiusProperty.DefaultValue;

        public Color Color { set; get; } = (Color)DropShadow.ColorProperty.DefaultValue;

        public Point Offset { set; get; } = (Point)DropShadow.OffsetProperty.DefaultValue;

        public float Opacity { set; get; } = (float)DropShadow.OpacityProperty.DefaultValue;

        public DropShadow ProvideValue(IServiceProvider serviceProvider)
        {
            return new DropShadow
            {
                BlurRadius = BlurRadius,
                Color = Color,
                Offset = Offset,
                Opacity = Opacity
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<DropShadow>).ProvideValue(serviceProvider);
        }
    }
}