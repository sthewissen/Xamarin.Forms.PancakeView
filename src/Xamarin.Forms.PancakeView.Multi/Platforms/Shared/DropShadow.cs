using System;
namespace Xamarin.Forms.PancakeView
{
    public class DropShadow : BindableObject
    {
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
            nameof(Offset), typeof(Point), typeof(DropShadow), new Point(1, 1));

        public Point Offset
        {
            get => (Point)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(
            nameof(Elevation), typeof(float), typeof(DropShadow), 10f);

        public float Elevation
        {
            get => (float)GetValue(ElevationProperty);
            set => SetValue(ElevationProperty, value);
        }

        public static readonly BindableProperty TranslationZProperty = BindableProperty.Create(
            nameof(TranslationZ), typeof(float), typeof(DropShadow), 10f);

        public float TranslationZ
        {
            get => (float)GetValue(TranslationZProperty);
            set => SetValue(TranslationZProperty, value);
        }

        //public override bool Equals(object obj)
        //{
        //    if (!(obj is DropShadow dest))
        //        return false;

        //    return Color == dest.Color;
        //}

        //public override int GetHashCode()
        //{
        //    return -1234567890 + Color.GetHashCode();
        //}
    }
}
