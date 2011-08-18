using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;
using Canguro.Model;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Model Command to add a Restraint or Ground Displacement to selected Joints
    /// </summary>
    public class AddRestraintDisplacementLoadCmd : Canguro.Commands.ModelCommand
    {
        public override string Title
        {
            get
            {
                return Culture.Get("addRestraintDisplacement");
            }
        }

        public override void Run(Canguro.Controller.CommandServices services)
        {
             GroundDisplacementLoad load = new GroundDisplacementLoad();

             //services.GetProperties(Title, load, false);

             if (Canguro.Controller.Grid.LoadEditFrm.EditLoad(load) == System.Windows.Forms.DialogResult.OK)
             {
                 List<Item> selection = services.GetSelection();

                 foreach (Item item in selection)
                 {
                     if (item is Joint)
                         ((Joint)item).Loads.Add((GroundDisplacementLoad)load.Clone());
                 }
             }
         }
    }
}
