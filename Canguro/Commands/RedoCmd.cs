using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Redo a previously undone action.
    /// </summary>
    public class RedoCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Redoes the last undone action by calling UndoManager.Redo()
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            if (services.Model.Undo.CanRedo)
                services.Model.Undo.Redo();
            services.Model.ChangeModel();
        }
    }
}
