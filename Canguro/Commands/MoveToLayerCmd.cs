using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Move all selected items to the current Layer.
    /// </summary>
    class MoveToLayerCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Sets the Layer property of all the selected Items to point to the Active Layer
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Layer layer = services.Model.ActiveLayer;
            foreach (Item item in services.Model.JointList)
                if (item != null && item.IsSelected)
                    item.Layer = layer;
            foreach (Item item in services.Model.LineList)
                if (item != null && item.IsSelected)
                    item.Layer = layer;
        }
    }
}
