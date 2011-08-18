using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    public class InvalidIndexException : System.Exception
    {
        public InvalidIndexException()
        {
        }

        public InvalidIndexException(string message)
            : base(message)
        { 
        }
    }
}
