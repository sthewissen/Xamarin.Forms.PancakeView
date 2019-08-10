using System;
using System.ComponentModel;
using System.Linq;
using Android.Graphics;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
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
            {
                if (_normalBitmap != null)
                {
                    _normalBitmap.Dispose();
                    _normalBitmap = null;
                }
                return;
            }

            if (_normalBitmap == null || _normalBitmap.Height != height || _normalBitmap.Width != width)
            {
                // If the user changes the orientation of the screen, make sure to destroy reference before
                // reassigning a new bitmap reference.
                if (_normalBitmap != null)
                {
                    _normalBitmap.Dispose();
                    _normalBitmap = null;
                }

                _normalBitmap = CreateBitmap(false, width, height);
            }

            Bitmap bitmap = _normalBitmap;

            using (var paint = new Paint())
            {
                canvas.DrawBitmap(bitmap, 0, 0, paint);
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

        Bitmap CreateBitmap(bool pressed, int width, int height)
        {
            Bitmap bitmap;

            using (Bitmap.Config config = Bitmap.Config.Argb8888)
            {
                bitmap = Bitmap.CreateBitmap(width, height, config);
            }

            using (var canvas = new ACanvas(bitmap))
            {
                DrawCanvas(canvas, width, height, pressed);
            }

            return bitmap;
        }

        void DrawBackground(ACanvas canvas, int width, int height, CornerRadius cornerRadius, bool pressed)
        {
            using (var paint = new Paint { AntiAlias = true })
            using (Path.Direction direction = Path.Direction.Cw)
            using (Paint.Style style = Paint.Style.Fill)
            {
                var path = new Path();

                if (_pancake.Sides != 4)
                {
                    path = PolygonUtils.GetPolygonCornerPath(width, height, _pancake.Sides, _pancake.CornerRadius.TopLeft, _pancake.OffsetAngle);
                }
                else
                {

                    using (var rect = new RectF(0, 0, width, height))
                    {
                        float topLeft = _convertToPixels(cornerRadius.TopLeft);
                        float topRight = _convertToPixels(cornerRadius.TopRight);
                        float bottomRight = _convertToPixels(cornerRadius.BottomRight);
                        float bottomLeft = _convertToPixels(cornerRadius.BottomLeft);

                        if (!_pancake.HasShadow || _pancake.Elevation > 0)
                            path.AddRoundRect(rect, new float[] { topLeft, topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft }, direction);
                        else
                            path.AddRoundRect(rect, new float[] { topLeft, topLeft, topLeft, topLeft, topLeft, topLeft, topLeft, topLeft }, direction);
                    }
                }

                if ((_pancake.BackgroundGradientStartColor != default(Color) && _pancake.BackgroundGradientEndColor != default(Color)) || (_pancake.BackgroundGradientStops != null && _pancake.BackgroundGradientStops.Any()))
                {
                    var angle = _pancake.BackgroundGradientAngle / 360.0;

                    // Calculate the new positions based on angle between 0-360.
                    var a = width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
                    var b = height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
                    var c = width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
                    var d = height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

                    if (_pancake.BackgroundGradientStops != null)
                    {
                        // A range of colors is given. Let's add them.
                        var orderedStops = _pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                        var colors = orderedStops.Select(x => x.Color.ToAndroid().ToArgb()).ToArray();
                        var locations = orderedStops.Select(x => x.Offset).ToArray();

                        var shader = new LinearGradient(width - (float)a, (float)b, width - (float)c, (float)d, colors, locations, Shader.TileMode.Clamp);
                        paint.SetShader(shader);
                    }
                    else
                    {
                        // Only two colors provided, use that.
                        var shader = new LinearGradient(width - (float)a, (float)b, width - (float)c, (float)d, _pancake.BackgroundGradientStartColor.ToAndroid(), _pancake.BackgroundGradientEndColor.ToAndroid(), Shader.TileMode.Clamp);
                        paint.SetShader(shader);
                    }
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
                e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStartColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientEndColorProperty.PropertyName ||
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
                    DrawCanvas(canvas, width, height, false);
                }

                InvalidateSelf();
            }
        }

        void DrawCanvas(ACanvas canvas, int width, int height, bool pressed)
        {
            DrawBackground(canvas, width, height, _pancake.CornerRadius, pressed);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                if (_normalBitmap != null)
                {
                    _normalBitmap.Dispose();
                    _normalBitmap = null;
                }

                _isDisposed = true;
            }

            base.Dispose(disposing);
        }
    }
}