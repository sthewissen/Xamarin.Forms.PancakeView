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
            if(!initialize)
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
        }

        private void OnLayoutUpdated(object sender, LayoutEventArgs e)
        {
            _skCanvasView.Geometry = Control.Geometry;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear();
            DrawBackground(canvas);
            DrawBorder(canvas);
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
                    path = DrawingExtensions.CreatePolygonPath(_skCanvasView.Geometry.Width, _skCanvasView.Geometry.Height,
                        pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);
                }
                else
                {
                    path = DrawingExtensions.CreateRoundedRectPath(_skCanvasView.Geometry.Width, _skCanvasView.Geometry.Height, pancake.CornerRadius);
                }

                if (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Any())
                {
                    var orderedStops = pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                    var gradientColors = orderedStops.Select(x => x.Color.ToNative().ToSKColor()).ToArray();
                    var gradientColorPos = orderedStops.Select(x => x.Offset).ToArray();
                    var startPoint = new SKPoint((float)(pancake.BackgroundGradientStartPoint.X * _skCanvasView.Geometry.Width), (float)(pancake.BackgroundGradientStartPoint.Y * _skCanvasView.Geometry.Height));
                    var endPoint = new SKPoint((float)(pancake.BackgroundGradientEndPoint.X * _skCanvasView.Geometry.Width), (float)(pancake.BackgroundGradientEndPoint.Y * _skCanvasView.Geometry.Height));
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
            using (var paint = new SKPaint())
            {
                var pancake = Element as PancakeView;
                if (pancake.Border != null && pancake.Border.Thickness != default)
                {
                    var border = pancake.Border;
                    var boderColor = border.Color == Color.Default ? Color.Default : border.Color;
                    paint.Style = SKPaintStyle.Stroke;
                    paint.Color = boderColor.ToNative().ToSKColor();
                    paint.StrokeWidth = border.Thickness;
                    paint.IsAntialias = true;

                    SKPath path;
                    if (pancake.Sides != 4)
                    {
                        path = DrawingExtensions.CreatePolygonPath(_skCanvasView.Geometry.Width, _skCanvasView.Geometry.Height,
                            pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);
                    }
                    else
                    {
                        path = DrawingExtensions.CreateRoundedRectPath(_skCanvasView.Geometry.Width, _skCanvasView.Geometry.Height, pancake.CornerRadius);
                    }

                    if (border.DashPattern.Pattern != null && border.DashPattern.Pattern.Length > 0)
                    {
                        var dashPattern = border.DashPattern.Pattern;
                        float[] patternInFloat = new float[dashPattern.Length];
                        patternInFloat = Array.ConvertAll(dashPattern, item => (float)item);
                        paint.PathEffect = SKPathEffect.CreateDash(patternInFloat, 0);
                    }

                    if (border.GradientStops != null && border.GradientStops.Any())
                    {
                        var startPoint = new SKPoint((float)(border.GradientStartPoint.X * _skCanvasView.Geometry.Width), (float)(border.GradientStartPoint.Y * _skCanvasView.Geometry.Height));
                        var endPoint = new SKPoint((float)(border.GradientEndPoint.X * _skCanvasView.Geometry.Width), (float)(border.GradientEndPoint.Y * _skCanvasView.Geometry.Height));
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
