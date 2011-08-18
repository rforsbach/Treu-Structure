using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Load;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Command to add a Distributed Line Load
    /// </summary>
    public class AddDistributedSpanLoadCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Gets the Locale dependent title for the command
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("addDistributedSpanLoad");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Creates, gets parameters and add a Distributed line load to the selected line elements.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            DistributedSpanLoad load = new DistributedSpanLoad();

            //services.GetProperties(Title, load, false);

            if (Canguro.Controller.Grid.LoadEditFrm.EditLoad(load) == System.Windows.Forms.DialogResult.OK)
            {

                List<Item> selection = services.GetSelection();

                foreach (Item item in selection)
                {
                    if (item is LineElement)
                        ((LineElement)item).Loads.Add((DistributedSpanLoad)load.Clone());
                }
            }
        }
    }
}
