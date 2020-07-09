using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thewissen.PancakeViewSample.PageModels;
using Xamarin.Forms;

namespace Thewissen.PancakeViewSample.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageModel();
        }
    }
}
