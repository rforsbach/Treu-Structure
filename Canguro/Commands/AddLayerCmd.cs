using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to add a new Layer to the Model
    /// </summary>
    public class AddLayerCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Adds a new Layer with the given Name and sets it as Active.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            services.StoreSelection();
            string name = services.GetString(Culture.Get("setLayerName"));
            string aux = name;
            bool valid = false;
            int i = 1;
            while (!valid)
            {
                valid = true;
                foreach (Layer l in services.Model.Layers)
                    if (l != null && l.Name.Equals(aux))
                        valid = false;
                if (!valid)
                    aux = name + "(" + i++ + ")";
            }
            Layer layer = new Layer(aux);
            services.Model.Layers.Add(layer);
            services.Model.ActiveLayer = layer;

            services.RestoreSelection();
        }
    }
}
