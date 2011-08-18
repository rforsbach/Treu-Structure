using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;
using Canguro.Model;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Model Command to add a Temperature Load to selected Line Elements (Not Implemented)
    /// </summary>
    public class AddTemperatureLineLoadCmd : Canguro.Commands.ModelCommand
    {

        /// <summary>
        /// Executes the command. 
        /// Creates, gets parameters and add a Distributed line load to the selected line elements.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            TemperatureLineLoad load = new TemperatureLineLoad();

            if (Canguro.Controller.Grid.LoadEditFrm.EditLoad(load) == System.Windows.Forms.DialogResult.OK)
            {

                List<Item> selection = services.GetSelection();

                foreach (Item item in selection)
                {
                    if (item is LineElement)
                        ((LineElement)item).Loads.Add((TemperatureLineLoad)load.Clone());
                }
            }
        }
    }
}
