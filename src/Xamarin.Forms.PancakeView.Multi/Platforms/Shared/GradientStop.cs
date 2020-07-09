using System;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.PancakeView
{
    public class GradientStop : BindableObject
    {
        public static readonly BindableProperty OffsetProperty = BindableProperty.Create(
          nameof(Offset), typeof(float), typeof(GradientStop), 0f);

        public float Offset
        {
            get => (float)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
            nameof(Color), typeof(Color), typeof(GradientStop), default(Color));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
    }
}
