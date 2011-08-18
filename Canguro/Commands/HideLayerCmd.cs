using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to hide all elements from the Active Layer
    /// </summary>
    class HideLayerCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Sets the IsSelected property of all the Items in the Active Layer to false.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Layer layer = services.Model.ActiveLayer;
            foreach (Item item in layer.Items)
                item.IsVisible = item.IsSelected = false;

            if (services.Model.HasResults)
                services.Model.Results.StressHelper.IsDirty = true;

            services.Model.ChangeModel();
        }
    }
}
