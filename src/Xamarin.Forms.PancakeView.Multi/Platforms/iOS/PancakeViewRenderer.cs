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
using Xamarin.Forms.PancakeView.Platforms.Shared;
using Xamarin.Forms.Platform.iOS;
using Controls = Xamarin.Forms.PancakeView;
using EnumsNET;

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

                if (NativeView.GestureRecognizers != null)
                {
                    foreach (var gesture in NativeView.GestureRecognizers)
                    {
                        _actualView.AddGestureRecognizer(gesture);
                    }
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
                    (e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderEdgesProperty.PropertyName))
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
                    (e.PropertyName == PancakeView.ElevationProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.SidesProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName) ||
                    (e.PropertyName == PancakeView.BorderEdgesProperty.PropertyName))
            {
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

            // Remove previous background layer if any
            var prevBackgroundLayer = _actualView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBackgroundLayer?.RemoveFromSuperLayer();

            UIBezierPath cornerPath = null;

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
                Path = cornerPath.CGPath
            };

            _actualView.Layer.Mask = maskLayer;
            _actualView.Layer.MasksToBounds = true;

            if ((pancake.BackgroundGradientStartColor != default(Color) && pancake.BackgroundGradientEndColor != default(Color)) || (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Any()))
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
            // TODO: remove
            DrawMultiPathBorder();
            return;

            //var pancake = Element as PancakeView;
            //var layerName = "borderLayer";

            //// remove previous background layer if any
            //var prevBorderLayer = _wrapperView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            //prevBorderLayer?.RemoveFromSuperLayer();

            //if (pancake.BorderThickness > 0)
            //{
            //    var borderLayer = new CAShapeLayer
            //    {
            //        StrokeColor = pancake.BorderColor == Color.Default ? UIColor.Clear.CGColor : pancake.BorderColor.ToCGColor(),
            //        FillColor = null,
            //        LineWidth = pancake.BorderThickness,
            //        Name = layerName
            //    };

            //    // Create arcs for the given corner radius.
            //    bool hasShadowOrElevation = pancake.HasShadow || pancake.Elevation > 0;

            //    borderLayer.Path = pancake.Sides != 4 ?
            //        ShapeUtils.CreatePolygonPath(Bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath :
            //        ShapeUtils.CreateRoundedRectPath(Bounds, pancake.CornerRadius).CGPath; // insetBounds?

            //    var layerPosition = new CGPoint(borderLayer.Path.BoundingBox.Width / 2, borderLayer.Path.BoundingBox.Height / 2);

            //    borderLayer.Frame = borderLayer.Path.BoundingBox;
            //    borderLayer.Position = layerPosition;

            //    // Dash pattern for the border.
            //    if (pancake.BorderIsDashed)
            //    {
            //        borderLayer.LineDashPattern = new NSNumber[] { new NSNumber(6), new NSNumber(3) };
            //    }

            //    if ((pancake.BorderGradientStartColor != default(Color) && pancake.BorderGradientEndColor != default(Color)) || (pancake.BorderGradientStops != null && pancake.BorderGradientStops.Any()))
            //    {
            //        var gradientFrame = Bounds.Inset(-pancake.BorderThickness, -pancake.BorderThickness);
            //        var gradientLayer = CreateGradientLayer(pancake.BorderGradientAngle, gradientFrame);
            //        gradientLayer.Position = new CGPoint((gradientFrame.Width / 2) - (pancake.BorderThickness), (gradientFrame.Height / 2) - (pancake.BorderThickness));

            //        // Create a clone from the border layer and use that one as the mask.
            //        // Why? Because the mask and the border somehow can't be the same, so
            //        // don't want to do adjustments to borderLayer because it would influence the border.
            //        var maskLayer = new CAShapeLayer()
            //        {
            //            Path = borderLayer.Path,
            //            Position = new CGPoint(pancake.BorderThickness, pancake.BorderThickness),
            //            FillColor = null,
            //            LineWidth = pancake.BorderThickness,
            //            StrokeColor = UIColor.Red.CGColor,
            //            LineDashPattern = borderLayer.LineDashPattern
            //        };

            //        gradientLayer.Mask = maskLayer;
            //        gradientLayer.Name = layerName;

            //        if (pancake.BorderGradientStops != null && pancake.BorderGradientStops.Count > 0)
            //        {
            //            // A range of colors is given. Let's add them.
            //            var orderedStops = pancake.BorderGradientStops.OrderBy(x => x.Offset).ToList();
            //            gradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
            //            gradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();
            //        }
            //        else
            //        {
            //            // Only two colors provided, use that.
            //            gradientLayer.Colors = new CGColor[] { pancake.BorderGradientStartColor.ToCGColor(), pancake.BorderGradientEndColor.ToCGColor() };
            //        }

            //        AddLayer(gradientLayer, -1, _wrapperView);
            //    }
            //    else
            //    {
            //        AddLayer(borderLayer, -1, _wrapperView);
            //    }
            //}
        }

        private void DrawMultiPathBorder()
        {
            var pancake = Element as PancakeView;
            var layerName = "borderLayer";

            // remove previous background layer if any
            var prevBorderLayer = _wrapperView.Layer.Sublayers?.FirstOrDefault(x => x.Name == layerName);
            prevBorderLayer?.RemoveFromSuperLayer();

            if (pancake.BorderEdges != Edge.None && pancake.BorderThickness != default)    
            {
                var thickness = pancake.BorderThickness;
                var edges = pancake.BorderEdges;

                if (edges.HasAnyFlags(Edge.Left))
                {
                    AddEdgeLayer(UIRectEdge.Left, thickness.Left);
                }
                if (edges.HasAnyFlags(Edge.Top))
                {
                    AddEdgeLayer(UIRectEdge.Top, thickness.Top);
                }
                if (edges.HasAnyFlags(Edge.Right))
                {
                    AddEdgeLayer(UIRectEdge.Right, thickness.Right);
                }
                if (edges.HasAnyFlags(Edge.Bottom))
                {
                    AddEdgeLayer(UIRectEdge.Bottom, thickness.Bottom);
                }
            }
        }

        void AddEdgeLayer(UIRectEdge edge, double thickness)
        {
            var pancake = Element as PancakeView;

            var strokeColor = pancake.BorderColor.ToCGColor();
            var lineWidth = thickness;
            var borderDrawingStyle = pancake.BorderDrawingStyle;

            AddLayer(Bounds.ToEdgeLayer(edge, strokeColor, lineWidth, pancake.CornerRadius, borderDrawingStyle), -1, _wrapperView);
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
                // _actualView.Layer.CornerRadius = (nfloat)pancake.CornerRadius.TopLeft;
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
                layer.ShadowPath = ShapeUtils.CreatePolygonPath(bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath;
            }
            else
            {
                layer.ShadowPath = ShapeUtils.CreateRoundedRectPath(bounds, pancake.CornerRadius).CGPath;
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
                layer.ShadowPath = ShapeUtils.CreatePolygonPath(bounds, pancake.Sides, pancake.CornerRadius.TopLeft, pancake.OffsetAngle).CGPath;
            }
            else
            {
                layer.ShadowPath = ShapeUtils.CreateRoundedRectPath(bounds, pancake.CornerRadius).CGPath;
            }

            layer.MasksToBounds = false;
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