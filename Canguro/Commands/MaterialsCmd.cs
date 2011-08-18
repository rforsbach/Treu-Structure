using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to show the Materials management interface.
    /// </summary>
    class MaterialsCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Opens the Materials Dialog.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Canguro.Commands.Forms.MaterialsGUI gui = new Canguro.Commands.Forms.MaterialsGUI();
            if (services.ShowDialog(gui) != DialogResult.OK)
                throw new Canguro.Controller.CancelCommandException();
        }
    }
}
