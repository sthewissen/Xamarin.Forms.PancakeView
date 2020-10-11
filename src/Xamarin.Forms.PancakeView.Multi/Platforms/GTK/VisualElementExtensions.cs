using System;
using Xamarin.Forms.Platform.GTK;

namespace Xamarin.Forms.PancakeView.Platforms.GTK
{
    public static class VisualElementExtensions
    {
        public static void Cleanup(this VisualElement self)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            IVisualElementRenderer renderer = Platform.GTK.Platform.GetRenderer(self);

            foreach (Element element in self.Descendants())
            {
                var visual = element as VisualElement;
                if (visual == null)
                    continue;

                IVisualElementRenderer childRenderer = Platform.GTK.Platform.GetRenderer(visual);
                if (childRenderer != null)
                {
                    childRenderer.Dispose();
                    Platform.GTK.Platform.SetRenderer(visual, null);
                }
            }

            if (renderer != null)
            {
                renderer.Dispose();
                Platform.GTK.Platform.SetRenderer(self, null);
            }
        }
    }
}
