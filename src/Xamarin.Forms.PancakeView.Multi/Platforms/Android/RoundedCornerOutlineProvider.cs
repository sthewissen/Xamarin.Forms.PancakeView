using Android.Graphics;
using Android.Views;

namespace Xamarin.Forms.PancakeView.Droid
{
    public class RoundedCornerOutlineProvider : ViewOutlineProvider
    {
        private readonly float _cornerRadius;
        private readonly int _border;

        public RoundedCornerOutlineProvider(float cornerRadius, int border)
        {
            _cornerRadius = cornerRadius;
            _border = border;
        }

        public override void GetOutline(global::Android.Views.View view, Outline outline)
        {
            // TODO: Figure out how to clip individual rounded corners with different radii.
            outline.SetRoundRect(new Rect(0, 0, view.Width, view.Height), _cornerRadius);
        }
    }
}
