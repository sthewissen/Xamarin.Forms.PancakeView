using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Thewissen.PancakeViewSample.PageModels
{
    public class DebugPageModel : FreshMvvm.FreshBasePageModel
    {
        private static readonly Random _randomGen = new Random();

        public Color RandomColor
        {
            get
            {
                return GetRandomColor();
            }
        }

        public ICommand CloseDebugModeCommand { get; set; }
        
        public DebugPageModel()
        {
            CloseDebugModeCommand = new Command(async (x) => await CoreMethods.PopPageModel(true, true));
        }

        public static Color GetRandomColor()
        {
            var color = Color.FromRgb((byte)_randomGen.Next(0, 255), (byte)_randomGen.Next(0, 255), (byte)_randomGen.Next(0, 255));
            return color;
        }
    }
}
