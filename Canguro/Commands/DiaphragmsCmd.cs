using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    class DiaphragmsCmd : ModelCommand
    {
        private readonly float tolerance = Properties.Settings.Default.JoinTolerance;
        private readonly string floor = Culture.Get("Floor");

        public override void Run(Canguro.Controller.CommandServices services)
        {
            services.Model.ConstraintList.Clear();
            List<Joint> joints = new List<Joint>();

            foreach (Joint j in services.Model.JointList)
                if (j != null)
                    joints.Add(j);

            List<LinkedList<int>> graph = services.Model.GetConnectivityGraph();
            joints.Sort(new HeightComparer());
            for (int i = 0; i < joints.Count; i++)
                if (joints[i].Constraint == null)
                    ConstraintFrom(joints[i], services.Model, graph);
        }

        private void ConstraintFrom(Joint j, Canguro.Model.Model model, List<LinkedList<int>> graph)
        {
            ItemList<Joint> joints = model.JointList;
            LinkedList<int> neighbors = graph[(int)j.Id];
            List<int> floorJoints = getNeighbors(j, graph, joints);
            if (floorJoints != null && floorJoints.Count > 1)
            {
                Constraint cons = new Constraint(floor + "_" + (int)j.Z);
                model.ConstraintList.Add(cons);
                j.Constraint = cons;
                foreach (int jid in floorJoints)
                    if (joints[jid] != null && Equal(j.Z, joints[jid].Z))
                        joints[jid].Constraint = j.Constraint;
            }
        }

        private List<int> getNeighbors(Joint joint, List<LinkedList<int>> graph, ItemList<Joint> joints)
        {
            Queue<int> queue = new Queue<int>();
            queue.Enqueue((int)joint.Id);
            List<int> ret = new List<int>();
            while (queue.Count > 0)
            {
                joint = joints[queue.Dequeue()];
                if (joint != null)
                {
                    ret.Add((int)joint.Id);
                    LinkedList<int> neighbors = graph[(int)joint.Id];
                    foreach (int jid in neighbors)
                        if (joints[jid] != null && Equal(joint.Z, joints[jid].Z) && !ret.Contains(jid) && !queue.Contains(jid))
                            queue.Enqueue(jid);
                }
            }
            return ret;
        }

        private bool Equal(float x, float y)
        {
            return (x - y < tolerance && x - y > -tolerance);
        }

        private class HeightComparer : IComparer<Joint>
        {

            #region IComparer<Joint> Members

            public int Compare(Joint x, Joint y)
            {
                return x.Z.CompareTo(y.Z);
            }

            #endregion
        }
    }
}
