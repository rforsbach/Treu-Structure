using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator.Commands
{
    class UndoCommand : RunnableCommand
    {
        protected override void Run()
        {
            controller.EndCommand();
            model.Undo.Undo();
        }
    }
}
