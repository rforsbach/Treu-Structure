using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Load;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to split a Line Element in many.
    /// </summary>
    class SplitCmd : Commands.ModelCommand
    {
        private SplitCmd() { }
        public static readonly SplitCmd Instance = new SplitCmd();

        /// <summary>
        /// Executes the command. 
        /// Gets a number of segments and divides all selected line elements in equal length segments.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            List<Item> selection = services.GetSelection();
            if (selection.Count == 0)
                return;

            List<LineElement> lineList = new List<LineElement>();
            foreach (Item item in selection)
                if (item != null && item is LineElement)
                    lineList.Add((LineElement)item);
            if (lineList.Count == 0)
                lineList.Add(services.GetLine());
            int parts = (int)services.GetSingle(Culture.Get("getSplitParts") + " [2-100]");
            parts = (parts < 2) ? 2 : (parts > 100) ? 100 : parts;

            foreach (LineElement line in lineList)
            {
                Joint ji = line.I;
                Joint jj = line.J;
                Joint last = jj;
                Microsoft.DirectX.Vector3 v = new Microsoft.DirectX.Vector3(jj.X - ji.X, jj.Y - ji.Y, jj.Z - ji.Z);
                v.Multiply(1.0f / parts);
                if (parts > 1 && v.LengthSq() > 0)
                {
                    LineElement newLine = line;
                    for (int i = 0; i < parts - 1; i++)
                    {
                        jj = new Joint(ji.X + v.X, ji.Y + v.Y, ji.Z + v.Z);
                        services.Model.JointList.Add(jj);
                        newLine = Split(newLine, jj, services.Model);
                        services.Model.LineList.Add(newLine);
                        ji = jj;
                    }
                }
            }
        }

        /// <summary>
        /// Splits a line element and it's loads with an intermediate joint.
        /// </summary>
        /// <param name="line">The line element to split</param>
        /// <param name="joint">The joint, which should be located inside the line element</param>
        /// <param name="model">The Model object</param>
        /// <returns>The new line element, added to the model</returns>
        internal static void Split(LineElement line, IList<Joint> joints, Canguro.Model.Model model)
        {
            //float intersect = getIntersection(line, joint);      
            //Commands.View.Selection.

            List<Utility.DistancedItem.dItem> dJoints = new List<Canguro.Utility.DistancedItem.dItem>(joints.Count);
            foreach (Joint j in joints)
            {
                if (line.I != j && line.J != j)
                    dJoints.Add(new Canguro.Utility.DistancedItem.dItem(getIntersection(line, j), j));
            }

            dJoints.Sort(Utility.DistancedItem.ReverseComparer.Instance);

            foreach (Utility.DistancedItem.dItem dItem in dJoints)
                Split(line, (Joint)dItem.item, dItem.distance, model);
        }

        /// <summary>
        /// Splits a line element and it's loads with an intermediate joint.
        /// </summary>
        /// <param name="line">The line element to split</param>
        /// <param name="joint">The joint, which should be located inside the line element</param>
        /// <param name="pos">The position of the intersection (between 0 - 1 means they intersect)</param>
        /// <param name="model">The Model object</param>
        /// <returns>The new line element, added to the model</returns>
        internal static LineElement Split(LineElement line, Joint joint, Canguro.Model.Model model)
        {
            return Split(line, joint, getIntersection(line, joint), model);
        }

        /// <summary>
        /// Splits a line element and it's loads with an intermediate joint.
        /// </summary>
        /// <param name="line">The line element to split</param>
        /// <param name="joint">The joint, which should be located inside the line element</param>
        /// <param name="pos">The position of the intersection (between 0 - 1 means they intersect)</param>
        /// <param name="model">The Model object</param>
        /// <returns>The new line element, added to the model</returns>
        internal static LineElement Split(LineElement line, Joint joint, float intersectPos, Canguro.Model.Model model)
        {
            if (line == null || joint == null || model == null || intersectPos <= 0 || intersectPos >= 1)
                return null;

            Layer layer = line.Layer;
            joint.Layer = layer;

            LineElement newLine = new LineElement(line.Properties, joint, line.J);
            newLine.Angle = line.Angle;
            newLine.CardinalPoint = line.CardinalPoint;
            newLine.Layer = layer;
            line.J = joint;

            LineEndOffsets off1 = line.EndOffsets;
            LineEndOffsets off2 = line.EndOffsets;
            off1.EndJ = 0;
            off2.EndI = 0;
            line.EndOffsets = off1;
            newLine.EndOffsets = off2;

            JointDOF dofi = line.DoFI;
            JointDOF dofj = line.DoFJ;
            line.DoFJ = new JointDOF(true);
            newLine.DoFJ = dofj;

            if (model.JointList[joint.Id] != joint)
                model.JointList.Add(joint);

            RelocateLoads(line, newLine, intersectPos, model);

            model.LineList.Add(newLine);
            return newLine;
        }

        /// <summary>
        /// Relocates all the LineLoads in the oldLine to keep the position in both, the old and new lines.
        /// </summary>
        /// <param name="oldLine">The line which used to be a whole</param>
        /// <param name="newLine">The line which forms part of the old line</param>
        /// <param name="x">The dividing point</param>
        /// <param name="model">The Model object</param>
        private static void RelocateLoads(LineElement oldLine, LineElement newLine, float x, Canguro.Model.Model model)
        {
            AssignedLineLoads loads = (AssignedLineLoads)oldLine.Loads;
            foreach (LoadCase lc in model.LoadCases.Values)
            {
                IList<Canguro.Model.Load.Load> list = loads[lc];
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        bool relocated = false;
                        Canguro.Model.Load.Load ll = list[i];
                        if (ll is ConcentratedSpanLoad)
                            relocated = RelocateConcentratedLoads(newLine.Loads, lc, x, (ConcentratedSpanLoad)ll);
                        else if (ll is DistributedSpanLoad)
                            relocated = RelocateDistributedLoads(newLine.Loads, lc, x, (DistributedSpanLoad)ll);
                        if (relocated)
                            loads[lc].RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Relocates a ConcentratedSpanLoad between the line element where it is and a new adjacent line element.
        /// </summary>
        /// <param name="newLineLoads">The AssignedLoads object of the new Line Element</param>
        /// <param name="lc">The Load Case to which the load belongs.</param>
        /// <param name="x">The dividing point of the two line elements [0, 1]</param>
        /// <param name="load">The Load to distribute in two elements</param>
        /// <returns>true if the load was moved to the new Line Element (so the caller removes it). false otherwise.</returns>
        private static bool RelocateConcentratedLoads(AssignedLoads newLineLoads, LoadCase lc, float x, ConcentratedSpanLoad load)
        {
            if (x > load.D)
            {
                load.D = load.D / x;
                return false;
            }
            else
            {
                load = (ConcentratedSpanLoad)load.Clone();
                load.Id = 0;
                load.D = (load.D - x) / (1f - x);
                newLineLoads.Add(load, lc);
                return true;
            }
        }

        /// <summary>
        /// Distributes a DistributedSpanLoad between the line element where it is and a new adjacent line element.
        /// </summary>
        /// <param name="newLineLoads">The AssignedLoads object of the new Line Element</param>
        /// <param name="lc">The Load Case to which the load belongs.</param>
        /// <param name="x">The dividing point of the two line elements [0, 1]</param>
        /// <param name="load">The Load to distribute in two elements</param>
        /// <returns>true if the load was moved to the new Line Element (so the caller removes it). false otherwise.</returns>
        private static bool RelocateDistributedLoads(AssignedLoads newLineLoads, LoadCase lc, float x, DistributedSpanLoad load)
        {
            if (load.Da < x && load.Db < x)
            {
                load.Da = load.Da / x;
                load.Db = load.Db / x;
                return false;
            }
            if (load.Da >= x && load.Db >= x)
            {
                load = (DistributedSpanLoad)load.Clone();
                load.Id = 0;
                load.Da = (load.Da - x) / (1 - x);
                load.Db = (load.Db - x) / (1 - x);
                newLineLoads.Add(load, lc);
                return true;
            }
            if (load.Da > load.Db)
            {
                float tmp = load.Db;
                load.Db = load.Da;
                load.Da = tmp;
                tmp = load.Lb;
                load.Lb = load.La;
                load.La = tmp;
            }
            DistributedSpanLoad nLoad = (DistributedSpanLoad)load.Clone();
            nLoad.Id = 0;
            load.Da = load.Da / x;
            load.Db = 1f;
            load.Lb = (load.La) + (x - load.Da) * (load.Lb - load.La) / (load.Db - load.Da);

            nLoad.Da = 0;
            nLoad.Db = (nLoad.Db - x) / (1 - x);
            nLoad.La = load.Lb;
            newLineLoads.Add(nLoad, lc);
            return false;
        }

        /// <summary>
        /// Calculates the intersection of the given Joint and LineElement and returns the 
        /// float value corresponding to the proportion of the vector from defined by the line Joints.
        /// </summary>
        /// <param name="line">The LineElement to intersect</param>
        /// <param name="joint">The Joint to intersect with line</param>
        /// <returns>A float in [0, 1). If the joint is in the line, the proportion, 0 otherwise.</returns>
        public static float getIntersection(LineElement line, Joint joint)
        {
            Microsoft.DirectX.Vector3 i = line.I.Position;
            Microsoft.DirectX.Vector3 j = line.J.Position;
            Microsoft.DirectX.Vector3 pos = joint.Position;
            float ret, retx, rety, retz;
            float eps = 0.001f;

            //retx = (j.X != i.X) ? (pos.X - i.X) / (j.X - i.X) : (equals(pos.X, i.X, eps)) ? float.NaN : 0;
            //rety = (j.Y != i.Y) ? (pos.Y - i.Y) / (j.Y - i.Y) : (equals(pos.Y, i.Y, eps)) ? retx : 0;
            //retz = (j.Z != i.Z) ? (pos.Z - i.Z) / (j.Z - i.Z) : (equals(pos.Z, i.Z, eps)) ? rety : 0;
            //rety = (!float.IsNaN(rety)) ? rety : retz;
            //retx = (!float.IsNaN(retx)) ? retx : rety;
            
            //return (equals(retx, rety, eps) && equals(rety, retz, eps) && retx - eps > 0 && retx + eps < 1) ? retx : 0;

            Microsoft.DirectX.Vector3 xmp = joint.Position - line.I.Position;
            Microsoft.DirectX.Vector3 dir = line.J.Position - line.I.Position;

            ret = (Math.Abs(xmp.X) > Math.Abs(xmp.Y)) ? ((Math.Abs(xmp.X) > Math.Abs(xmp.Z)) ? xmp.X / dir.X : xmp.Z / dir.Z) : ((Math.Abs(xmp.Y) > Math.Abs(xmp.Z)) ? xmp.Y / dir.Y : xmp.Z / dir.Z);
            return (equals(ret * dir.X, xmp.X, eps) && equals(ret * dir.Y, xmp.Y, eps) && equals(ret * dir.Z, xmp.Z, eps)) ? ret : 0;
        }

        /// <summary>
        /// Compares two Single values. Returns true if the difference is less than a given epsilon.
        /// </summary>
        /// <param name="a">The first Single value to compare</param>
        /// <param name="b">The second Single value to compare</param>
        /// <param name="epsilon">The largest value to consider a and b to be equal.</param>
        /// <returns>true if the a-b &lt; epsilon, false otherwise</returns>
        private static bool equals(float a, float b, float epsilon)
        {
            return (a - b < epsilon) && (a - b > -epsilon);
        }
    }
}
