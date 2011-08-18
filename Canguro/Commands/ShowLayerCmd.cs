using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Unhide all the Items in the Active Layer.
    /// </summary>
    class ShowLayerCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Sets the IsVisible property to true in all the Items in the Active Layer.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Layer layer = services.Model.ActiveLayer;
            foreach (Item item in layer.Items)
                item.IsVisible = true;
            
            if (services.Model.HasResults)
                services.Model.Results.StressHelper.IsDirty = true;

            services.Model.ChangeModel();
        }
    }
}
