using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using PropertyChanged;
using Thewissen.PancakeViewSample.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace Thewissen.PancakeViewSample.PageModels
{
    public class MainPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        static readonly Random _randomGen = new Random();
        public Color RandomColor => GetRandomColor();

        int currentBorderDashIndex = 0;
        int currentBorderDrawingStyleIndex = 0;

        List<DashPattern> DashPatterns = new List<DashPattern>
        {
            new DashPattern(10,5,2,5),
            new DashPattern(5,20),
            new DashPattern(10,5,15,5,20,5),
            new DashPattern(5,2)
        };

        List<BorderDrawingStyle> BorderDrawingStyles = new List<BorderDrawingStyle>
        {
           BorderDrawingStyle.Inside,
           BorderDrawingStyle.Outside,
           BorderDrawingStyle.Centered
        };

        public BorderDrawingStyle BorderDrawingStyle { get; set; }
        public DashPattern BorderDashPattern { get; set; }
        public Point ShadowOffset { get; set; } = new Point(20, 20);
        public Color ShadowColor { get; set; } = Color.Black;
        public Color BackgroundGradientColor1 { get; set; } = Color.DeepPink;
        public Color BackgroundGradientColor2 { get; set; } = Color.Orange;
        public Point BackgroundGradientStartPoint { get; set; } = new Point(0, 0);
        public Point BackgroundGradientEndPoint { get; set; } = new Point(1, 1);
        public Point BackgroundGradientStartPoint2 { get; set; } = new Point(0, 0);
        public Point BackgroundGradientEndPoint2 { get; set; } = new Point(1, 1);
        public Point BorderGradientStartPoint { get; set; } = new Point(0, 0);
        public Point BorderGradientEndPoint { get; set; } = new Point(1, 1);
        public Color BorderColor { get; set; } = Color.BlueViolet;
        public GradientStopCollection BackgroundGradientStops { get; set; } = new GradientStopCollection();
        public GradientStopCollection BorderGradientStops { get; set; } = new GradientStopCollection();

        [AlsoNotifyFor(nameof(CornerRadius))]
        public double CornerRadiusTopLeft { get; set; } = 40;

        [AlsoNotifyFor(nameof(CornerRadius))]
        public double CornerRadiusTopRight { get; set; } = 0;

        [AlsoNotifyFor(nameof(CornerRadius))]
        public double CornerRadiusBottomLeft { get; set; } = 25;

        [AlsoNotifyFor(nameof(CornerRadius))]
        public double CornerRadiusBottomRight { get; set; } = 5;

        public CornerRadius CornerRadius => new CornerRadius(CornerRadiusTopLeft, CornerRadiusTopRight, CornerRadiusBottomLeft, CornerRadiusBottomRight);

        public ICommand CycleBorderDashPatternCommand { get; set; }
        public ICommand CycleBorderDrawingStyleCommand { get; set; }
        public ICommand GenerateRandomColorCommand { get; set; }
        public ICommand GenerateRandomShadowOffsetCommand { get; set; }
        public ICommand GenerateRandomGradientCommand { get; set; }
        public ICommand GenerateRandomBorderGradientCommand { get; set; }
        public ICommand GenerateRandomPointCommand { get; set; }

        public MainPageModel()
        {
            CycleBorderDashPatternCommand = new Command(CycleBorderDash);
            CycleBorderDrawingStyleCommand = new Command(CycleBorderDrawingStyle);
            GenerateRandomShadowOffsetCommand = new Command(GenerateRandomShadowOffset);
            GenerateRandomColorCommand = new Command<SampleColorType>(GenerateRandomColor);
            GenerateRandomGradientCommand = new Command(() => BackgroundGradientStops = GetRandomGradient());
            GenerateRandomBorderGradientCommand = new Command(() => BorderGradientStops = GetRandomGradient());
            GenerateRandomPointCommand = new Command<SamplePointType>(GenerateRandomPoint);

            BorderDashPattern = DashPatterns.FirstOrDefault();
            BackgroundGradientStops = GetRandomGradient();
            BorderGradientStops = GetRandomGradient();
        }

        private void GenerateRandomPoint(SamplePointType type)
        {
            var point = new Point(Math.Round(_randomGen.Next(0, 100) / 100f, 2), Math.Round(_randomGen.Next(0, 100) / 100f, 2));

            switch (type)
            {
                case SamplePointType.BackgroundGradientStartPoint:
                    BackgroundGradientStartPoint = point;
                    break;
                case SamplePointType.BackgroundGradientEndPoint:
                    BackgroundGradientEndPoint = point;
                    break;
                case SamplePointType.BackgroundGradientStartPoint2:
                    BackgroundGradientStartPoint2 = point;
                    break;
                case SamplePointType.BackgroundGradientEndPoint2:
                    BackgroundGradientEndPoint2 = point;
                    break;
                case SamplePointType.BorderGradientStartPoint:
                    BorderGradientStartPoint = point;
                    break;
                case SamplePointType.BorderGradientEndPoint:
                    BorderGradientEndPoint = point;
                    break;
                default:
                    break;
            }
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
                case SampleColorType.BorderColor:
                    BorderColor = color;
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

        void CycleBorderDrawingStyle()
        {
            if (currentBorderDrawingStyleIndex == BorderDrawingStyles.Count - 1)
                currentBorderDrawingStyleIndex = 0;
            else
                currentBorderDrawingStyleIndex += 1;

            BorderDrawingStyle = BorderDrawingStyles[currentBorderDrawingStyleIndex];
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
