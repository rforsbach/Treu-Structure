using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.Model
{
    class FlipAreaJoints : Canguro.Commands.ModelCommand
    {
        private FlipAreaJoints() { }
        public static readonly FlipAreaJoints Instance = new FlipAreaJoints();

        /// <summary>
        /// Returns the Culture dependent title of the Command under the key "addJoint"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("flipAreaJoints");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Flips the joints in all selected AreaElements
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Canguro.Model.AreaElement area;
            List<Canguro.Model.Item> selection = services.GetSelection();

            foreach (Canguro.Model.Item item in selection)
            {
                if ((area = item as Canguro.Model.AreaElement) != null)
                    area.FlipJoints++;
            }

            //// Para que se refleje el cambio
            services.Model.ChangeModel();
        }
    }
}
