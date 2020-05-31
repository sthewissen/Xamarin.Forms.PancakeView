using System;
using Android.Graphics;
using Android.Views;

namespace Xamarin.Forms.PancakeView.Droid
{
    public class RoundedCornerOutlineProvider : ViewOutlineProvider
    {
        private readonly PancakeView _pancake;
        private readonly Func<double, float> _convertToPixels;

        public RoundedCornerOutlineProvider(PancakeView pancake, Func<double, float> convertToPixels)
        {
            _pancake = pancake;
            _convertToPixels = convertToPixels;
        }

        public override void GetOutline(global::Android.Views.View view, Outline outline)
        {
            //if (_pancake.Sides != 4)
            //{
            //    var hexPath = DrawingExtensions.CreatePolygonPath(view.Width, view.Height, _pancake.Sides,
            //        _pancake.Shadow != null ? 0 : _pancake.CornerRadius.TopLeft, _pancake.OffsetAngle);

            //    if (hexPath.IsConvex)
            //    {
            //        outline.SetConvexPath(hexPath);
            //    }
            //}
            //else
            //{
            //    var path = DrawingExtensions.CreateRoundedRectPath(view.Width, view.Height,
            //        _convertToPixels(_pancake.CornerRadius.TopLeft),
            //        _convertToPixels(_pancake.CornerRadius.TopRight),
            //        _convertToPixels(_pancake.CornerRadius.BottomRight),
            //        _convertToPixels(_pancake.CornerRadius.BottomLeft));

            //    if (path.IsConvex)
            //    {
            //        outline.SetConvexPath(path);
            //    }
            //}
        }
    }
}