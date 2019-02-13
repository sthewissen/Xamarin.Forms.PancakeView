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
    public class PancakeViewRenderer : VisualElementRenderer<ContentView>
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

        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
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

            AddCornerRadius(pancake);
            UpdateBorderLayer(pancake);
        }

        private void Setup(PancakeView pancake)
        {
            // Create the border layer
            if (pancake.BorderThickness > 0)
            {
                if (_borderLayer == null)
                    _borderLayer = new CAShapeLayer();

                // Set the border color to clear if it's not set.
                if (pancake.BorderColor == Xamarin.Forms.Color.Default)
                    _borderLayer.StrokeColor = UIColor.Clear.CGColor;
                else
                    _borderLayer.StrokeColor = pancake.BorderColor.ToCGColor();

                _borderLayer.FillColor = null;
                _borderLayer.LineWidth = pancake.BorderThickness * 2;

                // There's no border layer yet, insert it.
                if (Layer.Sublayers == null || (Layer.Sublayers != null && !Layer.Sublayers.Any(x => x.GetType() == typeof(CAShapeLayer))))
                    Layer.InsertSublayer(_borderLayer, 0);
            }

            if (pancake.HasShadow)
            {
                // TODO: Ideally we want to extract these to shadows.
                // However, we're very limited when it comes to shadows on Droid :(
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

        private void AddCornerRadius(PancakeView pancake)
        {
            if (pancake.CornerRadius.HorizontalThickness > 0 || pancake.CornerRadius.VerticalThickness > 0)
            {
                var cornerPath = new UIBezierPath();

                // Create arcs for the given corner radius.
                cornerPath.AddArc(new CGPoint((float)Bounds.X + Bounds.Width - pancake.CornerRadius.Top, (float)Bounds.Y + pancake.CornerRadius.Top), (float)pancake.CornerRadius.Top, (float)(Math.PI * 1.5), (float)Math.PI * 2, true);
                cornerPath.AddArc(new CGPoint((float)Bounds.X + Bounds.Width - pancake.CornerRadius.Right, (float)Bounds.Y + Bounds.Height - pancake.CornerRadius.Right), (float)pancake.CornerRadius.Right, 0, (float)(Math.PI * .5), true);
                cornerPath.AddArc(new CGPoint((float)Bounds.X + pancake.CornerRadius.Bottom, (float)Bounds.Y + Bounds.Height - pancake.CornerRadius.Bottom), (float)pancake.CornerRadius.Bottom, (float)(Math.PI * .5), (float)Math.PI, true);
                cornerPath.AddArc(new CGPoint((float)Bounds.X + pancake.CornerRadius.Left, (float)Bounds.Y + pancake.CornerRadius.Left), (float)pancake.CornerRadius.Left, (float)Math.PI, (float)(Math.PI * 1.5), true);

                var maskLayer = new CAShapeLayer
                {
                    Frame = Bounds,
                    Path = cornerPath.CGPath
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
                var insetBounds = Bounds.Inset(pancake.BorderThickness, pancake.BorderThickness);
                var cornerPath = UIBezierPath.FromRect(insetBounds);

                // Create arcs for the given corner radius.
                cornerPath.AddArc(new CGPoint((float)insetBounds.X + insetBounds.Width - pancake.CornerRadius.Top, (float)insetBounds.Y + pancake.CornerRadius.Top), (float)pancake.CornerRadius.Top, (float)(Math.PI * 1.5), (float)Math.PI * 2, true);
                cornerPath.AddArc(new CGPoint((float)insetBounds.X + insetBounds.Width - pancake.CornerRadius.Right, (float)insetBounds.Y + insetBounds.Height - pancake.CornerRadius.Right), (float)pancake.CornerRadius.Right, 0, (float)(Math.PI * .5), true);
                cornerPath.AddArc(new CGPoint((float)insetBounds.X + pancake.CornerRadius.Bottom, (float)insetBounds.Y + insetBounds.Height - pancake.CornerRadius.Bottom), (float)pancake.CornerRadius.Bottom, (float)(Math.PI * .5), (float)Math.PI, true);
                cornerPath.AddArc(new CGPoint((float)insetBounds.X + pancake.CornerRadius.Left, (float)insetBounds.Y + pancake.CornerRadius.Left), (float)pancake.CornerRadius.Left, (float)Math.PI, (float)(Math.PI * 1.5), true);

                _borderLayer.Frame = Bounds;
                _borderLayer.Path = cornerPath.CGPath;

                // Dash pattern for the border.
                if (pancake.BorderIsDashed)
                {
                    _borderLayer.LineDashPattern = new NSNumber[] { new NSNumber(5), new NSNumber(2) };
                }
            }
        }
    }
}
