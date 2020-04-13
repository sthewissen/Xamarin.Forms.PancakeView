using System.Windows;

namespace Xamarin.Forms.PancakeView.Platforms.WPF.Utils
{
    public static class AlignmentExtensions
    {
        public static VerticalAlignment ToNativeVerticalAlignment(this LayoutOptions alignment)
        {
            switch (alignment.Alignment)
            {
                case LayoutAlignment.Start:
                    return VerticalAlignment.Top;
                case LayoutAlignment.Center:
                    return VerticalAlignment.Center;
                case LayoutAlignment.End:
                    return VerticalAlignment.Bottom;
                case LayoutAlignment.Fill:
                    return VerticalAlignment.Stretch;
                default:
                    return VerticalAlignment.Stretch;
            }
        }
    }
}
