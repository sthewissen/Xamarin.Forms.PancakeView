using System;
using Xamarin.Forms;

namespace Thewissen.PancakeViewSample.Tizen
{
    class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            ElmSharp.Utility.AppendGlobalFontPath(this.DirectoryInfo.Resource);
            MainWindow.IndicatorMode = ElmSharp.IndicatorMode.Hide;
            var app = new App();
            app.MainPage.BackgroundColor = Color.White;
            LoadApplication(app);
        }

        static void Main(string[] args)
        {
            var app = new Program();
            Forms.Init(app, true);
            app.Run(args);
        }
    }
}
