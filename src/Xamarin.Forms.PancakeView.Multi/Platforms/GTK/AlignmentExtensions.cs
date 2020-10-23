using Pango;

namespace Xamarin.Forms.PancakeView.Platforms.GTK
{
	public static class AlignmentExtensions
    {
        public static Alignment ToNativeVerticalAlignment(this LayoutOptions alignment)
        {
            switch (alignment.Alignment)
            {
                case LayoutAlignment.Start:
                    return Alignment.Left;
                case LayoutAlignment.Center:
                    return Alignment.Center;
                case LayoutAlignment.End:
                    return Alignment.Right;
                case LayoutAlignment.Fill:
                    return Alignment.Center;
                default:
                    return Alignment.Center;
            }
        }
    }
}
