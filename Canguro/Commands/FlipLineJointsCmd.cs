using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    class FlipLineJointsCmd : ModelCommand
    {
        public override void Run(Canguro.Controller.CommandServices services)
        {
            List<Item> selection = services.GetSelection();
            foreach (Item item in selection)
                if (item is LineElement)
                {
                    LineElement l = (LineElement)item;
                    Joint tmp = l.I;
                    l.I = l.J;
                    l.J = tmp;
                }
        }
    }
}
