using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;
using Canguro.Model;

namespace Canguro.Commands.Load
{
    class UniformLineLoadCmd : Canguro.Commands.ModelCommand
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
            set { type = value; }
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
                newLoad.Db = 1;
                newLoad.Direction = direction;
                newLoad.La = load;
                newLoad.Lb = load;
                newLoad.Type = type;

                List<Item> selection = services.GetSelection();

                foreach (Item item in selection)
                {
                    if (item is LineElement)
                        ((LineElement)item).Loads.Add((DistributedSpanLoad)newLoad.Clone());
                }
            }
        }
    }
}
