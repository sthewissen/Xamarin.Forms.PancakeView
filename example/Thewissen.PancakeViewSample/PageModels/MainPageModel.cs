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
        #region border dash

        int currentBorderDashIndex = 0;

        List<DashPattern> DashPatterns = new List<DashPattern>
        {
            new DashPattern(10,5,2,5),
            new DashPattern(5,20),
            new DashPattern(10,5,15,5,20,5),
            new DashPattern(5,2)
        };
        private int shadowColorInt;

        public DashPattern BorderDashPattern { get; set; }

        #endregion

        #region shadow

        [AlsoNotifyFor(nameof(ShadowOffset))]
        public float ShadowOffsetX { get; set; } = 10;

        [AlsoNotifyFor(nameof(ShadowOffset))]
        public float ShadowOffsetY { get; set; } = 10;

        public Point ShadowOffset => new Point(ShadowOffsetX, ShadowOffsetY);

        public int ShadowColorInt { get; set; }

        #endregion

        public ICommand OpenDebugModeCommand { get; set; }
        public ICommand CycleBorderDashPatternCommand { get; set; }

        public MainPageModel()
        {
            OpenDebugModeCommand = new Command(async (x) => await CoreMethods.PushPageModel<DebugPageModel>(true, true));

            // Set up the border dash sample.
            CycleBorderDashPatternCommand = new Command(CycleBorderDash);
            BorderDashPattern = DashPatterns.FirstOrDefault();
        }

        void CycleBorderDash()
        {
            if (currentBorderDashIndex == DashPatterns.Count - 1)
                currentBorderDashIndex = 0;
            else
                currentBorderDashIndex += 1;

            BorderDashPattern = DashPatterns[currentBorderDashIndex];
        }
    }
}
