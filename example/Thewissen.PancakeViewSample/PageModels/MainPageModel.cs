using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Thewissen.PancakeViewSample.PageModels
{
    public class MainPageModel : FreshMvvm.FreshBasePageModel
    {        
        public ICommand OpenDebugModeCommand { get; set; }
        public ICommand OpenPolygonMoreCommand { get; set; }

        public MainPageModel()
        {
            OpenDebugModeCommand = new Command(async (x) => await CoreMethods.PushPageModel<DebugPageModel>(true, true));
            OpenPolygonMoreCommand = new Command(async (x) => await CoreMethods.PushPageModel<RegularPolygonPageModel>(true, true));
        }
    }
}
