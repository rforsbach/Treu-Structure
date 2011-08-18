using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    public class SelectConnectedCmd : ModelCommand
    {
        public override void Run(Canguro.Controller.CommandServices services)
        {
            foreach (LineElement l in services.Model.LineList)
                if (l != null && l.I != null && l.J != null && (l.I.IsSelected || l.J.IsSelected))
                    l.IsSelected = true;
            services.Model.ChangeSelection(null);
        }
    }
}
