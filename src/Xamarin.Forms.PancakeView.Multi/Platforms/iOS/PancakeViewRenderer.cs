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
        private nfloat _topLeft;
        private nfloat _topRight;
        private nfloat _bottomLeft;
        private nfloat _bottomRight;

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

                foreach (var item in NativeView.Subviews)
                {
                    _actualView.AddSubview(item);
                }

                _wrapperView.AddSubview(_actualView);

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
            else if ((e.PropertyName == PancakeView.BackgroundGradientStartColorProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BackgroundGradientEndColorProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName) ||
                    (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && Element.IsVisible) ||
                    (e.PropertyName == PancakeView.OffsetAngleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderIsDashedProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderDrawingStyleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderGradientAngleProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderGradientEndColorProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderGradientStartColorProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderGradientStopsProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.HasShadowProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.ElevationProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.SidesProperty.PropertyName))
            {
                Validate(Element as PancakeView);
                SetNeedsDisplay();
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

            var elementCornerRadius = (Element as PancakeView).CornerRadius;

            _topLeft = (float)elementCornerRadius.TopLeft;
            _topRight = (float)elementCornerRadius.TopRight;
            _bottomLeft = (float)elementCornerRadius.BottomLeft;
            _bottomRight = (float)elementCornerRadius.BottomRight;

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

            UIBezierPath cornerPath = null;

            if (pancake.Sides != 4)
            {
                cornerPath = GetPolygonPath(Bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle);
            }
            else
            {
                cornerPath = new UIBezierPath();
                cornerPath.AddArc(new CGPoint((float)Bounds.X + Bounds.Width - _topRight, (float)Bounds.Y + _topRight), _topRight, (float)(Math.PI * 1.5), (float)Math.PI * 2, true);
                cornerPath.AddArc(new CGPoint((float)Bounds.X + Bounds.Width - _bottomRight, (float)Bounds.Y + Bounds.Height - _bottomRight), _bottomRight, 0, (float)(Math.PI * .5), true);
                cornerPath.AddArc(new CGPoint((float)Bounds.X + _bottomLeft, (float)Bounds.Y + Bounds.Height - _bottomLeft), _bottomLeft, (float)(Math.PI * .5), (float)Math.PI, true);
                cornerPath.AddArc(new CGPoint((float)Bounds.X + _topLeft, (float)Bounds.Y + _topLeft), (float)_topLeft, (float)Math.PI, (float)(Math.PI * 1.5), true);
            }

            // The layer used to mask other layers we draw on the background.
            var maskLayer = new CAShapeLayer
            {
                Frame = Bounds,
                Path = cornerPath.CGPath
            };

            _actualView.Layer.Mask = maskLayer;
            _actualView.Layer.MasksToBounds = true;

            if ((pancake.BackgroundGradientStartColor != default(Color) && pancake.BackgroundGradientEndColor != default(Color)) || (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Any()))
            {
                // Create a gradient layer that draws our background.
                var gradientLayer = CreateGradientLayer(pancake.BackgroundGradientAngle);
                gradientLayer.Name = layerName;

                if (pancake.BackgroundGradientStops != null)
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

                AddOrUpdateLayer(gradientLayer, 0, _actualView, layerName);
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

                AddOrUpdateLayer(shapeLayer, 0, _actualView, layerName);
            }
        }

        private void DrawBorder()
        {
            var pancake = Element as PancakeView;
            var layerName = "borderLayer";

            if (pancake.BorderThickness > 0)
            {
                var borderLayer = new CAShapeLayer
                {
                    StrokeColor = pancake.BorderColor == Color.Default ? UIColor.Clear.CGColor : pancake.BorderColor.ToCGColor(),
                    FillColor = null,
                    LineWidth = pancake.BorderThickness,
                    Name = layerName
                };

                var frameBounds = Bounds;
                var insetBounds = pancake.BorderDrawingStyle == BorderDrawingStyle.Outside ?
                    Bounds.Inset(-(pancake.BorderThickness / 2), -(pancake.BorderThickness / 2)) :
                    Bounds.Inset(pancake.BorderThickness / 2, pancake.BorderThickness / 2);

                // Create arcs for the given corner radius.
                bool hasShadowOrElevation = pancake.HasShadow || pancake.Elevation > 0;

                if (!hasShadowOrElevation)
                {
                    borderLayer.Path = pancake.Sides != 4 ?
                        GetPolygonPath(Bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath :
                        CreateCornerPath(pancake.CornerRadius.TopLeft, pancake.CornerRadius.TopRight, pancake.CornerRadius.BottomRight, pancake.CornerRadius.BottomLeft, insetBounds);
                }
                else
                {
                    borderLayer.Path = pancake.Sides != 4 ?
                        GetPolygonPath(Bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath :
                        CreateCornerPath(pancake.CornerRadius.TopLeft, pancake.CornerRadius.TopLeft, pancake.CornerRadius.TopLeft, pancake.CornerRadius.TopLeft, insetBounds);
                }

                borderLayer.Frame = frameBounds;
                borderLayer.Position = new CGPoint(frameBounds.Width / 2, frameBounds.Height / 2);

                // Dash pattern for the border.
                if (pancake.BorderIsDashed)
                {
                    borderLayer.LineDashPattern = new NSNumber[] { new NSNumber(6), new NSNumber(3) };
                }

                if ((pancake.BorderGradientStartColor != default(Color) && pancake.BorderGradientEndColor != default(Color)) || (pancake.BorderGradientStops != null && pancake.BorderGradientStops.Any()))
                {
                    var gradientLayer = CreateGradientLayer(pancake.BorderGradientAngle);
                    gradientLayer.Mask = borderLayer;
                    gradientLayer.Name = "borderLayer";

                    if (pancake.BorderGradientStops != null)
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

                    AddOrUpdateLayer(gradientLayer, -1, _wrapperView, layerName);
                }
                else
                {
                    AddOrUpdateLayer(borderLayer, -1, _wrapperView, layerName);
                }
            }
        }

        private void DrawShadow()
        {
            var pancake = Element;

            bool hasShadowOrElevation = pancake.HasShadow || pancake.Elevation > 0;
            nfloat cornerRadius = (nfloat)pancake.CornerRadius.TopLeft;

            if (pancake.HasShadow)
            {
                DrawDefaultShadow(_wrapperView.Layer, Bounds, cornerRadius);
            }

            if (pancake.Elevation > 0)
            {
                DrawElevation(_wrapperView.Layer, pancake.Elevation, Bounds, cornerRadius);
            }

            if (hasShadowOrElevation)
            {
                _actualView.Layer.CornerRadius = (nfloat)pancake.CornerRadius.TopLeft;
                _actualView.ClipsToBounds = true;
            }
            else
            {
                _wrapperView.Layer.ShadowOpacity = 0;
            }

            // Set the rasterization for performance optimization.
            _wrapperView.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            _wrapperView.Layer.ShouldRasterize = true;

            _actualView.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            _actualView.Layer.ShouldRasterize = true;
        }

        private void DrawDefaultShadow(CALayer layer, CGRect bounds, nfloat cornerRadius)
        {
            var pancake = Element as PancakeView;

            // Ideally we want to be able to have individual corner radii + shadows
            // However, on iOS we can only do one radius + shadow.
            layer.CornerRadius = cornerRadius;
            layer.ShadowRadius = 10;
            layer.ShadowColor = UIColor.Black.CGColor;
            layer.ShadowOpacity = 0.4f;
            layer.ShadowOffset = new SizeF();

            if (pancake.Sides != 4)
            {
                layer.ShadowPath = GetPolygonPath(bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath;
            }
            else
            {
                layer.ShadowPath = UIBezierPath.FromRoundedRect(bounds, cornerRadius).CGPath;
            }
        }

        private void DrawElevation(CALayer layer, int elevation, CGRect bounds, nfloat cornerRadius)
        {
            // Source: https://medium.com/material-design-for-ios/part-1-elevation-e48ff795c693
            var pancake = Element as PancakeView;

            layer.CornerRadius = cornerRadius;
            layer.ShadowRadius = elevation;
            layer.ShadowColor = UIColor.Black.CGColor;
            layer.ShadowOpacity = 0.24f;
            layer.ShadowOffset = new CGSize(0, elevation);

            if (pancake.Sides != 4)
            {
                layer.ShadowPath = GetPolygonPath(bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath;
            }
            else
            {
                layer.ShadowPath = UIBezierPath.FromRoundedRect(bounds, cornerRadius).CGPath;
            }

            layer.MasksToBounds = false;
        }

        private CAGradientLayer CreateGradientLayer(int angle)
        {
            var pancake = Element as PancakeView;

            var totalAngle = angle / 360.0;

            // Calculate the new positions based on angle between 0-360.
            var a = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.75) / 2)), 2);
            var b = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.0) / 2)), 2);
            var c = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.25) / 2)), 2);
            var d = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.5) / 2)), 2);

            // Create a gradient layer that draws our background.
            return new CAGradientLayer
            {
                Frame = pancake.Sides != 4 ? GetPolygonPath(Bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath.PathBoundingBox : Bounds,
                StartPoint = new CGPoint(1 - a, b),
                EndPoint = new CGPoint(1 - c, d)
            };
        }

        private CGPath CreateCornerPath(double topLeft, double topRight, double bottomRight, double bottomLeft, CGRect insetBounds)
        {
            var cornerPath = new CGPath();

            // Start of our path is where the top left horizontal starts.
            cornerPath.MoveToPoint(new CGPoint(topLeft + insetBounds.X, insetBounds.Y));

            // Top line + top right corner
            cornerPath.AddLineToPoint(new CGPoint(insetBounds.Width - topRight, insetBounds.Y));
            cornerPath.AddArc((float)(insetBounds.X + insetBounds.Width - topRight), (float)(insetBounds.Y + topRight), (float)topRight, (float)(Math.PI * 1.5), (float)Math.PI * 2, false);

            // Right side + bottom right corner
            cornerPath.AddLineToPoint(insetBounds.Width + insetBounds.X, (float)(insetBounds.Height - bottomRight));
            cornerPath.AddArc((float)(insetBounds.X + insetBounds.Width - bottomRight), (float)(insetBounds.Y + insetBounds.Height - bottomRight), (float)bottomRight, 0, (float)(Math.PI * .5), false);

            // Bottom side + bottom left corner
            cornerPath.AddLineToPoint((float)(insetBounds.X + bottomLeft), insetBounds.Height + insetBounds.Y);
            cornerPath.AddArc((float)(insetBounds.X + bottomLeft), (float)(insetBounds.Y + insetBounds.Height - bottomLeft), (float)bottomLeft, (float)(Math.PI * .5), (float)Math.PI, false);

            // Left side + top left corner
            cornerPath.AddLineToPoint(insetBounds.X, (float)(insetBounds.Y + topLeft));
            cornerPath.AddArc((float)(insetBounds.X + topLeft), (float)(insetBounds.Y + topLeft), (float)topLeft, (float)Math.PI, (float)(Math.PI * 1.5), false);

            return cornerPath;
        }

        public void AddOrUpdateLayer(CALayer layer, int position, UIView viewToAddTo, string layerToRemove)
        {
            // If there is already a layer with the given name, remove it before inserting.
            if (viewToAddTo.Layer.Sublayers != null)
            {
                var existingLayer = viewToAddTo.Layer.Sublayers.FirstOrDefault(x => x.Name == layerToRemove);

                if (existingLayer != null)
                    existingLayer.RemoveFromSuperLayer();
            }

            if (position > -1)
                viewToAddTo.Layer.InsertSublayer(layer, position);
            else
                viewToAddTo.Layer.AddSublayer(layer);
        }

        private static UIBezierPath GetPolygonPath(CGRect rect, int sides, double cornerRadius = 0.0, double rotationOffset = 0.0)
        {
            var offsetRadians = rotationOffset * Math.PI / 180;
            var path = new UIBezierPath();
            var theta = 2 * Math.PI / sides;

            var width = (-cornerRadius + Math.Min(rect.Size.Width, rect.Size.Height)) / 2;
            var center = new CGPoint(rect.Width / 2, rect.Height / 2);

            var radius = width + cornerRadius - (Math.Cos(theta) * cornerRadius) / 2;

            var angle = offsetRadians;
            var corner = new CGPoint(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
            path.MoveTo(new CGPoint(corner.X + cornerRadius * Math.Cos(angle + theta), corner.Y + cornerRadius * Math.Sin(angle + theta)));

            for (var i = 0; i < sides; i++)
            {
                angle += theta;
                corner = new CGPoint(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
                var tip = new CGPoint(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
                var start = new CGPoint(corner.X + cornerRadius * Math.Cos(angle - theta), corner.Y + cornerRadius * Math.Sin(angle - theta));
                var end = new CGPoint(corner.X + cornerRadius * Math.Cos(angle + theta), corner.Y + cornerRadius * Math.Sin(angle + theta));

                path.AddLineTo(start);
                path.AddQuadCurveToPoint(end, tip);
            }
            path.ClosePath();
            return path;
        }

        private static UIBezierPath GetPolygonCornerPath(CGRect rect, int sides, double cornerRadius = 0.0, double rotationOffset = 0.0)
        {
            var offsetRadians = rotationOffset * Math.PI / 180;
            var path = new UIBezierPath();
            var theta = 2 * Math.PI / sides;

            var width = (-cornerRadius + Math.Min(rect.Size.Width, rect.Size.Height)) / 2;
            var center = new CGPoint(rect.Width / 2, rect.Height / 2);

            var radius = width + cornerRadius - (Math.Cos(theta) * cornerRadius) / 2;

            var angle = offsetRadians;
            var corner = new CGPoint(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
            path.MoveTo(new CGPoint(corner.X + cornerRadius * Math.Cos(angle + theta), corner.Y + cornerRadius * Math.Sin(angle + theta)));

            for (var i = 0; i < sides; i++)
            {
                angle += theta;
                corner = new CGPoint(center.X + (radius - cornerRadius) * Math.Cos(angle), center.Y + (radius - cornerRadius) * Math.Sin(angle));
                var tip = new CGPoint(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
                var start = new CGPoint(corner.X + cornerRadius * Math.Cos(angle - theta), corner.Y + cornerRadius * Math.Sin(angle - theta));
                var end = new CGPoint(corner.X + cornerRadius * Math.Cos(angle + theta), corner.Y + cornerRadius * Math.Sin(angle + theta));

                path.AddLineTo(start);
                path.AddQuadCurveToPoint(end, tip);
            }
            path.ClosePath();
            return path;
        }
    }
}