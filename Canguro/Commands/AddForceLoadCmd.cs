using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;
using Canguro.Model;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Command to add a Force Load to selected Joints.
    /// </summary>
    public class AddForceLoadCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Returns the Culture dependent title of the Command under the key "addForceLoad"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("addForceLoad");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Asks the user for parameters and adds a Force Load to all Selected Joints.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            ForceLoad load = new ForceLoad();

            //services.GetProperties(Title, load, false);

            if (Canguro.Controller.Grid.LoadEditFrm.EditLoad(load) == System.Windows.Forms.DialogResult.OK)
            {

                List<Item> selection = services.GetSelection();

                foreach (Item item in selection)
                {
                    if (item is Joint)
                        ((Joint)item).Loads.Add((ForceLoad)load.Clone());
                }
                services.Model.ChangeModel();
            }
        }
    }
}
