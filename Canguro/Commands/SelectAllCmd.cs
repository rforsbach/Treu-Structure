using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to select all the Items in the Model.
    /// </summary>
    class SelectAllCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Sets the IsSelected property of all the Items to true.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            foreach (Item item in services.Model.JointList)
                if (item != null)
                    item.IsSelected = true;

            foreach (Item item in services.Model.LineList)
                if (item != null)
                    item.IsSelected = true;

            services.Model.ChangeSelection(null);
        }
    }
}

