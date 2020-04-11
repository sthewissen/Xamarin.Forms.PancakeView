using System;
namespace Xamarin.Forms.PancakeView.Platforms.Shared
{
    [Flags]
    public enum Edge
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,
        All = Left | Top | Right | Bottom
    }
}
