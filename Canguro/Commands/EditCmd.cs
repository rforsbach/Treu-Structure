using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Edit any Item using a Property Grid.
    /// </summary>
    public class EditCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Opens the properties window to edit the selected Item.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Item item = services.GetItem();
            services.GetProperties(Culture.Get("editCmdTitle"), item, false);
        }
    }
}
