using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View
{
    public class EnterDataEventArgs : EventArgs
    {
        public readonly string Data;

        public EnterDataEventArgs(string data)
        {
            Data = data;
        }
    }

    public delegate void EnterDataEventHandler(object sender, EnterDataEventArgs e);
}
