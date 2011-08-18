using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.View;
using Canguro.Utility;

namespace Canguro.Controller.Snap
{
    class SnapController
    {
        #region Constants
        public const float SnapViewDistance = 400f;
        public const float EffectiveSnapDistance = 81f;
        public const float SnapEpsilon = 0.001f;
        public const float NoJointMagnetPenalty = 9;
        #endregion

        #region Object State
        /// <summary>
        /// The Snap Painter
        /// </summary>
        private SnapPainter painter;
        private bool isActive = false;
        private PointMagnetsCollection points;
        private LineMagnetsCollection lines;
        private AreaMagnet area;
        private List<Magnet> paintMagnets;
        private Magnet snapMagnet;
        #endregion

        public SnapController()
        {
            paintMagnets = new List<Magnet>();
            snapMagnet = null;

            points = new PointMagnetsCollection();
            lines = new LineMagnetsCollection();
            area = new AreaMagnet(PointMagnet.ZeroMagnet.Position, CommonAxes.GlobalAxes[2]);

            painter = new SnapPainter();
        }

        #region Properties
        /// <summary>
        /// Gets or sets whether Snapping is active or not
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>
        /// Gets the only AreaMagnet active at any time
        /// </summary>
        public AreaMagnet Area
        {
            get { return area; }
        }
        
        /// <summary>
        /// Gets the best Snap Magnet based on the last MouseMove event
        /// </summary>
        public Magnet SnapMagnet
        {
            get { return snapMagnet; }
        }

        /// <summary>
        /// Gets or sets the primary point used for Snap
        /// </summary>
        public PointMagnet PrimaryPoint
        {
            get { return points.PrimaryPoint; }
            set { points.PrimaryPoint = value; }
        }
        #endregion

        private void pickItems(System.Windows.Forms.MouseEventArgs e)
        {
            //////////////////////////////////////////////////
            List<Item> pickedItems = Canguro.View.GraphicViewManager.Instance.PickItem(e.X, e.Y);
            if (pickedItems == null) return;

            foreach (Item item in pickedItems)
            {
                // Get secondary points
                if (item is Joint)
                    points.Add(new PointMagnet((Joint)item));

                // Get secondary lines
                if (item is LineElement)
                    lines.Add(new LineMagnet((LineElement)item));
            }
        }

        private void recalcPrimaryDependant(GraphicView activeView)
        {
            if (points.NeedRecalcPrimaryPointDependant)
            {
                lines.RecalcPrimaryDependant(activeView, points.PrimaryPoint);
                area.RecalcPrimaryDependant(activeView, points.PrimaryPoint, lines.GlobalAxes);
                points.RecalcPrimaryDependant(activeView);
            }
        }

        /// <summary>
        /// Method to try to create a midpoint magnet if the mouse is close to one
        /// </summary>
        /// <param name="lm">The Line Magnet being followed</param>
        /// <param name="activeView">The view in which the snap is taking place</param>
        /// <param name="e">The MouseEventArgs of the last MouseMove event</param>
        /// <returns>The MidPoint magnet or null if none found</returns>
        private PointMagnet createMidPoint(LineMagnet lm, GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            if (lm == null) return null;

            LineElement l;
            if ((l = lm.Line) != null)
            {
                PointMagnet midPtMagnet = new PointMagnet(Vector3.Scale(l.I.Position + l.J.Position, 0.5f), 
                    PointMagnetType.MidPoint);
                if (midPtMagnet.Snap(activeView, e.Location) < SnapViewDistance)
                {
                    midPtMagnet.RelatedMagnets.Add(lm);
                    points.Add(midPtMagnet);
                    return midPtMagnet;
                }
            }

            return null;
        }

        /// <summary>
        /// Method to add a perpendicular point magnet if the mouse is close to where it is
        /// </summary>
        private PointMagnet addPerpendicularMagnet(LineMagnet lm, PointMagnet pm, float lmDot, GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            float r = Vector3.Dot(lm.Direction, pm.Position - lm.Position) / lmDot;
            PointMagnet perpPtMagnet = new PointMagnet(lm.Position + Vector3.Scale(lm.Direction, r), 
                PointMagnetType.Perpendicular);
            if (perpPtMagnet.Snap(activeView, e.Location) < SnapViewDistance)
            {
                perpPtMagnet.RelatedMagnets.Add(pm);
                perpPtMagnet.RelatedMagnets.Add(lm);
                points.Add(perpPtMagnet);
                return perpPtMagnet;
            }

            return null;
        }

        /// <summary>
        /// Returns a perpendicular point magnet P if there's a point X and a line L which can be united
        /// by a perpendicular line:
        /// 
        ///                      L  
        ///                      |
        ///                      |
        ///    X - - - - - - - - P
        ///                      |
        ///                      |
        /// 
        /// </summary>
        /// <param name="lm">The Line Magnet being followed</param>
        /// <param name="activeView">The view in which the snap is taking place</param>
        /// <param name="e">The MouseEventArgs of the last MouseMove event</param>
        /// <returns>The perpendicular point magnet or null if none found</returns>
        private PointMagnet createPerpendicularMagnet(LineMagnet lm, GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            if (lm == null) return null;
            float lmDot = Vector3.Dot(lm.Direction, lm.Direction);

            if (points.PrimaryPoint != null)
            {
                PointMagnet perp = addPerpendicularMagnet(lm, points.PrimaryPoint, lmDot, activeView, e);
                if (perp != null) return perp;
            }
            if (points.LastPoint != null)
                return addPerpendicularMagnet(lm, points.LastPoint, lmDot, activeView, e);

            return null;
        }

        /// <summary>
        /// Gets an interesting point by following the line in reverse (as a mirror) and trying
        /// to find Joints at the same distance from the LineMagnet's position
        /// </summary>
        /// <param name="lm">The Line Magnet being followed</param>
        /// <param name="activeView">The view in which the snap is taking place</param>
        /// <param name="e">The MouseEventArgs of the last MouseMove event</param>
        /// <returns>A point magnet with the interesting point or null if none found</returns>
        private PointMagnet createInterestingDistance(LineMagnet lm, GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            if (lm == null) return null;

            Vector3 pos = lm.Position;
            activeView.Project(ref pos);
            Vector3 reversePickPos = pos + pos - new Vector3(e.X, e.Y, 0f);

            List<Item> pickedItems = Canguro.View.GraphicViewManager.Instance.PickItem((int)reversePickPos.X, (int)reversePickPos.Y);
            if (pickedItems == null) return null;

            foreach (Item item in pickedItems)
            {
                Joint j;
                // Check if Joint si over line lm
                if ((j = item as Joint) != null)
                {
                    Vector3 ptInLine = Vector3.Cross(lm.Direction, j.Position - lm.Position);
                    if (Vector3.Dot(ptInLine, ptInLine) < float.Epsilon)
                    {
                        PointMagnet distMagnet = new PointMagnet(lm.Position + lm.Position - j.Position, PointMagnetType.SimplePoint);
                        distMagnet.RelatedMagnets.Add(new PointMagnet(j));
                        points.Add(distMagnet);
                        return distMagnet;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Calculate the intersection point of two coplanar lines
        /// Source: http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
        /// </summary>
        /// <param name="l1">First LineMagnet</param>
        /// <param name="l2">Second LineMagnet</param>
        /// <returns>The intersection point magnet or null if none found</returns>
        private PointMagnet createIntersection(LineMagnet l1, LineMagnet l2, GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            if (l1 != null && l2 != null)
            {
                float numer, denom;
                float d1, d2, d3, d4, d5;
                Vector3 p13 = l1.Position - l2.Position;
                Vector3 p21 = l1.Direction;
                Vector3 p43 = l2.Direction;

                d1 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
                d2 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
                d3 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
                d4 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
                d5 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

                denom = d5 * d4 - d2 * d2;
                if (Math.Abs(denom) < float.Epsilon)
                    return null;
                numer = d1 * d2 - d3 * d4;

                float r = numer / denom;
                float s = (d1 + d2 * r) / d4;

                Vector3 pa = l1.Position + Vector3.Scale(p21, r);
                Vector3 pb = l2.Position + Vector3.Scale(p43, s);

                if ((pa - pb).Length() > 0.0001)
                    return null;

                // Create magnet
                PointMagnet intPtMagnet = new PointMagnet(pa,
                    PointMagnetType.Intersection);

                if (intPtMagnet.Snap(activeView, e.Location) < SnapViewDistance)
                {
                    intPtMagnet.RelatedMagnets.Add(l1);
                    intPtMagnet.RelatedMagnets.Add(l2);
                    return intPtMagnet;
                }
            }

            return null;
        }

        /// <summary>
        /// Method to perform calculations based on the user interaction by
        /// moving the mouse
        /// </summary>
        /// <param name="sender">The object calling this method</param>
        /// <param name="e">The Mouse event args</param>
        /// <returns>A boolean indicating whether a repaint is necessary</returns>
        public bool MouseMove(GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            bool ret = true;

            paintMagnets.Clear();
            pickItems(e);
            recalcPrimaryDependant(activeView);

            // Check snap to acquired magnets
            points.Snap(activeView, e);
            lines.Snap(activeView, e);

            // Get max snap Magnet (the one closer to the mouse position)
            Magnet bestMagnet;
            float snap, bestSnap, bestPointSnap = points.GetBestSnap(out bestMagnet);
            if (bestMagnet == null)
                bestSnap = lines.GetBestSnap(out bestMagnet);
            else
                bestSnap = bestPointSnap;

            // Update CurrentLine's positions if mouse moved over any of it's joints (if LineMagnet has a Line)
            if (lines.CurrentLine != null && bestMagnet != null && bestMagnet is PointMagnet && 
                lines.CurrentLine.Line != null && ((PointMagnet)bestMagnet).Joint != null)
            {
                Joint newJoint = ((PointMagnet)bestMagnet).Joint;

                if (newJoint == lines.CurrentLine.Line.I || newJoint == lines.CurrentLine.Line.J)
                    lines.CurrentLine.Position = newJoint.Position;
            }

            #region Intersection
            // Check intersections between LineMagnets. If any, create corresponding Magnet
            if (lines.CurrentLine != null && bestMagnet is LineMagnet && bestMagnet != lines.CurrentLine)
            {
                PointMagnet intersectionMagnet = createIntersection(lines.CurrentLine, bestMagnet as LineMagnet, activeView, e);
                if (intersectionMagnet != null)
                {
                    snap = intersectionMagnet.Snap(activeView, e.Location) + SnapEpsilon;
                    if (snap < SnapViewDistance)
                    {
                        // Paint helper line
                        paintMagnets.Add(bestMagnet);

                        // Paint intersection
                        paintMagnets.Add(intersectionMagnet);
                    }

                    if (snap < bestPointSnap && snap < EffectiveSnapDistance)
                    {
                        bestSnap = snap;
                        bestMagnet = intersectionMagnet;
                    }
                }
            }
            #endregion

            #region Midpoint
            // Create midpoint magnet (if mouse is close to a Line)
            foreach (Magnet m in lines)
            {
                PointMagnet midMagnet = createMidPoint(m as LineMagnet, activeView, e);
                if (midMagnet != null)
                {
                    snap = midMagnet.Snap(activeView, e.Location) + SnapEpsilon;
                    if (snap < bestPointSnap && snap < EffectiveSnapDistance)
                    {
                        bestSnap = snap;
                        bestMagnet = midMagnet;
                    }
                }
            }
            #endregion

            #region Perpendicular point
            // Create perpendicular magnets (if mouse is close to a line)
            PointMagnet perpMagnet = createPerpendicularMagnet(bestMagnet as LineMagnet, activeView, e);
            if (perpMagnet != null)
            {
                snap = perpMagnet.Snap(activeView, e.Location) + SnapEpsilon;
                if (snap < (bestPointSnap - SnapEpsilon) && snap < EffectiveSnapDistance)
                {
                    bestSnap = snap;
                    bestMagnet = perpMagnet;
                }
            }
            #endregion

            #region Interesting Distance
            // If following a LineMagnet, then set next interesting distance as a PointMagnet
            PointMagnet distMagnet = createInterestingDistance(bestMagnet as LineMagnet, activeView, e);
            if (distMagnet != null)
            {
                snap = distMagnet.Snap(activeView, e.Location) + SnapEpsilon;
                if (snap < bestPointSnap)
                {
                    bestSnap = snap;
                    bestMagnet = distMagnet;
                }
            }
            #endregion

            // Choose magnets to paint
            if ((bestMagnet is LineMagnet) && (lines.CurrentLine == null))
                lines.CurrentLine = (LineMagnet)bestMagnet;
            else if ((bestMagnet != null) && !bestMagnet.Equals(lines.CurrentLine))
                paintMagnets.Add(bestMagnet);            
            
            // Set the Magnet to snap to if the user clicks the mouse
            snapMagnet = null;
            if (bestMagnet != null)
            {
                if (bestMagnet is PointMagnet && bestMagnet.LastSnapFitness < EffectiveSnapDistance)
                    snapMagnet = bestMagnet;
                else if (lines.CurrentLine != null)
                    snapMagnet = lines.CurrentLine;
                else if (bestMagnet is LineMagnet)
                    snapMagnet = bestMagnet;
            }
            else
            {
                area.Snap(activeView, e.Location);
                snapMagnet = (Magnet)area.Clone();
            }

            // TODO: Probably change the IsActive property to some enum, so as to activate snapping to
            // specific objects only (i.e. Joint snapping when CommandServices.GetJoint() is working)
            return ret;
        }

        /// <summary>
        /// This method paints all the gadgets added by the Snaps
        /// (i.e. 
        /// </summary>
        public void Paint(Device device, GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            for (int i = 0; i < paintMagnets.Count; i++)
            {
                Magnet m = paintMagnets[i];
                if (m != null)
                {
                    if (m is PointMagnet)
                    {
                        byte alpha = 255;
                        if (m.LastSnapFitness >= EffectiveSnapDistance)
                            alpha = (byte)((1f - (m.LastSnapFitness - EffectiveSnapDistance) /
                                           (SnapViewDistance - EffectiveSnapDistance)) * 128f + 64f);

                        switch (((PointMagnet)m).Type)
                        {
                            case PointMagnetType.Perpendicular:
                                if (alpha == 255 && m.RelatedMagnets.Count > 0 && m.RelatedMagnets[0] is PointMagnet)
                                    painter.PaintLineSegment(device, activeView, m.Position, m.RelatedMagnets[0].Position, LineMagnetType.FollowHelper);
                                break;
                            case PointMagnetType.SimplePoint:
                                if (alpha == 255 && m.RelatedMagnets.Count > 0 && m.RelatedMagnets[0] is PointMagnet)
                                    painter.PaintPointSymbol(device, activeView, m.RelatedMagnets[0].Position, PointMagnetType.SimplePoint, 192);
                                break;
                        }

                        painter.PaintPointSymbol(device, activeView, m.Position, ((PointMagnet)m).Type, alpha);
                    }
                    else if (m is LineMagnet)
                    {
                        LineMagnetType lmt = ((LineMagnet)m).Type;
                        if (lmt == LineMagnetType.FollowProjection && lines.CurrentLine != null)
                            lmt = LineMagnetType.FollowHelper;

                        painter.PaintLineSegment(device, activeView, m.Position, m.SnapPositionInt, lmt);
                    }
                }
            }
            
            if (lines.CurrentLine != null)
                painter.PaintLineSegment(device, activeView, lines.CurrentLine.Position, lines.CurrentLine.SnapPositionInt, lines.CurrentLine.Type);            
        }

        public void Reset(GraphicView activeView)
        {
            points.Clear();
            lines.Clear();
        }

        public void ResetStatus(GraphicView activeView)
        {
            points.Clear();
            lines.Reset();
        }
    }
}
