using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;
using Canguro.Model;

namespace Canguro.Commands.Load
{
    public class TemperatureGradientLineLoadCmd : ModelCommand
    {

        /// <summary>
        /// Executes the command. 
        /// Creates, gets parameters and add a Distributed line load to the selected line elements.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            TemperatureGradientLineLoad load = new TemperatureGradientLineLoad();

            if (Canguro.Controller.Grid.LoadEditFrm.EditLoad(load) == System.Windows.Forms.DialogResult.OK)
            {

                List<Item> selection = services.GetSelection();

                foreach (Item item in selection)
                {
                    if (item is LineElement)
                        ((LineElement)item).Loads.Add((TemperatureGradientLineLoad)load.Clone());
                }
            }
        }
    }
}
