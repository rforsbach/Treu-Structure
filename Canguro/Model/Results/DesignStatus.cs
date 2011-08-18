using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results
{
    public enum DesignStatus : byte
    {
        NoMesages, SeeErrMsg, SeeWarnMsg, SeeErrMsgAndWarnMsg,
    }
}
