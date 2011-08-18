using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Controller.Snap
{
    public class PointMagnetsCollection : ICollection<PointMagnet>
    {
        private PointMagnet primaryPt = null;
        private PointMagnet lastPt = null;
        private LinkedList<PointMagnet> secondaryPts = new LinkedList<PointMagnet>();
        public readonly PointMagnet ZeroPt = PointMagnet.ZeroMagnet;
        private bool needRecalcPrimaryPointDependant = false;
        private Dictionary<float, List<Magnet>> snapSqDistances = new Dictionary<float, List<Magnet>>();

        public const int MaxSecondaryPoints = 4;

        /// <summary>
        /// Gets or sets the Primary Point, defined by selection or fixed by the command
        /// </summary>
        public PointMagnet PrimaryPoint
        {
            get { return primaryPt; }
            set
            {
                if (value != primaryPt)
                {
                    primaryPt = value;
                    needRecalcPrimaryPointDependant = true;
                }
            }
        }

        public PointMagnet LastPoint
        {
            get { return lastPt; }
            set { lastPt = value; }
        }

        /// <summary>
        /// Gets the queue of most recently Points hovered by the mouse
        /// </summary>
        public LinkedList<PointMagnet> SecondaryPoints
        {
          get { return secondaryPts; }
        }

        public bool NeedRecalcPrimaryPointDependant
        {
            get { return (primaryPt != null) && needRecalcPrimaryPointDependant; }
        }

        public void RecalcPrimaryDependant(Canguro.View.GraphicView activeView)
        {
            needRecalcPrimaryPointDependant = false;
        }

        public void Snap(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            float snap;
            snapSqDistances.Clear();
            foreach (Magnet m in this)
            {
                if ((m != null) && (snap = m.Snap(activeView, e.Location)) < SnapController.SnapViewDistance)
                {
                    if ((m is PointMagnet) && ((PointMagnet)m).Joint == null)
                        snap += SnapController.NoJointMagnetPenalty;

                    if (!snapSqDistances.ContainsKey(snap))
                        snapSqDistances.Add(snap, new List<Magnet>());

                    snapSqDistances[snap].Add(m);
                }
            }
        }

        public float GetBestSnap(out Magnet magnet)
        {
            float minSnap = SnapController.SnapViewDistance;
            magnet = null;
            foreach (float key in snapSqDistances.Keys)
                if (key < minSnap)
                    minSnap = key;

            if (minSnap < SnapController.SnapViewDistance)
            {
                foreach (Magnet m in snapSqDistances[minSnap])
                {
                    if (magnet == null)
                        magnet = m;
                    else if (((PointMagnet)m).Type == PointMagnetType.EndPoint)
                        magnet = m;

                    if (((PointMagnet)m).Type == PointMagnetType.EndPoint)
                        return minSnap;
                }
            }

            return minSnap;
        }

        #region ICollection<PointMagnet> Members

        public void Add(PointMagnet item)
        {
            if ((item == null) || item.Equals(primaryPt) || item.Equals(ZeroPt)) return;            
            lastPt = item;
            
            if (!secondaryPts.Contains(item))
            {
                secondaryPts.AddFirst(item);
                if (secondaryPts.Count > MaxSecondaryPoints)
                    secondaryPts.RemoveLast();
            }
        }

        public void Clear()
        {
            lastPt = null;
            secondaryPts.Clear();
            snapSqDistances.Clear();
        }

        public bool Contains(PointMagnet item)
        {
            if (primaryPt.Equals(item)) return true;
            if (ZeroPt.Equals(item)) return true;

            return secondaryPts.Contains(item);
        }

        public void CopyTo(PointMagnet[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count) throw new ArgumentException();

            array[arrayIndex] = primaryPt;
            secondaryPts.CopyTo(array, arrayIndex + 1);
            array[arrayIndex + secondaryPts.Count + 1] = ZeroPt;
        }

        public int Count
        {
            get { return secondaryPts.Count + 2; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(PointMagnet item)
        {
            return secondaryPts.Remove(item);
        }
        #endregion

        #region IEnumerable<PointMagnet> Members

        IEnumerator<PointMagnet> IEnumerable<PointMagnet>.GetEnumerator()
        {
            return new PointMagnetListEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new PointMagnetListEnumerator(this);
        }

        #endregion

        public class PointMagnetListEnumerator : System.Collections.IEnumerator, IEnumerator<PointMagnet>
        {
            private PointMagnetsCollection collection;
            private int index = 0;
            private IEnumerator<PointMagnet> secondaryEnumerator;

            public PointMagnetListEnumerator(PointMagnetsCollection collection)
            {
                this.collection = collection;
                secondaryEnumerator = collection.SecondaryPoints.GetEnumerator();
            }

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get 
                {
                    if (index <= 0)
                        throw new InvalidOperationException();
                    switch (index)
                    {
                        case 1:
                            return collection.primaryPt;
                        case 2:
                            return secondaryEnumerator.Current;
                        case 3:
                            return collection.ZeroPt;
                        default:
                            return null;
                    }
                }
            }

            public bool MoveNext()
            {
                switch (index)
                {
                    case 0:
                        index++;
                        return true;
                    case 1:
                        index++;
                        if (!secondaryEnumerator.MoveNext())
                            index++;
                        return true;
                    case 2:
                        if (!secondaryEnumerator.MoveNext())
                            index++;
                        return true;
                    case 3:
                        index++;
                        return true;
                    default:
                        index = 0;
                        return false;
                }
            }

            public void Reset()
            {
                index = 0;
                secondaryEnumerator.Reset();
            }

            #endregion

            #region IEnumerator<PointMagnet> Members

            PointMagnet IEnumerator<PointMagnet>.Current
            {
                get
                {
                    if (index <= 0)
                        throw new InvalidOperationException();
                    switch (index)
                    {
                        case 1:
                            return collection.primaryPt;
                        case 2:
                            return secondaryEnumerator.Current;
                        case 3:
                            return collection.ZeroPt;
                        default:
                            return null;
                    }
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                collection = null;
                secondaryEnumerator.Dispose();
            }

            #endregion
        }   
    }
}
