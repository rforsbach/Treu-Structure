using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Delete the current active layer and all its objects.
    /// </summary>
    public class DeleteLayer : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Deletes all Items in a Layer
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            int count = 0;
            foreach (Layer layer in services.Model.Layers)
                if (layer != null)
                    count++;

            services.Model.UnSelectAll();
            if (services.Model.ActiveLayer.Items.Count > 0)
                System.Windows.Forms.MessageBox.Show(Culture.Get("layerHasObjectsError"), Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            else if (count <= 1)
                System.Windows.Forms.MessageBox.Show(Culture.Get("lastLayerError"), Culture.Get("error"), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            else
            {
                Layer deletedLayer = services.Model.ActiveLayer;
                foreach (Layer active in services.Model.Layers)
                    if (active != null && active != deletedLayer)
                    {
                        services.Model.ActiveLayer = active;
                        break;
                    }
                Layer activeLayer = services.Model.ActiveLayer;

                foreach (Item item in services.Model.LineList)
                    if (item != null && item.IsSelected)
                        item.Layer = activeLayer;

                foreach (Item item in services.Model.JointList)
                    if (item != null && item.IsSelected)
                        item.Layer = activeLayer;

                services.Model.Layers.Remove(deletedLayer);
            }
        }
    }
}
