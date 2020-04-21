using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Xamarin.Forms.PancakeView
{
    public class PancakeView : ContentView
    {
        public static readonly BindableProperty SidesProperty = BindableProperty.Create(nameof(Sides),
            typeof(int), typeof(PancakeView), defaultValue: 4);

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius),
            typeof(CornerRadius), typeof(PancakeView), default(CornerRadius));

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(nameof(BorderThickness),
            typeof(float), typeof(PancakeView), default(float));

        public static readonly BindableProperty BorderDashPatternProperty = BindableProperty.Create(nameof(BorderDashPattern),
            typeof(DashPattern), typeof(PancakeView), defaultValue: default(DashPattern), defaultValueCreator: bindable =>
            {
                return new DashPattern();
            });

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor),
            typeof(Color), typeof(PancakeView), default(Color));

        public static readonly BindableProperty BorderDrawingStyleProperty = BindableProperty.Create(nameof(BorderDrawingStyle),
            typeof(BorderDrawingStyle), typeof(PancakeView), defaultValue: BorderDrawingStyle.Inside);

        public static readonly BindableProperty BackgroundGradientAngleProperty = BindableProperty.Create(nameof(BackgroundGradientAngle),
            typeof(int), typeof(PancakeView), defaultValue: default(int));

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
                    // We do this to propagate property changed one level up from GradientStops to PancakeView.
                    var pancake = ((PancakeView)bindable);
                    pancake.RemoveBackgroundGradientStopsPropertyPropagation();
                    pancake.SetupGradientStopPropagation((IList<GradientStop>)oldvalue, (IList<GradientStop>)newvalue);
                }
            }, propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                if (newvalue != null)
                {
                    // We do this to propagate property changed one level up from GradientStops to PancakeView.
                    var pancake = ((PancakeView)bindable);
                    pancake.AddBackgroundGradientStopsPropertyPropagation();
                    pancake.SetupGradientStopPropagation((IList<GradientStop>)oldvalue, (IList<GradientStop>)newvalue);
                }
            });

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

        public static readonly BindableProperty ShadowProperty = BindableProperty.Create(nameof(Shadow),
            typeof(DropShadow), typeof(PancakeView), defaultValue: default(DropShadow),
            propertyChanging: (bindable, oldvalue, newvalue) =>
            {
                if (oldvalue != null)
                {
                    // We do this to propagate property changed one level up from DropShadow to PancakeView.
                    var pancake = ((PancakeView)bindable);
                    pancake.RemoveShadowPropertyPropagation();
                }
            }, propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                if (newvalue != null)
                {
                    // We do this to propagate property changed one level up from DropShadow to PancakeView.
                    var pancake = ((PancakeView)bindable);
                    pancake.AddShadowPropertyPropagation();
                }
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

        public int Sides
        {
            get { return (int)GetValue(SidesProperty); }
            set { SetValue(SidesProperty, value); }
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

        [Obsolete("This property has been obsoleted. Please use BorderDashPattern instead.")]
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

        public DropShadow Shadow
        {
            get { return (DropShadow)GetValue(ShadowProperty); }
            set { SetValue(ShadowProperty, value); }
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

        void AddBackgroundGradientStopsPropertyPropagation()
        {
            if (BackgroundGradientStops != null)
            {
                BackgroundGradientStops.CollectionChanged += BackgroundGradientStops_CollectionChanged;
            }
        }

        void RemoveBackgroundGradientStopsPropertyPropagation()
        {
            if (BackgroundGradientStops != null)
            {
                BackgroundGradientStops.CollectionChanged -= BackgroundGradientStops_CollectionChanged;
            }
        }

        void SetupGradientStopPropagation(IList<GradientStop> oldItems, IList<GradientStop> newItems)
        {
            if (oldItems != null)
            {
                foreach (object item in oldItems)
                {
                    var bo = item as GradientStop;

                    if (bo != null)
                    {
                        bo.PropertyChanging -= OnBackgroundGradientStopChanging;
                        bo.PropertyChanged -= OnBackgroundGradientStopChanged;
                    }
                }
            }

            if (newItems != null)
            {
                foreach (object item in newItems)
                {
                    var bo = item as GradientStop;

                    if (bo != null)
                    {
                        bo.PropertyChanging += OnBackgroundGradientStopChanging;
                        bo.PropertyChanged += OnBackgroundGradientStopChanged;
                    }
                }
            }
        }

        void BackgroundGradientStops_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(BackgroundGradientStops));
        }

        void OnBackgroundGradientStopChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(BackgroundGradientStops));
        }

        void OnBackgroundGradientStopChanging(object sender, PropertyChangingEventArgs e)
        {
            OnPropertyChanging(nameof(BackgroundGradientStops));
        }

        void AddShadowPropertyPropagation()
        {
            if (Shadow != null)
            {
                Shadow.PropertyChanging += OnShadowChanging;
                Shadow.PropertyChanged += OnShadowChanged;
            }
        }

        void RemoveShadowPropertyPropagation()
        {
            if (Shadow != null)
            {
                Shadow.PropertyChanged -= OnShadowChanged;
                Shadow.PropertyChanging -= OnShadowChanging;
            }
        }

        void OnShadowChanging(object sender, PropertyChangingEventArgs e)
        {
            OnPropertyChanging(nameof(Shadow));
        }

        void OnShadowChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Shadow));
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var shadow = (DropShadow)GetValue(ShadowProperty);

            if (shadow != null)
            {
                SetInheritedBindingContext(shadow, BindingContext);
            }

            var backgroundGradientStops = (GradientStopCollection)GetValue(BackgroundGradientStopsProperty);

            if (backgroundGradientStops != null)
            {
                foreach (var item in backgroundGradientStops)
                    SetInheritedBindingContext(item, BindingContext);
            }

            var borderGradientStops = (GradientStopCollection)GetValue(BorderGradientStopsProperty);

            if (borderGradientStops != null)
            {
                foreach (var item in borderGradientStops)
                    SetInheritedBindingContext(item, BindingContext);
            }
        }
    }
}