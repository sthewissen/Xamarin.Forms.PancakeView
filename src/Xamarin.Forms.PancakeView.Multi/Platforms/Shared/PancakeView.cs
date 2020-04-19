using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xamarin.Forms.PancakeView
{
    public class PancakeView : ContentView
    {
        // Propagates changes from our custom BindableObject implementations
        // up to the actual PancakeView. That way it knows it should redraw.
        private PropertyPropagator propertyChangedPropagator;
        private PropertyPropagator PropertyChangedPropagator
        {
            get
            {
                if (propertyChangedPropagator == null)
                {
                    propertyChangedPropagator = new PropertyPropagator(this, OnPropertyChanged,
                        nameof(Shadow.Color), nameof(Shadow.BlurRadius), nameof(Shadow.Offset), nameof(Shadow.Opacity)
                    );
                }

                return propertyChangedPropagator;
            }
        }

        public static readonly BindableProperty SidesProperty = BindableProperty.Create(nameof(Sides),
            typeof(int), typeof(PancakeView), defaultValue: 4);

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius),
            typeof(CornerRadius), typeof(PancakeView), default(CornerRadius));

        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(nameof(HasShadow),
            typeof(bool), typeof(PancakeView), default(bool));

        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(nameof(Elevation),
            typeof(int), typeof(PancakeView), 0);

        public static readonly BindableProperty ShadowProperty = BindableProperty.Create(nameof(Shadow),
            typeof(DropShadow), typeof(PancakeView), defaultValue: default(DropShadow));

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(nameof(BorderThickness),
            typeof(float), typeof(PancakeView), default(float));

        public static readonly BindableProperty BorderIsDashedProperty = BindableProperty.Create(nameof(BorderIsDashed),
            typeof(bool), typeof(PancakeView), default(bool));

        public static readonly BindableProperty BorderDashPatternProperty = BindableProperty.Create(nameof(BorderDashPattern),
            typeof(DashPattern), typeof(PancakeView), defaultValue: default(DashPattern), defaultValueCreator: bindable =>
            {
                return new DashPattern();
            });

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor),
            typeof(Color), typeof(PancakeView), default(Color));

        public static readonly BindableProperty BorderDrawingStyleProperty = BindableProperty.Create(nameof(BorderDrawingStyle),
            typeof(BorderDrawingStyle), typeof(PancakeView), defaultValue: BorderDrawingStyle.Inside);

        public static readonly BindableProperty BackgroundGradientStartColorProperty = BindableProperty.Create(nameof(BackgroundGradientStartColor),
            typeof(Color), typeof(PancakeView), defaultValue: default(Color));

        public static readonly BindableProperty BackgroundGradientEndColorProperty = BindableProperty.Create(nameof(BackgroundGradientEndColor),
            typeof(Color), typeof(PancakeView), defaultValue: default(Color));

        public static readonly BindableProperty BackgroundGradientAngleProperty = BindableProperty.Create(nameof(BackgroundGradientAngle),
            typeof(int), typeof(PancakeView), defaultValue: default(int));

        public static readonly BindableProperty BackgroundGradientStopsProperty = BindableProperty.Create(nameof(BackgroundGradientStops),
            typeof(GradientStopCollection), typeof(PancakeView), defaultValue: default(GradientStopCollection),
            defaultValueCreator: bindable =>
            {
                return new GradientStopCollection();
            });

        public static readonly BindableProperty BorderGradientStartColorProperty = BindableProperty.Create(nameof(BorderGradientStartColor),
            typeof(Color), typeof(PancakeView), defaultValue: default(Color));

        public static readonly BindableProperty BorderGradientEndColorProperty = BindableProperty.Create(nameof(BorderGradientEndColor),
            typeof(Color), typeof(PancakeView), defaultValue: default(Color));

        public static readonly BindableProperty BorderGradientAngleProperty = BindableProperty.Create(nameof(BorderGradientAngle),
            typeof(int), typeof(PancakeView), defaultValue: default(int));

        public static readonly BindableProperty BorderGradientStopsProperty = BindableProperty.Create(nameof(BorderGradientStops),
            typeof(GradientStopCollection), typeof(PancakeView), defaultValue: default(GradientStopCollection),
            defaultValueCreator: bindable =>
            {
                return new GradientStopCollection();
            });

        public static readonly BindableProperty OffsetAngleProperty = BindableProperty.Create(nameof(OffsetAngle),
            typeof(double), typeof(PancakeView), default(double));

        public int Sides
        {
            get { return (int)GetValue(SidesProperty); }
            set { SetValue(SidesProperty, value); }
        }

        [Obsolete("This property has been replaced by the BackgroundGradientStops property. Please use that instead.")]
        public Color BackgroundGradientStartColor
        {
            get { return (Color)GetValue(BackgroundGradientStartColorProperty); }
            set { SetValue(BackgroundGradientStartColorProperty, value); }
        }

        [Obsolete("This property has been replaced by the BackgroundGradientStops property. Please use that instead.")]
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

        public GradientStopCollection BackgroundGradientStops
        {
            get { return (GradientStopCollection)GetValue(BackgroundGradientStopsProperty); }
            set { SetValue(BackgroundGradientStopsProperty, value); }
        }

        [Obsolete("This property has been replaced by the BorderGradientStops property. Please use that instead.")]
        public Color BorderGradientStartColor
        {
            get { return (Color)GetValue(BorderGradientStartColorProperty); }
            set { SetValue(BorderGradientStartColorProperty, value); }
        }

        [Obsolete("This property has been replaced by the BorderGradientStops property. Please use that instead.")]
        public Color BorderGradientEndColor
        {
            get { return (Color)GetValue(BorderGradientEndColorProperty); }
            set { SetValue(BorderGradientEndColorProperty, value); }
        }

        public int BorderGradientAngle
        {
            get { return (int)GetValue(BorderGradientAngleProperty); }
            set { SetValue(BorderGradientAngleProperty, value); }
        }

        public GradientStopCollection BorderGradientStops
        {
            get { return (GradientStopCollection)GetValue(BorderGradientStopsProperty); }
            set { SetValue(BorderGradientStopsProperty, value); }
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

        [Obsolete("This property has been replaced by the BorderDashPattern property. Please use that instead.")]
        public bool BorderIsDashed
        {
            get { return (bool)GetValue(BorderIsDashedProperty); }
            set { SetValue(BorderIsDashedProperty, value); }
        }

        public DashPattern BorderDashPattern
        {
            get { return (DashPattern)GetValue(BorderDashPatternProperty); }
            set { SetValue(BorderDashPatternProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        [Obsolete("This property has been replaced by the Shadow property. Please use that instead.")]
        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

        [Obsolete("This property has been replaced by the Shadow property. Please use that instead.")]
        public int Elevation
        {
            get { return (int)GetValue(ElevationProperty); }
            set { SetValue(ElevationProperty, value); }
        }

        public DropShadow Shadow
        {
            get { return PropertyChangedPropagator.GetValue<DropShadow>(ShadowProperty); }
            set { PropertyChangedPropagator.SetValue(ShadowProperty, ref value); }
        }

        public BorderDrawingStyle BorderDrawingStyle
        {
            get { return (BorderDrawingStyle)GetValue(BorderDrawingStyleProperty); }
            set { SetValue(BorderDrawingStyleProperty, value); }
        }

        public double OffsetAngle
        {
            get { return (double)GetValue(OffsetAngleProperty); }
            set { SetValue(OffsetAngleProperty, value); }
        }
    }
}