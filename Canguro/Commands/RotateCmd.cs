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
    /// Model Command to Move the selected Item's Joints around a given point and with a given angle.
    /// </summary>
    public class RotateCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Moves the selected Item's Joints around a given point and with a given angle.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Dictionary<Joint, Joint> joints = new Dictionary<Joint, Joint>();
            ItemList<Joint> jList = services.Model.JointList;
            ItemList<LineElement> lList = services.Model.LineList;

            List<Item> selection = services.GetSelection();
            if (selection.Count == 0)
                return;

            foreach (Item item in selection)
            {
                if (item is Joint)
                    joints.Add((Joint)item, null);
                else if (item is LineElement)
                {
                    LineElement l = (LineElement)item;
                    if (!joints.ContainsKey(l.I))
                        joints.Add(l.I, null);
                    if (!joints.ContainsKey(l.J))
                        joints.Add(l.J, null);
                }
            }

            Microsoft.DirectX.Vector3 v, v2;

            float angle = float.Parse(services.GetString(Culture.Get("getRotationAngle")));
            angle *= (float)Math.PI / 180.0F;
            Controller.Snap.Magnet m = services.GetPoint(Culture.Get("getRotationCenter"));
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

                j.X = pos.X;
                j.Y = pos.Y;
                j.Z = pos.Z;
            }
        }
    }
}