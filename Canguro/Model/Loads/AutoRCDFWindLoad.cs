using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    public class AutoRCDFWindLoad : AutoWindLoad
    {
        public override string Name
        {
            get { return "Mexican"; }
        }

        public override string ToString()
        {
            return "RCDF";
        }
    }
}
