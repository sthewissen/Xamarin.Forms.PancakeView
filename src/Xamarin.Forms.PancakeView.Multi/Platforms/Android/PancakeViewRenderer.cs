using System;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
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
                var pancake = Element as PancakeView;

                // HACK: When there are no children we add a Grid element to trigger DrawChild.
                // This should be improved upon, but I haven't found out a nice way to be able to clip
                // the children and add the border on top without using DrawChild.
                if (pancake.Content == null)
                {
                    pancake.Content = new Grid();
                }

                this.SetBackground(_drawable = new PancakeDrawable(pancake, Context.ToPixels));

                SetupShadow(pancake);
            }
        }

        private void SetupShadow(PancakeView pancake)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                // clear previous shadow/elevation
                this.Elevation = 0;
                this.TranslationZ = 0;

                bool hasShadowOrElevation = pancake.Shadow != null;

                if (pancake.Shadow != null)
                {
                    ViewCompat.SetElevation(this, Context.ToPixels(pancake.Shadow.BlurRadius));

#if MONOANDROID90
                    // Color only exists on Pie and beyond.
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                    {
                        this.SetOutlineAmbientShadowColor(pancake.Shadow.Color.ToAndroid());
                        this.SetOutlineSpotShadowColor(pancake.Shadow.Color.ToAndroid());
                    }
#endif
                }

                if (hasShadowOrElevation)
                {
                    // To have shadow show up, we need to clip.
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

            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == PancakeView.BorderProperty.PropertyName)
            {
                Invalidate();
            }
            else if (e.PropertyName == PancakeView.SidesProperty.PropertyName ||
                e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.ShadowProperty.PropertyName)
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
                _drawable = null;
                _disposed = true;
            }
        }

        protected override void OnDraw(ACanvas canvas)
        {
            if (Element == null) return;

            var pancake = Element as PancakeView;

            SetClipChildren(true);

            //Create path to clip the child
            if (pancake.Sides != 4)
            {
                using (var path = DrawingExtensions.CreatePolygonPath(Width, Height, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle))
                {
                    canvas.Save();
                    canvas.ClipPath(path);
                }
            }
            else
            {
                using (var path = DrawingExtensions.CreateRoundedRectPath(Width, Height,
                    Context.ToPixels(pancake.CornerRadius.TopLeft),
                    Context.ToPixels(pancake.CornerRadius.TopRight),
                    Context.ToPixels(pancake.CornerRadius.BottomRight),
                    Context.ToPixels(pancake.CornerRadius.BottomLeft)))
                {
                    canvas.Save();
                    canvas.ClipPath(path);
                }
            }

            DrawBorder(canvas, pancake);
        }

        protected override bool DrawChild(ACanvas canvas, global::Android.Views.View child, long drawingTime)
        {
            if (Element == null) return false;

            var pancake = Element as PancakeView;

            SetClipChildren(true);

            //Create path to clip the child         
            if (pancake.Sides != 4)
            {
                using (var path = DrawingExtensions.CreatePolygonPath(Width, Height, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle))
                {
                    canvas.Save();
                    canvas.ClipPath(path);
                }
            }
            else
            {
                using (var path = DrawingExtensions.CreateRoundedRectPath(Width, Height,
                        Context.ToPixels(pancake.CornerRadius.TopLeft),
                        Context.ToPixels(pancake.CornerRadius.TopRight),
                        Context.ToPixels(pancake.CornerRadius.BottomRight),
                        Context.ToPixels(pancake.CornerRadius.BottomLeft)))
                {
                    canvas.Save();
                    canvas.ClipPath(path);
                }
            }

            // Draw the child first so that the border shows up above it.        
            var result = base.DrawChild(canvas, child, drawingTime);
            canvas.Restore();

            DrawBorder(canvas, pancake);

            return result;
        }

        private void DrawBorder(ACanvas canvas, PancakeView pancake)
        {
            if (pancake.Border != null && pancake.Border.Thickness != default)
            {
                var borderThickness = Context.ToPixels(pancake.Border.Thickness.Left);
                var halfBorderThickness = borderThickness / 2;
                bool hasShadowOrElevation = pancake.Shadow != null && Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;

                // TODO: This doesn't look entirely right yet when using it with rounded corners.
                using (var paint = new Paint { AntiAlias = true })
                using (Path.Direction direction = Path.Direction.Cw)
                using (Paint.Style style = Paint.Style.Stroke)
                using (var rect = new RectF(pancake.Border.DrawingStyle == BorderDrawingStyle.Outside && !hasShadowOrElevation ? -halfBorderThickness : halfBorderThickness,
                                            pancake.Border.DrawingStyle == BorderDrawingStyle.Outside && !hasShadowOrElevation ? -halfBorderThickness : halfBorderThickness,
                                            pancake.Border.DrawingStyle == BorderDrawingStyle.Outside && !hasShadowOrElevation ? canvas.Width + halfBorderThickness : canvas.Width - halfBorderThickness,
                                            pancake.Border.DrawingStyle == BorderDrawingStyle.Outside && !hasShadowOrElevation ? canvas.Height + halfBorderThickness : canvas.Height - halfBorderThickness))
                {
                    Path path = null;
                    if (pancake.Sides != 4)
                    {
                        path = DrawingExtensions.CreatePolygonPath(Width, Height, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);
                    }
                    else

                    {
                        path = DrawingExtensions.CreateRoundedRectPath(Width, Height,
                            Context.ToPixels(pancake.CornerRadius.TopLeft),
                            Context.ToPixels(pancake.CornerRadius.TopRight),
                            Context.ToPixels(pancake.CornerRadius.BottomRight),
                            Context.ToPixels(pancake.CornerRadius.BottomLeft));
                    }

                    if (pancake.Border.DashPattern.Pattern != null && pancake.Border.DashPattern.Pattern.Length > 0)
                    {
                        var items = pancake.Border.DashPattern.Pattern.Select(x => Context.ToPixels(Convert.ToSingle(x))).ToArray();

                        // dashes merge when thickness is increased
                        // off-distance should be scaled according to thickness
                        for (int i = 0; i < items.Count(); i++)
                        {
                            if (i % 2 != 0)
                            {
                                items[i] = items[i] * ((float)pancake.Border.Thickness.Left * 0.5f);
                            }
                        }

                        paint.SetPathEffect(new DashPathEffect(items, 0));
                    }

                    if (pancake.Border.GradientStops != null && pancake.Border.GradientStops.Any())
                    {
                        // A range of colors is given. Let's add them.
                        var orderedStops = pancake.Border.GradientStops.OrderBy(x => x.Offset).ToList();
                        var colors = orderedStops.Select(x => x.Color.ToAndroid().ToArgb()).ToArray();
                        var locations = orderedStops.Select(x => x.Offset).ToArray();

                        var shader = new LinearGradient((float)(canvas.Width * pancake.Border.GradientStartPoint.X),
                             (float)(canvas.Height * pancake.Border.GradientStartPoint.Y),
                             (float)(canvas.Width * pancake.Border.GradientEndPoint.X),
                             (float)(canvas.Height * pancake.Border.GradientEndPoint.Y),
                             colors, locations, Shader.TileMode.Clamp);
                        paint.SetShader(shader);
                    }
                    else
                    {
                        paint.Color = pancake.Border.Color.ToAndroid();
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