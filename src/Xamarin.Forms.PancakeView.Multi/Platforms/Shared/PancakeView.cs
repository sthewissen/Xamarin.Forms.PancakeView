using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Xamarin.Forms.PancakeView
{
    public class PancakeView : ContentView
    {
        public static readonly BindableProperty SidesProperty = BindableProperty.Create(nameof(Sides), typeof(int), typeof(PancakeView), defaultValue: 4);
        public static readonly BindableProperty OffsetAngleProperty = BindableProperty.Create(nameof(OffsetAngle), typeof(double), typeof(PancakeView), default(double));

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(PancakeView), default(CornerRadius));

        public static readonly BindableProperty BackgroundGradientStartPointProperty = BindableProperty.Create(nameof(BackgroundGradientStartPoint), typeof(Point), typeof(Border), new Point(0, 0));
        public static readonly BindableProperty BackgroundGradientEndPointProperty = BindableProperty.Create(nameof(BackgroundGradientEndPoint), typeof(Point), typeof(Border), new Point(1, 0));

        public static readonly BindableProperty BackgroundGradientStopsProperty = BindableProperty.Create(nameof(BackgroundGradientStops),
            typeof(GradientStopCollection), typeof(PancakeView), defaultValue: default(GradientStopCollection),
            defaultValueCreator: bindable =>
            {
                return new GradientStopCollection();
            },
            propertyChanging: (bindable, oldvalue, newvalue) =>
            {
                if (oldvalue != null)
                {
                    (bindable as PancakeView).SetupInternalCollectionPropertyPropagation(true);
                }
            },
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                if (newvalue != null)
                {
                    (bindable as PancakeView).SetupInternalCollectionPropertyPropagation();
                }
            });

        public static readonly BindableProperty ShadowProperty = BindableProperty.Create(nameof(Shadow),
            typeof(DropShadow), typeof(PancakeView), defaultValue: default(DropShadow),
            propertyChanging: (bindable, oldvalue, newvalue) =>
            {
                if (bindable != null && oldvalue != null)
                {
                    (bindable as PancakeView).SetupInternalPropertyPropagation(oldvalue as DropShadow, true);
                }
            },
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                if (bindable != null && newvalue != null)
                {
                    (bindable as PancakeView).SetupInternalPropertyPropagation(newvalue as DropShadow);
                }
            });

        public static readonly BindableProperty BorderProperty = BindableProperty.Create(nameof(Border),
            typeof(Border), typeof(PancakeView), defaultValue: default(Border),
            propertyChanging: (bindable, oldvalue, newvalue) =>
            {
                if (bindable != null && oldvalue != null)
                {
                    (bindable as PancakeView).SetupInternalPropertyPropagation(oldvalue as Border, true);
                }
            },
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                if (bindable != null && newvalue != null)
                {
                    (bindable as PancakeView).SetupInternalPropertyPropagation(newvalue as Border);
                }
            });

        public int Sides
        {
            get { return (int)GetValue(SidesProperty); }
            set
            {
                // min value for sides is 3
                if (value < 3)
                    throw new ArgumentException($"Please provide a valid value for {nameof(Sides)}.", nameof(Sides));

                SetValue(SidesProperty, value);
            }
        }

        public double OffsetAngle
        {
            get { return (double)GetValue(OffsetAngleProperty); }
            set
            {
                if (value < 0 || value > 360)
                    throw new ArgumentException($"Please provide a valid {nameof(OffsetAngle)}.", nameof(OffsetAngle));

                SetValue(OffsetAngleProperty, value);
            }
        }

        public GradientStopCollection BackgroundGradientStops
        {
            get { return (GradientStopCollection)GetValue(BackgroundGradientStopsProperty); }
            set { SetValue(BackgroundGradientStopsProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public DropShadow Shadow
        {
            get { return (DropShadow)GetValue(ShadowProperty); }
            set { SetValue(ShadowProperty, value); }
        }

        public Border Border
        {
            get { return (Border)GetValue(BorderProperty); }
            set { SetValue(BorderProperty, value); }
        }

        public Point BackgroundGradientStartPoint
        {
            get => (Point)GetValue(BackgroundGradientStartPointProperty);
            set => SetValue(BackgroundGradientStartPointProperty, value);
        }

        public Point BackgroundGradientEndPoint
        {
            get => (Point)GetValue(BackgroundGradientEndPointProperty);
            set => SetValue(BackgroundGradientEndPointProperty, value);
        }

        //public new IVisual Visual => VisualMarker.Default;

        public PancakeView()
        {
            Visual = VisualMarker.Default;
        }

        void SetupInternalCollectionPropertyPropagation(bool teardown = false)
        {
            if (teardown && BackgroundGradientStops != null)
            {
                BackgroundGradientStops.CollectionChanged -= InternalCollectionChanged;
            }
            else if (BackgroundGradientStops != null)
            {
                BackgroundGradientStops.CollectionChanged += InternalCollectionChanged;
            }
        }

        void InternalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(BackgroundGradientStopsProperty.PropertyName);

        void SetupInternalPropertyPropagation(BindableObject bindableObject, bool teardown = false)
        {
            if (teardown)
            {
                bindableObject.PropertyChanged -= InternalPropertyChanged;
            }
            else
            {
                bindableObject.PropertyChanged += InternalPropertyChanged;
            }
        }

        void InternalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // Propagate the binding context down to the shadow object.
            var shadow = (DropShadow)GetValue(ShadowProperty);
            if (shadow != null)
            {
                SetInheritedBindingContext(shadow, BindingContext);
            }

            // Propagate the binding context down to the border object.
            var border = (Border)GetValue(BorderProperty);
            if (border != null)
            {
                SetInheritedBindingContext(border, BindingContext);
            }

            // Propagate the binding context down to all the gradient stop objects.
            var backgroundGradientStops = (GradientStopCollection)GetValue(BackgroundGradientStopsProperty);
            if (backgroundGradientStops != null)
            {
                foreach (var item in backgroundGradientStops)
                    SetInheritedBindingContext(item, BindingContext);
            }

            // Propagate the binding context down to all the gradient stop objects.
            var borderGradientStops = (GradientStopCollection)GetValue(BorderGradientStopsProperty);
            if (borderGradientStops != null)
            {
                foreach (var item in borderGradientStops)
                    SetInheritedBindingContext(item, BindingContext);
            }
        }

        #region SOON TO BE DEPRECATED

        public static readonly BindableProperty BackgroundGradientAngleProperty = BindableProperty.Create(nameof(BackgroundGradientAngle), typeof(double), typeof(PancakeView), default(double));

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(nameof(BorderThickness), typeof(float), typeof(PancakeView), default(float));

        public static readonly BindableProperty BorderDashPatternProperty = BindableProperty.Create(nameof(BorderDashPattern), typeof(DashPattern), typeof(PancakeView), defaultValue: default(DashPattern), defaultValueCreator: bindable => { return new DashPattern(); });

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(PancakeView), default(Color));

        public static readonly BindableProperty BorderDrawingStyleProperty = BindableProperty.Create(nameof(BorderDrawingStyle), typeof(BorderDrawingStyle), typeof(PancakeView), defaultValue: BorderDrawingStyle.Inside);

        public static readonly BindableProperty BorderGradientAngleProperty = BindableProperty.Create(nameof(BorderGradientAngle), typeof(int), typeof(PancakeView), defaultValue: default(int));

        public static readonly BindableProperty BorderGradientStopsProperty = BindableProperty.Create(nameof(BorderGradientStops),
            typeof(GradientStopCollection), typeof(PancakeView), defaultValue: default(GradientStopCollection),
            defaultValueCreator: bindable =>
            {
                return new GradientStopCollection();
            });

        public static readonly BindableProperty BorderGradientStartColorProperty = BindableProperty.Create(nameof(BorderGradientStartColor),
            typeof(Color), typeof(PancakeView), defaultValue: default(Color));

        public static readonly BindableProperty BorderGradientEndColorProperty = BindableProperty.Create(nameof(BorderGradientEndColor),
            typeof(Color), typeof(PancakeView), defaultValue: default(Color));

        public static readonly BindableProperty BackgroundGradientStartColorProperty = BindableProperty.Create(nameof(BackgroundGradientStartColor),
            typeof(Color), typeof(PancakeView), defaultValue: default(Color));

        public static readonly BindableProperty BackgroundGradientEndColorProperty = BindableProperty.Create(nameof(BackgroundGradientEndColor),
            typeof(Color), typeof(PancakeView), defaultValue: default(Color));

        public static readonly BindableProperty BorderIsDashedProperty = BindableProperty.Create(nameof(BorderIsDashed),
            typeof(bool), typeof(PancakeView), default(bool));

        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(nameof(HasShadow),
            typeof(bool), typeof(PancakeView), default(bool));

        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(nameof(Elevation),
            typeof(int), typeof(PancakeView), 0);


        [Obsolete("This property has been obsoleted. Please use BackgroundGradientStartPoint and BackgroundGradientEndPoint instead.")]
        public double BackgroundGradientAngle
        {
            get { return (double)GetValue(BackgroundGradientAngleProperty); }
            set
            {
                if (value < 0 || value > 360)
                    throw new ArgumentException($"Please provide a valid {nameof(BackgroundGradientAngle)}.", nameof(BackgroundGradientAngle));

                SetValue(BackgroundGradientAngleProperty, value);
            }
        }

        [Obsolete("This property has been obsoleted. Please use Border instead.")]
        public float BorderThickness
        {
            get { return (float)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use Border instead.")]
        public DashPattern BorderDashPattern
        {
            get { return (DashPattern)GetValue(BorderDashPatternProperty); }
            set { SetValue(BorderDashPatternProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use Border instead.")]
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use Border instead.")]
        public BorderDrawingStyle BorderDrawingStyle
        {
            get { return (BorderDrawingStyle)GetValue(BorderDrawingStyleProperty); }
            set { SetValue(BorderDrawingStyleProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use Border instead.")]
        public int BorderGradientAngle
        {
            get { return (int)GetValue(BorderGradientAngleProperty); }
            set { SetValue(BorderGradientAngleProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use Border instead.")]
        public GradientStopCollection BorderGradientStops
        {
            get { return (GradientStopCollection)GetValue(BorderGradientStopsProperty); }
            set { SetValue(BorderGradientStopsProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use BackgroundGradientStops instead.")]
        public Color BackgroundGradientStartColor
        {
            get { return (Color)GetValue(BackgroundGradientStartColorProperty); }
            set { SetValue(BackgroundGradientStartColorProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use BackgroundGradientStops instead.")]
        public Color BackgroundGradientEndColor
        {
            get { return (Color)GetValue(BackgroundGradientEndColorProperty); }
            set { SetValue(BackgroundGradientEndColorProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use BorderGradientStops instead.")]
        public Color BorderGradientStartColor
        {
            get { return (Color)GetValue(BorderGradientStartColorProperty); }
            set { SetValue(BorderGradientStartColorProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use BorderGradientStops instead.")]
        public Color BorderGradientEndColor
        {
            get { return (Color)GetValue(BorderGradientEndColorProperty); }
            set { SetValue(BorderGradientEndColorProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use BorderDashPattern instead.")]
        public bool BorderIsDashed
        {
            get { return (bool)GetValue(BorderIsDashedProperty); }
            set { SetValue(BorderIsDashedProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use Shadow instead.")]
        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

        [Obsolete("This property has been obsoleted. Please use Shadow instead.")]
        public int Elevation
        {
            get { return (int)GetValue(ElevationProperty); }
            set { SetValue(ElevationProperty, value); }
        }

        #endregion
    }
}