using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Canguro.Model;
using Canguro.Controller.Snap;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to paste the items copied to the clipboard into the Model.
    /// </summary>
    public class PasteCmd : Canguro.Commands.ModelCommand
    {
        //public override void Run(Canguro.Controller.CommandServices services)
        //{
        //    objectCount = 0;
        //    if (Clipboard.ContainsData("Canguro"))
        //    {
        //        Stream stream = (Stream)Clipboard.GetData("Canguro");
        //        Magnet magnet = services.GetPoint(Culture.Get("pasteCmdTitle"));

        //        BinaryFormatter bformatter = new BinaryFormatter();
        //        Microsoft.DirectX.Vector3 pivot = (Microsoft.DirectX.Vector3)bformatter.Deserialize(stream);
        //        Microsoft.DirectX.Vector3 v = magnet.SnapPosition - pivot;
        //        List<Joint> newJoints = new List<Joint>();
        //        List<LineElement> newLines = new List<LineElement>();
        //        ItemList<Joint> jList = services.Model.JointList;
        //        ItemList<LineElement> lList = services.Model.LineList;
        //        Dictionary<uint, Joint> jSelection = new Dictionary<uint, Joint>();
        //        Joint nJoint;
        //        LineElement nLine;

        //        objectCount = (int) bformatter.Deserialize(stream);
        //        for (int i = 0; i < objectCount; i++)
        //        {
        //            Element elem = (Element)bformatter.Deserialize(stream);

        //            if (elem is Joint)
        //            {
        //                Joint j = (Joint)elem;
        //                jList.Add(nJoint = new Joint(j.X + v.X, j.Y + v.Y, j.Z + v.Z));
        //                nJoint.Masses = j.Masses;
        //                nJoint.DoF = j.DoF;
        //                jSelection.Add(j.Id, nJoint);
        //                newJoints.Add(nJoint);
        //                CopyLoads(services.Model, j, nJoint);
        //            }
        //            if (elem is LineElement)
        //            {
        //                LineElement l = (LineElement)elem;
        //                lList.Add(nLine = new LineElement(l.Properties));
        //                nLine.I = jSelection[l.I.Id];
        //                nLine.J = jSelection[l.J.Id];
        //                nLine.Angle = l.Angle;
        //                newLines.Add(nLine);
        //                CopyLoads(services.Model, l, nLine);
        //            }
        //            JoinCmd.Join(services.Model, newJoints, newLines);
        //        }
        //    }
        //}

        /// <summary>
        /// Executes the command. 
        /// Pastes the Items in the Clpboard under the key Canguro, in the current Model.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            objectCount = 0;
            if (Clipboard.ContainsData("Canguro"))
            {
                object[] data = (object[])Clipboard.GetData("Canguro");
                Dictionary<uint, Joint> joints = (Dictionary<uint, Joint>)data[0];
                Dictionary<uint, Joint> jSelection = new Dictionary<uint, Joint>();
                IList<LineElement> lines = (IList<LineElement>)data[1];
                IList<AreaElement> areas = (IList<AreaElement>)data[2];
                Microsoft.DirectX.Vector3 pivot = (Microsoft.DirectX.Vector3)data[3];

                ItemList<Joint> jList = services.Model.JointList;
                ItemList<LineElement> lList = services.Model.LineList;
                ItemList<AreaElement> aList = services.Model.AreaList;

                Joint nJoint;
                LineElement nLine;
                AreaElement nArea;
                Magnet magnet = services.GetPoint(Culture.Get("pasteCmdTitle"));
                if (magnet == null)
                    objectCount = 0;
                else
                {
                    objectCount = joints.Count + lines.Count;
                    Microsoft.DirectX.Vector3 v = magnet.SnapPosition - pivot;
                    List<Joint> newJoints = new List<Joint>();
                    List<LineElement> newLines = new List<LineElement>();
                    List<AreaElement> newAreas = new List<AreaElement>();

                    Dictionary<string, Layer> layers = new Dictionary<string, Layer>();
                    foreach (Layer l in services.Model.Layers)
                        if (l != null)
                            layers.Add(l.Name, l);


                    foreach (uint jid in joints.Keys)
                    {
                        Joint j = (joints[jid] == null) ? jList[jid] : joints[jid];
                        jList.Add(nJoint = new Joint(j.X + v.X, j.Y + v.Y, j.Z + v.Z));
                        nJoint.Masses = j.Masses;
                        if (!layers.ContainsKey(j.Layer.Name))
                        {
                            Layer lay = new Layer(j.Layer.Name);
                            services.Model.Layers.Add(lay);
                            layers.Add(lay.Name, lay);
                        }
                        nJoint.Layer = layers[j.Layer.Name];
                        nJoint.DoF = j.DoF;
                        jSelection.Add(jid, nJoint);
                        newJoints.Add(nJoint);
                        CopyLoads(services.Model, j, nJoint);
                    }
                    foreach (LineElement l in lines)
                    {
                        if (!layers.ContainsKey(l.Layer.Name))
                        {
                            Layer lay = new Layer(l.Layer.Name);
                            services.Model.Layers.Add(lay);
                            layers.Add(lay.Name, lay);
                        }
                        lList.Add(nLine = new LineElement(l, jSelection[l.I.Id], jSelection[l.J.Id]));
                        nLine.Layer = layers[l.Layer.Name];
                        newLines.Add(nLine);
                        CopyLoads(services.Model, l, nLine);
                    }
                    foreach (AreaElement a in areas)
                    {
                        if (!layers.ContainsKey(a.Layer.Name))
                        {
                            Layer lay = new Layer(a.Layer.Name);
                            services.Model.Layers.Add(lay);
                            layers.Add(lay.Name, lay);
                        }

                        aList.Add(nArea = new AreaElement(a, jSelection[a.J1.Id], jSelection[a.J2.Id], jSelection[a.J3.Id], (a.J4 != null) ? jSelection[a.J4.Id] : null));
                        if (a.J4 != null)
                            nArea.J4 = jSelection[a.J4.Id];
                        nArea.Layer = layers[a.Layer.Name];
                        newAreas.Add(nArea);
                        CopyLoads(services.Model, a, nArea);
                    }
                    JoinCmd.Join(services.Model, newJoints, newLines, newAreas);
                }
            }
        }

        private void CopyLoads(Canguro.Model.Model model, Element from, Element to)
        {
            from.Loads.Repair();
            Canguro.Model.Load.LoadCase active = model.ActiveLoadCase;
            foreach (Canguro.Model.Load.LoadCase lCase in model.LoadCases.Values)
            {
                ItemList<Canguro.Model.Load.Load> copy = from.Loads[lCase];
                ItemList<Canguro.Model.Load.Load> list = to.Loads[lCase];
                if (list != null && list.Count > 0)
                    for (int i = list.Count; i > 0; i--)
                        list.RemoveAt(i - 1);
                if (copy != null)
                    foreach (Canguro.Model.Load.Load l in copy)
                        if (l != null)
                        {
                            Canguro.Model.Load.Load nl = (Canguro.Model.Load.Load)l.Clone();
                            nl.Id = 0;
                            to.Loads.Add(nl, lCase);
                        }
            }
        }

        private int objectCount = 0;
        public int ObjectCount
        {
            get { return objectCount; }
        }
    }
}
