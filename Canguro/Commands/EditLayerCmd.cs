using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Edit the Active Layer.
    /// </summary>
    public class EditLayerCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Opens the properties window to edit the Active Layer
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Layer layer = services.Model.ActiveLayer;
            services.GetProperties(Culture.Get("editCmdTitle"), layer, false);
        }
    }
}
