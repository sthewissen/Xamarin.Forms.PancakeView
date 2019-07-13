using System;
using Thewissen.PancakeViewSample.PageModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Thewissen.PancakeViewSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var page = FreshMvvm.FreshPageModelResolver.ResolvePageModel<MainPageModel>();
            MainPage = new FreshMvvm.FreshNavigationContainer(page);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
