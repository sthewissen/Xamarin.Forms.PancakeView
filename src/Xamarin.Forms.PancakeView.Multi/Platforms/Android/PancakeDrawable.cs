using Android.Graphics;
using Android.Graphics.Drawables;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.Android;
using ACanvas = Android.Graphics.Canvas;

namespace Xamarin.Forms.PancakeView.Droid
{
    public class PancakeDrawable : Drawable
    {
        readonly PancakeView _pancake;
        readonly Func<double, float> _convertToPixels;
        Bitmap _normalBitmap;
        bool _isDisposed;

        public override int Opacity
        {
            get { return 0; }
        }

        public PancakeDrawable(PancakeView pancake, Func<double, float> convertToPixels)
        {
            _pancake = pancake;
            _convertToPixels = convertToPixels;
            _pancake.PropertyChanged += PancakeViewOnPropertyChanged;
        }

        public override void Draw(ACanvas canvas)
        {
            int width = Bounds.Width();
            int height = Bounds.Height();

            if (width <= 0 || height <= 0)
                return;

            if (_normalBitmap == null || _normalBitmap.Handle == IntPtr.Zero ||
                _normalBitmap.Height != height || _normalBitmap.Width != width)
                Reset();

            _normalBitmap = _normalBitmap ?? CreateBitmap(width, height);

            canvas.DrawBitmap(_normalBitmap, 0, 0, new Paint());
        }

        public void Reset()
        {
            if (_normalBitmap != null)
            {
                if (_normalBitmap.Handle != IntPtr.Zero)
                {
                    _normalBitmap.Recycle();
                    _normalBitmap.Dispose();
                }
                _normalBitmap = null;
            }
        }

        public override void SetAlpha(int alpha)
        {
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
        }

        protected override bool OnStateChange(int[] state)
        {
            return false;
        }

        Bitmap CreateBitmap(int width, int height)
        {
            Bitmap bitmap;

            using (Bitmap.Config config = Bitmap.Config.Argb8888)
            {
                bitmap = Bitmap.CreateBitmap(width, height, config);
            }

            using (var canvas = new ACanvas(bitmap))
            {
                DrawCanvas(canvas, width, height);
            }

            return bitmap;
        }

        void DrawBackground(ACanvas canvas, int width, int height, CornerRadius cornerRadius)
        {
            using (var paint = new Paint { AntiAlias = true })
            using (Path.Direction direction = Path.Direction.Cw)
            using (Paint.Style style = Paint.Style.Fill)
            {
                var path = new Path();

                if (_pancake.Sides != 4)
                {
                    path = DrawingExtensions.CreatePolygonPath(width, height, _pancake.Sides, _pancake.CornerRadius.TopLeft, _pancake.OffsetAngle);
                }
                else
                {
                    float topLeft = _convertToPixels(cornerRadius.TopLeft);
                    float topRight = _convertToPixels(cornerRadius.TopRight);
                    float bottomRight = _convertToPixels(cornerRadius.BottomRight);
                    float bottomLeft = _convertToPixels(cornerRadius.BottomLeft);

                    path = DrawingExtensions.CreateRoundedRectPath(width, height, topLeft, topRight, bottomRight, bottomLeft);
                }

                if (_pancake.BackgroundGradientStops != null && _pancake.BackgroundGradientStops.Any())
                {
                    // A range of colors is given. Let's add them.
                    var orderedStops = _pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                    var colors = orderedStops.Select(x => x.Color.ToAndroid().ToArgb()).ToArray();
                    var locations = orderedStops.Select(x => x.Offset).ToArray();

                    var shader = new LinearGradient((float)(canvas.Width * _pancake.BackgroundGradientStartPoint.X),
                         (float)(canvas.Height * _pancake.BackgroundGradientStartPoint.Y),
                         (float)(canvas.Width * _pancake.BackgroundGradientEndPoint.X),
                         (float)(canvas.Height * _pancake.BackgroundGradientEndPoint.Y),
                         colors, locations, Shader.TileMode.Clamp);

                    paint.SetShader(shader);
                }
                else
                {
                    global::Android.Graphics.Color color = _pancake.BackgroundColor.ToAndroid();
                    paint.SetStyle(style);
                    paint.Color = color;
                }

                canvas.DrawPath(path, paint);
            }
        }

        void PancakeViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
                e.PropertyName == PancakeView.CornerRadiusProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStartPointProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientEndPointProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName ||
                e.PropertyName == PancakeView.SidesProperty.PropertyName ||
                e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName)
            {
                if (_normalBitmap == null)
                    return;

                using (var canvas = new ACanvas(_normalBitmap))
                {
                    int width = Bounds.Width();
                    int height = Bounds.Height();
                    canvas.DrawColor(global::Android.Graphics.Color.Black, PorterDuff.Mode.Clear);
                    DrawCanvas(canvas, width, height);
                }

                InvalidateSelf();
            }
        }

        void DrawCanvas(ACanvas canvas, int width, int height)
        {
            DrawBackground(canvas, width, height, _pancake.CornerRadius);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
                return;

            _isDisposed = true;

            if (disposing)
                Reset();

            base.Dispose(disposing);
        }
    }
}