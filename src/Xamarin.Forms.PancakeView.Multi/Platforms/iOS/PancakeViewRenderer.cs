using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView.iOS;
using Xamarin.Forms.Platform.iOS;
using Controls = Xamarin.Forms.PancakeView;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.iOS
{
    public class PancakeViewRenderer : ViewRenderer<PancakeView, UIView>
    {
        private UIView _actualView;
        private UIView _wrapperView;
        private UIColor _colorToRender;
        private CGSize _previousSize;

        /// <summary>
        /// This method ensures that we don't get stripped out by the linker.
        /// </summary>
        public static new void Init()
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
                _actualView = new UIView();
                _wrapperView = new UIView();

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
                SetNeedsDisplay();
            }
        }

        public override void LayoutSubviews()
        {
            if (_previousSize != Bounds.Size)
                SetNeedsDisplay();

            base.LayoutSubviews();
        }

        public override void Draw(CGRect rect)
        {
            _actualView.Frame = Bounds;
            _wrapperView.Frame = Bounds;

            DrawBackground();
            DrawShadow();
            DrawBorder();

            base.Draw(rect);

            _previousSize = Bounds.Size;
        }

        private void SetCornerRadius()
        {
            if (Element == null)
                return;

            SetNeedsDisplay();
        }

        protected override void SetBackgroundColor(Color color)
        {
            if (Element == null)
                return;

            var elementColor = Element.BackgroundColor;

            if (!elementColor.IsDefault)
                _colorToRender = elementColor.ToUIColor();
            else
                _colorToRender = color.ToUIColor();

            SetNeedsDisplay();
        }

        private void DrawBackground()
        {
            var layerName = "backgroundLayer";

            // Remove previous background layer if any
            var prevBackgroundLayer = _actualView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBackgroundLayer?.RemoveFromSuperLayer();

            UIBezierPath cornerPath = null;

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
                Path = cornerPath.CGPath
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
                    Path = cornerPath.CGPath,
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

            // Remove previous border layers, if any
            var prevBorderLayer = _wrapperView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBorderLayer?.RemoveFromSuperLayer();
            prevBorderLayer = _actualView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBorderLayer?.RemoveFromSuperLayer();

            if (Element.Border != null && Element.Border.Thickness != default)
            {
                var borderLayer = new CAShapeLayer
                {
                    StrokeColor = Element.Border.Color == Color.Default ? UIColor.Clear.CGColor : Element.Border.Color.ToCGColor(),
                    FillColor = null,
                    Name = layerName
                };

                switch (Element.Border.DrawingStyle)
                {
                    case BorderDrawingStyle.Inside:
                        borderLayer.LineWidth = (nfloat)Element.Border.Thickness.Left * 2;
                        break;
                    case BorderDrawingStyle.Outside:
                        borderLayer.LineWidth = (nfloat)Element.Border.Thickness.Left * 2;
                        break;
                    case BorderDrawingStyle.Centered:
                        borderLayer.LineWidth = (nfloat)Element.Border.Thickness.Left;
                        break;
                }

                borderLayer.Path = Element.Sides != 4 ?
                    Bounds.CreatePolygonPath(Element.Sides, Element.CornerRadius.TopLeft, Element.OffsetAngle).CGPath :
                    Bounds.CreateRoundedRectPath(Element.CornerRadius).CGPath;

                var layerPosition = new CGPoint(borderLayer.Path.BoundingBox.Width / 2, borderLayer.Path.BoundingBox.Height / 2);

                borderLayer.Frame = borderLayer.Path.BoundingBox;
                borderLayer.Position = layerPosition;

                // Dash pattern for the border.
                if (Element.Border.DashPattern.Pattern != null && Element.Border.DashPattern.Pattern.Length > 0)
                {
                    var items = Element.Border.DashPattern.Pattern.Select(x => new NSNumber(x)).ToArray();
                    borderLayer.LineDashPattern = items;
                }

                if (Element.Border.GradientStops != null && Element.Border.GradientStops.Any())
                {
                    var gradientFrame = Bounds.Inset(-(nfloat)Element.Border.Thickness.Left, -(nfloat)Element.Border.Thickness.Left);
                    var gradientLayer = CreateGradientLayer(Element.Border.GradientAngle, gradientFrame);
                    gradientLayer.Position = new CGPoint((gradientFrame.Width / 2) - ((nfloat)Element.Border.Thickness.Left), (gradientFrame.Height / 2) - ((nfloat)Element.Border.Thickness.Left));

                    // Create a clone from the border layer and use that one as the mask.
                    // Why? Because the mask and the border somehow can't be the same, so
                    // don't want to do adjustments to borderLayer because it would influence the border.
                    var maskLayer = new CAShapeLayer()
                    {
                        Path = borderLayer.Path,
                        Position = new CGPoint(Element.Border.Thickness.Left, Element.Border.Thickness.Left),
                        FillColor = null,
                        LineWidth = (nfloat)Element.Border.Thickness.Left,
                        StrokeColor = UIColor.Red.CGColor,
                        LineDashPattern = borderLayer.LineDashPattern
                    };

                    gradientLayer.Mask = maskLayer;
                    gradientLayer.Name = layerName;

                    // A range of colors is given. Let's add them.
                    var orderedStops = Element.Border.GradientStops.OrderBy(x => x.Offset).ToList();
                    gradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
                    gradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();

                    if (Element.Border.DrawingStyle == BorderDrawingStyle.Centered)
                        AddLayer(gradientLayer, -1, _wrapperView);
                    else if (Element.Border.DrawingStyle == BorderDrawingStyle.Inside)
                        AddLayer(gradientLayer, -1, _actualView);
                    else
                        AddLayer(gradientLayer, 0, _wrapperView);
                }
                else
                {
                    if (Element.Border.DrawingStyle == BorderDrawingStyle.Centered)
                        AddLayer(borderLayer, -1, _wrapperView);
                    else if (Element.Border.DrawingStyle == BorderDrawingStyle.Inside)
                        AddLayer(borderLayer, -1, _actualView);
                    else
                        AddLayer(borderLayer, 0, _wrapperView);
                }
            }
        }

        private void DrawShadow()
        {
            if (Element.Shadow != null)
            {
                // Draw a shadow on the wrapper layer and clip the original view to match.
                DrawDefaultShadow(_wrapperView.Layer, Bounds);
                _actualView.ClipsToBounds = true;
            }
            else
            {
                // Reset to zero.
                _wrapperView.Layer.ShadowOpacity = 0;
            }

            // Set the rasterization for performance optimization.
            _wrapperView.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            _wrapperView.Layer.ShouldRasterize = true;
            _actualView.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            _actualView.Layer.ShouldRasterize = true;
        }

        private void DrawDefaultShadow(CALayer layer, CGRect bounds)
        {
            layer.ShadowRadius = Element.Shadow.BlurRadius;
            layer.ShadowColor = Element.Shadow.Color.ToCGColor();
            layer.ShadowOpacity = Element.Shadow.Opacity;
            layer.ShadowOffset = new SizeF((float)Element.Shadow.Offset.X, (float)Element.Shadow.Offset.Y);

            if (Element.Sides != 4)
            {
                layer.ShadowPath = bounds.CreatePolygonPath(Element.Sides, Element.CornerRadius.TopLeft, Element.OffsetAngle).CGPath;
            }
            else
            {
                layer.ShadowPath = bounds.CreateRoundedRectPath(Element.CornerRadius).CGPath;
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

        public void AddLayer(CALayer layer, int position, UIView viewToAddTo)
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
    }
}