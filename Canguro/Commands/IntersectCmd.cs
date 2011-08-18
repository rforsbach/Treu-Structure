using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    class IntersectCmd : ModelCommand
    {
        public override void Run(Canguro.Controller.CommandServices services)
        {
            List<Item> selecton = services.GetSelection();
            List<Joint> joints = new List<Joint>();
            List<LineElement> lines = new List<LineElement>();
            foreach (Item item in selecton)
            {
                if (item is Joint)
                    joints.Add((Joint)item);
                else if (item is LineElement)
                    lines.Add((LineElement)item);
            }

            JoinCmd.Intersect(services.Model, joints, lines);
        }
    }
}
