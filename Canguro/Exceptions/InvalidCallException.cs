using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    public class InvalidCallException : System.Exception
    {
        public InvalidCallException(string message)
            : base(message)
        { }
    }
}
