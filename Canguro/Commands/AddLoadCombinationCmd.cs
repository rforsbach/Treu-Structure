using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Model Command to add new Load Combinations.
    /// </summary>
    public class AddLoadCombinationCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Opens the LoadCombinationsDialog
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            if (services.ShowDialog(new Canguro.Commands.Forms.LoadCombinationsDialog(services.Model))
                == System.Windows.Forms.DialogResult.Cancel)
                services.Model.Undo.Rollback();
        }
    }
}
