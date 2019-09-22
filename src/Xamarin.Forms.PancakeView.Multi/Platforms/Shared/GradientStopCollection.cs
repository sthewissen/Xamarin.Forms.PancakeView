using System;
using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Forms.PancakeView
{
    public class GradientStopCollection : IList<GradientStop>
    {
        readonly IList<GradientStop> _internalList;

        public int Count => _internalList.Count;

        public bool IsReadOnly => false;

        public GradientStop this[int index]
        {
            get => _internalList[index];
            set => _internalList[index] = value;
        }

        public GradientStopCollection()
        {
            _internalList = new List<GradientStop>();
        }

        public int IndexOf(GradientStop item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, GradientStop item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _internalList.RemoveAt(index);
        }

        public void Add(GradientStop item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _internalList.Add(item);
        }

        public void Clear()
        {
            _internalList.Clear();
        }

        public bool Contains(GradientStop item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(GradientStop[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(GradientStop item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return _internalList.Remove(item);
        }

        public IEnumerator<GradientStop> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_internalList).GetEnumerator();
        }
    }
}