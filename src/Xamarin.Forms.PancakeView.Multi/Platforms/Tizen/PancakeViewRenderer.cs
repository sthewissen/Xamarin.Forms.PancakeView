using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using Xamarin.Forms.Platform.Tizen.Native;
using Xamarin.Forms.PancakeView.Tizen;
using Controls = Xamarin.Forms.PancakeView;
using SkiaSharp;
using SkiaSharp.Views.Tizen;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.Tizen
{
    public class PancakeViewRenderer : LayoutRenderer
    {
        SKCanvasView _skCanvasView;

        public PancakeViewRenderer() : base()
        {
        }

        /// <summary>
        /// This method ensures that we don't get stripped out by the linker.
        /// </summary>
        public static void Init()
        {
#pragma warning disable 0219
            var ignore1 = typeof(PancakeViewRenderer);
            var ignore2 = typeof(PancakeView);
#pragma warning restore 0219
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
        {
            if (Control == null)
            {
                SetNativeControl(new Canvas(Forms.NativeParent));
                _skCanvasView = new SKCanvasView(Forms.NativeParent);
                _skCanvasView.PaintSurface += OnPaintSurface;
                _skCanvasView.Show();
                Control.Children.Add(_skCanvasView);
                Interop.evas_object_clip_unset(_skCanvasView);
                Control.LayoutUpdated += OnLayoutUpdated;
            }
            base.OnElementChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (Control != null)
                {
                    Control.LayoutUpdated -= OnLayoutUpdated;
                }
                if (_skCanvasView != null)
                {
                    _skCanvasView.PaintSurface -= OnPaintSurface;
                    _skCanvasView.Unrealize();
                    _skCanvasView = null;
                }
            }
        }

        protected override void UpdateBackgroundColor(bool initialize)
        {
            if (!initialize)
            {
                Invalidate();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == PancakeView.BorderProperty.PropertyName ||
                e.PropertyName == PancakeView.CornerRadiusProperty.PropertyName ||
                e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.SidesProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStartPointProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientEndPointProperty.PropertyName)
            {
                Invalidate();
            }
            else if (e.PropertyName == PancakeView.ShadowProperty.PropertyName)
            {
                UpdateCanvasGeometry();
                Invalidate();
            }
        }

        private void OnLayoutUpdated(object sender, LayoutEventArgs e)
        {
            UpdateCanvasGeometry();
        }

        private void UpdateCanvasGeometry()
        {
            var pancake = Element as PancakeView;
            if (pancake.Shadow != null)
            {
                double left = 0;
                double top = 0;
                double right = 0;
                double bottom = 0;
                var scaledOffsetX = Forms.ConvertToScaledPixel(pancake.Shadow.Offset.X);
                var scaledOffsetY = Forms.ConvertToScaledPixel(pancake.Shadow.Offset.Y);
                var scaledBlurRadius = Forms.ConvertToScaledPixel(pancake.Shadow.BlurRadius);
                var spreadSize = scaledBlurRadius * 2 + scaledBlurRadius;
                var sl = scaledOffsetX - spreadSize;
                var sr = scaledOffsetX + spreadSize;
                var st = scaledOffsetY - spreadSize;
                var sb = scaledOffsetY + spreadSize;
                if (left > sl) left = sl;
                if (top > st) top = st;
                if (right < sr) right = sr;
                if (bottom < sb) bottom = sb;

                var geometry = Control.Geometry;
                _skCanvasView.Geometry = new ElmSharp.Rect(geometry.X + (int)left, geometry.Y + (int)top, geometry.Width + (int)right - (int)left, geometry.Height + (int)bottom - (int)top);
            }
            else
            {
                _skCanvasView.Geometry = Control.Geometry;
            }
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear();
            DrawShadow(canvas);
            DrawBackground(canvas);
            DrawBorder(canvas);
        }

        private void DrawShadow(SKCanvas canvas)
        {
            var pancake = Element as PancakeView;
            if (pancake.Shadow != null)
            {
                SKPath path;
                if (pancake.Sides != 4)
                {
                    path = DrawingExtensions.CreatePolygonPath(Control.Geometry.Width, Control.Geometry.Height,
                        pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);
                }
                else
                {
                    var left = Control.Geometry.Left - _skCanvasView.Geometry.Left;
                    var top = Control.Geometry.Top - _skCanvasView.Geometry.Top;
                    path = DrawingExtensions.CreateRoundedRectPath(left, top, left + Control.Geometry.Width, top + Control.Geometry.Height, pancake.CornerRadius);
                }

                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.StrokeAndFill;

                    var shadow = pancake.Shadow;
                    var scaledOffsetX = Forms.ConvertToScaledPixel(shadow.Offset.X);
                    var scaledOffsetY = Forms.ConvertToScaledPixel(shadow.Offset.Y);
                    var scaledBlurRadius = Forms.ConvertToScaledPixel(shadow.BlurRadius);

                    canvas.Save();
                    canvas.ClipPath(path, SKClipOperation.Difference, true);
                    paint.ImageFilter = SKImageFilter.CreateDropShadow(
                        scaledOffsetX,
                        scaledOffsetY,
                        scaledBlurRadius,
                        scaledBlurRadius,
                        shadow.Color.MultiplyAlpha(shadow.Opacity).ToNative().ToSKColor(),
                        SKDropShadowImageFilterShadowMode.DrawShadowOnly);
                    canvas.DrawPath(path, paint);
                    canvas.Restore();

                    canvas.Save();
                    canvas.ClipPath(path, SKClipOperation.Intersect, true);
                    canvas.DrawPath(path, paint);
                    canvas.Restore();
                }
            }
        }

        private void DrawBackground(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
            {
                paint.Style = SKPaintStyle.Fill;
                paint.IsAntialias = true;

                var pancake = Element as PancakeView;
                SKPath path;
                if (pancake.Sides != 4)
                {
                    path = DrawingExtensions.CreatePolygonPath(Control.Geometry.Width, Control.Geometry.Height,
                        pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);
                }
                else
                {
                    var left = Control.Geometry.Left - _skCanvasView.Geometry.Left;
                    var top = Control.Geometry.Top - _skCanvasView.Geometry.Top;
                    path = DrawingExtensions.CreateRoundedRectPath(left, top, left + Control.Geometry.Width, top + Control.Geometry.Height, pancake.CornerRadius);
                }

                if (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Any())
                {
                    var orderedStops = pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                    var gradientColors = orderedStops.Select(x => x.Color.ToNative().ToSKColor()).ToArray();
                    var gradientColorPos = orderedStops.Select(x => x.Offset).ToArray();
                    var startPoint = new SKPoint((float)(pancake.BackgroundGradientStartPoint.X * Control.Geometry.Width), (float)(pancake.BackgroundGradientStartPoint.Y * Control.Geometry.Height));
                    var endPoint = new SKPoint((float)(pancake.BackgroundGradientEndPoint.X * Control.Geometry.Width), (float)(pancake.BackgroundGradientEndPoint.Y * Control.Geometry.Height));
                    paint.Shader = SKShader.CreateLinearGradient(startPoint, endPoint, gradientColors, gradientColorPos, SKShaderTileMode.Clamp);
                }
                else
                {
                    paint.Color = pancake.BackgroundColor.ToNative().ToSKColor();
                }
                canvas.ClipPath(path, SKClipOperation.Intersect, true);
                canvas.DrawPath(path, paint);
            }
        }

        private void DrawBorder(SKCanvas canvas)
        {
            var pancake = Element as PancakeView;
            if (pancake.Border != null && pancake.Border.Thickness != default)
            {
                using (var paint = new SKPaint())
                {
                    var border = pancake.Border;
                    paint.Style = SKPaintStyle.Stroke;
                    paint.Color = border.Color.ToNative().ToSKColor();
                    paint.StrokeWidth = Forms.ConvertToScaledPixel(border.Thickness);
                    paint.IsAntialias = true;

                    SKPath path;
                    if (pancake.Sides != 4)
                    {
                        path = DrawingExtensions.CreatePolygonPath(Control.Geometry.Width, Control.Geometry.Height,
                            pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);
                    }
                    else
                    {
                        var left = Control.Geometry.Left - _skCanvasView.Geometry.Left;
                        var top = Control.Geometry.Top - _skCanvasView.Geometry.Top;
                        path = DrawingExtensions.CreateRoundedRectPath(left, top, left + Control.Geometry.Width, top + Control.Geometry.Height, pancake.CornerRadius);
                    }

                    if (border.DashPattern.Pattern != null &&
                        border.DashPattern.Pattern.Length > 0 &&
                        border.DashPattern.Pattern.Length % 2 == 0)
                    {
                        var dashPattern = border.DashPattern.Pattern;
                        float[] patternInFloat = new float[dashPattern.Length];
                        patternInFloat = Array.ConvertAll(dashPattern, item => (float)item);
                        paint.PathEffect = SKPathEffect.CreateDash(patternInFloat, 0);
                    }

                    if (border.GradientStops != null && border.GradientStops.Any())
                    {
                        var startPoint = new SKPoint((float)(border.GradientStartPoint.X * Control.Geometry.Width), (float)(border.GradientStartPoint.Y * Control.Geometry.Height));
                        var endPoint = new SKPoint((float)(border.GradientEndPoint.X * Control.Geometry.Width), (float)(border.GradientEndPoint.Y * Control.Geometry.Height));
                        var orderedStops = border.GradientStops.OrderBy(x => x.Offset).ToList();
                        var gradientColors = orderedStops.Select(x => x.Color.ToNative().ToSKColor()).ToArray();
                        var gradientColorPos = orderedStops.Select(x => x.Offset).ToArray();
                        paint.Shader = SKShader.CreateLinearGradient(startPoint, endPoint, gradientColors, gradientColorPos, SKShaderTileMode.Clamp);
                    }
                    canvas.ClipPath(path, SKClipOperation.Intersect, true);
                    canvas.DrawPath(path, paint);
                }
            }
        }

        private void Invalidate()
        {
            _skCanvasView.Invalidate();
        }
    }
}
