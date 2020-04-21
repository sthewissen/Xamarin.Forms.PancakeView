using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Xamarin.Forms.PancakeView
{
    public class GradientStopCollection : ObservableCollection<GradientStop>
    {
        protected override void InsertItem(int index, GradientStop item) => base.InsertItem(index, item ?? throw new ArgumentNullException(nameof(item)));
        protected override void SetItem(int index, GradientStop item) => base.SetItem(index, item ?? throw new ArgumentNullException(nameof(item)));

        protected override void ClearItems()
        {
            var removed = new List<GradientStop>(this);
            base.ClearItems();
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
        }
    }
}