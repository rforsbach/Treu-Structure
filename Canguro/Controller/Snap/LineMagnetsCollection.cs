using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Utility;

namespace Canguro.Controller.Snap
{
    class LineMagnetsCollection : ICollection<LineMagnet>
    {
        private LineMagnet[] primaryLines = new LineMagnet[4];
        private LineMagnet currentLine = null;
        private LinkedList<LineMagnet> secondaryLines = new LinkedList<LineMagnet>();
        private LineMagnet[] globalAxes = new LineMagnet[3];
        private Dictionary<float, Magnet> snapSqDistances = new Dictionary<float, Magnet>();

        public const int MaxSecondaryLines = 4;

        public LineMagnetsCollection()
        {
            globalAxes[0] = new LineMagnet(PointMagnet.ZeroMagnet.Position, CommonAxes.GlobalAxes[0], LineMagnetType.FollowXAxis);
            globalAxes[1] = new LineMagnet(PointMagnet.ZeroMagnet.Position, CommonAxes.GlobalAxes[1], LineMagnetType.FollowYAxis);
            globalAxes[2] = new LineMagnet(PointMagnet.ZeroMagnet.Position, CommonAxes.GlobalAxes[2], LineMagnetType.FollowZAxis);
        }

        /// <summary>
        /// Gets the Lines defined by adjacency to the primary point
        /// </summary>
        public LineMagnet[] PrimaryLines
        {
            get { return primaryLines; }
        }

        /// <summary>
        /// Gets the queue of thye most recent Lines hovered by the mouse
        /// </summary>
        public LinkedList<LineMagnet> SecondaryLines
        {
            get { return secondaryLines; }
        }

        public LineMagnet[] GlobalAxes
        {
            get { return globalAxes; }
        }

        public LineMagnet CurrentLine
        {
            get { return currentLine; }
            set
            {
                if (value != null)
                {
                    LinkedListNode<LineMagnet> magnetNode = secondaryLines.Find(value);
                    if (magnetNode != null)
                        secondaryLines.Remove(magnetNode);
                }
                else if (currentLine != null)
                    secondaryLines.AddFirst(currentLine);

                currentLine = value;
            }
        }

        public void RecalcPrimaryDependant(Canguro.View.GraphicView activeView, PointMagnet primaryPoint)
        {
            if (primaryPoint != null)
            {
                resetGlobalAxes(primaryPoint.Position);
            }
        }

        private void resetGlobalAxes(Vector3 position)
        {
            // Move GlobalAxes to lay on the primaryPoint
            globalAxes[0].Position = position;
            globalAxes[1].Position = position;
            globalAxes[2].Position = position;
        }

        public void Snap(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                float snap, snapDelta = 0f;
                snapSqDistances.Clear();
                bool foundCurrentLine = false;

                LineMagnet currLineTmp = currentLine;

                foreach (Magnet m in this)
                {
                    if ((m != null) && (snap = m.Snap(activeView, e.Location)) < SnapController.SnapViewDistance)
                    {
                        if (m == currentLine)
                        {
                            foundCurrentLine = true;
                            
                            // Penalize the snap of Axes LineMagnets to allow other magnets
                            // to appear and become active
                            if (currentLine != null &&
                               (currentLine.Type == LineMagnetType.FollowXAxis ||
                                currentLine.Type == LineMagnetType.FollowYAxis ||
                                currentLine.Type == LineMagnetType.FollowZAxis))
                                snap = Math.Min(snap + 10f, SnapController.SnapViewDistance - SnapController.SnapEpsilon);
                        }
                        snapSqDistances.Add(snap + snapDelta, m);
                        snapDelta += 0.01f;
                    }
                }

                if (!foundCurrentLine)
                    CurrentLine = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
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
                magnet = snapSqDistances[minSnap];

            // Override CurrentLine if it's an axis and another LineMagnet
            // attached to a LineElement si found
            if (magnet != null && currentLine != null &&
               (currentLine.Type == LineMagnetType.FollowXAxis ||
                currentLine.Type == LineMagnetType.FollowYAxis ||
                currentLine.Type == LineMagnetType.FollowZAxis) &&
                ((LineMagnet)magnet).Type != LineMagnetType.FollowXAxis &&
                ((LineMagnet)magnet).Type != LineMagnetType.FollowYAxis &&
                ((LineMagnet)magnet).Type != LineMagnetType.FollowZAxis)
                CurrentLine = (LineMagnet)magnet;

            return minSnap;
        }

        #region ICollection<LineMagnet> Members

        public void Add(LineMagnet item)
        {
            if (item == null) return;

            if (!Contains(item))
            {                
                secondaryLines.AddFirst(item);
                if (secondaryLines.Count > MaxSecondaryLines)
                    secondaryLines.RemoveLast();
            }            
        }

        public void Clear()
        {
            primaryLines = new LineMagnet[4];
            currentLine = null;
            secondaryLines.Clear();
            snapSqDistances.Clear();
        }

        public void Reset()
        {
            resetGlobalAxes(PointMagnet.ZeroMagnet.Position);
            Clear();
        }

        public bool Contains(LineMagnet item)
        {
            foreach (LineMagnet lm in primaryLines)
                if (lm != null && lm.Equals(item)) return true;

            if (item != null && item.Equals(currentLine)) return true;
            
            foreach (LineMagnet lm in globalAxes)
                if (lm.Equals(item)) return true;

            return secondaryLines.Contains(item);
        }

        public void CopyTo(LineMagnet[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count) throw new ArgumentException();

            foreach (LineMagnet lm in primaryLines)
                array[arrayIndex++] = lm;

            array[arrayIndex++] = currentLine;
            
            secondaryLines.CopyTo(array, arrayIndex);
            
            foreach (LineMagnet lm in globalAxes)
                array[arrayIndex + secondaryLines.Count] = lm;
        }

        public int Count
        {
            get { return secondaryLines.Count + 8; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(LineMagnet item)
        {
            return secondaryLines.Remove(item);
        }

        #endregion

        #region IEnumerable<LineMagnet> Members

        IEnumerator<LineMagnet> IEnumerable<LineMagnet>.GetEnumerator()
        {
            return new LineMagnetListEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new LineMagnetListEnumerator(this);
        }

        #endregion

        public class LineMagnetListEnumerator : System.Collections.IEnumerator, IEnumerator<LineMagnet>
        {
            private LineMagnetsCollection collection;
            private int index = -1;
            private IEnumerator<LineMagnet> secondaryEnumerator;

            public LineMagnetListEnumerator(LineMagnetsCollection collection)
            {
                this.collection = collection;
                secondaryEnumerator = collection.SecondaryLines.GetEnumerator();
            }

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (index <= -1)
                        throw new InvalidOperationException();
                    switch (index)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            return collection.primaryLines[index];
                        case 4:
                            return collection.currentLine;
                        case 5:
                            return secondaryEnumerator.Current;
                        case 6:
                        case 7:
                        case 8:
                            return collection.globalAxes[index - 6];
                        default:
                            return null;
                    }
                }
            }

            public bool MoveNext()
            {
                switch (index)
                {
                    case -1:
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        index++;
                        return true;
                    case 4:
                        index++;
                        if (!secondaryEnumerator.MoveNext())
                            index++;
                        return true;
                    case 5:
                        if (!secondaryEnumerator.MoveNext())
                            index++;
                        return true;
                    case 6:
                    case 7:
                    case 8:
                        index++;
                        return true;
                    default:
                        index = 0;
                        return false;
                }
            }

            public void Reset()
            {
                index = -1;
                secondaryEnumerator.Reset();
            }

            #endregion

            #region IEnumerator<PointMagnet> Members

            LineMagnet IEnumerator<LineMagnet>.Current
            {
                get
                {
                    if (index <= -1)
                        throw new InvalidOperationException();
                    switch (index)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            return collection.primaryLines[index];
                        case 4:
                            return collection.currentLine;
                        case 5:
                            return secondaryEnumerator.Current;
                        case 6:
                        case 7:
                        case 8:
                            return collection.globalAxes[index-6];
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
