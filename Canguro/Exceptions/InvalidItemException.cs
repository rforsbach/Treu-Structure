using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    public class InvalidItemException : System.Exception
    {
        public InvalidItemException(string message)
            : base(message)
        {
        }

        public InvalidItemException()
        {
        }
    }
}
