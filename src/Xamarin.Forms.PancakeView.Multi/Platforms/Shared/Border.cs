using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.PancakeView
{
    public class Border : BindableObject, IPropagateChanges
    {
        public Action PropagatePropertyChanged { get; set; }

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(nameof(BorderThickness),
            typeof(Thickness), typeof(PancakeView), default(Thickness));

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor),
            typeof(Color), typeof(PancakeView), default(Color));

        public static readonly BindableProperty BorderDashPatternProperty = BindableProperty.Create(nameof(BorderDashPattern),
            typeof(DashPattern), typeof(PancakeView), defaultValue: default(DashPattern),
            defaultValueCreator: bindable => { return new DashPattern(); },
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var dash = (DashPattern)newValue;

                // Needs to be an even number of parts, but if its null or 0 elements, we simply don't dash.
                if (dash.Pattern.Length != 0 && (dash.Pattern?.Length >= 2 && dash.Pattern.Length % 2 != 0))
                    throw new ArgumentException($"{nameof(BorderDashPattern)} must contain an even number of entries (>=2).", nameof(BorderDashPattern));
            });

        public static readonly BindableProperty BorderDrawingStyleProperty = BindableProperty.Create(nameof(BorderDrawingStyle),
            typeof(BorderDrawingStyle), typeof(PancakeView), defaultValue: BorderDrawingStyle.Inside);

        public static readonly BindableProperty BorderGradientAngleProperty = BindableProperty.Create(nameof(BorderGradientAngle),
            typeof(int), typeof(PancakeView), defaultValue: default(int),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                // Angle needs to be between 0-360.
                if ((int)newValue < 0 || (int)newValue > 360)
                    throw new ArgumentException($"Please provide a valid {nameof(BorderGradientAngle)}.", nameof(BorderGradientAngle));
            });

        public static readonly BindableProperty BorderGradientStopsProperty = BindableProperty.Create(nameof(BorderGradientStops),
            typeof(GradientStopCollection), typeof(PancakeView), defaultValue: default(GradientStopCollection),
            defaultValueCreator: bindable =>
            {
                return new GradientStopCollection();
            },
            propertyChanging: (bindable, oldvalue, newvalue) =>
            {
                if (oldvalue != null)
                {
                    (bindable as Border).AddRemovePropagation(false);
                }
            },
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                if (newvalue != null)
                {
                    (bindable as Border).AddRemovePropagation(true);
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
            set { SetValue(BorderDashPatternProperty, value); }
        }

        public BorderDrawingStyle BorderDrawingStyle
        {
            get { return (BorderDrawingStyle)GetValue(BorderDrawingStyleProperty); }
            set { SetValue(BorderDrawingStyleProperty, value); }
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

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == BorderColorProperty.PropertyName ||
                propertyName == BorderDashPatternProperty.PropertyName ||
                propertyName == BorderDrawingStyleProperty.PropertyName ||
                propertyName == BorderGradientAngleProperty.PropertyName ||
                propertyName == BorderThicknessProperty.PropertyName ||
                propertyName == BorderGradientStopsProperty.PropertyName)
            {
                PropagatePropertyChanged?.Invoke();
            }
        }

        private void AddRemovePropagation(bool add)
        {
            if (add)
                BorderGradientStops.CollectionChanged += BorderGradientStops_CollectionChanged;
            else
                BorderGradientStops.CollectionChanged -= BorderGradientStops_CollectionChanged;
        }

        private void BorderGradientStops_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropagatePropertyChanged?.Invoke();
        }
    }
}
