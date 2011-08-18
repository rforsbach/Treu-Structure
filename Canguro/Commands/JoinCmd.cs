using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Controller.Snap;
using Microsoft.DirectX;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Join the Model.
    /// Deletes repeated Joints and Lines and isolated Joints.
    /// </summary>
    class JoinCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Executes Join with all the Items and asks to renumber the JointList and LineList objects.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Join(services.Model, services.Model.JointList, services.Model.LineList, services.Model.AreaList);
            RepairJoints(services.Model);

            string msg = "";
            bool conn;
            Canguro.Utility.AnalysisUtils.CanAnalyze(services.Model, ref msg, out conn);
            services.Model.ChangeModel();
            msg = Culture.Get("confirmCompactIDs");
            if (!conn)
                msg = Culture.Get("structureIsDisconnectedWrn") + "\n\n" + msg;

            if (System.Windows.Forms.MessageBox.Show(msg, Culture.Get("confirm"), 
                System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                services.Model.JointList.Compact();
                services.Model.LineList.Compact();
                services.Model.AreaList.Compact();
            }

            services.Model.ChangeModel();
        }

        /// <summary>
        /// Finds repeated Joints and Line Elements and deletes them. Deletes only Items in the
        /// parameters and keeps the Item with the smallest ID.
        /// </summary>
        /// <param name="model">The Model object</param>
        /// <param name="joints">List of Joints to check</param>
        /// <param name="lines">List of Line Elements to check</param>
        /// <param name="areas">List of Area Elements to check</param>
        public static void Join(Canguro.Model.Model model, IList<Joint> joints, IList<LineElement> lines, IList<AreaElement> areas)
        {            
            System.Windows.Forms.Cursor cursor = System.Windows.Forms.Cursor.Current;
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            try
            {
                Dictionary<uint, uint> deleteJoints = new Dictionary<uint, uint>();
                ItemList<Joint> jList = model.JointList;
                ItemList<LineElement> lList = model.LineList;
                ItemList<AreaElement> aList = model.AreaList;
                bool[] usedJoints = new bool[jList.Count];

                // Find repeated joints
                for (uint ii = 0; ii < joints.Count; ii++)
                {
                    Joint j = joints[(int)ii];
                    if (j != null)
                    {
                        for (uint jj = 0; jj < j.Id; jj++)
                        {
                            Joint mj = jList[jj];
                            if (mj != null && equals(j, mj)
                                && !deleteJoints.ContainsKey(j.Id))
                            {
                                deleteJoints.Add(j.Id, jj);
                                break;
                            }
                        }
                    }
                    System.Windows.Forms.Application.DoEvents();
                }

                // Move lines to current joints.
                foreach (LineElement l in lines)
                {
                    if (l != null && l.I != null && deleteJoints.ContainsKey(l.I.Id))
                        l.I = jList[deleteJoints[l.I.Id]];
                    if (l != null && l.J != null && deleteJoints.ContainsKey(l.J.Id))
                        l.J = jList[deleteJoints[l.J.Id]];
                }

                // Delete repeated and Zero-lenght lines.
                for (int i = 0; i < lines.Count; i++)
                {
                    LineElement l = lines[i];
                    if (l != null)
                    {
                        if (l.I.Id == l.J.Id || l.Length < 0.0001F)
                            lList.RemoveAt((int)l.Id);
                        else
                            for (int j = 0; j < (int)l.Id; j++)
                            {
                                LineElement ml = lList[j];
                                if (ml != null && (ml.Id < l.Id && equals(l, ml)))
                                {
                                    lList.RemoveAt((int)l.Id);
                                    ml.IsVisible = true;
                                    break;
                                }
                            }
                    }
                }

                // Move areas to current joints.
                foreach (AreaElement a in areas)
                {
                    if (a != null && a.J1 != null && deleteJoints.ContainsKey(a.J1.Id))
                        a.J1 = jList[deleteJoints[a.J1.Id]];
                    if (a != null && a.J2 != null && deleteJoints.ContainsKey(a.J2.Id))
                        a.J2 = jList[deleteJoints[a.J2.Id]];
                    if (a != null && a.J3 != null && deleteJoints.ContainsKey(a.J3.Id))
                        a.J3 = jList[deleteJoints[a.J3.Id]];
                    if (a != null && a.J4 != null && deleteJoints.ContainsKey(a.J4.Id))
                        a.J4 = jList[deleteJoints[a.J4.Id]];
                }

                // Delete repeated and Zero-area Areas.
                for (int i = 0; i < areas.Count; i++)
                {
                    AreaElement a = areas[i];
                    if (a != null)
                    {
                        if ((a.J4 != null && (a.J1.Id == a.J2.Id || a.J1.Id == a.J3.Id || a.J1.Id == a.J4.Id || a.J2.Id == a.J3.Id || a.J2.Id == a.J4.Id || a.J3.Id == a.J4.Id)) ||
                            (a.J1.Id == a.J2.Id || a.J1.Id == a.J3.Id || a.J2.Id == a.J3.Id) || a.Area < 0.0001F)
                            aList.RemoveAt((int)a.Id);
                        else
                            for (int j = 0; j < (int)a.Id; j++)
                            {
                                AreaElement ml = aList[j];
                                if (ml != null && (ml.Id < a.Id && equals(a, ml)))
                                {
                                    aList.RemoveAt((int)a.Id);
                                    ml.IsVisible = true;
                                    break;
                                }
                            }
                    }
                }

                // Delete repeated joints.
                foreach (int id in deleteJoints.Keys)
                    jList.RemoveAt(id);

                // Delete unused joints only if lines or areas are used.
                if (lines.Count > 0 || areas.Count > 0)
                {
                    foreach (LineElement l in lList)
                        if (l != null)
                        {
                            usedJoints[l.I.Id] = true;
                            usedJoints[l.J.Id] = true;
                        }
                    foreach (AreaElement a in aList)
                        if (a != null)
                        {
                            usedJoints[a.J1.Id] = true;
                            usedJoints[a.J2.Id] = true;
                            usedJoints[a.J3.Id] = true;
                            usedJoints[a.J4.Id] = true;
                        }
                    for (int i = 0; i < joints.Count; i++)
                    {
                        Joint j = joints[i];
                        if (j != null && !usedJoints[j.Id] && jList[j.Id] == j)
                            jList.RemoveAt((int)j.Id);
                    }
                }
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = cursor;
            }
        }

        private static bool equals(Joint j, Joint i)
        {
            float delta = Properties.Settings.Default.JoinTolerance;
            Microsoft.DirectX.Vector3 d = j.Position - i.Position;
            if (d.LengthSq() < delta)
                return true;
            return false;
        }

        private static bool equals(LineElement j, LineElement i)
        {
            return ((j.I.Id == i.I.Id && j.J.Id == i.J.Id) ||
                (j.I.Id == i.J.Id && j.J.Id == i.I.Id));
        }

        private static bool equals(AreaElement j, AreaElement i)
        {
            return (((j.J4 != null && i.J4 != null) && (j.J1.Id == i.J1.Id || j.J1.Id == i.J2.Id || j.J1.Id == i.J3.Id || j.J1.Id == i.J4.Id) &&
                     (j.J2.Id == i.J1.Id || j.J2.Id == i.J2.Id || j.J2.Id == i.J3.Id || j.J2.Id == i.J4.Id) &&
                     (j.J3.Id == i.J1.Id || j.J3.Id == i.J2.Id || j.J3.Id == i.J3.Id || j.J3.Id == i.J4.Id) &&
                     (j.J4.Id == i.J1.Id || j.J4.Id == i.J2.Id || j.J4.Id == i.J3.Id || j.J4.Id == i.J4.Id)) ||

                     ((j.J4 == null && i.J4 == null) && (j.J1.Id == i.J1.Id || j.J1.Id == i.J2.Id || j.J1.Id == i.J3.Id) &&
                     (j.J2.Id == i.J1.Id || j.J2.Id == i.J2.Id || j.J2.Id == i.J3.Id) &&
                     (j.J3.Id == i.J1.Id || j.J3.Id == i.J2.Id || j.J3.Id == i.J3.Id) &&
                     (j.J4.Id == i.J1.Id || j.J4.Id == i.J2.Id || j.J4.Id == i.J3.Id)));
        }

        /// <summary>
        /// Detects inconsistencies in the model and repairs them.
        /// If a line is connected to a joint outside the model, 
        /// it's added (if ID is free) or changed (if a joint with same ID exists).
        /// </summary>
        /// <param name="model"></param>
        public static void RepairJoints(Canguro.Model.Model model)
        {
            foreach (LineElement line in model.LineList)
            {
                if (line != null && line.I != null && line.J != null)
                {
                    Joint j = line.I;
                    if (model.JointList[j.Id] == null)
                        model.JointList.Add(j);
                    else if (model.JointList[j.Id] != j)
                        line.I = model.JointList[j.Id];

                    j = line.J;
                    if (model.JointList[j.Id] == null)
                        model.JointList.Add(j);
                    else if (model.JointList[j.Id] != j)
                        line.J = model.JointList[j.Id];
                }
            }

            foreach (AreaElement area in model.AreaList)
            {
                if (area != null && area.J1 != null && area.J2 != null && area.J3 != null)
                {
                    for (int i = 0; i < ((area.J4 != null) ? 4 : 3); i++)
                    {
                        Joint j = area[i];
                        if (model.JointList[j.Id] == null)
                            model.JointList.Add(j);
                        else if (model.JointList[j.Id] != j)
                            area[i] = model.JointList[j.Id];
                    }
                }
            }
        }

        public static void Intersect(Canguro.Model.Model model, IList<Joint> joints, IList<LineElement> lines)
        {
            System.Windows.Forms.Cursor cursor = System.Windows.Forms.Cursor.Current;
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                int nJoints = (joints == null) ? 0 : joints.Count;
                int nLines = (lines == null) ? 0 : lines.Count;

                Joint joint;
                for (int i = 0; i < nLines; i++)
                {
                    System.Windows.Forms.Application.DoEvents();
                    for (int j = i + 1; j < nLines; j++)
                        if (lines[i] != null && lines[j] != null)
                        {
                            joint = Intersect(model, lines[i], lines[j]);
                            if (joint != null)
                                joints.Add(joint);
                        }
                }

                //for (int i = 0; i < nJoints; i++)
                //{
                //    Joint joint = joints[i];
                //    if (joint != null)
                foreach (LineElement line in lines)
                    if (line != null)
                    {
                        SplitCmd.Split(line, joints, model);
                        System.Windows.Forms.Application.DoEvents();
                    }
                //}
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = cursor;
            }
        }

        public static Joint Intersect(Canguro.Model.Model model, LineElement l1, LineElement l2)
        {
            if (l1 != null && l2 != null && l1 != l2 && l1.I != l2.I && l1.J != l2.J && l1.I != l2.J && l1.J != l2.I)
            {
                float numer, denom;
                float d1, d2, d3, d4, d5;
                Vector3 p13 = l1.I.Position - l2.I.Position;
                Vector3 p21 = l1.J.Position - l1.I.Position;
                Vector3 p43 = l2.J.Position - l2.I.Position;

                d1 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
                d2 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
                d3 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
                d4 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
                d5 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

                denom = d5 * d4 - d2 * d2;
                if (Math.Abs(denom) < 0.0001)
                    return null;
                numer = d1 * d2 - d3 * d4;

                float r = numer / denom;
                float s = (d1 + d2 * r) / d4;

                float scale = model.UnitSystem.FromInternational(1, Canguro.Model.UnitSystem.Units.Distance);
                Vector3 pa = Vector3.Scale(l1.I.Position, scale) + Vector3.Scale(p21, r * scale);
                Vector3 pb = Vector3.Scale(l2.I.Position, scale) + Vector3.Scale(p43, s * scale);                               
                
                if (r > (-0.001) && r < 1.001 && s > (-0.001) && s < 1.001 && (pa - pb).Length() < 0.001 &&
                    ((pa - l1.I.Position).LengthSq() > 0.000001f) && ((pa - l1.J.Position).LengthSq() > 0.000001f) &&
                    ((pa - l2.I.Position).LengthSq() > 0.000001f) && ((pa - l2.J.Position).LengthSq() > 0.000001f))
                {
                    Joint joint = new Joint(pa.X, pa.Y, pa.Z);
                    model.JointList.Add(joint);
                    return joint;
                    //SplitCmd.Split(l1, joint, model);
                    //SplitCmd.Split(l2, joint, model);
                }
                //// Create magnet
                //PointMagnet intPtMagnet = new PointMagnet(l1.Position + Vector3.Scale(l1.Direction, r),
                //    PointMagnetType.Intersection);
                //if (intPtMagnet.Snap(activeView, e.Location) < SnapViewDistance)
                //{
                //    intPtMagnet.RelatedMagnets.Add(l1);
                //    intPtMagnet.RelatedMagnets.Add(l2);
                //    return intPtMagnet;
                //}
            }

            return null;
        }
    }
}
