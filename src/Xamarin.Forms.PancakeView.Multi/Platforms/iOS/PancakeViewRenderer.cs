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
                SetBackgroundColor(Element.BackgroundColor);
            else if (e.PropertyName == BoxView.CornerRadiusProperty.PropertyName)
                SetCornerRadius();
            else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && Element.IsVisible)
                SetNeedsDisplay();
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

        private void DrawBackground()
        {
            var pancake = Element as PancakeView;
            var cornerPath = new UIBezierPath();

            cornerPath.AddArc(new CGPoint((float)Bounds.X + Bounds.Width - _topRight, (float)Bounds.Y + _topRight), _topRight, (float)(Math.PI * 1.5), (float)Math.PI * 2, true);
            cornerPath.AddArc(new CGPoint((float)Bounds.X + Bounds.Width - _bottomRight, (float)Bounds.Y + Bounds.Height - _bottomRight), _bottomRight, 0, (float)(Math.PI * .5), true);
            cornerPath.AddArc(new CGPoint((float)Bounds.X + _bottomLeft, (float)Bounds.Y + Bounds.Height - _bottomLeft), _bottomLeft, (float)(Math.PI * .5), (float)Math.PI, true);
            cornerPath.AddArc(new CGPoint((float)Bounds.X + _topLeft, (float)Bounds.Y + _topLeft), (float)_topLeft, (float)Math.PI, (float)(Math.PI * 1.5), true);

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
                var angle = pancake.BackgroundGradientAngle / 360.0;

                // Calculate the new positions based on angle between 0-360.
                var a = Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
                var b = Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
                var c = Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
                var d = Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

                // Create a gradient layer that draws our background.
                var gradientLayer = new CAGradientLayer
                {
                    Frame = Bounds,
                    StartPoint = new CGPoint(1 - a, b),
                    EndPoint = new CGPoint(1 - c, d),
                };

                if (pancake.BackgroundGradientStops != null)
                {
                    // A range of colors is given. Let's add them.
                    var orderedStops = pancake.BackgroundGradientStops.OrderBy(x => x.Location).ToList();
                    gradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
                    gradientLayer.Locations = orderedStops.Select(x => new NSNumber(x.Location)).ToArray();
                }
                else
                {
                    // Only two colors provided, use that.
                    gradientLayer.Colors = new CGColor[] { pancake.BackgroundGradientStartColor.ToCGColor(), pancake.BackgroundGradientEndColor.ToCGColor() };
                }

                // If there is already a gradient background layer, remove it before inserting.
                if (_actualView.Layer.Sublayers == null || (_actualView.Layer.Sublayers != null && !_actualView.Layer.Sublayers.Any(x => x.GetType() == typeof(CAGradientLayer))))
                {
                    // There's no gradient layer yet, insert it.
                    _actualView.Layer.InsertSublayer(gradientLayer, 0);
                }
                else
                {
                    // Remove the current gradient layer and insert it again.
                    var gradLayer = _actualView.Layer.Sublayers.FirstOrDefault(x => x.GetType() == typeof(CAGradientLayer));

                    if (gradLayer != null)
                        gradLayer.RemoveFromSuperLayer();

                    _actualView.Layer.InsertSublayer(gradientLayer, 0);
                }
            }
            else
            {
                // Create a shape layer that draws our background.
                var shapeLayer = new CAShapeLayer
                {
                    Frame = Bounds,
                    Path = cornerPath.CGPath,
                    MasksToBounds = true,
                    FillColor = _colorToRender.CGColor
                };

                // If there is already a background layer, remove it before inserting.
                if (_actualView.Layer.Sublayers == null || (_actualView.Layer.Sublayers != null && !_actualView.Layer.Sublayers.Any(x => x.GetType() == typeof(CAShapeLayer))))
                {
                    // There's no background layer yet, insert it.
                    _actualView.Layer.InsertSublayer(shapeLayer, 0);
                }
                else
                {
                    // Remove the current background layer and insert it again.
                    var gradLayer = _actualView.Layer.Sublayers.FirstOrDefault(x => x.GetType() == typeof(CAShapeLayer));

                    if (gradLayer != null)
                        gradLayer.RemoveFromSuperLayer();

                    _actualView.Layer.InsertSublayer(shapeLayer, 0);
                }
            }
        }

        private void DrawBorder()
        {
            var pancake = Element as PancakeView;

            if (pancake.BorderThickness > 0)
            {
                var borderLayer = new CAShapeLayer
                {
                    StrokeColor = pancake.BorderColor == Color.Default ? UIColor.Clear.CGColor : pancake.BorderColor.ToCGColor(),
                    FillColor = null,
                    LineWidth = pancake.BorderThickness,
                    Name = "borderLayer"
                };

                var frameBounds = Bounds;
                var insetBounds = pancake.BorderDrawingStyle == BorderDrawingStyle.Outside ? Bounds.Inset(-(pancake.BorderThickness / 2), -(pancake.BorderThickness / 2)) : Bounds.Inset(pancake.BorderThickness / 2, pancake.BorderThickness / 2);

                // Create arcs for the given corner radius.
                var cornerPath = new CGPath();

                if (!pancake.HasShadow)
                {
                    // Start of our path is where the top left horizontal starts.
                    cornerPath.MoveToPoint(new CGPoint(pancake.CornerRadius.TopLeft + insetBounds.X, insetBounds.Y));

                    // Top line + top right corner
                    cornerPath.AddLineToPoint(new CGPoint(insetBounds.Width - pancake.CornerRadius.TopRight, insetBounds.Y));
                    cornerPath.AddArc((float)(insetBounds.X + insetBounds.Width - pancake.CornerRadius.TopRight), (float)(insetBounds.Y + pancake.CornerRadius.TopRight), (float)pancake.CornerRadius.TopRight, (float)(Math.PI * 1.5), (float)Math.PI * 2, false);

                    // Right side + bottom right corner
                    cornerPath.AddLineToPoint(insetBounds.Width + insetBounds.X, (float)(insetBounds.Height - pancake.CornerRadius.BottomRight));
                    cornerPath.AddArc((float)(insetBounds.X + insetBounds.Width - pancake.CornerRadius.BottomRight), (float)(insetBounds.Y + insetBounds.Height - pancake.CornerRadius.BottomRight), (float)pancake.CornerRadius.BottomRight, 0, (float)(Math.PI * .5), false);

                    // Bottom side + bottom left corner
                    cornerPath.AddLineToPoint((float)(insetBounds.X + pancake.CornerRadius.BottomLeft), insetBounds.Height + insetBounds.Y);
                    cornerPath.AddArc((float)(insetBounds.X + pancake.CornerRadius.BottomLeft), (float)(insetBounds.Y + insetBounds.Height - pancake.CornerRadius.BottomLeft), (float)pancake.CornerRadius.BottomLeft, (float)(Math.PI * .5), (float)Math.PI, false);

                    // Left side + top left corner
                    cornerPath.AddLineToPoint(insetBounds.X, (float)(insetBounds.Y + pancake.CornerRadius.TopLeft));
                    cornerPath.AddArc((float)(insetBounds.X + pancake.CornerRadius.TopLeft), (float)(insetBounds.Y + pancake.CornerRadius.TopLeft), (float)pancake.CornerRadius.TopLeft, (float)Math.PI, (float)(Math.PI * 1.5), false);
                }
                else
                {
                    // Start of our path is where the top left horizontal starts.
                    cornerPath.MoveToPoint(new CGPoint(pancake.CornerRadius.TopLeft + insetBounds.X, insetBounds.Y));

                    // Top line + top right corner
                    cornerPath.AddLineToPoint(new CGPoint(insetBounds.Width - pancake.CornerRadius.TopLeft, insetBounds.Y));
                    cornerPath.AddArc((float)(insetBounds.X + insetBounds.Width - pancake.CornerRadius.TopLeft), (float)(insetBounds.Y + pancake.CornerRadius.TopLeft), (float)pancake.CornerRadius.TopLeft, (float)(Math.PI * 1.5), (float)Math.PI * 2, false);

                    // Right side + bottom right corner
                    cornerPath.AddLineToPoint(insetBounds.Width + insetBounds.X, (float)(insetBounds.Height - pancake.CornerRadius.TopLeft));
                    cornerPath.AddArc((float)(insetBounds.X + insetBounds.Width - pancake.CornerRadius.TopLeft), (float)(insetBounds.Y + insetBounds.Height - pancake.CornerRadius.TopLeft), (float)pancake.CornerRadius.TopLeft, 0, (float)(Math.PI * .5), false);

                    // Bottom side + bottom left corner
                    cornerPath.AddLineToPoint((float)(insetBounds.X + pancake.CornerRadius.TopLeft), insetBounds.Height + insetBounds.Y);
                    cornerPath.AddArc((float)(insetBounds.X + pancake.CornerRadius.TopLeft), (float)(insetBounds.Y + insetBounds.Height - pancake.CornerRadius.TopLeft), (float)pancake.CornerRadius.TopLeft, (float)(Math.PI * .5), (float)Math.PI, false);

                    // Left side + top left corner
                    cornerPath.AddLineToPoint(insetBounds.X, (float)(insetBounds.Y + pancake.CornerRadius.TopLeft));
                    cornerPath.AddArc((float)(insetBounds.X + pancake.CornerRadius.TopLeft), (float)(insetBounds.Y + pancake.CornerRadius.TopLeft), (float)pancake.CornerRadius.TopLeft, (float)Math.PI, (float)(Math.PI * 1.5), false);
                }

                borderLayer.Path = cornerPath;
                borderLayer.Frame = frameBounds;
                borderLayer.Position = new CGPoint(frameBounds.Width / 2, frameBounds.Height / 2);

                // Dash pattern for the border.
                if (pancake.BorderIsDashed)
                {
                    borderLayer.LineDashPattern = new NSNumber[] { new NSNumber(6), new NSNumber(3) };
                }

                // There's no border layer yet, insert it.
                if (_wrapperView.Layer.Sublayers == null || (_wrapperView.Layer.Sublayers != null && !_wrapperView.Layer.Sublayers.Any(x => x.GetType() == typeof(CAShapeLayer) && x.Name == "borderLayer")))
                {
                    _wrapperView.Layer.AddSublayer(borderLayer);
                }
                else
                {
                    var existingBorderLayer = _wrapperView.Layer.Sublayers.FirstOrDefault(x => x.GetType() == typeof(CAShapeLayer) && x.Name == "borderLayer");

                    if (existingBorderLayer != null)
                        existingBorderLayer.RemoveFromSuperLayer();

                    _wrapperView.Layer.AddSublayer(borderLayer);
                }
            }
        }

        public override void LayoutSubviews()
        {
            if (_previousSize != Bounds.Size)
                SetNeedsDisplay();

            base.LayoutSubviews();
        }

        private void Validate(PancakeView pancake)
        {
            // Angle needs to be between 0-360.
            if (pancake.BackgroundGradientAngle < 0 || pancake.BackgroundGradientAngle > 360)
                throw new ArgumentException("Please provide a valid background gradient angle.", nameof(Controls.PancakeView.BackgroundGradientAngle));
        }

        void SetCornerRadius()
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

        private void DrawShadow()
        {
            var pancake = Element as PancakeView;

            if (pancake.HasShadow)
            {
                // Ideally we want to be able to have individual corner radii + shadows
                // However, on iOS we can only do one radius + shadow.
                _wrapperView.Layer.CornerRadius = (nfloat)pancake.CornerRadius.TopLeft;
                _wrapperView.Layer.ShadowRadius = 10;
                _wrapperView.Layer.ShadowColor = UIColor.Black.CGColor;
                _wrapperView.Layer.ShadowOpacity = 0.4f;
                _wrapperView.Layer.ShadowOffset = new SizeF();
                _wrapperView.Layer.ShadowPath = UIBezierPath.FromRoundedRect(Bounds, (nfloat)pancake.CornerRadius.TopLeft).CGPath;

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
    }
}
