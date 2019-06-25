using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView.UWP;
using Xamarin.Forms.Platform.UWP;
using Controls = Xamarin.Forms.PancakeView;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.UWP
{
    public class PancakeViewRenderer : FrameRenderer
    {
        /// <summary>
        /// This method ensures that we don't get stripped out by the linker.
        /// </summary>
        public static new void Init()
        {
#pragma warning disable 0219
            var ignore1 = typeof(PancakeViewRenderer);
            var ignore2 = typeof(PancakeView);
#pragma warning restore 0219
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var pancake = (Element as PancakeView);

                // Angle needs to be between 0-360.
                if (pancake.BackgroundGradientAngle < 0 || pancake.BackgroundGradientAngle > 360)
                    throw new ArgumentException("Please provide a valid background gradient angle.", nameof(PancakeView.BackgroundGradientAngle));

                this.Setup(pancake);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var pancake = (Element as PancakeView);

            // If the border is changed, we need to change the border layer we created.
            if (e.PropertyName == PancakeView.CornerRadiiProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStartColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientEndColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderIsDashedProperty.PropertyName ||
                e.PropertyName == PancakeView.DrawBorderOnOutsideProperty.PropertyName)
            {
                this.Setup(pancake);
            }
        }

        private void Setup(PancakeView pancake)
        {
            // Create the border layer
            if (pancake.BorderThickness > 0)
            {
                this.Control.BorderThickness = new Windows.UI.Xaml.Thickness(pancake.BorderThickness);

                this.Control.BorderBrush = pancake.BorderColor.ToBrush();

                // TODO: border dashed
            }
            else
            {
                this.Control.BorderThickness = new Windows.UI.Xaml.Thickness(0);

                this.Control.BorderBrush = null;
            }

            // TODO: DrawBorderOnOutside

            this.Control.CornerRadius = new Windows.UI.Xaml.CornerRadius(pancake.CornerRadii.TopLeft, pancake.CornerRadii.TopRight, pancake.CornerRadii.BottomRight, pancake.CornerRadii.BottomLeft);

            if (pancake.BackgroundGradientStartColor != default(Xamarin.Forms.Color) && pancake.BackgroundGradientEndColor != default(Xamarin.Forms.Color))
            {
                var gs1 = new GradientStop { Offset = 0, Color = pancake.BackgroundGradientStartColor.ToWindowsColor() };
                var gs2 = new GradientStop { Offset = 1, Color = pancake.BackgroundGradientEndColor.ToWindowsColor() };
                var gc = new GradientStopCollection { gs1, gs2 };
                this.Control.Background = new LinearGradientBrush(gc, (pancake.BackgroundGradientAngle + 90) % 360);
            }
            // NOTE: FrameRenderer handles basic BackgroundColor
        }
    }
}
