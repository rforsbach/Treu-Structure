using System;
using System.Collections.Generic;

namespace Canguro.Utility
{
    public class DistancedItem
    {
        public struct dItem
        {
            public float distance;
            public Canguro.Model.Item item;

            public dItem(float distance, Canguro.Model.Item item)
            {
                this.distance = distance;
                this.item = item;
            }
        }

        public class Comparer : IComparer<dItem>
        {
            private Comparer() { }
            public static readonly Comparer Instance = new Comparer();

            public int Compare(dItem a, dItem b)
            {
                if (a.distance < b.distance)
                    return -1;
                else if (a.distance > b.distance)
                    return 1;
                else
                {
                    if (((a.item is Canguro.Model.Joint) && !(b.item is Canguro.Model.Joint)) ||
                        ((a.item is Canguro.Model.LineElement) && !(b.item is Canguro.Model.Joint) && !(b.item is Canguro.Model.LineElement)))
                        return -1;
                    else if (((b.item is Canguro.Model.Joint) && !(a.item is Canguro.Model.Joint)) ||
                        ((b.item is Canguro.Model.LineElement) && !(a.item is Canguro.Model.Joint) && !(a.item is Canguro.Model.LineElement)))
                        return 1;

                    return 0;
                }
            }
        }

        public class ReverseComparer : IComparer<dItem>
        {
            private ReverseComparer() { }
            public static readonly ReverseComparer Instance = new ReverseComparer();

            public int Compare(dItem a, dItem b)
            {
                return -Comparer.Instance.Compare(a, b);
            }
        }
    }
}
