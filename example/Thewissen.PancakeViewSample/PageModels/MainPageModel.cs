using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PropertyChanged;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace Thewissen.PancakeViewSample.PageModels
{
    public class MainPageModel : FreshMvvm.FreshBasePageModel
    {
        private static readonly Random _randomGen = new Random();
        public Color RandomColor => GetRandomColor();

        #region border dash

        int currentBorderDashIndex = 0;

        List<DashPattern> DashPatterns = new List<DashPattern>
        {
            new DashPattern(10,5,2,5),
            new DashPattern(5,20),
            new DashPattern(10,5,15,5,20,5),
            new DashPattern(5,2)
        };

        public DashPattern BorderDashPattern { get; set; }

        #endregion

        #region shadow

        public Point ShadowOffset { get; set; } = new Point(20, 20);

        public Color ShadowColor { get; set; } = Color.Black;

        #endregion

        public ICommand OpenDebugModeCommand { get; set; }
        public ICommand CycleBorderDashPatternCommand { get; set; }
        public ICommand GenerateRandomShadowColorCommand { get; set; }
        public ICommand GenerateRandomShadowOffsetCommand { get; set; }

        public MainPageModel()
        {
            OpenDebugModeCommand = new Command(async (x) => await CoreMethods.PushPageModel<DebugPageModel>(true, true));

            // Set up the border dash sample.
            CycleBorderDashPatternCommand = new Command(CycleBorderDash);
            GenerateRandomShadowColorCommand = new Command(GenerateRandomShadowColor);
            GenerateRandomShadowOffsetCommand = new Command(GenerateRandomShadowOffset);
            BorderDashPattern = DashPatterns.FirstOrDefault();
        }

        void GenerateRandomShadowColor(object obj)
        {
            ShadowColor = GetRandomColor();
        }

        void GenerateRandomShadowOffset(object obj)
        {
            ShadowOffset = new Point(_randomGen.Next(-40, 40), _randomGen.Next(-40, 40));
        }

        void CycleBorderDash()
        {
            if (currentBorderDashIndex == DashPatterns.Count - 1)
                currentBorderDashIndex = 0;
            else
                currentBorderDashIndex += 1;

            BorderDashPattern = DashPatterns[currentBorderDashIndex];
        }

        public static Color GetRandomColor()
        {
            var color = Color.FromRgb((byte)_randomGen.Next(0, 255), (byte)_randomGen.Next(0, 255), (byte)_randomGen.Next(0, 255));
            return color;
        }
    }
}
