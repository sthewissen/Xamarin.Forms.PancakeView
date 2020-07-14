using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.PancakeView
{
    [AcceptEmptyServiceProvider]
    public class BorderMarkupExtension : BindableObject, IMarkupExtension<Border>
    {
        public static BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(BorderMarkupExtension), DropShadow.ColorProperty.DefaultValue);

        public int Thickness { set; get; } = (int)Border.ThicknessProperty.DefaultValue;

        public Color Color
        {
            set => SetValue(ColorProperty, value);
            get => (Color)GetValue(ColorProperty);
        }

        public DashPattern DashPattern { set; get; } = (DashPattern)Border.DashPatternProperty.DefaultValue;

        public Point GradientStartPoint { set; get; } = (Point)Border.GradientStartPointProperty.DefaultValue;

        public Point GradientEndPoint { set; get; } = (Point)Border.GradientEndPointProperty.DefaultValue;

        public Border ProvideValue(IServiceProvider serviceProvider)
        {
            return new Border
            {
                Color = Color,
                DashPattern = DashPattern,
                GradientStartPoint = GradientStartPoint,
                GradientEndPoint = GradientEndPoint,
                Thickness = Thickness
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<Border>).ProvideValue(serviceProvider);
        }
    }
}
