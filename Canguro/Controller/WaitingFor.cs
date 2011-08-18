using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Controller
{
    public enum WaitingFor : byte
    {
        None,
        Text,
        SimpleValue,

        Point,
        Joint,
        Line,
        Area,
        Any,

        Many,
    }
}
