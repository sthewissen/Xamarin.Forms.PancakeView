﻿using System;
using Xamarin.Forms.PancakeView.Platforms.Shared;

namespace Xamarin.Forms.PancakeView
{
    public class PancakeView : ContentView
    {
        #region properties

        public static readonly BindableProperty SidesProperty = BindableProperty.Create(nameof(Sides), typeof(int), typeof(PancakeView), defaultValue: 4);
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(PancakeView), default(CornerRadius));
        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(nameof(HasShadow), typeof(bool), typeof(PancakeView), default(bool));
        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(nameof(Elevation), typeof(int), typeof(PancakeView), 0);


        public static readonly BindableProperty BorderEdgesProperty = BindableProperty.Create(
            propertyName: nameof(BorderEdges),
            returnType: typeof(Edge),
            declaringType: typeof(PancakeView),
            defaultValue: Edge.All
            );

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(nameof(BorderThickness), typeof(Thickness), typeof(PancakeView), default(Thickness));
        public static readonly BindableProperty BorderIsDashedProperty = BindableProperty.Create(nameof(BorderIsDashed), typeof(bool), typeof(PancakeView), default(bool));
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(PancakeView), default(Color));
        public static readonly BindableProperty BorderDrawingStyleProperty = BindableProperty.Create(nameof(BorderDrawingStyle), typeof(BorderDrawingStyle), typeof(PancakeView), defaultValue: BorderDrawingStyle.Inside);

        public static readonly BindableProperty BackgroundGradientStartColorProperty = BindableProperty.Create(nameof(BackgroundGradientStartColor), typeof(Color), typeof(PancakeView), defaultValue: default(Color));
        public static readonly BindableProperty BackgroundGradientEndColorProperty = BindableProperty.Create(nameof(BackgroundGradientEndColor), typeof(Color), typeof(PancakeView), defaultValue: default(Color));
        public static readonly BindableProperty BackgroundGradientAngleProperty = BindableProperty.Create(nameof(BackgroundGradientAngle), typeof(int), typeof(PancakeView), defaultValue: default(int));

        public static readonly BindableProperty BackgroundGradientStopsProperty = BindableProperty.Create(nameof(BackgroundGradientStops), typeof(GradientStopCollection), typeof(PancakeView), defaultValue: default(GradientStopCollection),
        defaultValueCreator: bindable =>
        {
            return new GradientStopCollection();
        });

        public static readonly BindableProperty BorderGradientStartColorProperty = BindableProperty.Create(nameof(BorderGradientStartColor), typeof(Color), typeof(PancakeView), defaultValue: default(Color));
        public static readonly BindableProperty BorderGradientEndColorProperty = BindableProperty.Create(nameof(BorderGradientEndColor), typeof(Color), typeof(PancakeView), defaultValue: default(Color));
        public static readonly BindableProperty BorderGradientAngleProperty = BindableProperty.Create(nameof(BorderGradientAngle), typeof(int), typeof(PancakeView), defaultValue: default(int));

        public static readonly BindableProperty BorderGradientStopsProperty = BindableProperty.Create(nameof(BorderGradientStops), typeof(GradientStopCollection), typeof(PancakeView), defaultValue: default(GradientStopCollection),
        defaultValueCreator: bindable =>
        {
            return new GradientStopCollection();
        });

        public static readonly BindableProperty OffsetAngleProperty = BindableProperty.Create(nameof(OffsetAngle), typeof(double), typeof(PancakeView), default(double));

        public Edge BorderEdges
        {
            get => (Edge)GetValue(BorderEdgesProperty);
            set => SetValue(BorderEdgesProperty, value);
        }

        public int Sides
        {
            get { return (int)GetValue(SidesProperty); }
            set { SetValue(SidesProperty, value); }
        }

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

        public GradientStopCollection BackgroundGradientStops
        {
            get { return (GradientStopCollection)GetValue(BackgroundGradientStopsProperty); }
            set { SetValue(BackgroundGradientStopsProperty, value); }
        }

        public Color BorderGradientStartColor
        {
            get { return (Color)GetValue(BorderGradientStartColorProperty); }
            set { SetValue(BorderGradientStartColorProperty, value); }
        }

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

        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
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

        public int Elevation
        {
            get { return (int)GetValue(ElevationProperty); }
            set { SetValue(ElevationProperty, value); }
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

        #endregion
    }
}