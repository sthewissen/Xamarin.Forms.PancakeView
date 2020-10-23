using Gtk;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView.Platforms.GTK;
using Xamarin.Forms.Platform.GTK;
using Xamarin.Forms.Platform.GTK.Extensions;
using Controls = Xamarin.Forms.PancakeView;

[assembly: ExportRenderer(typeof(Controls.PancakeView), typeof(PancakeViewRenderer))]
namespace Xamarin.Forms.PancakeView.Platforms.GTK
{
	public class PancakeViewRenderer : ViewRenderer<PancakeView, Gtk.Frame>
	{
		VisualElement _currentView;
		readonly Gtk.Frame _rounding;
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
			_rounding = new Gtk.Frame();
			_rounding.ModifyBg(StateType.Normal, Color.White.ToGtkColor());
		}

		protected override void OnElementChanged(ElementChangedEventArgs<PancakeView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
					SetNativeControl(new Gtk.Frame());

				var pancake = (Element as PancakeView);

				UpdateContent();
				UpdateBackgroundColor();
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
			else if (e.PropertyName == PancakeView.ShadowProperty.PropertyName)
			{
				UpdateShadow(pancake);
			}
			else if (e.PropertyName == PancakeView.BackgroundColorProperty.PropertyName ||
				e.PropertyName == PancakeView.BackgroundGradientEndPointProperty.PropertyName ||
				e.PropertyName == PancakeView.BackgroundGradientStartPointProperty.PropertyName ||
				e.PropertyName == PancakeView.BackgroundGradientStopsProperty.PropertyName)
			{
				UpdateBackgroundColor();
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

			}
		}

		void UpdateContent()
		{
			if (_currentView != null)
			{
				_currentView.Cleanup(); // cleanup old view
			}

			_currentView = Element.Content;
			var render = Platform.GTK.Platform.GetRenderer(_currentView) ?? Platform.GTK.Platform.CreateRenderer(_currentView);

			Control.Child = _currentView != null ? render.Container : null;

			//Update vertical content Alignment
			if (Control.Child != null)
			{
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
				Control.ModifyBg(StateType.Normal, Element.BackgroundColor.IsDefault ? Gdk.Color.Zero : Element.BackgroundColor.ToGtkColor());
			}
		}
	}
}