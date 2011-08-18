using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que administra la relación entre cargas y objetos bidimensionales (areas)
    /// </summary>
    [Serializable]
    public class AssignedAreaLoads : AssignedLoads
    {
        /// <summary>
        /// Carga que representa el peso propio del elemento.
        /// </summary>
        private UniformLoad weight;

        public AssignedAreaLoads(Item item)
            : base(item)
        {
            if (!(item is AreaElement))
                throw new InvalidItemException();
            weight = new UniformLoad();

            // TODO: Add specific variables for this uniform load to represent self weight           
            //weight.Da = 0;
            //weight.Db = 1;
            //weight.Direction = LineLoad.LoadDirection.Gravity;
            //weight.La = 1;
            //weight.Lb = 1;
        }
    }
}
