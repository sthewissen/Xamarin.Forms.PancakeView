using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace Thewissen.PancakeViewSample.PageModels
{
    public class DebugPageModel : FreshMvvm.FreshBasePageModel
    {
        private static readonly Random _randomGen = new Random();

        private int borderThickness;
        private bool borderDrawingInside = true;
        private bool hasGradientBorder;
        private bool hasGradientBackground;
        private bool hasShadow;
        private bool hasGradientStopsBorder;
        private bool hasGradientStopsBackground;
        private bool hasIrregularCornerRadius;
        private bool borderIsDashed;
        private bool testClipping;
        private int elevation;
        private BorderDrawingStyle borderDrawingStyle;
        private int sides = 4;
        private int offsetAngle;
        private int borderGradientAngle;
        private int backgroundGradientAngle;
        private CornerRadius cornerRadius = 20;
        private int backgroundColorR;
        private int backgroundColorG;
        private int backgroundColorB;
        private int backgroundColorA = 255;

        private int sliderCornerRadius = 20;

        public ICommand CloseDebugModeCommand { get; set; }
        public Color RandomColor => GetRandomColor();

        public Color BackgroundColor => Color.FromRgba(BackgroundColorR, BackgroundColorG, BackgroundColorB, BackgroundColorA);

        public int BackgroundColorR
        {
            get => backgroundColorR; set
            {
                backgroundColorR = value;
                RaisePropertyChanged(nameof(BackgroundColorR));
                RaisePropertyChanged(nameof(BackgroundColor));
            }
        }

        public int BackgroundColorG
        {
            get => backgroundColorG; set
            {
                backgroundColorG = value;
                RaisePropertyChanged(nameof(BackgroundColorG));
                RaisePropertyChanged(nameof(BackgroundColor));
            }
        }

        public int BackgroundColorB
        {
            get => backgroundColorB; set
            {
                backgroundColorB = value;
                RaisePropertyChanged(nameof(BackgroundColorB));
                RaisePropertyChanged(nameof(BackgroundColor));
            }
        }

        public int BackgroundColorA
        {
            get => backgroundColorA; set
            {
                backgroundColorA = value;
                RaisePropertyChanged(nameof(BackgroundColorA));
                RaisePropertyChanged(nameof(BackgroundColor));
            }
        }

        public bool HasGradientBackground
        {
            get => hasGradientBackground; set
            {
                hasGradientBackground = value;
                RaisePropertyChanged(nameof(HasGradientBackground));
            }
        }

        public bool HasGradientStopsBackground
        {
            get => hasGradientStopsBackground; set
            {
                hasGradientStopsBackground = value;
                RaisePropertyChanged(nameof(HasGradientStopsBackground));
            }
        }

        public int BackgroundGradientAngle
        {
            get => backgroundGradientAngle;
            set
            {
                backgroundGradientAngle = value;
                RaisePropertyChanged(nameof(BackgroundGradientAngle));
            }
        }

        public int BorderThickness
        {
            get => borderThickness; set
            {
                borderThickness = value;
                RaisePropertyChanged(nameof(BorderThickness));
            }
        }

        public BorderDrawingStyle BorderDrawingStyle
        {
            get => borderDrawingStyle; set
            {
                borderDrawingStyle = value;
                RaisePropertyChanged(nameof(BorderDrawingStyle));
            }
        }

        public bool BorderDrawingInside
        {
            get => borderDrawingInside; set
            {
                borderDrawingInside = value;
                BorderDrawingStyle = borderDrawingInside ? BorderDrawingStyle.Inside : BorderDrawingStyle.Outside;
                RaisePropertyChanged(nameof(BorderDrawingInside));
            }
        }

        public bool BorderIsDashed
        {
            get => borderIsDashed; set
            {
                borderIsDashed = value;
                RaisePropertyChanged(nameof(BorderIsDashed));
            }
        }

        public bool HasGradientBorder
        {
            get => hasGradientBorder; set
            {
                hasGradientBorder = value;
                RaisePropertyChanged(nameof(HasGradientBorder));
            }
        }

        public bool HasGradientStopsBorder
        {
            get => hasGradientStopsBorder; set
            {
                hasGradientStopsBorder = value;
                RaisePropertyChanged(nameof(HasGradientStopsBorder));
            }
        }

        public int BorderGradientAngle
        {
            get => borderGradientAngle;
            set
            {
                borderGradientAngle = value;
                RaisePropertyChanged(nameof(BorderGradientAngle));
            }
        }

        public bool HasShadow
        {
            get => hasShadow; set
            {
                hasShadow = value;
                RaisePropertyChanged(nameof(HasShadow));
            }
        }

        public int Elevation
        {
            get => elevation; set
            {
                elevation = value;
                RaisePropertyChanged(nameof(Elevation));
            }
        }

        public bool TestClipping
        {
            get => testClipping; set
            {
                testClipping = value;
                RaisePropertyChanged(nameof(TestClipping));
            }
        }

        public int Sides
        {
            get => sides;
            set
            {
                sides = value;
                RaisePropertyChanged(nameof(Sides));
            }
        }

        public int OffsetAngle
        {
            get => offsetAngle;
            set
            {
                offsetAngle = value;
                RaisePropertyChanged(nameof(OffsetAngle));
            }
        }

        public CornerRadius CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = value;
                RaisePropertyChanged(nameof(CornerRadius));
            }
        }

        public int SliderCornerRadius
        {
            get => sliderCornerRadius;
            set
            {
                sliderCornerRadius = value;
                RaisePropertyChanged(nameof(SliderCornerRadius));
                CornerRadius = value;
            }
        }

        public bool HasIrregularCornerRadius
        {
            get => hasIrregularCornerRadius; set
            {
                hasIrregularCornerRadius = value;
                RaisePropertyChanged(nameof(HasIrregularCornerRadius));

                if(hasIrregularCornerRadius)
                {
                    CornerRadius = new CornerRadius(10, 30, 10, 50);
                }
                else
                {
                    CornerRadius = SliderCornerRadius;
                }
            }
        }

        public DebugPageModel()
        {
            CloseDebugModeCommand = new Command(async (x) => await CoreMethods.PopPageModel(true, true));

            var color = GetRandomColor();

            BackgroundColorR = (int)(255 * color.R);
            BackgroundColorG = (int)(255 * color.G);
            BackgroundColorB = (int)(255 * color.B);

            RaisePropertyChanged(nameof(BackgroundColor));
        }

        public static Color GetRandomColor()
        {
            var color = Color.FromRgb((byte)_randomGen.Next(0, 255), (byte)_randomGen.Next(0, 255), (byte)_randomGen.Next(0, 255));
            return color;
        }
    }
}
