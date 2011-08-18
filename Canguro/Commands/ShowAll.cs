using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Unhide all the Items in the Model.
    /// </summary>
    class ShowAllCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Sets all the Items IsVisible property to true.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            foreach (Joint j in services.Model.JointList)
                if (j != null)
                    j.IsVisible = true;

            foreach (LineElement l in services.Model.LineList)
                if (l != null)
                    l.IsVisible = true;

            if (services.Model.HasResults)
                services.Model.Results.StressHelper.IsDirty = true;

            services.Model.ChangeModel();
        }
    }
}
