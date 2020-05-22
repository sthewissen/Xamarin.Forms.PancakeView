using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.PancakeView
{
    [AcceptEmptyServiceProvider]
    public class BorderMarkupExtension : IMarkupExtension<Border>
    {
        public Thickness Thickness { set; get; } = (Thickness)Border.ThicknessProperty.DefaultValue;

        public Color Color { set; get; } = (Color)Border.ColorProperty.DefaultValue;

        public DashPattern DashPattern { set; get; } = (DashPattern)Border.DashPatternProperty.DefaultValue;

        public int GradientAngle { set; get; } = (int)Border.GradientAngleProperty.DefaultValue;

        public Border ProvideValue(IServiceProvider serviceProvider)
        {
            return new Border
            {
                Color = Color,
                DashPattern = DashPattern,
                GradientAngle = GradientAngle,
                Thickness = Thickness
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<Border>).ProvideValue(serviceProvider);
        }
    }
}
