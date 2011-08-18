using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to hide all selected Items
    /// </summary>
    class HideSelectionCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Sets the IsSelected property of all the Items to false.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            foreach (Joint j in services.Model.JointList)
                if (j != null && !j.IsSelected)
                    j.IsVisible = j.IsSelected = false;

            foreach (LineElement l in services.Model.LineList)
                if (l != null && !l.IsSelected)
                    l.IsVisible = l.IsSelected = false;

            if (services.Model.HasResults)
                services.Model.Results.StressHelper.IsDirty = true;

            services.Model.ChangeModel();
        }
    }
}
