using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using AppKit;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView.MacOS;
using Xamarin.Forms.Platform.MacOS;
using Controls = Xamarin.Forms.PancakeView;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.MacOS
{
    public class PancakeViewRenderer : ViewRenderer<PancakeView, NSView>
    {
        private NSView _actualView;
        private NSView _wrapperView;
        private NSColor _colorToRender;
        private CGSize _previousSize;

        public override bool WantsDefaultClipping => false;

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

            if (e.NewElement != null)
            {
                _actualView = new NoClippingView();
                _wrapperView = new NoClippingView();

                // Add the subviews to the actual view.
                foreach (var item in NativeView.Subviews)
                {
                    _actualView.AddSubview(item);
                }

                // If this contains GestureRecognizers, hook those up to the _actualView.
                // This is what the user sees and interacts with.
                if (NativeView.GestureRecognizers != null)
                {
                    foreach (var gesture in NativeView.GestureRecognizers)
                    {
                        _actualView.AddGestureRecognizer(gesture);
                    }
                }

                _wrapperView.AddSubview(_actualView);

                // Override the native control.
                SetNativeControl(_wrapperView);

                SetBackgroundColor(Element.BackgroundColor);
                SetCornerRadius();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                SetBackgroundColor(Element.BackgroundColor);
            }
            else if (e.PropertyName == BoxView.CornerRadiusProperty.PropertyName)
            {
                SetCornerRadius();
            }
            else if (e.PropertyName == PancakeView.BorderProperty.PropertyName)
            {
                DrawBorder();
            }
            else if ((e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName) ||
                    (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && Element.IsVisible) ||
                    (e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.SidesProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.ShadowProperty.PropertyName))
            {
                NeedsDisplay = true;
            }
        }

        public override void Layout()
        {
            if (_previousSize != Bounds.Size)
                NeedsDisplay = true;

            base.Layout();
        }

        public override void DrawRect(CGRect rect)
        {
            _actualView.Frame = Bounds;
            _wrapperView.Frame = Bounds;

            DrawBackground();
            DrawShadow();
            DrawBorder();

            base.DrawRect(rect);

            _previousSize = Bounds.Size;
        }

        private void SetCornerRadius()
        {
            if (Element == null)
                return;

            NeedsDisplay = true;
        }

        protected override void SetBackgroundColor(Color color)
        {
            if (Element == null)
                return;

            var elementColor = Element.BackgroundColor;

            if (!elementColor.IsDefault)
                _colorToRender = elementColor.ToNSColor();
            else
                _colorToRender = color.ToNSColor();

            NeedsDisplay = true;
        }

        private void DrawBackground()
        {
            var layerName = "backgroundLayer";

            // Remove previous background layer if any
            var prevBackgroundLayer = _actualView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBackgroundLayer?.RemoveFromSuperLayer();

            NSBezierPath cornerPath = null;

            if (Element.Sides != 4)
            {
                cornerPath = Bounds.CreatePolygonPath(Element.Sides, Element.CornerRadius.TopLeft, Element.OffsetAngle);
            }
            else
            {
                cornerPath = Bounds.CreateRoundedRectPath(Element.CornerRadius);
            }

            // The layer used to mask other layers we draw on the background.
            var maskLayer = new CAShapeLayer
            {
                Frame = Bounds,
                Path = cornerPath.ToCGPath()
            };

            _actualView.Layer.Mask = maskLayer;
            _actualView.Layer.MasksToBounds = true;

            if (Element.BackgroundGradientStops != null && Element.BackgroundGradientStops.Any())
            {
                // Create a gradient layer that draws our background.
                var gradientLayer = CreateGradientLayer(Element.BackgroundGradientAngle, Bounds);
                gradientLayer.Name = layerName;

                // A range of colors is given. Let's add them.
                var orderedStops = Element.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                gradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
                gradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();

                AddLayer(gradientLayer, 0, _actualView);
            }
            else
            {
                // Create a shape layer that draws our background.
                var shapeLayer = new CAShapeLayer
                {
                    Frame = Bounds,
                    Path = cornerPath.ToCGPath(),
                    MasksToBounds = true,
                    FillColor = _colorToRender.CGColor,
                    Name = layerName
                };

                AddLayer(shapeLayer, 0, _actualView);
            }
        }

        private void DrawBorder()
        {
            var layerName = "borderLayer";

            // remove previous background layer if any
            var prevBorderLayer = _actualView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBorderLayer?.RemoveFromSuperLayer();

            if (Element.Border != null && Element.Border.BorderThickness != default)
            {
                var borderLayer = new CAShapeLayer
                {
                    StrokeColor = Element.Border.BorderColor == Color.Default ? NSColor.Clear.CGColor : Element.Border.BorderColor.ToCGColor(),
                    FillColor = null,
                    LineWidth = (nfloat)Element.Border.BorderThickness.Left,
                    Name = layerName
                };

                // Create arcs for the given corner radius.
                borderLayer.Path = Element.Sides != 4 ?
                    Bounds.CreatePolygonPath(Element.Sides, Element.CornerRadius.TopLeft, Element.OffsetAngle).ToCGPath() :
                    Bounds.CreateRoundedRectPath(Element.CornerRadius).ToCGPath();

                var layerPosition = new CGPoint(borderLayer.Path.BoundingBox.Width / 2, borderLayer.Path.BoundingBox.Height / 2);

                borderLayer.Frame = borderLayer.Path.BoundingBox;
                borderLayer.Position = layerPosition;
                borderLayer.MasksToBounds = false;

                // Dash pattern for the border.
                if (Element.Border.BorderDashPattern.Pattern != null && Element.Border.BorderDashPattern.Pattern.Length > 0)
                {
                    var items = Element.Border.BorderDashPattern.Pattern.Select(x => new NSNumber(x)).ToArray();
                    borderLayer.LineDashPattern = items;
                }

                if (Element.Border.BorderGradientStops != null && Element.Border.BorderGradientStops.Any())
                {
                    var gradientFrame = Bounds.Inset(-(nfloat)Element.Border.BorderThickness.Left, -(nfloat)Element.Border.BorderThickness.Left);
                    var gradientLayer = CreateGradientLayer(Element.Border.BorderGradientAngle, gradientFrame);
                    gradientLayer.Position = new CGPoint((gradientFrame.Width / 2) - ((nfloat)Element.Border.BorderThickness.Left), (gradientFrame.Height / 2) - ((nfloat)Element.Border.BorderThickness.Left));

                    // Create a clone from the border layer and use that one as the mask.
                    // Why? Because the mask and the border somehow can't be the same, so
                    // don't want to do adjustments to borderLayer because it would influence the border.
                    var maskLayer = new CAShapeLayer()
                    {
                        Path = borderLayer.Path,
                        Position = new CGPoint(Element.Border.BorderThickness.Left, Element.Border.BorderThickness.Left),
                        FillColor = null,
                        LineWidth = (nfloat)Element.Border.BorderThickness.Left,
                        StrokeColor = NSColor.Red.CGColor,
                        LineDashPattern = borderLayer.LineDashPattern
                    };

                    gradientLayer.Mask = maskLayer;
                    gradientLayer.Name = layerName;

                    // A range of colors is given. Let's add them.
                    var orderedStops = Element.Border.BorderGradientStops.OrderBy(x => x.Offset).ToList();
                    gradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
                    gradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();

                    AddLayer(gradientLayer, -1, _actualView);
                }
                else
                {
                    AddLayer(borderLayer, -1, _actualView);
                }
            }
        }

        private void DrawShadow()
        {
            var cornerRadius = (nfloat)Element.CornerRadius.TopLeft;

            if (Element.Shadow != null)
            {
                // HACK: Somehow this is needed for _wrapperView.Layer's shadow related properties to work.
                _wrapperView.Shadow = new NSShadow();
                DrawDefaultShadow(_wrapperView.Layer, Bounds, cornerRadius);
            }
            else
            {
                // Reset to zero.
                _wrapperView.Layer.ShadowOpacity = 0;
            }

            // Set the rasterization for performance optimization.
            _wrapperView.Layer.RasterizationScale = NSScreen.MainScreen.BackingScaleFactor;
            _wrapperView.Layer.ShouldRasterize = true;
            _actualView.Layer.RasterizationScale = NSScreen.MainScreen.BackingScaleFactor;
            _actualView.Layer.ShouldRasterize = true;
        }

        private void DrawDefaultShadow(CALayer layer, CGRect bounds, nfloat cornerRadius)
        {
            // Ideally we want to be able to have individual corner radii + shadows
            // However, on iOS we can only do one radius + shadow.
            layer.CornerRadius = cornerRadius;
            layer.ShadowRadius = Element.Shadow.BlurRadius;
            layer.ShadowColor = Element.Shadow.Color.ToCGColor();
            layer.ShadowOpacity = Element.Shadow.Opacity;
            layer.ShadowOffset = new SizeF((float)Element.Shadow.Offset.X, (float)Element.Shadow.Offset.Y);

            if (Element.Sides != 4)
            {
                layer.ShadowPath = bounds.CreatePolygonPath(Element.Sides, Element.CornerRadius.TopLeft, Element.OffsetAngle).ToCGPath();
            }
            else
            {
                layer.ShadowPath = bounds.CreateRoundedRectPath(Element.CornerRadius).ToCGPath();
            }
        }

        private CAGradientLayer CreateGradientLayer(int angle, CGRect rect)
        {
            var totalAngle = angle / 360.0;

            // Calculate the new positions based on angle between 0-360.
            var a = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.75) / 2)), 2);
            var b = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.0) / 2)), 2);
            var c = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.25) / 2)), 2);
            var d = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.5) / 2)), 2);

            // Create a gradient layer that draws our background.
            return new CAGradientLayer
            {
                Frame = rect,
                StartPoint = new CGPoint(1 - a, b),
                EndPoint = new CGPoint(1 - c, d)
            };
        }

        public void AddLayer(CALayer layer, int position, NSView viewToAddTo)
        {
            // If there is already a layer with the given name, remove it before inserting.
            if (layer != null)
            {
                // There's no background layer yet, insert it.
                if (position > -1)
                    viewToAddTo.Layer.InsertSublayer(layer, position);
                else
                    viewToAddTo.Layer.AddSublayer(layer);
            }
        }

        class NoClippingView : NSView
        {
            public override bool WantsDefaultClipping => false;

            public NoClippingView()
            {
                WantsLayer = true;
                Layer = new NoClippingLayer();
            }
        }

        class NoClippingLayer : CALayer
        {
            public override bool MasksToBounds => false;
        }
    }
}