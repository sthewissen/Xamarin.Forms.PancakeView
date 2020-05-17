using System;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.PancakeView
{
    public class DropShadow : BindableObject, IPropagateChanges
    {
        public Action PropagatePropertyChanged { get; set; }

        public static readonly BindableProperty BlurRadiusProperty = BindableProperty.Create(
            nameof(BlurRadius), typeof(float), typeof(DropShadow), 10.0f);

        public float BlurRadius
        {
            get => (float)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        public static readonly BindableProperty OpacityProperty = BindableProperty.Create(
            nameof(Opacity), typeof(float), typeof(DropShadow), 0.4f);

        public float Opacity
        {
            get => (float)GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
            nameof(Color), typeof(Color), typeof(DropShadow), Color.Black);

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly BindableProperty OffsetProperty = BindableProperty.Create(
            nameof(Offset), typeof(Point), typeof(DropShadow), default(Point));

        public Point Offset
        {
            get => (Point)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == DropShadow.BlurRadiusProperty.PropertyName ||
                propertyName == DropShadow.ColorProperty.PropertyName ||
                propertyName == DropShadow.OffsetProperty.PropertyName ||
                propertyName == DropShadow.OpacityProperty.PropertyName)
            {
                PropagatePropertyChanged?.Invoke();
            }
        }
    }
}
