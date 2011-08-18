using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Undo
{
    public interface Undoable
    {
        void Undo();
    }
}
