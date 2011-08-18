using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Controller.Snap;
using Canguro.Model;
using Microsoft.DirectX;

namespace Canguro.Commands.Model
{
    public class ArcCircularCmd : ModelCommand
    {
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
            try {
                Microsoft.DirectX.Vector3[] pivots = new Microsoft.DirectX.Vector3[3];

                // Get 3 Points

                Joint j1 = services.GetJoint((IList<LineElement>)null);
                services.TrackingService = Canguro.Controller.Tracking.LineTrackingService.Instance;
                services.TrackingService.SetPoint(j1.Position);
                services.Model.ChangeModel();

                Joint j2 = services.GetJoint((IList<LineElement>)null);
                services.TrackingService.SetPoint(j2.Position);
                services.Model.ChangeModel();
                Joint j3 = services.GetJoint((IList<LineElement>)null);
                services.TrackingService = null;
                services.Model.ChangeModel();

                pivots[0] = j1.Position;
                pivots[1] = j2.Position;
                pivots[2] = j3.Position;

                Vector3 v1 = pivots[0] - pivots[1];
                Vector3 v2 = pivots[1] - pivots[2];
                Vector3 N = Vector3.Cross(v1, v2);

                if (N.LengthSq() < 0.0001) // If Colinear, take perpendicular to the active view.
                    System.Windows.Forms.MessageBox.Show(Culture.Get("ColinearPoints"), Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                else {
                    int segments = (int)services.GetSingle(Culture.Get("getSplitParts") + " [2-100]");
                    Vector3 C = calcCenter(pivots[0], pivots[1], pivots[2]);
                    MakeArc(services.Model, C, N, j1, j3, j2, segments);
                }
            } finally {
                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;
            }
        }

        private void MakeArc(Canguro.Model.Model model, Vector3 C, Vector3 N, Joint from, Joint until, Joint passing, int segments)
        {
            Vector3 p0 = from.Position;
            Vector3 p1 = until.Position;
            Vector3 p2 = passing.Position;
            Vector3 a = Vector3.Normalize(C - p0);
            Vector3 b = Vector3.Normalize(C - p1);
            Vector3 c = Vector3.Normalize(C - p2);
            N.Normalize();
            float ang = (float)Math.Acos(Vector3.Dot(a, b));
            float p2Ang = (float)Math.Acos(Vector3.Dot(a, c));

            ang = (Vector3.Dot(Vector3.Cross(a, N), b) > 0) ? 2f * (float)Math.PI-ang : ang;
            p2Ang = (Vector3.Dot(Vector3.Cross(a, N), c) > 0) ? 2f * (float)Math.PI - p2Ang : p2Ang;


            List<Joint> joints = new List<Joint>();
            joints.Add(from);
            float angle = 0;
            ang /= segments;
            
            for (int i = 0; i < segments - 1; i++)
            {
                angle += ang;

                Matrix trans1 = new Matrix();
                trans1.Translate(-C);
                Matrix rot = new Matrix();
                rot.RotateAxis(N, angle);
                Matrix trans2 = new Matrix();
                trans2.Translate(C);
                rot = trans1 * rot * trans2;
                Vector3 pos = from.Position;
                pos.TransformCoordinate(rot);
                if (Math.Abs(angle) > Math.Abs(p2Ang) && Math.Abs(angle) < Math.Abs(p2Ang + ang))
                    joints.Add(passing);
                Joint joint = new Joint(pos.X, pos.Y, pos.Z);
                joints.Add(joint);
                model.JointList.Add(joint);
            }
            joints.Add(until);
            StraightFrameProps props = new StraightFrameProps();
            for (int i = 1; i < joints.Count; i++)
            {
                model.LineList.Add(new LineElement(props, joints[i - 1], joints[i]));
            }
        }



        private static Vector3 calcCenter(Vector3 pa, Vector3 pb, Vector3 pc)
        {
            Vector3 u =  Vector3.Add(pb, - pa);
            Vector3 v =  Vector3.Add(pc, - pa);
            Vector3 N = Vector3.Normalize( Vector3.Cross(u, v));

            
          //  float distance = -( N.X * puntoa.X + N.Y * puntoa.Y + N.Z * puntoa.Z);

            Vector3 alpha = Vector3.Normalize(Vector3.Cross(N, u));
            Vector3 beta = Vector3.Normalize(Vector3.Cross(N, v));


            Vector3 alphaInicial = pa + Vector3.Scale(u, (float)0.5f);
            Vector3 betaInicial = pa + Vector3.Scale(v, (float)0.5f);

            return Intersect(alphaInicial, alphaInicial + alpha, betaInicial, betaInicial + beta);
        }

        public static Vector3 Intersect(Vector3 alphaI, Vector3 alphaJ, Vector3 betaI, Vector3 betaJ) 
        {
             //LineElement l1, LineElement l2
            
               float numer, denom;
                float d1, d2, d3, d4, d5;
                Vector3 p13 = alphaI - betaI;
                Vector3 p21 = alphaJ - alphaI;
                Vector3 p43 = betaJ - betaI;

                d1 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
                d2 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
                d3 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
                d4 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
                d5 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

                denom = d5 * d4 - d2 * d2;
                if (Math.Abs(denom) < 0.0001)
                    return new Vector3(0,0,0);
                numer = d1 * d2 - d3 * d4;

                float r = numer / denom;
                float s = (d1 + d2 * r) / d4;

                //float scale = model.UnitSystem.FromInternational(1, Canguro.Model.UnitSystem.Units.Distance);
                Vector3 pa = alphaI + Vector3.Scale(p21, r);

               return pa;
        }
    }
}
