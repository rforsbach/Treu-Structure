using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Controller
{
    [Flags]
    public enum SelectionFilter
    {
        Joints = 1,
        Lines = 2,
        Areas = 4,
    }
}
