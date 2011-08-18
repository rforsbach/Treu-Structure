using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Move the selected items according to a given vector.
    /// </summary>
    public class MoveCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Moves the selected Item's Joint according to a given vector.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            List<Canguro.Model.Joint> selection = new List<Canguro.Model.Joint>();
            List<Item> selectedItems = services.GetSelection();
            if (selectedItems.Count == 0)
                return;

            foreach (Item item in selectedItems)
            {
                if (item is Joint)
                    selection.Add((Joint)item);
                else if (item is LineElement)
                {
                    LineElement l = (LineElement)item;
                    if (!selection.Contains(l.I))
                        selection.Add(l.I);
                    if (!selection.Contains(l.J))
                        selection.Add(l.J);
                }
            }

            Microsoft.DirectX.Vector3 v;

            if (selection.Count > 0 && services.GetVector(out v))
            {
                foreach (Joint j in selection)
                {
                    j.X += v.X;
                    j.Y += v.Y;
                    j.Z += v.Z;
                }
            }
        }
    }
}
