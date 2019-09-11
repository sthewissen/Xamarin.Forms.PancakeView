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
        private PancakeDrawable _drawable;

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

                Validate(pancake);

                this.SetBackground(_drawable = new PancakeDrawable(pancake, Context.ToPixels));

                SetupShadow(pancake);
            }
        }

        private void Validate(PancakeView pancake)
        {
            // Angle needs to be between 0-360.
            if (pancake.BackgroundGradientAngle < 0 || pancake.BackgroundGradientAngle > 360)
                throw new ArgumentException("Please provide a valid background gradient angle.", nameof(Controls.PancakeView.BackgroundGradientAngle));

            if (pancake.OffsetAngle < 0 || pancake.OffsetAngle > 360)
                throw new ArgumentException("Please provide a valid offset angle.", nameof(Controls.PancakeView.OffsetAngle));

            // min value for sides is 3
            if (pancake.Sides < 3)
                throw new ArgumentException("Please provide a valid value for sides.", nameof(Controls.PancakeView.Sides));

        }

        private void SetupShadow(PancakeView pancake)
        {
            // clear previous shadow/elevation
            this.Elevation = 0;
            this.TranslationZ = 0;

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
                if (pancake.Sides == 4 || (pancake.Sides != 4 && pancake.CornerRadius.TopLeft == 0))
                {
                    // To have shadow show up, we need to clip. However, clipping means that individual corners are lost :(
                    this.OutlineProvider = new RoundedCornerOutlineProvider(pancake, Context.ToPixels);
                    this.ClipToOutline = true;
                }
                else
                {
                    this.OutlineProvider = null;
                    this.ClipToOutline = false;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var pancake = Element as PancakeView;
            Validate(pancake);

            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == PancakeView.BorderColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderIsDashedProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderDrawingStyleProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientEndColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientStartColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientStopsProperty.PropertyName)
            {
                Invalidate();
            }
            else if (e.PropertyName == PancakeView.SidesProperty.PropertyName ||
                e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.HasShadowProperty.PropertyName ||
                e.PropertyName == PancakeView.ElevationProperty.PropertyName)
            {
                SetupShadow(pancake);
            }
            else if (e.PropertyName == PancakeView.CornerRadiusProperty.PropertyName)
            {
                Invalidate();
                SetupShadow(pancake);
            }
            else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                _drawable.Dispose();
                this.SetBackground(_drawable = new PancakeDrawable(pancake, Context.ToPixels));
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && !_disposed)
            {
                _drawable?.Dispose();
                _disposed = true;
            }
        }

        protected override void OnDraw(ACanvas canvas)
        {
            if (Element == null) return;

            var control = (PancakeView)Element;

            SetClipChildren(true);

            //Create path to clip the child
            if (control.Sides != 4)
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

            DrawBorder(canvas, control);
        }

        protected override bool DrawChild(ACanvas canvas, global::Android.Views.View child, long drawingTime)
        {
            if (Element == null) return false;

            var control = (PancakeView)Element;

            SetClipChildren(true);

            //Create path to clip the child         
            if (control.Sides != 4)
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
            if (control.BorderThickness > 0)
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
                    if (control.Sides != 4)
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
                        // dashes merge when thickness is increased
                        // off-distance should be scaled according to thickness
                        paint.SetPathEffect(new DashPathEffect(new float[] { 10, 5 * control.BorderThickness }, 0));
                    }

                    if ((control.BorderGradientStartColor != default(Color) && control.BorderGradientEndColor != default(Color)) || (control.BorderGradientStops != null && control.BorderGradientStops.Any()))
                    {
                        var angle = control.BorderGradientAngle / 360.0;

                        // Calculate the new positions based on angle between 0-360.
                        var a = canvas.Width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
                        var b = canvas.Height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
                        var c = canvas.Width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
                        var d = canvas.Height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

                        if (control.BorderGradientStops != null && control.BorderGradientStops.Count > 0)
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
}