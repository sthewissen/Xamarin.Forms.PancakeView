using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Xamarin.Forms.PancakeView
{
    public class PancakeView : ContentView, IBorderElement
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

        int IBorderElement.CornerRadiusDefaultValue => default;
        Color IBorderElement.BorderColorDefaultValue => default;
        double IBorderElement.BorderWidthDefaultValue => default;
        int IBorderElement.CornerRadius => (int)CornerRadius.TopLeft;
        Color IBorderElement.BorderColor => Border.Color;
        double IBorderElement.BorderWidth => Border.Thickness;

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
        }

        bool IBorderElement.IsCornerRadiusSet() => true;
        bool IBorderElement.IsBackgroundColorSet() => true;
        bool IBorderElement.IsBackgroundSet() => true;
        bool IBorderElement.IsBorderColorSet() => true;
        bool IBorderElement.IsBorderWidthSet() => true;
        void IBorderElement.OnBorderColorPropertyChanged(Color oldValue, Color newValue) => Border.Color = newValue;
    }
}