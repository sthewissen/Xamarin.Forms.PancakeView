using System;
namespace Xamarin.Forms.PancakeView
{
    public interface IPropagateChanges
    {
        Action PropagatePropertyChanged { get; set; }
    }
}
