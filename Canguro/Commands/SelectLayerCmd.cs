using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to select all the Items in the Active Layer
    /// </summary>
    class SelectLayerCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Sets the IsSelected property of all the Items in the Active Layer to true.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Layer layer = services.Model.ActiveLayer;
            foreach (Item item in layer.Items)
                item.IsSelected = true;
            services.Model.ChangeSelection(null);
        }
    }
}
