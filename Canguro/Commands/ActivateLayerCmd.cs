using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Command to activate the layer containing the selected items.
    /// </summary>
    class ActivateLayerCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Activates the layer containing the selected items
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Layer layer = null;
            bool oneLayer = false;
            foreach (Joint j in services.Model.JointList)
            {
                if (j != null && j.IsSelected)
                {
                    if (layer == null)
                    {
                        layer = j.Layer;
                        oneLayer = true;
                    }
                    else if (layer.Id != j.Layer.Id)
                    {
                        oneLayer = false;
                        break;
                    }
                }
            }
            if (oneLayer || layer == null)
            {
                foreach (LineElement l in services.Model.LineList)
                {
                    if (l != null && l.IsSelected)
                    {
                        if (layer == null)
                        {
                            layer = l.Layer;
                            oneLayer = true;
                        }
                        else if (layer.Id != l.Layer.Id)
                        {
                            oneLayer = false;
                            break;
                        }
                    }
                }
            }
            if (oneLayer)
                services.Model.ActiveLayer = layer;
            else
            {
                Item item = services.GetItem();
                services.Model.ActiveLayer = item.Layer;
            }
            services.Model.ChangeModel();
        }
    }
}
