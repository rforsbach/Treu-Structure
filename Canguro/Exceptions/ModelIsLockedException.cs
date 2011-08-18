using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    public class ModelIsLockedException : Exception
    {
        public ModelIsLockedException(string message) : base(message)
        {
        }

        public ModelIsLockedException()
            : base(Culture.Get("modelIsLockedExceptionMessage"))
        {
        }
    }
}
