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
            for (int i = 0; i < GradientStops.Count; i++)
                SetInheritedBindingContext(GradientStops[i], BindingContext);
        }

        public static readonly BindableProperty ThicknessProperty = BindableProperty.Create(nameof(Thickness),
            typeof(int), typeof(Border), default(int));

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color),
            typeof(Color), typeof(Border), default(Color));

        public static readonly BindableProperty DashPatternProperty = BindableProperty.Create(nameof(DashPattern),
            typeof(DashPattern), typeof(Border), defaultValue: default(DashPattern),
            defaultValueCreator: bindable => { return new DashPattern(); });

        public static readonly BindableProperty DrawingStyleProperty = BindableProperty.Create(nameof(DrawingStyle),
            typeof(BorderDrawingStyle), typeof(Border), defaultValue: BorderDrawingStyle.Inside);

        public static readonly BindableProperty GradientStartPointProperty = BindableProperty.Create(
             nameof(GradientStartPoint), typeof(Point), typeof(Border), new Point(0, 0));

        public static readonly BindableProperty GradientEndPointProperty = BindableProperty.Create(
            nameof(GradientEndPoint), typeof(Point), typeof(Border), new Point(1, 0));

        public static readonly BindableProperty GradientStopsProperty = BindableProperty.Create(nameof(GradientStops),
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

        public int Thickness
        {
            get { return (int)GetValue(ThicknessProperty); }
            set
            {
                if (value < 0)
                    throw new ArgumentException($"{nameof(Thickness)} must be greater than or equal to zero.", nameof(Thickness));

                SetValue(ThicknessProperty, value);
            }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public DashPattern DashPattern
        {
            get { return (DashPattern)GetValue(DashPatternProperty); }
            set
            {
                if (value.Pattern != null && value.Pattern.Length != 0 && (value.Pattern?.Length >= 2 && value.Pattern.Length % 2 != 0))
                    throw new ArgumentException($"{nameof(DashPattern)} must contain an even number of entries (>=2).", nameof(DashPattern));

                SetValue(DashPatternProperty, value);
            }
        }

        public Point GradientStartPoint
        {
            get => (Point)GetValue(GradientStartPointProperty);
            set => SetValue(GradientStartPointProperty, value);
        }

        public Point GradientEndPoint
        {
            get => (Point)GetValue(GradientEndPointProperty);
            set => SetValue(GradientEndPointProperty, value);
        }

        public BorderDrawingStyle DrawingStyle
        {
            get { return (BorderDrawingStyle)GetValue(DrawingStyleProperty); }
            set { SetValue(DrawingStyleProperty, value); }
        }

        public GradientStopCollection GradientStops
        {
            get { return (GradientStopCollection)GetValue(GradientStopsProperty); }
            set { SetValue(GradientStopsProperty, value); }
        }

        void SetupInternalCollectionPropertyPropagation(bool teardown = false)
        {
            if (teardown && GradientStops != null)
            {
                GradientStops.CollectionChanged -= InternalCollectionChanged;
            }
            else if (GradientStops != null)
            {
                GradientStops.CollectionChanged += InternalCollectionChanged;
            }
        }

        void InternalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(PancakeView.BorderProperty.PropertyName);

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == ColorProperty.PropertyName ||
                propertyName == DashPatternProperty.PropertyName ||
                propertyName == DrawingStyleProperty.PropertyName ||
                propertyName == GradientStartPointProperty.PropertyName ||
                propertyName == GradientEndPointProperty.PropertyName ||
                propertyName == ThicknessProperty.PropertyName ||
                propertyName == nameof(GradientStops))
            {
                OnPropertyChanged(PancakeView.BorderProperty.PropertyName);
            }
        }
    }
}
