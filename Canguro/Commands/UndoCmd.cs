using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Undo the last action or command's changes to the Model.
    /// </summary>
    public class UndoCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Calls UndoManager.Undo(). Undoes the last Command/Action.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            if (services.Model.Undo.CanUndo)
                services.Model.Undo.Undo();
            services.Model.ChangeModel();
        }
    }
}
