using System;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView.Droid;
using Xamarin.Forms.Platform.Android;
using ACanvas = Android.Graphics.Canvas;
using Controls = Xamarin.Forms.PancakeView;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.Droid
{
    public class PancakeViewRenderer : VisualElementRenderer<ContentView>
    {
        bool _disposed;

        public PancakeViewRenderer(Context context) : base(context)
        {
        }

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

                // HACK: When there are no children we add a Grid element to trigger DrawChild.
                // This should be inmproved upon, but I haven't found out a nice way to be able to clip
                // the children and add the border on top without using DrawChild.
                if (pancake.Content == null)
                {
                    pancake.Content = new Grid();
                }

                // Angle needs to be between 0-360.
                if (pancake.BackgroundGradientAngle < 0 || pancake.BackgroundGradientAngle > 360)
                    throw new ArgumentException("Please provide a valid background gradient angle.", nameof(Controls.PancakeView.BackgroundGradientAngle));

                UpdateBackground();

                // If it has a shadow, give it a default Droid looking shadow.
                if (pancake.HasShadow)
                {
                    this.Elevation = 10;
                    this.TranslationZ = 10;

                    // To have shadow show up, we need to clip. However, clipping means that individual corners are lost :(
                    this.OutlineProvider = new RoundedCornerOutlineProvider(Context.ToPixels(pancake.CornerRadius.TopLeft), (int)Context.ToPixels(pancake.BorderThickness));
                    this.ClipToOutline = true;
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
                e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientStartColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientEndColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientStopsProperty.PropertyName ||
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

        protected override bool DrawChild(ACanvas canvas, global::Android.Views.View child, long drawingTime)
        {
            if (Element == null) return false;

            var control = (PancakeView)Element;

            SetClipChildren(true);

            //Create path to clip the child         
            using (var path = new Path())
            {
                path.AddRoundRect(new RectF(0, 0, Width, Height), GetRadii(control), Path.Direction.Ccw);

                canvas.Save();
                canvas.ClipPath(path);
            }

            // Draw the child first so that the border shows up above it.        
            var result = base.DrawChild(canvas, child, drawingTime);
            canvas.Restore();

            DrawBorder(canvas, control);

            return result;
        }

        private float[] GetRadii(PancakeView control)
        {
            float topLeft = Context.ToPixels(control.CornerRadius.TopLeft);
            float topRight = Context.ToPixels(control.CornerRadius.TopRight);
            float bottomRight = Context.ToPixels(control.CornerRadius.BottomRight);
            float bottomLeft = Context.ToPixels(control.CornerRadius.BottomLeft);

            var radii = new[] { topLeft, topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft };

            if (control.HasShadow)
            {
                radii = new[] { topLeft, topLeft, topLeft, topLeft, topLeft, topLeft, topLeft, topLeft };
            }

            return radii;
        }

        private void DrawBorder(ACanvas canvas, PancakeView control)
        {
            var borderThickness = Context.ToPixels(control.BorderThickness);
            var halfBorderThickness = borderThickness / 2;

            // TODO: This doesn't look entirely right yet when using it with rounded corners.
            using (var paint = new Paint { AntiAlias = true })
            using (var path = new Path())
            using (Path.Direction direction = Path.Direction.Cw)
            using (Paint.Style style = Paint.Style.Stroke)

            using (var rect = new RectF(control.BorderDrawingStyle == BorderDrawingStyle.Outside && !control.HasShadow ? -halfBorderThickness : halfBorderThickness,
                                        control.BorderDrawingStyle == BorderDrawingStyle.Outside && !control.HasShadow ? -halfBorderThickness : halfBorderThickness,
                                        control.BorderDrawingStyle == BorderDrawingStyle.Outside && !control.HasShadow ? canvas.Width + halfBorderThickness : canvas.Width - halfBorderThickness,
                                        control.BorderDrawingStyle == BorderDrawingStyle.Outside && !control.HasShadow ? canvas.Height + halfBorderThickness : canvas.Height - halfBorderThickness))
            {
                path.AddRoundRect(rect, GetRadii(control), direction);

                if (control.BorderIsDashed)
                {
                    paint.SetPathEffect(new DashPathEffect(new float[] { 10, 20 }, 0));
                }

                if ((_pancake.BorderGradientStartColor != default(Color) && _pancake.BorderGradientEndColor != default(Color)) || (_pancake.BorderGradientStops != null && _pancake.BorderGradientStops.Any()))
                {
                    var angle = _pancake.BorderGradientAngle / 360.0;

                    // Calculate the new positions based on angle between 0-360.
                    var a = width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
                    var b = height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
                    var c = width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
                    var d = height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

                    if (_pancake.BorderGradientStops != null)
                    {
                        // A range of colors is given. Let's add them.
                        var orderedStops = _pancake.BorderGradientStops.OrderBy(x => x.Offset).ToList();
                        var colors = orderedStops.Select(x => x.Color.ToAndroid().ToArgb()).ToArray();
                        var locations = orderedStops.Select(x => x.Offset).ToArray();

                        var shader = new LinearGradient(width - (float)a, (float)b, width - (float)c, (float)d, colors, locations, Shader.TileMode.Clamp);
                        paint.SetShader(shader);
                    }
                    else
                    {
                        // Only two colors provided, use that.
                        var shader = new LinearGradient(width - (float)a, (float)b, width - (float)c, (float)d, _pancake.BorderGradientStartColor.ToAndroid(), _pancake.BorderGradientEndColor.ToAndroid(), Shader.TileMode.Clamp);
                        paint.SetShader(shader);
                    }
                }
                else
                {
                    paint.Color = control.BorderColor.ToAndroid();
                }

                paint.StrokeCap = Paint.Cap.Square;
                paint.StrokeWidth = borderThickness;
                paint.SetStyle(style);

                canvas.DrawPath(path, paint);
            }
        }
    }

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

    class PancakeDrawable : Drawable
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
            using (var path = new Path())
            using (Path.Direction direction = Path.Direction.Cw)
            using (Paint.Style style = Paint.Style.Fill)
            using (var rect = new RectF(0, 0, width, height))
            {
                float topLeft = _convertToPixels(cornerRadius.TopLeft);
                float topRight = _convertToPixels(cornerRadius.TopRight);
                float bottomRight = _convertToPixels(cornerRadius.BottomRight);
                float bottomLeft = _convertToPixels(cornerRadius.BottomLeft);

                if (!_pancake.HasShadow)
                    path.AddRoundRect(rect, new float[] { topLeft, topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft }, direction);
                else
                    path.AddRoundRect(rect, new float[] { topLeft, topLeft, topLeft, topLeft, topLeft, topLeft, topLeft, topLeft }, direction);

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