using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Utility
{
    public class ReverseComparer<T> : IComparer<T>
    {
        public int Compare(T object1, T object2)
        {
            return -((IComparable<T>)object1).CompareTo(object2);
        }
    }
}
