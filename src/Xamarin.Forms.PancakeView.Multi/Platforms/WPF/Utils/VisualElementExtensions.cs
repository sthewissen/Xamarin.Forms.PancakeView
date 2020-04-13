using System;
using Xamarin.Forms.Platform.WPF;

namespace Xamarin.Forms.PancakeView.Platforms.WPF.Utils
{
    public static class VisualElementExtensions
    {
        public static void Cleanup(this VisualElement self)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            IVisualElementRenderer renderer = Platform.WPF.Platform.GetRenderer(self);

            foreach (Element element in self.Descendants())
            {
                var visual = element as VisualElement;
                if (visual == null)
                    continue;

                IVisualElementRenderer childRenderer = Platform.WPF.Platform.GetRenderer(visual);
                if (childRenderer != null)
                {
                    childRenderer.Dispose();
                    Platform.WPF.Platform.SetRenderer(visual, null);
                }
            }

            if (renderer != null)
            {
                renderer.Dispose();
                Platform.WPF.Platform.SetRenderer(self, null);
            }
        }
    }
}
