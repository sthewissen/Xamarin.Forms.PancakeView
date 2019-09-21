using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView.UWP;
using Xamarin.Forms.Platform.UWP;
using Controls = Xamarin.Forms.PancakeView;
using System.Numerics;
using Windows.UI.Xaml.Hosting;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.UWP
{
    public class PancakeViewRenderer : ViewRenderer<PancakeView, Border>
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

        public PancakeViewRenderer()
        {
            AutoPackage = false;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            // We need an automation peer so we can interact with this in automated tests
            if (Control == null)
            {
                return new FrameworkElementAutomationPeer(this);
            }

            return new FrameworkElementAutomationPeer(Control);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PancakeView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                    SetNativeControl(new Border());

                var pancake = (Element as PancakeView);

                // Angle needs to be between 0-360.
                if (pancake.BackgroundGradientAngle < 0 || pancake.BackgroundGradientAngle > 360)
                    throw new ArgumentException("Please provide a valid background gradient angle.", nameof(PancakeView.BackgroundGradientAngle));

                if (pancake.BorderGradientAngle < 0 || pancake.BorderGradientAngle > 360)
                    throw new ArgumentException("Please provide a valid border gradient angle.", nameof(PancakeView.BorderGradientAngle));

                PackChild();
                UpdateBackgroundColor();
                UpdateBorder(pancake);
                UpdateCornerRadius(pancake);
                UpdateShadow(pancake);
            }
        }
        
        void PackChild()
        {
            if (Element.Content == null)
                return;

            IVisualElementRenderer renderer = Element.Content.GetOrCreateRenderer();
            Control.Child = renderer.ContainerElement;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var pancake = (Element as PancakeView);

            // If the border is changed, we need to change the border layer we created.
            if (e.PropertyName == PancakeView.ContentProperty.PropertyName)
            {
                PackChild();
            }
            else if (e.PropertyName == PancakeView.CornerRadiusProperty.PropertyName)
            {
                UpdateCornerRadius(pancake);
            }
            else if (e.PropertyName == PancakeView.HasShadowProperty.PropertyName ||
                e.PropertyName == PancakeView.ElevationProperty.PropertyName)
            {
                UpdateShadow(pancake);
            }
            else if(e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStartColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientEndColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName)
            {
                UpdateBackgroundColor();
            }
            else if(e.PropertyName == PancakeView.BorderGradientAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientStartColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientEndColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderGradientStopsProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderThicknessProperty.PropertyName ||
                e.PropertyName == PancakeView.BorderIsDashedProperty.PropertyName)
            {
                UpdateBorder(pancake);
            }
        }

        private void UpdateShadow(PancakeView pancake)
        {
            if (Control != null)
            {

            }
        }

        private void UpdateCornerRadius(PancakeView pancake)
        {
            if(Control!= null)
            {
                this.Control.CornerRadius = new Windows.UI.Xaml.CornerRadius(pancake.CornerRadius.TopLeft, pancake.CornerRadius.TopRight, pancake.CornerRadius.BottomRight, pancake.CornerRadius.BottomLeft);
            }
        }

        private void UpdateBorder(PancakeView pancake)
        {
            //// Create the border layer
            if (Control != null)
            {
                this.Control.BorderThickness = new Windows.UI.Xaml.Thickness(pancake.BorderThickness);

                if ((pancake.BorderGradientStartColor != default(Color) && pancake.BorderGradientEndColor != default(Color)) || (pancake.BorderGradientStops != null && pancake.BorderGradientStops.Any()))
                {
                    // Create a gradient layer that draws our background.
                    if (pancake.BorderGradientStops != null && pancake.BorderGradientStops.Count > 0)
                    {
                        // A range of colors is given. Let's add them.
                        var orderedStops = pancake.BorderGradientStops.OrderBy(x => x.Offset).ToList();
                        var gc = new GradientStopCollection();

                        foreach (var item in orderedStops)
                            gc.Add(new Windows.UI.Xaml.Media.GradientStop { Offset = item.Offset, Color = item.Color.ToWindowsColor() });

                        this.Control.BorderBrush = new LinearGradientBrush(gc, pancake.BorderGradientAngle);
                    }
                    else
                    {
                        var gs1 = new Windows.UI.Xaml.Media.GradientStop { Offset = 0, Color = pancake.BorderGradientStartColor.ToWindowsColor() };
                        var gs2 = new Windows.UI.Xaml.Media.GradientStop { Offset = 1, Color = pancake.BorderGradientEndColor.ToWindowsColor() };
                        var gc = new GradientStopCollection { gs1, gs2 };
                        this.Control.BorderBrush = new LinearGradientBrush(gc, pancake.BorderGradientAngle);
                    }
                }
                else
                {
                    this.Control.BorderBrush = pancake.BorderColor.IsDefault ? null : pancake.BorderColor.ToBrush();
                }
            }
        }

        protected override void UpdateBackgroundColor()
        {
            // background color change must be handled separately
            // because the background would protrude through the border if the corners are rounded
            // as the background would be applied to the renderer's FrameworkElement
            var pancake = (PancakeView)Element;

            if (Control != null)
            {
                if ((pancake.BackgroundGradientStartColor != default(Color) && pancake.BackgroundGradientEndColor != default(Color)) || (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Any()))
                {
                    // Create a gradient layer that draws our background.
                    if (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Count > 0)
                    {
                        // A range of colors is given. Let's add them.
                        var orderedStops = pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                        var gc = new GradientStopCollection();

                        foreach (var item in orderedStops)
                            gc.Add(new Windows.UI.Xaml.Media.GradientStop { Offset = item.Offset, Color = item.Color.ToWindowsColor() });

                        this.Control.Background = new LinearGradientBrush(gc, pancake.BackgroundGradientAngle);
                    }
                    else
                    {
                        var gs1 = new Windows.UI.Xaml.Media.GradientStop { Offset = 0, Color = pancake.BackgroundGradientStartColor.ToWindowsColor() };
                        var gs2 = new Windows.UI.Xaml.Media.GradientStop { Offset = 1, Color = pancake.BackgroundGradientEndColor.ToWindowsColor() };
                        var gc = new GradientStopCollection { gs1, gs2 };
                        this.Control.Background = new LinearGradientBrush(gc, pancake.BackgroundGradientAngle);
                    }
                }
                else
                {
                    Control.Background = Element.BackgroundColor.IsDefault ? null : Element.BackgroundColor.ToBrush();
                }
            }
        }
    }
}