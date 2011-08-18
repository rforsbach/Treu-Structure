using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to make an array of copies of selected Items
    /// </summary>
    public class ArrayCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Asks for selected Items repetitions and a Vector, and makes the copies.
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
                    for (int i = 0; i < ((a.J4 != null) ? 4 : 3); i++)
                    {
                        if (!jSelection.ContainsKey(a[i]))
                        {
                            jSelection.Add(a[i], null);
                            joints.Add(a[i], null);
                        }
                    }
                }
            }

            Microsoft.DirectX.Vector3 v;
            services.GetVector(out v);

            uint n;
            string str = services.GetString(Culture.Get("getArrayRepetition"));
            n = Convert.ToUInt32(str);

            List<Joint> newJoints = new List<Joint>();
            List<LineElement> newLines = new List<LineElement>();
            List<AreaElement> newAreas = new List<AreaElement>();
            for (int i = 1; i <= n; i++)
            {
                foreach (Joint j in joints.Keys)
                {
                    jList.Add(nJoint = new Joint(j.X + i * v.X, j.Y + i * v.Y, j.Z + i * v.Z));
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
                    aList.Add(nArea = new AreaElement(a, jSelection[a.J1], jSelection[a.J2], jSelection[a.J3], jSelection[a.J4]));
                    newAreas.Add(nArea);
                }
                services.ReportProgress((uint)(100 * i / n));
                System.Windows.Forms.Application.DoEvents();
            }
            JoinCmd.Join(services.Model, newJoints, newLines, newAreas);
        }
    }
}
