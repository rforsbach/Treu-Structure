using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;
using Canguro.Model;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Model Command to Add a Spring or Ground Displacement to selected Joints (Same as AddRestraintDisplacementLoadCmd)
    /// </summary>
    public class AddSpringDisplacementLoadCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Returns the Culture dependent title of the Command under the key "addSpringDisplacementLoad"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("addSpringDisplacementLoad");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Asks the User for parameters and adds the Load to all selected Joints.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            GroundDisplacementLoad load = new GroundDisplacementLoad();

            services.GetProperties(Title, load);

            Joint joint;
            while ((joint = services.GetJoint()) != null)
            {
                // TODO: Checar validez
                joint.Loads.Add(load);
                // Para que se refleje el cambio inmediatamente
                services.Model.ChangeModel();
            }
        }
    }
}
