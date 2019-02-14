using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView.Droid;
using Xamarin.Forms.Platform.Android;
using AButton = Android.Widget.Button;
using ACanvas = Android.Graphics.Canvas;
using Controls = Xamarin.Forms.PancakeView;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.Droid
{
    public class PancakeViewRenderer : VisualElementRenderer<ContentView>
    {
        bool _disposed;

        public PancakeViewRenderer(Context context) : base(context) { }

        /// <summary>
        /// This method ensures that we don't get stripped out by the linker.
        /// </summary>
        public static void Init()
        {
#pragma warning disable 0219
            var ignore1 = typeof(PancakeViewRenderer);
            var ignore2 = typeof(Controls.PancakeView);
#pragma warning restore 0219
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && e.OldElement == null)
            {
                var pancake = (Element as PancakeView);

                // Angle needs to be between 0-360.
                if (pancake.BackgroundGradientAngle < 0 || pancake.BackgroundGradientAngle > 360)
                    throw new ArgumentException("Please provide a valid background gradient angle.", nameof(Controls.PancakeView.BackgroundGradientAngle));

                UpdateBackground();

                // Clips the content to the box we provide.
                this.OutlineProvider = new RoundedCornerOutlineProvider(pancake.CornerRadius, pancake.BorderThickness * 4);
                this.ClipToOutline = true;

                // If it has a shadow, give it a default Droid looking shadow.
                if (pancake.HasShadow)
                {
                    this.Elevation = 10;
                    this.TranslationZ = 10;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
                e.PropertyName == PancakeView.CornerRadiusProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStartColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientEndColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderIsDashedProperty.PropertyName)
            {
                UpdateBackground();
            }
        }

        void UpdateBackground()
        {
            this.SetBackground(new PancakeDrawable(Element as PancakeView, Context.ToPixels));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && !_disposed)
            {
                Background.Dispose();
                _disposed = true;
            }
        }

        class PancakeDrawable : Drawable
        {
            readonly PancakeView _pancake;
            readonly Func<double, float> _convertToPixels;
            bool _isDisposed;
            Bitmap _normalBitmap;

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

            void DrawBackground(ACanvas canvas, int width, int height, Thickness cornerRadius, bool pressed)
            {
                using (var paint = new Paint { AntiAlias = true })
                using (var path = new Path())
                using (Path.Direction direction = Path.Direction.Cw)
                using (Paint.Style style = Paint.Style.Fill)
                using (var rect = new RectF(0, 0, width, height))
                {
                    float topLeft = _convertToPixels(cornerRadius.Left);
                    float topRight = _convertToPixels(cornerRadius.Top);
                    float bottomRight = _convertToPixels(cornerRadius.Right);
                    float bottomLeft = _convertToPixels(cornerRadius.Bottom);

                    path.AddRoundRect(rect, new float[] { topLeft, topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft }, direction);

                    if (_pancake.BackgroundGradientStartColor != default(Color) && _pancake.BackgroundGradientEndColor != default(Color))
                    {
                        var angle = _pancake.BackgroundGradientAngle / 360.0;

                        // Calculate the new positions based on angle between 0-360.
                        var a = width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
                        var b = height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
                        var c = width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
                        var d = height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

                        var shader = new LinearGradient(width - (float)a, (float)b, width - (float)c, (float)d, _pancake.BackgroundGradientStartColor.ToAndroid(), _pancake.BackgroundGradientEndColor.ToAndroid(), Shader.TileMode.Clamp);
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

            void DrawOutline(ACanvas canvas, int width, int height, Thickness cornerRadius)
            {
                // TODO: This doesn't look entirely right yet when using it with rounded corners.

                using (var paint = new Paint { AntiAlias = true })
                using (var path = new Path())
                using (Path.Direction direction = Path.Direction.Cw)
                using (Paint.Style style = Paint.Style.Stroke)
                using (var rect = new RectF(0 + _pancake.BorderThickness, 0 + _pancake.BorderThickness, width - _pancake.BorderThickness, height - _pancake.BorderThickness))
                {
                    float topLeft = _convertToPixels(cornerRadius.Left);
                    float topRight = _convertToPixels(cornerRadius.Top);
                    float bottomRight = _convertToPixels(cornerRadius.Right);
                    float bottomLeft = _convertToPixels(cornerRadius.Bottom);

                    path.AddRoundRect(rect, new float[] { topLeft, topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft }, direction);

                    if(_pancake.BorderIsDashed)
                    {
                        paint.SetPathEffect(new DashPathEffect(new float[] { 10, 20 }, 0));
                    }

                    paint.StrokeCap = Paint.Cap.Round;
                    paint.StrokeWidth = _pancake.BorderThickness;
                    paint.SetStyle(style);
                    paint.Color = _pancake.BorderColor.ToAndroid();

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
                    e.PropertyName == PancakeView.BorderColorProperty.PropertyName ||
                    e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName ||
                    e.PropertyName == PancakeView.BorderIsDashedProperty.PropertyName)
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
                DrawOutline(canvas, width, height, _pancake.CornerRadius);
            }
        }
    }

    public class RoundedCornerOutlineProvider : ViewOutlineProvider
    {
        private readonly Thickness _cornerRadius;
        private readonly int _border;

        public RoundedCornerOutlineProvider(Thickness cornerRadius, int border)
        {
            _cornerRadius = cornerRadius;
            _border = border;
        }

        public override void GetOutline(global::Android.Views.View view, Outline outline)
        {
            // TODO: Figure out how to clip individual rounded corners with different radii.
            outline.SetRoundRect(-1 * (_border / 2) - 1, -1 * (_border / 2), view.Width + (_border / 2) + 2, view.Height + (_border / 2), (float)(_cornerRadius.Top + (_border)));
        }
    }
}
