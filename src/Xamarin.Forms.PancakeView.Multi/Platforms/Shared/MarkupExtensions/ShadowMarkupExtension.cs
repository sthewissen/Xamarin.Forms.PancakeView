using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.PancakeView
{
    [AcceptEmptyServiceProvider]
    public class ShadowMarkupExtension : BindableObject, IMarkupExtension<DropShadow>
    {
        public static BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(ShadowMarkupExtension), DropShadow.ColorProperty.DefaultValue); 

        public float BlurRadius { set; get; } = (float)DropShadow.BlurRadiusProperty.DefaultValue;

        public Color Color
        {
            set => SetValue(ColorProperty, value);
            get => (Color)GetValue(ColorProperty);
        }

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