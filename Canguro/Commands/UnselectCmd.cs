using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Deselect all the Items in the Model.
    /// </summary>
    class UnselectCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Sets all items IsSelected property to false.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            foreach (Item item in services.Model.JointList)
                if (item != null)
                    item.IsSelected = false;

            foreach (Item item in services.Model.LineList)
                if (item != null)
                    item.IsSelected = false;

            services.Model.ChangeModel();
        }
    }
}

