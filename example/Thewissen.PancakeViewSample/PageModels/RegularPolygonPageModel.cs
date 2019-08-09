using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Thewissen.PancakeViewSample.PageModels
{
    public class RegularPolygonPageModel : FreshMvvm.FreshBasePageModel
    {
        public ICommand ClosePolygonPageCommand { get; set; }

        public RegularPolygonPageModel()
        {
            ClosePolygonPageCommand = new Command(async (x) => await CoreMethods.PopPageModel(true, true));
        }


        private int _sides = 6;
        private double _offsetAngle;
        private double _cornerRadius;

        public int Sides
        {
            get => _sides;
            set
            {
                _sides = value;
                RaisePropertyChanged("Sides");
            }
        }

        public double OffsetAngle
        {
            get => _offsetAngle;
            set
            {
                _offsetAngle = value;
                RaisePropertyChanged("OffsetAngle");
            }
        }

        public double CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = value;
                RaisePropertyChanged("CornerRadius");
            }
        }
    }
}
