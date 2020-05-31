using System;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView.Droid;
using Xamarin.Forms.Platform.Android;
using ACanvas = Android.Graphics.Canvas;
using Controls = Xamarin.Forms.PancakeView;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.Droid
{
    public class PancakeViewRenderer : VisualElementRenderer<PancakeView>
    {
        Drawable _defaultDrawable;
        PancakeDrawable _drawable;
        bool _disposed;
        bool _drawableEnabled;

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

        protected override void OnElementChanged(ElementChangedEventArgs<PancakeView> e)
        {
            base.OnElementChanged(e);

            Reset();

            SetClipChildren(false);
            SetClipToPadding(false);

            UpdateDrawable();

            //if (e.OldElement != null)
            //{
            //    if (e.OldElement is INotifyPropertyChanged oldElement)
            //        oldElement.PropertyChanged -= DrawablePropertyChanged;
            //}

            //if (e.NewElement != null)
            //{
            //    if (e.NewElement is INotifyPropertyChanged newElement)
            //        newElement.PropertyChanged += DrawablePropertyChanged;
            //}

            //if (e.NewElement != null && e.OldElement == null)
            //{
            //    var pancake = Element as PancakeView;

            //    // HACK: When there are no children we add a Grid element to trigger DrawChild.
            //    // This should be improved upon, but I haven't found out a nice way to be able to clip
            //    // the children and add the border on top without using DrawChild.
            //    if (pancake.Content == null)
            //    {
            //        pancake.Content = new Grid();
            //    }

            //    this.SetBackground(_drawable = new PancakeDrawable(pancake, Context.ToPixels));

            //    //SetupShadow(pancake);
            //}
        }

        public void Reset()
        {
            if (_drawableEnabled)
            {
                _drawableEnabled = false;
                _drawable?.Reset();
                _drawable = null;
            }
        }

        protected override void OnDraw(ACanvas canvas)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
                e.PropertyName == PancakeView.CornerRadiusProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName ||
                e.PropertyName == PancakeView.ShadowProperty.PropertyName ||
                e.PropertyName == PancakeView.SidesProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderProperty.PropertyName ||
                e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName)
            {
                Reset();
                UpdateDrawable();
            }
        }

        public void UpdateDrawable()
        {
            if (Element == null)
                return;

            bool cornerRadiusIsDefault = Element.CornerRadius == (CornerRadius)PancakeView.CornerRadiusProperty.DefaultValue;
            bool backgroundColorIsDefault = !Element.BackgroundGradientStops.Any() && Element.BackgroundColor == (Color)VisualElement.BackgroundColorProperty.DefaultValue;
            bool borderColorIsDefault = Element.Border == null || Element.Border.Color == (Color)Border.ColorProperty.DefaultValue;
            bool borderWidthIsDefault = Element.Border == null || Element.Border.Thickness == (Thickness)Border.ThicknessProperty.DefaultValue;

            if (backgroundColorIsDefault && borderColorIsDefault && borderWidthIsDefault && cornerRadiusIsDefault)
            {
                if (!_drawableEnabled)
                    return;

                if (_defaultDrawable != null)
                    this.SetBackground(_defaultDrawable);

                _drawableEnabled = false;
                Reset();
            }
            else
            {
                if (_drawable == null)
                    _drawable = new PancakeDrawable(Element, Context.ToPixels);

                _drawable.SetPadding(this.PaddingTop, this.PaddingLeft);

                if (Element.Shadow != null)
                {
                    _drawable.SetShadow((float)Element.Shadow.Offset.Y, (float)Element.Shadow.Offset.X, Element.Shadow.Color, Element.Shadow.BlurRadius);
                }

                if (_drawableEnabled)
                    return;

                if (_defaultDrawable == null)
                    _defaultDrawable = this.Background;

                if (!backgroundColorIsDefault)
                {
                    this.SetBackground(_drawable);
                }

                _drawableEnabled = true;
            }

            this.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _drawable?.Dispose();
                    _drawable = null;

                    _defaultDrawable?.Dispose();
                    _defaultDrawable = null;
                }

                _disposed = true;
            }
        }

        //        private void SetupShadow(PancakeView pancake)
        //        {
        //            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        //            {
        //                // clear previous shadow/elevation
        //                this.Elevation = 0;
        //                this.TranslationZ = 0;

        //                bool hasShadowOrElevation = pancake.Shadow != null;

        //                if (pancake.Shadow != null)
        //                {
        //                    ViewCompat.SetElevation(this, Context.ToPixels(pancake.Shadow.BlurRadius));

        //#if MONOANDROID90
        //                    // Color only exists on Pie and beyond.
        //                    if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
        //                    {
        //                        this.SetOutlineAmbientShadowColor(pancake.Shadow.Color.ToAndroid());
        //                        this.SetOutlineSpotShadowColor(pancake.Shadow.Color.ToAndroid());
        //                    }
        //#endif
        //                }

        //                if (hasShadowOrElevation)
        //                {
        //                    // To have shadow show up, we need to clip.
        //                    this.OutlineProvider = new RoundedCornerOutlineProvider(pancake, Context.ToPixels);
        //                    this.ClipToOutline = true;
        //                }
        //                else
        //                {
        //                    this.OutlineProvider = null;
        //                    this.ClipToOutline = false;
        //                }
        //            }
        //        }

        //protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    var pancake = Element as PancakeView;

        //    base.OnElementPropertyChanged(sender, e);

        //    if (e.PropertyName == PancakeView.BorderProperty.PropertyName ||
        //    //{
        //    //    Invalidate();
        //    //}
        //    //else if (
        //        e.PropertyName == PancakeView.SidesProperty.PropertyName ||
        //        e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName ||
        //        e.PropertyName == PancakeView.ShadowProperty.PropertyName ||
        //    //    )
        //    //{
        //    //    SetupShadow(pancake);
        //    //}
        //    //else if (
        //        e.PropertyName == PancakeView.CornerRadiusProperty.PropertyName || //)
        //    //{
        //    //    Invalidate();
        //    //    SetupShadow(pancake);
        //    //}
        //    //else if (
        //        e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
        //    {
        //        _drawable.Dispose();
        //        this.SetBackground(_drawable = new PancakeDrawable(pancake, Context.ToPixels));
        //    }
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);

        //    if (disposing && !_disposed)
        //    {
        //        _drawable?.Dispose();
        //        _disposed = true;
        //    }
        //}

        //protected override void OnDraw(ACanvas canvas)
        //{
        //    if (Element == null) return;

        //    var pancake = Element as PancakeView;

        //    SetClipChildren(true);

        //    //Create path to clip the child
        //    if (pancake.Sides != 4)
        //    {
        //        //using (var path = DrawingExtensions.CreatePolygonPath(Width, Height, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle))
        //        //{
        //        //    canvas.Save();
        //        //    canvas.ClipPath(path);
        //        //}
        //    }
        //    else
        //    {
        //        //var clipPath = DrawingExtensions.CreateRoundedRectPath(Width, Height,
        //        //    Context.ToPixels(pancake.CornerRadius.TopLeft),
        //        //    Context.ToPixels(pancake.CornerRadius.TopRight),
        //        //    Context.ToPixels(pancake.CornerRadius.BottomRight),
        //        //    Context.ToPixels(pancake.CornerRadius.BottomLeft)))

        //        //var saveCount = canvas.SaveLayer(0, 0, Width, Height, null);
        //        //canvas.DrawPath(clipPath, Paint);
        //        //canvas.DrawPath(clipPath, _strokePaint);
        //        //canvas.DrawPath(_maskPath, _maskPaint);
        //        //canvas.RestoreToCount(saveCount);

        //        using (var path = DrawingExtensions.CreateRoundedRectPath(Width, Height,
        //            Context.ToPixels(pancake.CornerRadius.TopLeft),
        //            Context.ToPixels(pancake.CornerRadius.TopRight),
        //            Context.ToPixels(pancake.CornerRadius.BottomRight),
        //            Context.ToPixels(pancake.CornerRadius.BottomLeft)))
        //        {
        //            canvas.Save();
        //            //canvas.ClipPath(path);
        //        }
        //    }

        //    //DrawBorder(canvas, pancake);
        //}

        //protected override bool DrawChild(ACanvas canvas, global::Android.Views.View child, long drawingTime)
        //{
        //    if (Element == null) return false;

        //    var pancake = Element as PancakeView;

        //    SetClipChildren(true);

        //    //Create path to clip the child         
        //    if (pancake.Sides != 4)
        //    {
        //        var path = DrawingExtensions.CreatePolygonPath(Width, Height, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);

        //        canvas.Save();
        //        canvas.ClipPath(path);
        //    }
        //    else
        //    {
        //        // adjust border radius so outer edge of stroke is same radius as border radius of background
        //        float topLeft = Context.ToPixels(Element.CornerRadius.TopLeft);
        //        float topRight = Context.ToPixels(Element.CornerRadius.TopRight);
        //        float bottomRight = Context.ToPixels(Element.CornerRadius.BottomRight);
        //        float bottomLeft = Context.ToPixels(Element.CornerRadius.BottomLeft);

        //        var path = DrawingExtensions.CreateRoundedRectPath(Width, Height, topLeft, topRight, bottomRight, bottomLeft);
        //        canvas.Save();
        //        canvas.ClipPath(path);
        //    }

        //    // Draw the child first so that the border shows up above it.        
        //    var result = base.DrawChild(canvas, child, drawingTime);
        //    canvas.Restore();

        //    //canvas.DrawBitmap(_borderDrawable)
        //    //DrawBorder(canvas, pancake);

        //    return result;
        //}

        //private void DrawBorder(ACanvas canvas, PancakeView pancake)
        //{
        //    if (pancake.Border != null && pancake.Border.Thickness != default)
        //    {
        //        float borderWidth = Context.ToPixels(Element.Border.Thickness.Left);
        //        float inset = borderWidth / 2;

        //        // adjust border radius so outer edge of stroke is same radius as border radius of background
        //        float topLeft = Math.Max(Context.ToPixels(Element.CornerRadius.TopLeft) - inset, 0);
        //        float topRight = Math.Max(Context.ToPixels(Element.CornerRadius.TopRight) - inset, 0);
        //        float bottomRight = Math.Max(Context.ToPixels(Element.CornerRadius.BottomRight) - inset, 0);
        //        float bottomLeft = Math.Max(Context.ToPixels(Element.CornerRadius.BottomLeft) - inset, 0);

        //        RectF rect = new RectF(0, 0, (float)Width, (float)Height);
        //        rect.Inset(inset + PaddingLeft, inset + PaddingTop);

        //        var paint = new Paint { AntiAlias = true };
        //        Path path = null;

        //        if (pancake.Sides != 4)
        //            path = DrawingExtensions.CreatePolygonPath(rect, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);

        //        else
        //            path = DrawingExtensions.CreateRoundedRectPath(rect, topLeft, topRight, bottomRight, bottomLeft);

        //        if (pancake.Border.DashPattern.Pattern != null && pancake.Border.DashPattern.Pattern.Length > 0)
        //        {
        //            var items = pancake.Border.DashPattern.Pattern.Select(x => Context.ToPixels(Convert.ToSingle(x))).ToArray();
        //            paint.SetPathEffect(new DashPathEffect(items, 0));
        //        }

        //        if (pancake.Border.GradientStops != null && pancake.Border.GradientStops.Any())
        //        {
        //            var angle = pancake.Border.GradientAngle / 360.0;

        //            // Calculate the new positions based on angle between 0-360.
        //            var a = canvas.Width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
        //            var b = canvas.Height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
        //            var c = canvas.Width * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
        //            var d = canvas.Height * Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

        //            // A range of colors is given. Let's add them.
        //            var orderedStops = pancake.Border.GradientStops.OrderBy(x => x.Offset).ToList();
        //            var colors = orderedStops.Select(x => x.Color.ToAndroid().ToArgb()).ToArray();
        //            var locations = orderedStops.Select(x => x.Offset).ToArray();

        //            var shader = new LinearGradient(canvas.Width - (float)a, (float)b, canvas.Width - (float)c, (float)d, colors, locations, Shader.TileMode.Clamp);
        //            paint.SetShader(shader);
        //        }
        //        else
        //        {
        //            paint.Color = pancake.Border.Color.ToAndroid();
        //        }

        //        paint.StrokeWidth = borderWidth;
        //        paint.SetStyle(Paint.Style.Stroke);

        //        canvas.DrawPath(path, paint);
        //    }
        //}
    }
}