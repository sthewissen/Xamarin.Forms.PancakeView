using Android.App;
using Android.Content.PM;
using Android.OS;
// using Xamarin.Forms.PancakeView.Droid;

namespace Thewissen.PancakeViewSample.Droid
{
    [Activity(Label = "PancakeViewSample", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // PancakeViewRenderer.Init();
        }
    }
}