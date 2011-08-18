using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Commands.Forms;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to open the Sections Dialog.
    /// </summary>
    class SectionsCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Opens the Sections Dialog
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            SectionsGUI gui = new SectionsGUI();
            if (services.ShowDialog(gui) == System.Windows.Forms.DialogResult.Cancel)
                throw new Canguro.Controller.CancelCommandException();

            services.Model.ChangeModel(true);
        }
    }
}
