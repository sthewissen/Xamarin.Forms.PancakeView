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
using Windows.UI.Composition;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.UWP
{
    public class PancakeViewRenderer : ViewRenderer<PancakeView, Windows.UI.Xaml.Controls.Border>
    {
        //create the shadow effect 
        private Windows.UI.Xaml.Shapes.Rectangle rectangle; 
        private Windows.UI.Xaml.Controls.Grid container;
        private Windows.UI.Xaml.Controls.Border content;
        private SpriteVisual visual;

        /// <summary>
        /// This method ensures that we don't get stripped out by the linker.
        /// </summary>
        public static void Init()
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
                    SetNativeControl(new Windows.UI.Xaml.Controls.Border());

                var pancake = (Element as PancakeView);

                PackChild();
                UpdateBackgroundColor();
                UpdateBorder(pancake);
                UpdateCornerRadius(pancake);
                UpdateShadow(pancake);
            }
        }

        void PackChild()
        {
            container = new Windows.UI.Xaml.Controls.Grid { HorizontalAlignment = Control.HorizontalAlignment, VerticalAlignment = Control.VerticalAlignment };
            content = new Windows.UI.Xaml.Controls.Border { HorizontalAlignment = Control.HorizontalAlignment, VerticalAlignment = Control.VerticalAlignment };

            rectangle = new Windows.UI.Xaml.Shapes.Rectangle { Fill = new SolidColorBrush(Windows.UI.Colors.Transparent) };
            
            container.Children.Add(rectangle);
            container.Children.Add(content);
            Control.Child = (container);

            this.UpdateChild();
        }

        void UpdateChild()
        {
            var updateContainer = Control.Child as Windows.UI.Xaml.Controls.Grid;
            if (updateContainer != null && updateContainer.Children.Count == 2)
            {
                if (updateContainer.Children[1] is Windows.UI.Xaml.Controls.Border updateContent)
                {
                    if (Element.Content != null)
                    {
                        IVisualElementRenderer renderer = Element.Content.GetOrCreateRenderer();
                        updateContent.Child = (renderer.ContainerElement);
                    }
                }
            }
        }


        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var pancake = (Element as PancakeView);

            // If the border is changed, we need to change the border layer we created.
            if (e.PropertyName == PancakeView.ContentProperty.PropertyName)
            {
                UpdateChild();
            }
            else if (e.PropertyName == PancakeView.CornerRadiusProperty.PropertyName)
            {
                UpdateCornerRadius(pancake);
                UpdateShadow(pancake);
            }
            else if (e.PropertyName == PancakeView.WidthProperty.PropertyName ||
                e.PropertyName == PancakeView.HeightProperty.PropertyName ||
                e.PropertyName == PancakeView.ShadowProperty.PropertyName)
            {
                UpdateShadow(pancake);
            }
            else if (e.PropertyName == PancakeView.BackgroundGradientAngleProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName)
            {
                UpdateBackgroundColor();
            }
            else if (e.PropertyName == PancakeView.BorderProperty.PropertyName)
            {
                UpdateBorder(pancake);
            }
        }

        private void UpdateShadow(PancakeView pancake)
        {
            //For now gets the shadow only when the CornerRadius has the same value for all sides. 
            if (Control != null && pancake.Shadow != null && pancake.Width > 0 && pancake.Height > 0 &&
                pancake.CornerRadius.TopLeft == pancake.CornerRadius.BottomRight &&
                pancake.CornerRadius.TopLeft == pancake.CornerRadius.BottomLeft &&
                pancake.CornerRadius.BottomRight == pancake.CornerRadius.TopRight)
            {
                rectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                rectangle.Width = pancake.Width;
                rectangle.Height = pancake.Height;
                rectangle.RadiusX = pancake.CornerRadius.TopRight + 5;
                rectangle.RadiusY = pancake.CornerRadius.TopRight + 5;

                var compositor = ElementCompositionPreview.GetElementVisual(rectangle).Compositor;
                visual = compositor.CreateSpriteVisual();
                visual.Size = new Vector2((float)pancake.Width, (float)pancake.Height);

                var shadow = compositor.CreateDropShadow();
                shadow.BlurRadius = pancake.Shadow.BlurRadius;
                shadow.Mask = rectangle.GetAlphaMask();
                shadow.Opacity = pancake.Shadow.Opacity;
                shadow.Color = pancake.Shadow.Color.ToWindowsColor();
                shadow.Offset = new Vector3((float)pancake.Shadow.Offset.X, (float)pancake.Shadow.Offset.Y, 0);
                visual.Shadow = shadow;

                ElementCompositionPreview.SetElementChildVisual(rectangle, visual);
            }
            else
            {
                if (rectangle != null)
                {
                    rectangle.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
                }

                if (visual != null)
                {
                    visual.Shadow = null;
                    ElementCompositionPreview.SetElementChildVisual(rectangle, null);
                }
            }
        }

        private void UpdateCornerRadius(PancakeView pancake)
        {
            if (content != null)
            {
                this.content.CornerRadius = new Windows.UI.Xaml.CornerRadius(pancake.CornerRadius.TopLeft, pancake.CornerRadius.TopRight, pancake.CornerRadius.BottomRight, pancake.CornerRadius.BottomLeft);
            }
        }

        private void UpdateBorder(PancakeView pancake)
        {
            //// Create the border layer
            if (content != null && pancake.Border != null)
            {
                this.content.BorderThickness = new Windows.UI.Xaml.Thickness(pancake.Border.Thickness.Left);

                if (pancake.Border.GradientStops != null && pancake.Border.GradientStops.Any())
                {
                    // A range of colors is given. Let's add them.
                    var orderedStops = pancake.Border.GradientStops.OrderBy(x => x.Offset).ToList();
                    var gc = new Windows.UI.Xaml.Media.GradientStopCollection();

                    foreach (var item in orderedStops)
                        gc.Add(new Windows.UI.Xaml.Media.GradientStop { Offset = item.Offset, Color = item.Color.ToWindowsColor() });

                    var gradient = new LinearGradientBrush(gc, pancake.Border.GradientAngle);
                    gradient.StartPoint = new Windows.Foundation.Point(0.5, 0);
                    gradient.EndPoint = new Windows.Foundation.Point(0.5, 1);

                    this.content.BorderBrush = gradient;
                }
                else
                {
                    this.content.BorderBrush = pancake.Border.Color.IsDefault ? null : pancake.Border.Color.ToBrush();
                }
            }
        }

        protected override void UpdateBackgroundColor()
        {
            // background color change must be handled separately
            // because the background would protrude through the border if the corners are rounded
            // as the background would be applied to the renderer's FrameworkElement
            var pancake = (PancakeView)Element;

            if (content != null)
            {
                if (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Any())
                {
                    var totalAngle = pancake.BackgroundGradientAngle / 360.0;

                    // Calculate the new positions based on angle between 0-360.
                    var a = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.75) / 2)), 2);
                    var b = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.0) / 2)), 2);
                    var c = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.25) / 2)), 2);
                    var d = Math.Pow(Math.Sin(2 * Math.PI * ((totalAngle + 0.5) / 2)), 2);

                    // A range of colors is given. Let's add them.
                    var orderedStops = pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                    var gc = new Windows.UI.Xaml.Media.GradientStopCollection();

                    foreach (var item in orderedStops)
                        gc.Add(new Windows.UI.Xaml.Media.GradientStop { Offset = item.Offset, Color = item.Color.ToWindowsColor() });

                    var gradient = new LinearGradientBrush(gc, 0);
                    gradient.StartPoint = new Windows.Foundation.Point(1-a, b);
                    gradient.EndPoint = new Windows.Foundation.Point(1-c, d);
                    this.content.Background = gradient;
                }
                else
                {
                    content.Background = Element.BackgroundColor.IsDefault ? null : Element.BackgroundColor.ToBrush();
                }
            }
        }
    }
}