using System.Collections.Generic;

namespace Xamarin.Forms.PancakeView
{
    public class PancakeView : ContentView
    {
        #region properties

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(PancakeView), default(CornerRadius));
        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(nameof(HasShadow), typeof(bool), typeof(PancakeView), default(bool));

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(nameof(BorderThickness), typeof(float), typeof(PancakeView), default(float));
        public static readonly BindableProperty BorderIsDashedProperty = BindableProperty.Create(nameof(BorderIsDashed), typeof(bool), typeof(PancakeView), default(bool));
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(PancakeView), default(Color));
        public static readonly BindableProperty BorderDrawingStyleProperty = BindableProperty.Create(nameof(BorderDrawingStyle), typeof(BorderDrawingStyle), typeof(PancakeView), defaultValue: BorderDrawingStyle.Inside);

        public static readonly BindableProperty BackgroundGradientStartColorProperty = BindableProperty.Create(nameof(BackgroundGradientStartColor), typeof(Color), typeof(PancakeView), defaultValue: default(Color));
        public static readonly BindableProperty BackgroundGradientEndColorProperty = BindableProperty.Create(nameof(BackgroundGradientEndColor), typeof(Color), typeof(PancakeView), defaultValue: default(Color));
        public static readonly BindableProperty BackgroundGradientAngleProperty = BindableProperty.Create(nameof(BackgroundGradientAngle), typeof(int), typeof(PancakeView), defaultValue: default(int));

        public static readonly BindableProperty BackgroundGradientStopsProperty = BindableProperty.Create(nameof(BackgroundGradientStops), typeof(IEnumerable<GradientStop>), typeof(PancakeView), defaultValue: default(IEnumerable<GradientStop>));

        public Color BackgroundGradientStartColor
        {
            get { return (Color)GetValue(BackgroundGradientStartColorProperty); }
            set { SetValue(BackgroundGradientStartColorProperty, value); }
        }

        public Color BackgroundGradientEndColor
        {
            get { return (Color)GetValue(BackgroundGradientEndColorProperty); }
            set { SetValue(BackgroundGradientEndColorProperty, value); }
        }

        public int BackgroundGradientAngle
        {
            get { return (int)GetValue(BackgroundGradientAngleProperty); }
            set { SetValue(BackgroundGradientAngleProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public float BorderThickness
        {
            get { return (float)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public bool BorderIsDashed
        {
            get { return (bool)GetValue(BorderIsDashedProperty); }
            set { SetValue(BorderIsDashedProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

        public BorderDrawingStyle BorderDrawingStyle
        {
            get { return (BorderDrawingStyle)GetValue(BorderDrawingStyleProperty); }
            set { SetValue(BorderDrawingStyleProperty, value); }
        }

        public IEnumerable<GradientStop> BackgroundGradientStops
        {
            get { return (IEnumerable<GradientStop>)GetValue(BackgroundGradientStopsProperty); }
            set { SetValue(BackgroundGradientStopsProperty, value); }
        }

        #endregion
    }
}