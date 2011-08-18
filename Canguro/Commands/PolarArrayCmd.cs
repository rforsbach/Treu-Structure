using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Controller.Snap;
using Canguro.Controller.Tracking;
using Microsoft.DirectX;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to make copies of the selected items around a given rotation axis.
    /// </summary>
    public class PolarArrayCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Copies the selected Items in a series around a given rotation axis.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Dictionary<Joint, Joint> joints = new Dictionary<Joint, Joint>();
            Dictionary<Joint, Joint> jSelection = new Dictionary<Joint, Joint>();
            List<LineElement> lines = new List<LineElement>();
            List<LineElement> lSelection = new List<LineElement>();
            List<AreaElement> areas = new List<AreaElement>();
            List<AreaElement> aSelection = new List<AreaElement>();
            ItemList<Joint> jList = services.Model.JointList;
            ItemList<LineElement> lList = services.Model.LineList;
            ItemList<AreaElement> aList = services.Model.AreaList;
            Joint nJoint;
            LineElement nLine;
            AreaElement nArea;

            List<Item> selection = services.GetSelection();
            if (selection.Count == 0)
                return;

            foreach (Item item in selection)
            {
                if (item is Joint)
                {
                    jSelection.Add((Joint)item, null);
                    joints.Add((Joint)item, null);
                }
                else if (item is LineElement)
                {
                    LineElement l = (LineElement)item;
                    lSelection.Add(l);
                    lines.Add(l);
                    if (!jSelection.ContainsKey(l.I))
                    {
                        jSelection.Add(l.I, null);
                        joints.Add(l.I, null);
                    }
                    if (!jSelection.ContainsKey(l.J))
                    {
                        jSelection.Add(l.J, null);
                        joints.Add(l.J, null);
                    }
                }
                else if (item is AreaElement)
                {
                    AreaElement a = (AreaElement)item;
                    aSelection.Add(a);
                    areas.Add(a);
                    if (!jSelection.ContainsKey(a.J1))
                    {
                        jSelection.Add(a.J1, null);
                        joints.Add(a.J1, null);
                    }
                    if (!jSelection.ContainsKey(a.J2))
                    {
                        jSelection.Add(a.J2, null);
                        joints.Add(a.J2, null);
                    }
                    if (!jSelection.ContainsKey(a.J3))
                    {
                        jSelection.Add(a.J3, null);
                        joints.Add(a.J3, null);
                    }
                    if (a.J4 != null && !jSelection.ContainsKey(a.J4))
                    {
                        jSelection.Add(a.J4, null);
                        joints.Add(a.J4, null);
                    }
                }
            }

            Microsoft.DirectX.Vector3 v, v2;
            uint n = (uint)services.GetSingle(Culture.Get("getArrayRepetition"));

            float dAngle = float.Parse(services.GetString(Culture.Get("getPolarArrayAngle")));
            dAngle *= (float)Math.PI / 180.0F;
            float angle = 0.0F;

            Controller.Snap.Magnet m = services.GetPoint(Culture.Get("getPolarRotationCenter"));
            if (m == null) return;
            v = m.SnapPosition;

            services.TrackingService = LineTrackingService.Instance;
            services.TrackingService.SetPoint(m.SnapPositionInt);

            m = services.GetPoint(Culture.Get("getPolarRotationCenter"));
            if (m == null) return;
            v2 = m.SnapPosition;
            if (v2.Equals(v))
            {
                Canguro.View.GraphicView view = Canguro.View.GraphicViewManager.Instance.ActiveView;
                Vector3 v1Tmp = new Vector3(0, 0, 0);
                Vector3 v2Tmp = new Vector3(0, 0, 1);
                view.Unproject(ref v1Tmp);
                view.Unproject(ref v2Tmp);
                v2 = v2 + v1Tmp - v2Tmp;
            }
            services.TrackingService = null;

            List<Joint> newJoints = new List<Joint>();
            List<LineElement> newLines = new List<LineElement>();
            List<AreaElement> newAreas = new List<AreaElement>();


            for (int i = 1; i <= n; i++)
            {
                angle += dAngle;

                Matrix trans1 = new Matrix();
                trans1.Translate(-v);
                Matrix rot = new Matrix();
                rot.RotateAxis(v2 - v, angle);
                Matrix trans2 = new Matrix();
                trans2.Translate(v);
                rot = trans1 * rot * trans2;

                foreach (Joint j in joints.Keys)
                {
                    Vector3 pos = new Vector3(j.X, j.Y, j.Z);
                    pos.TransformCoordinate(rot);

                    jList.Add(nJoint = new Joint(pos.X, pos.Y, pos.Z));
                    nJoint.Masses = j.Masses;
                    nJoint.DoF = j.DoF;
                    jSelection[j] = nJoint;
                    newJoints.Add(nJoint);
                }
                foreach (LineElement l in lines)
                {
                    lList.Add(nLine = new LineElement(l, jSelection[l.I], jSelection[l.J]));
                    newLines.Add(nLine);
                }
                foreach (AreaElement a in areas)
                {
                    aList.Add(nArea = new AreaElement(a, jSelection[a.J1], jSelection[a.J2], jSelection[a.J3], (a.J4 != null) ? jSelection[a.J4] : null));
                    newAreas.Add(nArea);
                }
            }
            JoinCmd.Join(services.Model, newJoints, newLines, newAreas);
        }
    }
}

