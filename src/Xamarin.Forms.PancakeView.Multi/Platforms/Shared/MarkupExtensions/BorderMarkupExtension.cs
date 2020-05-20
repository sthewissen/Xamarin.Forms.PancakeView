using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.PancakeView
{
    [AcceptEmptyServiceProvider]
    public class BorderMarkupExtension : IMarkupExtension<Border>
    {
        public Thickness BorderThickness { set; get; } = (Thickness)Border.BorderThicknessProperty.DefaultValue;

        public Color BorderColor { set; get; } = (Color)Border.BorderColorProperty.DefaultValue;

        public DashPattern BorderDashPattern { set; get; } = (DashPattern)Border.BorderDashPatternProperty.DefaultValue;

        public int BorderGradientAngle { set; get; } = (int)Border.BorderGradientAngleProperty.DefaultValue;

        public Border ProvideValue(IServiceProvider serviceProvider)
        {
            return new Border
            {
                BorderColor = BorderColor,
                BorderDashPattern = BorderDashPattern,
                BorderGradientAngle = BorderGradientAngle,
                BorderThickness = BorderThickness
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<Border>).ProvideValue(serviceProvider);
        }
    }
}
