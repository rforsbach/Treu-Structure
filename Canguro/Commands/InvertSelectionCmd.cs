using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to select all unselected items and deselect the rest.
    /// </summary>
    class InvertSelectionCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Selects all unselected items and deselect the rest.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            foreach (Item item in services.Model.JointList)
                if (item != null)
                    item.IsSelected = !item.IsSelected;

            foreach (Item item in services.Model.LineList)
                if (item != null)
                    item.IsSelected = !item.IsSelected;

            services.Model.ChangeSelection(null);
        }
    }
}
