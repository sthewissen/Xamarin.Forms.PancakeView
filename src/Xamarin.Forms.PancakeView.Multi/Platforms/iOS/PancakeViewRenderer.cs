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
                Validate(Element as PancakeView);

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

            Validate(Element as PancakeView);

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

        private void Validate(PancakeView pancake)
        {
            // Angle needs to be between 0-360.
            if (pancake.BackgroundGradientAngle < 0 || pancake.BackgroundGradientAngle > 360)
                throw new ArgumentException("Please provide a valid background gradient angle.", nameof(PancakeView.BackgroundGradientAngle));

            if (pancake.OffsetAngle < 0 || pancake.OffsetAngle > 360)
                throw new ArgumentException("Please provide a valid offset angle.", nameof(PancakeView.OffsetAngle));

            // min value for sides is 3
            if (pancake.Sides < 3)
                throw new ArgumentException("Please provide a valid value for sides.", nameof(PancakeView.Sides));
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
            var pancake = Element as PancakeView;
            var layerName = "backgroundLayer";

            // Remove previous background layer if any
            var prevBackgroundLayer = _actualView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBackgroundLayer?.RemoveFromSuperLayer();

            UIBezierPath cornerPath = null;

            if (pancake.Sides != 4)
            {
                cornerPath = Bounds.CreatePolygonPath(pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);
            }
            else
            {
                cornerPath = Bounds.CreateRoundedRectPath(pancake.CornerRadius);
            }

            // The layer used to mask other layers we draw on the background.
            var maskLayer = new CAShapeLayer
            {
                Frame = Bounds,
                Path = cornerPath.CGPath
            };

            _actualView.Layer.Mask = maskLayer;
            _actualView.Layer.MasksToBounds = true;

            if (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Any())
            {
                // Create a gradient layer that draws our background.
                var gradientLayer = CreateGradientLayer(pancake.BackgroundGradientAngle, Bounds);
                gradientLayer.Name = layerName;

                // A range of colors is given. Let's add them.
                var orderedStops = pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
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
            var pancake = Element as PancakeView;
            var layerName = "borderLayer";

            // remove previous background layer if any
            var prevBorderLayer = _wrapperView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBorderLayer?.RemoveFromSuperLayer();

            if (pancake.Border != null && !pancake.Border.BorderThickness.IsEmpty)
            {
                var borderLayer = new CAShapeLayer
                {
                    StrokeColor = pancake.Border.BorderColor == Color.Default ? UIColor.Clear.CGColor : pancake.Border.BorderColor.ToCGColor(),
                    FillColor = null,
                    LineWidth = (nfloat)pancake.Border.BorderThickness.Left,
                    Name = layerName
                };

                borderLayer.Path = pancake.Sides != 4 ?
                    Bounds.CreatePolygonPath(pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath :
                    Bounds.CreateRoundedRectPath(pancake.CornerRadius).CGPath;

                var layerPosition = new CGPoint(borderLayer.Path.BoundingBox.Width / 2, borderLayer.Path.BoundingBox.Height / 2);

                borderLayer.Frame = borderLayer.Path.BoundingBox;
                borderLayer.Position = layerPosition;

                // Dash pattern for the border.
                if (pancake.Border.BorderDashPattern.Pattern != null && pancake.Border.BorderDashPattern.Pattern.Length > 0)
                {
                    var items = pancake.Border.BorderDashPattern.Pattern.Select(x => new NSNumber(x)).ToArray();
                    borderLayer.LineDashPattern = items;
                }

                if (pancake.Border.BorderGradientStops != null && pancake.Border.BorderGradientStops.Any())
                {
                    var gradientFrame = Bounds.Inset(-(nfloat)pancake.Border.BorderThickness.Left, -(nfloat)pancake.Border.BorderThickness.Left);
                    var gradientLayer = CreateGradientLayer(pancake.Border.BorderGradientAngle, gradientFrame);
                    gradientLayer.Position = new CGPoint((gradientFrame.Width / 2) - ((nfloat)pancake.Border.BorderThickness.Left), (gradientFrame.Height / 2) - ((nfloat)pancake.Border.BorderThickness.Left));

                    // Create a clone from the border layer and use that one as the mask.
                    // Why? Because the mask and the border somehow can't be the same, so
                    // don't want to do adjustments to borderLayer because it would influence the border.
                    var maskLayer = new CAShapeLayer()
                    {
                        Path = borderLayer.Path,
                        Position = new CGPoint(pancake.Border.BorderThickness.Left, pancake.Border.BorderThickness.Left),
                        FillColor = null,
                        LineWidth = (nfloat)pancake.Border.BorderThickness.Left,
                        StrokeColor = UIColor.Red.CGColor,
                        LineDashPattern = borderLayer.LineDashPattern
                    };

                    gradientLayer.Mask = maskLayer;
                    gradientLayer.Name = layerName;

                    // A range of colors is given. Let's add them.
                    var orderedStops = pancake.Border.BorderGradientStops.OrderBy(x => x.Offset).ToList();
                    gradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
                    gradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();

                    AddLayer(gradientLayer, -1, _wrapperView);
                }
                else
                {
                    AddLayer(borderLayer, -1, _wrapperView);
                }
            }
        }

        private void DrawShadow()
        {
            var pancake = Element;

            if (pancake.Shadow != null)
            {
                // Draw a shadow on the wrapper layer and clip the original view to match.
                DrawDefaultShadow(pancake.Shadow, _wrapperView.Layer, Bounds);
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

        private void DrawDefaultShadow(DropShadow shadow, CALayer layer, CGRect bounds)
        {
            var pancake = Element as PancakeView;

            layer.ShadowRadius = shadow.BlurRadius;
            layer.ShadowColor = shadow.Color.ToCGColor();
            layer.ShadowOpacity = shadow.Opacity;
            layer.ShadowOffset = new SizeF((float)shadow.Offset.X, (float)shadow.Offset.Y);

            if (pancake.Sides != 4)
            {
                layer.ShadowPath = bounds.CreatePolygonPath(pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath;
            }
            else
            {
                layer.ShadowPath = bounds.CreateRoundedRectPath(pancake.CornerRadius).CGPath;
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