using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PropertyChanged;
using Thewissen.PancakeViewSample.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace Thewissen.PancakeViewSample.PageModels
{
    public class MainPageModel : FreshMvvm.FreshBasePageModel
    {
        private static readonly Random _randomGen = new Random();
        public Color RandomColor => GetRandomColor();

        int currentBorderDashIndex = 0;

        List<DashPattern> DashPatterns = new List<DashPattern>
        {
            new DashPattern(10,5,2,5),
            new DashPattern(5,20),
            new DashPattern(10,5,15,5,20,5),
            new DashPattern(5,2)
        };

        public DashPattern BorderDashPattern { get; set; }
        public Point ShadowOffset { get; set; } = new Point(20, 20);
        public Color ShadowColor { get; set; } = Color.Black;
        public Color BackgroundGradientColor1 { get; set; } = Color.DeepPink;
        public Color BackgroundGradientColor2 { get; set; } = Color.Orange;
        public GradientStopCollection BackgroundGradientStops { get; set; } = new GradientStopCollection();

        [AlsoNotifyFor(nameof(CornerRadius))]
        public double CornerRadiusTopLeft { get; set; } = 40;

        [AlsoNotifyFor(nameof(CornerRadius))]
        public double CornerRadiusTopRight { get; set; } = 0;

        [AlsoNotifyFor(nameof(CornerRadius))]
        public double CornerRadiusBottomLeft { get; set; } = 25;

        [AlsoNotifyFor(nameof(CornerRadius))]
        public double CornerRadiusBottomRight { get; set; } = 5;

        public CornerRadius CornerRadius => new CornerRadius(CornerRadiusTopLeft, CornerRadiusTopRight, CornerRadiusBottomLeft, CornerRadiusBottomRight);

        public ICommand OpenDebugModeCommand { get; set; }
        public ICommand CycleBorderDashPatternCommand { get; set; }
        public ICommand GenerateRandomColorCommand { get; set; }
        public ICommand GenerateRandomShadowOffsetCommand { get; set; }
        public ICommand GenerateRandomGradientCommand { get; set; }

        public MainPageModel()
        {
            OpenDebugModeCommand = new Command(async (x) => await CoreMethods.PushPageModel<DebugPageModel>(true, true));

            CycleBorderDashPatternCommand = new Command(CycleBorderDash);
            GenerateRandomShadowOffsetCommand = new Command(GenerateRandomShadowOffset);
            GenerateRandomColorCommand = new Command<SampleColorType>(GenerateRandomColor);
            GenerateRandomGradientCommand = new Command(() => BackgroundGradientStops = GetRandomGradient());

            BorderDashPattern = DashPatterns.FirstOrDefault();
            BackgroundGradientStops = GetRandomGradient();
        }

        void GenerateRandomColor(SampleColorType type)
        {
            var color = GetRandomColor();

            switch (type)
            {
                case SampleColorType.BackgroundGradientColor1:
                    BackgroundGradientColor1 = color;
                    break;
                case SampleColorType.BackgroundGradientColor2:
                    BackgroundGradientColor2 = color;
                    break;
                case SampleColorType.ShadowColor:
                    ShadowColor = color;
                    break;
                default:
                    break;
            }
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

        public static GradientStopCollection GetRandomGradient()
        {
            var gradient = new GradientStopCollection();
            var itemCount = _randomGen.Next(2, 7);

            for (int i = 0; i < itemCount; i++)
            {
                gradient.Add(new GradientStop { Color = GetRandomColor(), Offset = (1.0f / itemCount) * i });
            }

            return gradient;
        }

        public static Color GetRandomColor()
        {
            var color = Color.FromRgb((byte)_randomGen.Next(0, 255), (byte)_randomGen.Next(0, 255), (byte)_randomGen.Next(0, 255));
            return color;
        }
    }
}
