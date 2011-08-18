using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Command to delete all the selected items.
    /// </summary>
    public class DeleteCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Deletes all the selected items except joints connected to undeleted elements.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            List<Item> selection = services.GetSelection();
            ItemList<Joint> jList = Canguro.Model.Model.Instance.JointList;
            ItemList<LineElement> lList = Canguro.Model.Model.Instance.LineList;
            ItemList<AreaElement> aList = Canguro.Model.Model.Instance.AreaList;
            bool[] hasElement = new bool[jList.Count];
            int size;

            size = lList.Count; 
            for (int i=1; i<size; i++)
            {
                LineElement obj = lList[i];
                if (obj != null)
                {
                    if (obj.IsSelected)
                        lList.Remove(obj);
                    else
                    {
                        hasElement[obj.I.Id] = true;
                        hasElement[obj.J.Id] = true;
                    }
                }
            }

            size = aList.Count;
            for (int i = 1; i < size; i++)
            {
                AreaElement obj = aList[i];
                if (obj != null)
                {
                    if (obj.IsSelected)
                        aList.Remove(obj);
                    else
                    {
                        hasElement[obj.J1.Id] = true;
                        hasElement[obj.J2.Id] = true;
                        hasElement[obj.J3.Id] = true;
                        hasElement[obj.J4.Id] = true;
                    }
                }
            }

            size = jList.Count; 
            for (int i = 1; i < size; i++)
            {
                Joint obj = jList[i];
                if (i == 500)
                    i = 500;
                if (obj != null && obj.IsSelected && !hasElement[obj.Id])
                    jList.Remove(obj);
            }
        }
    }
}
