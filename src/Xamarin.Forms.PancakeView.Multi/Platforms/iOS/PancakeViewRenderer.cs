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
    public class PancakeViewRenderer : FrameRenderer
    {
        private CAGradientLayer _gradientLayer;
        private CAShapeLayer _borderLayer;

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

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var pancake = (Element as PancakeView);

                // Angle needs to be between 0-360.
                if (pancake.BackgroundGradientAngle < 0 || pancake.BackgroundGradientAngle > 360)
                    throw new ArgumentException("Please provide a valid background gradient angle.", nameof(Controls.PancakeView.BackgroundGradientAngle));

                Setup(pancake);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var pancake = (Element as PancakeView);

            // If the border is changed, we need to change the border layer we created. 
            if (e.PropertyName == PancakeView.BorderIsDashedProperty.PropertyName ||
                e.PropertyName == PancakeView.CornerRadiiProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName)
                Setup(pancake);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var pancake = (Element as PancakeView);

            // If both background gradient colors are set, we can add the gradient layer.
            if (pancake.BackgroundGradientStartColor != default(Color) && pancake.BackgroundGradientEndColor != default(Color))
            {
                AddOrUpdateGradientLayer(pancake);
            }

            if (!pancake.HasShadow)
            {
                AddCornerRadius(pancake);
            }

            UpdateBorderLayer(pancake);
        }

        private void Setup(PancakeView pancake)
        {
            // Create the border layer
            if (pancake.BorderThickness > 0 || pancake.BackgroundColor != default(Color))
            {
                if (_borderLayer == null)
                    _borderLayer = new CAShapeLayer();

                // Set the border color to clear if it's not set.
                if (pancake.BorderColor == Xamarin.Forms.Color.Default)
                    _borderLayer.StrokeColor = UIColor.Clear.CGColor;
                else
                    _borderLayer.StrokeColor = pancake.BorderColor.ToCGColor();

                if (pancake.BackgroundColor == default(Color))
                    _borderLayer.FillColor = null;
                else
                    _borderLayer.FillColor = pancake.BackgroundColor.ToCGColor();

                _borderLayer.LineWidth = pancake.BorderThickness * 2;

                // There's no border layer yet, insert it.
                if (Layer.Sublayers == null || (Layer.Sublayers != null && !Layer.Sublayers.Any(x => x.GetType() == typeof(CAShapeLayer))))
                    Layer.InsertSublayer(_borderLayer, 0);
            }

            if (pancake.HasShadow)
            {
                // TODO: Ideally we want to be able to have individual corner radii + shadows
                // However, on iOS we can only do one radius + shadow.
                Layer.CornerRadius = (nfloat)pancake.CornerRadii.TopLeft;
                Layer.ShadowRadius = 10;
                Layer.ShadowColor = UIColor.Black.CGColor;
                Layer.ShadowOpacity = 0.4f;
                Layer.ShadowOffset = new SizeF();
                Layer.MasksToBounds = false;
            }
            else
            {
                Layer.ShadowOpacity = 0;
                Layer.MasksToBounds = true;
            }

            // Set the rasterization for performance optimization.
            Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            Layer.ShouldRasterize = true;
        }

        private CGPath CreateCornerPath(PancakeView pancake)
        {
            var cornerPath = new UIBezierPath();

            // Create arcs for the given corner radius.
            cornerPath.AddArc(new CGPoint((float)Bounds.X + Bounds.Width - pancake.CornerRadii.TopRight, (float)Bounds.Y + pancake.CornerRadii.TopRight), (float)pancake.CornerRadii.TopRight, (float)(Math.PI * 1.5), (float)Math.PI * 2, true);
            cornerPath.AddArc(new CGPoint((float)Bounds.X + Bounds.Width - pancake.CornerRadii.BottomRight, (float)Bounds.Y + Bounds.Height - pancake.CornerRadii.BottomRight), (float)pancake.CornerRadii.BottomRight, 0, (float)(Math.PI * .5), true);
            cornerPath.AddArc(new CGPoint((float)Bounds.X + pancake.CornerRadii.BottomLeft, (float)Bounds.Y + Bounds.Height - pancake.CornerRadii.BottomLeft), (float)pancake.CornerRadii.BottomLeft, (float)(Math.PI * .5), (float)Math.PI, true);
            cornerPath.AddArc(new CGPoint((float)Bounds.X + pancake.CornerRadii.TopLeft, (float)Bounds.Y + pancake.CornerRadii.TopLeft), (float)pancake.CornerRadii.TopLeft, (float)Math.PI, (float)(Math.PI * 1.5), true);

            cornerPath.ClosePath();

            return cornerPath.CGPath;
        }

        private void AddCornerRadius(PancakeView pancake)
        {
            if (pancake.CornerRadii.BottomLeft + pancake.CornerRadii.BottomRight + pancake.CornerRadii.TopLeft + pancake.CornerRadii.TopRight > 0)
            {
                var maskLayer = new CAShapeLayer
                {
                    Frame = Bounds,
                    Path = this.CreateCornerPath(pancake),
                };

                Layer.Mask = maskLayer;
                Layer.MasksToBounds = true;
            }
        }

        private void AddOrUpdateGradientLayer(PancakeView pancake)
        {
            // Init the gradient layer.
            if (_gradientLayer == null)
            {
                var angle = pancake.BackgroundGradientAngle / 360.0;

                // Calculate the new positions based on angle between 0-360.
                var a = Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.75) / 2)), 2);
                var b = Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.0) / 2)), 2);
                var c = Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.25) / 2)), 2);
                var d = Math.Pow(Math.Sin(2 * Math.PI * ((angle + 0.5) / 2)), 2);

                _gradientLayer = new CAGradientLayer
                {
                    Frame = Bounds,
                    StartPoint = new CGPoint(1 - a, b),
                    EndPoint = new CGPoint(1 - c, d),
                    Colors = new CGColor[] { pancake.BackgroundGradientStartColor.ToCGColor(), pancake.BackgroundGradientEndColor.ToCGColor() }
                };
            }

            if (Layer.Sublayers == null || (Layer.Sublayers != null && !Layer.Sublayers.Any(x => x.GetType() == typeof(CAGradientLayer))))
            {
                // There's no gradient layer yet, insert it.
                Layer.InsertSublayer(_gradientLayer, 0);
            }
        }

        private void UpdateBorderLayer(PancakeView pancake)
        {
            // The border layer needs to be updated because the Bounds are only available later.
            if (pancake.BorderThickness > 0 && _borderLayer != null)
            {
                _borderLayer.Frame = Bounds;
                _borderLayer.Path = this.CreateCornerPath(pancake);

                // Dash pattern for the border.
                if (pancake.BorderIsDashed)
                {
                    _borderLayer.LineDashPattern = new NSNumber[] { new NSNumber(5), new NSNumber(2) };
                }
            }
        }
    }
}
