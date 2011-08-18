using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Controller
{
    [Flags]
    public enum UpdateAreaEvent
    {
        ResultsArrived = 1,
        ModelChanged = 2,
        SelectionChanged = 4,
        ModelReset = 8
    }
}
