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
                Validate(Element as PancakeView);

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

            Validate(Element as PancakeView);

            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                SetBackgroundColor(Element.BackgroundColor);
            }
            else if (e.PropertyName == BoxView.CornerRadiusProperty.PropertyName)
            {
                SetCornerRadius();
            }
            else if ((e.PropertyName == PancakeView.BorderColorProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderDrawingStyleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderGradientAngleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderGradientEndColorProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderGradientStartColorProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderGradientStopsProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderIsDashedProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderDashPatternProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName))
            {
                DrawBorder();
            }
            else if ((e.PropertyName == PancakeView.BackgroundGradientStartColorProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BackgroundGradientEndColorProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName) ||
                    (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && Element.IsVisible) ||
                    (e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.HasShadowProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.ShadowProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.ElevationProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.SidesProperty.PropertyName))
            {
                NeedsDisplay = true;
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

            // Needs to be an even number of parts, but if its null or 0 elements, we simply don't dash.
            if (pancake.BorderDashPattern.Pattern != null && pancake.BorderDashPattern.Pattern.Length != 0 &&
                (pancake.BorderDashPattern.Pattern?.Length >= 2 && pancake.BorderDashPattern.Pattern.Length % 2 != 0))
                throw new ArgumentException("BorderDashPattern must contain an even number of entries (>=2).", nameof(Controls.PancakeView.BorderDashPattern));
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
            var pancake = Element as PancakeView;
            var layerName = "backgroundLayer";

            // Remove previous background layer if any
            var prevBackgroundLayer = _actualView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBackgroundLayer?.RemoveFromSuperLayer();

            NSBezierPath cornerPath = null;

            if (pancake.Sides != 4)
            {
                cornerPath = ShapeUtils.CreatePolygonPath(Bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);
            }
            else
            {
                cornerPath = ShapeUtils.CreateRoundedRectPath(Bounds, pancake.CornerRadius);
            }

            // The layer used to mask other layers we draw on the background.
            var maskLayer = new CAShapeLayer
            {
                Frame = Bounds,
                Path = cornerPath.ToCGPath()
            };

            _actualView.Layer.Mask = maskLayer;
            _actualView.Layer.MasksToBounds = true;

            if ((pancake.BackgroundGradientStartColor != default(Color) && pancake.BackgroundGradientEndColor != default(Color)) ||
                (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Any()))
            {
                // Create a gradient layer that draws our background.
                var gradientLayer = CreateGradientLayer(pancake.BackgroundGradientAngle, Bounds);
                gradientLayer.Name = layerName;

                if (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Count > 0)
                {
                    // A range of colors is given. Let's add them.
                    var orderedStops = pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                    gradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
                    gradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();
                }
                else
                {
                    // Only two colors provided, use that.
                    gradientLayer.Colors = new CGColor[] { pancake.BackgroundGradientStartColor.ToCGColor(), pancake.BackgroundGradientEndColor.ToCGColor() };
                }

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
            var pancake = Element as PancakeView;
            var layerName = "borderLayer";

            // remove previous background layer if any
            var prevBorderLayer = _actualView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBorderLayer?.RemoveFromSuperLayer();

            if (pancake.BorderThickness > 0)
            {
                var borderLayer = new CAShapeLayer
                {
                    StrokeColor = pancake.BorderColor == Color.Default ? NSColor.Clear.CGColor : pancake.BorderColor.ToCGColor(),
                    FillColor = null,
                    LineWidth = pancake.BorderThickness,
                    Name = layerName
                };

                // Create arcs for the given corner radius.
                //bool hasShadowOrElevation = pancake.HasShadow || pancake.Elevation > 0;

                borderLayer.Path = pancake.Sides != 4 ?
                    ShapeUtils.CreatePolygonPath(Bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).ToCGPath() :
                    ShapeUtils.CreateRoundedRectPath(Bounds, pancake.CornerRadius).ToCGPath(); // insetBounds?

                var layerPosition = new CGPoint(borderLayer.Path.BoundingBox.Width / 2, borderLayer.Path.BoundingBox.Height / 2);

                borderLayer.Frame = borderLayer.Path.BoundingBox;
                borderLayer.Position = layerPosition;
                borderLayer.MasksToBounds = false;

                // Dash pattern for the border.
                if (pancake.BorderDashPattern.Pattern != null && pancake.BorderDashPattern.Pattern.Length > 0)
                {
                    var items = pancake.BorderDashPattern.Pattern.Select(x => new NSNumber(x)).ToArray();
                    borderLayer.LineDashPattern = items;
                }

                if ((pancake.BorderGradientStartColor != default(Color) && pancake.BorderGradientEndColor != default(Color)) || (pancake.BorderGradientStops != null && pancake.BorderGradientStops.Any()))
                {
                    var gradientFrame = Bounds.Inset(-pancake.BorderThickness, -pancake.BorderThickness);
                    var gradientLayer = CreateGradientLayer(pancake.BorderGradientAngle, gradientFrame);
                    gradientLayer.Position = new CGPoint((gradientFrame.Width / 2) - (pancake.BorderThickness), (gradientFrame.Height / 2) - (pancake.BorderThickness));

                    // Create a clone from the border layer and use that one as the mask.
                    // Why? Because the mask and the border somehow can't be the same, so
                    // don't want to do adjustments to borderLayer because it would influence the border.
                    var maskLayer = new CAShapeLayer()
                    {
                        Path = borderLayer.Path,
                        Position = new CGPoint(pancake.BorderThickness, pancake.BorderThickness),
                        FillColor = null,
                        LineWidth = pancake.BorderThickness,
                        StrokeColor = NSColor.Red.CGColor,
                        LineDashPattern = borderLayer.LineDashPattern
                    };

                    gradientLayer.Mask = maskLayer;
                    gradientLayer.Name = layerName;

                    if (pancake.BorderGradientStops != null && pancake.BorderGradientStops.Count > 0)
                    {
                        // A range of colors is given. Let's add them.
                        var orderedStops = pancake.BorderGradientStops.OrderBy(x => x.Offset).ToList();
                        gradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
                        gradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();
                    }
                    else
                    {
                        // Only two colors provided, use that.
                        gradientLayer.Colors = new CGColor[] { pancake.BorderGradientStartColor.ToCGColor(), pancake.BorderGradientEndColor.ToCGColor() };
                    }

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
            var pancake = Element;
            nfloat cornerRadius = (nfloat)pancake.CornerRadius.TopLeft;

            if (pancake.Shadow != null)
            {
                // HACK: Somehow this is needed for _wrapperView.Layer's shadow related properties to work.
                _wrapperView.Shadow = new NSShadow();
                DrawDefaultShadow(pancake.Shadow, _wrapperView.Layer, Bounds, cornerRadius);
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

        private void DrawDefaultShadow(DropShadow shadow, CALayer layer, CGRect bounds, nfloat cornerRadius)
        {
            var pancake = Element as PancakeView;

            // Ideally we want to be able to have individual corner radii + shadows
            // However, on iOS we can only do one radius + shadow.
            layer.CornerRadius = cornerRadius;
            layer.ShadowRadius = shadow.BlurRadius;
            layer.ShadowColor = shadow.Color.ToCGColor();
            layer.ShadowOpacity = shadow.Opacity;
            layer.ShadowOffset = new SizeF((float)shadow.Offset.X, (float)shadow.Offset.Y);

            if (pancake.Sides != 4)
            {
                layer.ShadowPath = ShapeUtils.CreatePolygonPath(bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).ToCGPath();
            }
            else
            {
                layer.ShadowPath = ShapeUtils.CreateRoundedRectPath(bounds, pancake.CornerRadius).ToCGPath();
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