using System;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.View;
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
                // This should be improved upon, but I haven't found out a nice way to be able to clip
                // the children and add the border on top without using DrawChild.
                if (pancake.Content == null)
                {
                    pancake.Content = new Grid();
                }

                // Angle needs to be between 0-360.
                if (pancake.BackgroundGradientAngle < 0 || pancake.BackgroundGradientAngle > 360)
                    throw new ArgumentException("Please provide a valid background gradient angle.", nameof(Controls.PancakeView.BackgroundGradientAngle));

                UpdateBackground();

                SetupShadow(pancake);
            }
        }

        private void SetupShadow(PancakeView pancake)
        {
            bool hasShadowOrElevation = pancake.HasShadow || pancake.Elevation > 0;

            // If it has a shadow, give it a default Droid looking shadow.
            if (pancake.HasShadow)
            {
                this.Elevation = 10;
                this.TranslationZ = 10;
            }

            // However if it has a specified elevation add the desired one
            if (pancake.Elevation > 0)
            {
                this.Elevation = 0;
                this.TranslationZ = 0;
                ViewCompat.SetElevation(this, Context.ToPixels(pancake.Elevation));
            }

            if (hasShadowOrElevation)
            {
                if (!pancake.IsRegular || (pancake.IsRegular && pancake.CornerRadius.TopLeft == 0))
                {
                    // To have shadow show up, we need to clip. However, clipping means that individual corners are lost :(
                    this.OutlineProvider = new RoundedCornerOutlineProvider(pancake, Context.ToPixels);
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
                e.PropertyName == PancakeView.BorderIsDashedProperty.PropertyName ||
                e.PropertyName == PancakeView.SidesProperty.PropertyName ||
                e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.IsRegularProperty.PropertyName)
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
            if (control.IsRegular)
            {
                using (var path = PolygonUtils.GetPolygonCornerPath(Width, Height, control.Sides, control.CornerRadius.TopLeft, control.OffsetAngle))
                {
                    canvas.Save();
                    canvas.ClipPath(path);
                }
            }
            else
            {
                using (var path = new Path())
                {
                    path.AddRoundRect(new RectF(0, 0, Width, Height), GetRadii(control), Path.Direction.Ccw);

                    canvas.Save();
                    canvas.ClipPath(path);
                }
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

            if (control.HasShadow || control.Elevation > 0)
            {
                radii = new[] { topLeft, topLeft, topLeft, topLeft, topLeft, topLeft, topLeft, topLeft };
            }

            return radii;
        }

        private void DrawBorder(ACanvas canvas, PancakeView control)
        {
            var borderThickness = Context.ToPixels(control.BorderThickness);
            var halfBorderThickness = borderThickness / 2;
            bool hasShadowOrElevation = control.HasShadow || control.Elevation > 0;

            // TODO: This doesn't look entirely right yet when using it with rounded corners.
            using (var paint = new Paint { AntiAlias = true })
            using (Path.Direction direction = Path.Direction.Cw)
            using (Paint.Style style = Paint.Style.Stroke)
            using (var rect = new RectF(control.BorderDrawingStyle == BorderDrawingStyle.Outside && !hasShadowOrElevation ? -halfBorderThickness : halfBorderThickness,
                                        control.BorderDrawingStyle == BorderDrawingStyle.Outside && !hasShadowOrElevation ? -halfBorderThickness : halfBorderThickness,
                                        control.BorderDrawingStyle == BorderDrawingStyle.Outside && !hasShadowOrElevation ? canvas.Width + halfBorderThickness : canvas.Width - halfBorderThickness,
                                        control.BorderDrawingStyle == BorderDrawingStyle.Outside && !hasShadowOrElevation ? canvas.Height + halfBorderThickness : canvas.Height - halfBorderThickness))
            {
                Path path = null;
                if (control.IsRegular)
                {
                    path = PolygonUtils.GetPolygonCornerPath(Width, Height, control.Sides, control.CornerRadius.TopLeft, control.OffsetAngle);
                }
                else
                {
                    path = new Path();
                    path.AddRoundRect(rect, GetRadii(control), direction);
                }

                if (control.BorderIsDashed)
                {
                    paint.SetPathEffect(new DashPathEffect(new float[] { 10, 20 }, 0));
                }

                if ((control.BorderGradientStartColor != default(Color) && control.BorderGradientEndColor != default(Color)) || (control.BorderGradientStops != null && control.BorderGradientStops.Any()))
                {
                    var angle = control.BorderGradientAngle / 360.0;

                    // Calculate the new positions based on angle between 0-360.
                    var a = canvas.Width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
                    var b = canvas.Height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
                    var c = canvas.Width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
                    var d = canvas.Height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

                    if (control.BorderGradientStops != null)
                    {
                        // A range of colors is given. Let's add them.
                        var orderedStops = control.BorderGradientStops.OrderBy(x => x.Offset).ToList();
                        var colors = orderedStops.Select(x => x.Color.ToAndroid().ToArgb()).ToArray();
                        var locations = orderedStops.Select(x => x.Offset).ToArray();

                        var shader = new LinearGradient(canvas.Width - (float)a, (float)b, canvas.Width - (float)c, (float)d, colors, locations, Shader.TileMode.Clamp);
                        paint.SetShader(shader);
                    }
                    else
                    {
                        // Only two colors provided, use that.
                        var shader = new LinearGradient(canvas.Width - (float)a, (float)b, canvas.Width - (float)c, (float)d, control.BorderGradientStartColor.ToAndroid(), control.BorderGradientEndColor.ToAndroid(), Shader.TileMode.Clamp);
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
}