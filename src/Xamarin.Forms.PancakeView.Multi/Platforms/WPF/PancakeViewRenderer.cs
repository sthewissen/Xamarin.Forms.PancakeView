using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xamarin.Forms.PancakeView.Platforms.WPF;
using Xamarin.Forms.Platform.WPF;
using Controls = Xamarin.Forms.PancakeView;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.Platforms.WPF
{
    public class PancakeViewRenderer : ViewRenderer<PancakeView, System.Windows.Controls.Border>
    {
        VisualElement _currentView;
        readonly VisualBrush _mask;
        readonly System.Windows.Controls.Border _rounding;
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
            _rounding = new System.Windows.Controls.Border
            {
                Background = Color.White.ToBrush(),
                SnapsToDevicePixels = true
            };
            var wb = new System.Windows.Data.Binding(nameof(System.Windows.Controls.Border.ActualWidth))
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = typeof(Border)
                }
            };
            _rounding.SetBinding(FrameworkElement.WidthProperty, wb);
            var hb = new System.Windows.Data.Binding(nameof(System.Windows.Controls.Border.ActualHeight))
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = typeof(Border)
                }
            };
            _rounding.SetBinding(FrameworkElement.HeightProperty, hb);
            _mask = new VisualBrush(_rounding);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PancakeView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                    SetNativeControl(new System.Windows.Controls.Border());

                var pancake = (Element as PancakeView);

                UpdateContent();
                UpdateBackground();
                UpdateBorder(pancake);
                UpdateCornerRadius(pancake);
                UpdateShadow(pancake);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var pancake = (Element as PancakeView);

            // If the border is changed, we need to change the border layer we created.
            if (e.PropertyName == PancakeView.ContentProperty.PropertyName)
            {
                UpdateContent();
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
            else if (e.PropertyName == PancakeView.BackgroundColorProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientEndPointProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStartPointProperty.PropertyName ||
                e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName)
            {
                UpdateBackground();
            }
            else if (e.PropertyName == PancakeView.BorderProperty.PropertyName)
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
            if (Control != null)
            {
                Control.CornerRadius = new System.Windows.CornerRadius(pancake.CornerRadius.TopLeft, pancake.CornerRadius.TopRight, pancake.CornerRadius.BottomRight, pancake.CornerRadius.BottomLeft);
                _rounding.CornerRadius = Control.CornerRadius;
            }
        }

        void UpdateContent()
        {
            if (_currentView != null)
            {
                _currentView.Cleanup(); // cleanup old view
            }

            _currentView = Element.Content;
            Control.OpacityMask = _mask;
            Control.Child = _currentView != null ? Platform.WPF.Platform.GetOrCreateRenderer(_currentView).GetNativeElement() : null;

            //Update vertical content Alignment
            if (Control.Child != null)
            {
                (Control.Child as FrameworkElement).VerticalAlignment = Element.Content.VerticalOptions.ToNativeVerticalAlignment();
            }

        }

        private void UpdateBorder(PancakeView pancake)
        {
            //// Create the border layer
            if (Control != null)
            {
                this.Control.BorderThickness = new System.Windows.Thickness(pancake.Border.Thickness);

                if (pancake.Border.GradientStops != null && pancake.Border.GradientStops.Any())
                {
                    // Create a gradient layer that draws our background.
                    if (pancake.Border.GradientStops != null && pancake.Border.GradientStops.Count > 0)
                    {
                        // A range of colors is given. Let's add them.
                        var orderedStops = pancake.Border.GradientStops.OrderBy(x => x.Offset).ToList();
                        var gc = new System.Windows.Media.GradientStopCollection();

                        foreach (var item in orderedStops)
                            gc.Add(new System.Windows.Media.GradientStop { Offset = item.Offset, Color = item.Color.ToMediaColor() });

                        var gradient = new LinearGradientBrush(gc, 0);
                        gradient.StartPoint = new System.Windows.Point(pancake.Border.GradientStartPoint.X, pancake.Border.GradientStartPoint.Y);
                        gradient.EndPoint = new System.Windows.Point(pancake.Border.GradientEndPoint.X, pancake.Border.GradientEndPoint.Y);

                        this.Control.BorderBrush = gradient;
                    }
                }
                else
                {
                    this.Control.BorderBrush = pancake.Border.Color.IsDefault ? null : pancake.Border.Color.ToBrush();
                }
            }
        }

        protected override void UpdateBackground()
        {
            // background color change must be handled separately
            // because the background would protrude through the border if the corners are rounded
            // as the background would be applied to the renderer's FrameworkElement
            var pancake = (PancakeView)Element;

            if (Control != null)
            {
                if (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Any())
                {
                    // Create a gradient layer that draws our background.
                    if (pancake.BackgroundGradientStops != null && pancake.BackgroundGradientStops.Count > 0)
                    {
                        // A range of colors is given. Let's add them.
                        var orderedStops = pancake.BackgroundGradientStops.OrderBy(x => x.Offset).ToList();
                        var gc = new System.Windows.Media.GradientStopCollection();

                        foreach (var item in orderedStops)
                            gc.Add(new System.Windows.Media.GradientStop { Offset = item.Offset, Color = item.Color.ToMediaColor() });

                        var gradient = new LinearGradientBrush(gc, 0);
                        gradient.StartPoint = new System.Windows.Point(pancake.BackgroundGradientStartPoint.X, pancake.BackgroundGradientStartPoint.Y);
                        gradient.EndPoint = new System.Windows.Point(pancake.BackgroundGradientEndPoint.X, pancake.BackgroundGradientEndPoint.Y);

                        this.Control.Background = gradient;
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