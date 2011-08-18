using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to scale the selected elements given a scale factor.
    /// This command only moves the Joints connecting the elements.
    /// </summary>
    public class ScaleCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Returns the Culture dependent title of the Command under the key "scaleTitle"
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("scaleTitle");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Moves the joint according to a given scale factor and pivot point.
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

            Microsoft.DirectX.Vector3 piv;
            float scale = services.GetSingle(Culture.Get("getScale"));

            Controller.Snap.Magnet m = services.GetPoint(Culture.Get("pivotScalePoint"));
            if (m == null) return;
            piv = m.SnapPosition;

            foreach (Canguro.Model.Joint j in selection)
            {
                j.X = (j.X - piv.X) * scale + piv.X;
                j.Y = (j.Y - piv.Y) * scale + piv.Y;
                j.Z = (j.Z - piv.Z) * scale + piv.Z;
            }


        }
    }
}
