using Android.Graphics;
using Android.Graphics.Drawables;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.Android;
using ACanvas = Android.Graphics.Canvas;
using AColor = Android.Graphics.Color;

namespace Xamarin.Forms.PancakeView.Droid
{
    public class PancakeDrawable : Drawable
    {
        readonly PancakeView _pancake;
        readonly Func<double, float> _convertToPixels;
        Bitmap _normalBitmap;
        bool _isDisposed;

        float _paddingLeft;
        float _paddingTop;
        Color _shadowColor;
        float _shadowDx;
        float _shadowDy;
        float _shadowRadius;

        public override int Opacity
        {
            get { return 0; }
        }

        float PaddingLeft
        {
            get { return (_paddingLeft / 2f) + _shadowDx; }
            set { _paddingLeft = value; }
        }

        float PaddingTop
        {
            get { return (_paddingTop / 2f) + _shadowDy; }
            set { _paddingTop = value; }
        }

        public override bool IsStateful
        {
            get { return false; }
        }

        public PancakeDrawable(PancakeView pancake, Func<double, float> convertToPixels)
        {
            _pancake = pancake;
            _convertToPixels = convertToPixels;
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

        public PancakeDrawable SetShadow(float dy, float dx, Color color, float radius)
        {
            _shadowDx = dx;
            _shadowDy = dy;
            _shadowColor = color;
            _shadowRadius = radius;
            return this;
        }

        public PancakeDrawable SetPadding(float top, float left)
        {
            _paddingTop = top;
            _paddingLeft = left;
            return this;
        }

        protected override bool OnStateChange(int[] state)
        {
            return false;
        }

        Bitmap CreateBitmap(int width, int height)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

            using (var canvas = new ACanvas(bitmap))
            {
                canvas.DrawColor(global::Android.Graphics.Color.Black, PorterDuff.Mode.Clear);

                SetBackground(canvas, width, height);
                SetOutline(canvas, width, height);
            }

            return bitmap;
        }

        void SetBackground(ACanvas canvas, int width, int height)
        {
            var bgPaint = new Paint { AntiAlias = true };
            var bgPath = new Path();

            float topLeft = _convertToPixels(_pancake.CornerRadius.TopLeft);
            float topRight = _convertToPixels(_pancake.CornerRadius.TopRight);
            float bottomRight = _convertToPixels(_pancake.CornerRadius.BottomRight);
            float bottomLeft = _convertToPixels(_pancake.CornerRadius.BottomLeft);

            RectF rect = new RectF(0, 0, width, height);
            rect.Inset(PaddingLeft, PaddingTop);

            if (_pancake.Sides != 4)
                bgPath = DrawingExtensions.CreatePolygonPath(rect, _pancake.Sides, topLeft, _pancake.OffsetAngle);
            else
                bgPath = DrawingExtensions.CreateRoundedRectPath(rect, topLeft, topRight, bottomRight, bottomLeft);

            if (_pancake.BackgroundGradientStops != null && _pancake.BackgroundGradientStops.Any())
            {
                var angle = _pancake.BackgroundGradientAngle / 360.0;

                // Calculate the new positions based on angle between 0-360.
                var a = width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
                var b = height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
                var c = width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
                var d = height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

                // A range of colors is given. Let's add them.
                var orderedStops = _pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                var colors = orderedStops.Select(x => x.Color.ToAndroid().ToArgb()).ToArray();
                var locations = orderedStops.Select(x => x.Offset).ToArray();

                var shader = new LinearGradient(width - (float)a, (float)b, width - (float)c,
                    (float)d, colors, locations, Shader.TileMode.Clamp);

                bgPaint.SetShader(shader);
            }
            else
            {
                global::Android.Graphics.Color color = _pancake.BackgroundColor.ToAndroid();
                bgPaint.SetStyle(Paint.Style.Fill);
                bgPaint.Color = color;
            }

            if (_pancake.Shadow != null)
            {
                bgPaint.SetShadowLayer(_shadowRadius, (float)_pancake.Shadow.Offset.X,
                    (float)_pancake.Shadow.Offset.Y, new AColor((int)(_shadowColor.R * 255), (int)(_shadowColor.G * 255),
                    (int)(_shadowColor.B * 255), (int)(_pancake.Shadow.Opacity * 255)));
            }

            canvas.DrawPath(bgPath, bgPaint);
        }

        public void SetOutline(ACanvas canvas, int width, int height)
        {
            if (_pancake.Border == null || _pancake.Border.Thickness.IsEmpty)
                return;

            var strokePaint = new Paint { AntiAlias = true };
            var strokePath = new Path();

            float borderWidth = _convertToPixels(_pancake.Border.Thickness.Left);
            float inset = borderWidth / 2;

            // adjust border radius so outer edge of stroke is same radius as border radius of background
            float topLeft = Math.Max(_convertToPixels(_pancake.CornerRadius.TopLeft) - inset, 0);
            float topRight = Math.Max(_convertToPixels(_pancake.CornerRadius.TopRight) - inset, 0);
            float bottomRight = Math.Max(_convertToPixels(_pancake.CornerRadius.BottomRight) - inset, 0);
            float bottomLeft = Math.Max(_convertToPixels(_pancake.CornerRadius.BottomLeft) - inset, 0);

            RectF rect = new RectF(0, 0, width, height);
            rect.Inset(inset + PaddingLeft, inset + PaddingTop);

            if (_pancake.Sides != 4)
                strokePath = DrawingExtensions.CreatePolygonPath(rect, _pancake.Sides, topLeft, _pancake.OffsetAngle);
            else
                strokePath = DrawingExtensions.CreateRoundedRectPath(rect, topLeft, topRight, bottomRight, bottomLeft);

            if (_pancake.Border.DashPattern.Pattern != null && _pancake.Border.DashPattern.Pattern.Length > 0)
            {
                var items = _pancake.Border.DashPattern.Pattern.Select(x => _convertToPixels(Convert.ToSingle(x))).ToArray();

                // Disabling this border tweak for now as it looks better without.
                // for (int i = 0; i < items.Count(); i++)
                // {
                //     if (i % 2 != 0)
                //     {
                //         items[i] = items[i] * ((float)_pancake.Border.Thickness.Left * 0.5f);
                //     }
                // }

                strokePaint.SetPathEffect(new DashPathEffect(items, 0));
            }

            if (_pancake.Border.GradientStops != null && _pancake.Border.GradientStops.Any())
            {
                var angle = _pancake.Border.GradientAngle / 360.0;

                // Calculate the new positions based on angle between 0-360.
                var a = canvas.Width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
                var b = canvas.Height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
                var c = canvas.Width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
                var d = canvas.Height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

                // A range of colors is given. Let's add them.
                var orderedStops = _pancake.Border.GradientStops.OrderBy(x => x.Offset).ToList();
                var colors = orderedStops.Select(x => x.Color.ToAndroid().ToArgb()).ToArray();
                var locations = orderedStops.Select(x => x.Offset).ToArray();

                var shader = new LinearGradient(canvas.Width - (float)a, (float)b, canvas.Width - (float)c, (float)d, colors, locations, Shader.TileMode.Clamp);
                strokePaint.SetShader(shader);
            }
            else
            {
                strokePaint.Color = _pancake.Border.Color.ToAndroid();
            }

            strokePaint.StrokeWidth = borderWidth;
            strokePaint.SetStyle(Paint.Style.Stroke);
            canvas.DrawPath(strokePath, strokePaint);
        }

        //void PancakeViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
        //        e.PropertyName == PancakeView.CornerRadiusProperty.PropertyName ||
        //        e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName ||
        //        e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName ||
        //        e.PropertyName == PancakeView.SidesProperty.PropertyName ||
        //        e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName)
        //    {
        //        if (_normalBitmap == null)
        //            return;

        //        using (var canvas = new ACanvas(_normalBitmap))
        //        {
        //            int width = Bounds.Width();
        //            int height = Bounds.Height();
        //            canvas.DrawColor(global::Android.Graphics.Color.Black, PorterDuff.Mode.Clear);
        //            DrawCanvas(canvas, width, height);
        //        }

        //        InvalidateSelf();
        //    }
        //}

        //void DrawCanvas(ACanvas canvas, int width, int height)
        //{
        //    DrawBackground(canvas, width, height);
        //}

        public override void SetAlpha(int alpha)
        {
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
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

            //if (_pancake != null)
            //    _pancake.PropertyChanged -= PancakeViewOnPropertyChanged;
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