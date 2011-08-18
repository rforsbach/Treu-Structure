using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Load;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Command for creating two distributed loads in the shape of a triangle.
    /// </summary>
    public class TriangleLineLoadCmd : ModelCommand
    {
        LineLoad.LoadDirection direction = LineLoad.LoadDirection.Gravity;
        public LineLoad.LoadDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        LineLoad.LoadType type = LineLoad.LoadType.Force;
        public LineLoad.LoadType Type
        {
            get { return type; }
        }

        float load = 1;
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Load1D)]
        public float Load
        {
            get { return load; }
            set { load = value; }
        }

        /// <summary>
        /// Executes the command. 
        /// Creates, gets parameters and add a Distributed line load to the selected line elements.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            if (Canguro.Controller.Grid.LoadEditFrm.EditLoad(this) == System.Windows.Forms.DialogResult.OK)
            {
                DistributedSpanLoad newLoad = new DistributedSpanLoad();
                newLoad.Da = 0;
                newLoad.Db = 0.5f;
                newLoad.Direction = direction;
                newLoad.La = 0;
                newLoad.Lb = load;
                newLoad.Type = type;
                DistributedSpanLoad newLoad2 = new DistributedSpanLoad();
                newLoad2.Da = 0.5f;
                newLoad2.Db = 1;
                newLoad2.Direction = direction;
                newLoad2.La = load;
                newLoad2.Lb = 0;
                newLoad2.Type = type;

                List<Item> selection = services.GetSelection();

                foreach (Item item in selection)
                {
                    if (item is LineElement)
                    {
                        ((LineElement)item).Loads.Add((DistributedSpanLoad)newLoad.Clone());
                        ((LineElement)item).Loads.Add((DistributedSpanLoad)newLoad2.Clone());
                    }
                }
            }
        }
    }
}
