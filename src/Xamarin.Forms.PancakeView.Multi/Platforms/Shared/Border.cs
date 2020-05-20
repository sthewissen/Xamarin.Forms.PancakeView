using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.PancakeView
{
    public class Border : BindableObject
    {
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            for (int i = 0; i < BorderGradientStops.Count; i++)
                SetInheritedBindingContext(BorderGradientStops[i], BindingContext);
        }

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(nameof(BorderThickness),
            typeof(Thickness), typeof(Border), default(Thickness));

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor),
            typeof(Color), typeof(Border), default(Color));

        public static readonly BindableProperty BorderDashPatternProperty = BindableProperty.Create(nameof(BorderDashPattern),
            typeof(DashPattern), typeof(Border), defaultValue: default(DashPattern),
            defaultValueCreator: bindable => { return new DashPattern(); });

        public static readonly BindableProperty BorderDrawingStyleProperty = BindableProperty.Create(nameof(BorderDrawingStyle),
            typeof(BorderDrawingStyle), typeof(Border), defaultValue: BorderDrawingStyle.Inside);

        public static readonly BindableProperty BorderGradientAngleProperty = BindableProperty.Create(nameof(BorderGradientAngle),
            typeof(int), typeof(Border), defaultValue: default(int));

        public static readonly BindableProperty BorderGradientStopsProperty = BindableProperty.Create(nameof(BorderGradientStops),
            typeof(GradientStopCollection), typeof(Border), defaultValue: default(GradientStopCollection),
            defaultValueCreator: bindable =>
            {
                return new GradientStopCollection();
            },
            propertyChanging: (bindable, oldvalue, newvalue) =>
            {
                if (oldvalue != null)
                {
                    (bindable as Border).SetupInternalCollectionPropertyPropagation(true);
                }
            },
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                if (newvalue != null)
                {
                    (bindable as Border).SetupInternalCollectionPropertyPropagation();
                }
            });

        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public DashPattern BorderDashPattern
        {
            get { return (DashPattern)GetValue(BorderDashPatternProperty); }
            set
            {
                if (value.Pattern != null && value.Pattern.Length != 0 && (value.Pattern?.Length >= 2 && value.Pattern.Length % 2 != 0))
                    throw new ArgumentException($"{nameof(BorderDashPattern)} must contain an even number of entries (>=2).", nameof(BorderDashPattern));

                SetValue(BorderDashPatternProperty, value);
            }
        }

        public BorderDrawingStyle BorderDrawingStyle
        {
            get { return (BorderDrawingStyle)GetValue(BorderDrawingStyleProperty); }
            set { SetValue(BorderDrawingStyleProperty, value); }
        }

        public int BorderGradientAngle
        {
            get { return (int)GetValue(BorderGradientAngleProperty); }
            set
            {
                if (value < 0 || value > 360)
                    throw new ArgumentException($"Please provide a valid {nameof(BorderGradientAngle)}.", nameof(BorderGradientAngle));

                SetValue(BorderGradientAngleProperty, value);
            }
        }

        public GradientStopCollection BorderGradientStops
        {
            get { return (GradientStopCollection)GetValue(BorderGradientStopsProperty); }
            set { SetValue(BorderGradientStopsProperty, value); }
        }

        void SetupInternalCollectionPropertyPropagation(bool teardown = false)
        {
            if (teardown && BorderGradientStops != null)
            {
                BorderGradientStops.CollectionChanged -= InternalCollectionChanged;
            }
            else if (BorderGradientStops != null)
            {
                BorderGradientStops.CollectionChanged += InternalCollectionChanged;
            }
        }

        void InternalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(PancakeView.BorderProperty.PropertyName);

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == BorderColorProperty.PropertyName ||
                propertyName == BorderDashPatternProperty.PropertyName ||
                propertyName == BorderDrawingStyleProperty.PropertyName ||
                propertyName == BorderGradientAngleProperty.PropertyName ||
                propertyName == BorderThicknessProperty.PropertyName ||
                propertyName == nameof(BorderGradientStops))
            {
                OnPropertyChanged(PancakeView.BorderProperty.PropertyName);
            }
        }
    }
}
