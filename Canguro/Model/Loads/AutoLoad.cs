using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    public class AutoLoad
    {
        public virtual string Name
        {
            get { return "None"; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
