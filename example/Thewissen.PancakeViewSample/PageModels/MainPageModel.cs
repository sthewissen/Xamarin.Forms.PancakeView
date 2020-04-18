using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Thewissen.PancakeViewSample.PageModels
{
    public class MainPageModel : FreshMvvm.FreshBasePageModel
    {

        #region shadow

        int shadowColorInt = 0;
        float shadowBlurRadius = 10f;
        float shadowOpacity = 0.4f;
        float shadowOffsetX = 1f;
        float shadowOffsetY = 1f;

        public float ShadowOffsetX
        {
            get => shadowOffsetX; set
            {
                shadowOffsetX = value;
                RaisePropertyChanged(nameof(ShadowOffsetX));
                RaisePropertyChanged(nameof(ShadowOffset));
            }
        }

        public float ShadowOffsetY
        {
            get => shadowOffsetY; set
            {
                shadowOffsetY = value;
                RaisePropertyChanged(nameof(ShadowOffsetY));
                RaisePropertyChanged(nameof(ShadowOffset));
            }
        }

        public Point ShadowOffset => new Point(ShadowOffsetX, ShadowOffsetY);

        public float ShadowBlurRadius
        {
            get => shadowBlurRadius; set
            {
                shadowBlurRadius = value;
                RaisePropertyChanged(nameof(ShadowBlurRadius));
            }
        }

        public float ShadowOpacity
        {
            get => shadowOpacity; set
            {
                shadowOpacity = value;
                RaisePropertyChanged(nameof(ShadowOpacity));
            }
        }

        public int ShadowColorInt
        {
            get => shadowColorInt; set
            {
                shadowColorInt = value;
                RaisePropertyChanged(nameof(ShadowColorInt));
                RaisePropertyChanged(nameof(ShadowColor));
            }
        }

        public Color ShadowColor
        {
            get
            {
                var color = Color.FromUint((uint)ShadowColorInt);
                return new Color(color.R, color.G, color.B, 1);
            }
        }

        #endregion

        public ICommand OpenDebugModeCommand { get; set; }

        public MainPageModel()
        {
            OpenDebugModeCommand = new Command(async (x) => await CoreMethods.PushPageModel<DebugPageModel>(true, true));
        }
    }
}
