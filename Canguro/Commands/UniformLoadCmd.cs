using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;
using Canguro.Model;

namespace Canguro.Commands.Load
{
    class UniformLoadCmd : Canguro.Commands.ModelCommand
    {
        LineLoad.LoadDirection direction;
        public LineLoad.LoadDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        LineLoad.LoadType type;
        public LineLoad.LoadType Type
        {
            get { return type; }
            set { type = value; }
        }

        float load;
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
            services.GetProperties(this);
            DistributedSpanLoad newLoad = new DistributedSpanLoad();
            newLoad.Da = 0;
            newLoad.Db = 1;
            newLoad.Direction = direction;
            newLoad.La = load;
            newLoad.Lb = load;
            newLoad.Type = type;

            //services.GetProperties(Title, load, false);

            if (Canguro.Controller.Grid.LoadEditFrm.EditLoad(newLoad) == System.Windows.Forms.DialogResult.OK)
            {
                List<Item> selection = GetSelection(services);

                foreach (Item item in selection)
                {
                    if (item is LineElement)
                        ((LineElement)item).Loads.Add((DistributedSpanLoad)newLoad.Clone());
                }
            }
        }
    }
}
