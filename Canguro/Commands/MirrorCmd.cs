using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Controller.Snap;
using Canguro.Model;
using Microsoft.DirectX;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to make a copy of the selected items with inverted positions with respect to a mirror plane.
    /// </summary>
    public class MirrorCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Makes a copy of the selected items with inverted positions with respect to a mirror plane, defined by 3 points.
        /// If the points are colinear, the 3rd point is taken to be perpendicular to the view.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Dictionary<uint, Joint> joints = new Dictionary<uint, Joint>();
            List<LineElement> lines = new List<LineElement>();
            List<AreaElement> areas = new List<AreaElement>();
            
            services.GetSelection(joints, lines, areas);

            Dictionary<uint, Joint> jSelection = new Dictionary<uint, Joint>();

            Microsoft.DirectX.Vector3[] pivots = new Microsoft.DirectX.Vector3[3];

            // Get 3 Points
            Magnet m = services.GetPoint(Culture.Get("selectPlainPoints"));
            pivots[0] = m.SnapPosition;

            m = services.GetPoint(Culture.Get("selectPlainPoints"));
            pivots[1] = m.SnapPosition;

                m = services.GetPoint(Culture.Get("selectPlainPoints"));
                pivots[2] = m.SnapPosition;
                Vector3 v1 = pivots[0] - pivots[1];
                Vector3 v2 = pivots[1] - pivots[2];
                if (Vector3.Cross(v1, v2).LengthSq() < 0.0001) // If Colinear, take perpendicular to the active view.
                {
                    Canguro.View.GraphicView view = Canguro.View.GraphicViewManager.Instance.ActiveView;
                    v1 = new Vector3(0, 0, 0);
                    v2 = new Vector3(0, 0, 1);
                    view.Unproject(ref v1);
                    view.Unproject(ref v2);
                    pivots[2] = pivots[2] + v1 - v2;
                }

            ItemList<Joint> jList = services.Model.JointList;
            ItemList<LineElement> lList = services.Model.LineList;
            ItemList<AreaElement> aList = services.Model.AreaList;

            Joint nJoint;
            LineElement nLine;
            AreaElement nArea;
            List<Joint> newJoints = new List<Joint>();
            List<LineElement> newLines = new List<LineElement>();
            List<AreaElement> newAreas = new List<AreaElement>();

            foreach (uint jid in joints.Keys)
            {
                Joint j = jList[jid];
                Vector3 currentPos = new Vector3(j.X, j.Y, j.Z);
                Vector3 newPos = Mirror(currentPos, pivots);
                jList.Add(nJoint = new Joint(newPos.X, newPos.Y, newPos.Z));
                nJoint.Masses = j.Masses;
                nJoint.DoF = j.DoF;
                jSelection.Add(jid, nJoint);
                newJoints.Add(nJoint);
            }
            foreach (LineElement l in lines)
            {
                lList.Add(nLine = new LineElement(l, jSelection[l.I.Id], jSelection[l.J.Id]));
                newLines.Add(nLine);
            }
            foreach (AreaElement a in areas)
            {
                aList.Add(nArea = new AreaElement(a, jSelection[a.J1.Id], jSelection[a.J2.Id], jSelection[a.J3.Id], (a.J4 != null) ? jSelection[a.J4.Id] : null));
                newAreas.Add(nArea);
            }
            JoinCmd.Join(services.Model, newJoints, newLines, newAreas);
        }

        private Vector3 Mirror(Vector3 point, Vector3[] plain)
        {
            Vector3 v1 = plain[0] - plain[1];
            Vector3 v2 = plain[0] - plain[2];
            Vector3 normal = Vector3.Cross(v1, v2);

            float r = Vector3.Dot(plain[0] - point, normal) / normal.LengthSq();
            Vector3 pos = point + Vector3.Scale(normal, r);
            return point + Vector3.Scale(pos - point, 2);
        }
    }
}
