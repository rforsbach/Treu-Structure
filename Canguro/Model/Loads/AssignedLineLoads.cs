using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que administra la relación entre cargas y objetos unidimensionales (barras)
    /// </summary>
    [Serializable]
    public class AssignedLineLoads : AssignedLoads
    {
        /// <summary>
        /// Carga que representa el peso propio del elemento.
        /// </summary>
        private DistributedSpanLoad weight;

        public AssignedLineLoads(Item item)
            : base(item)
        {
            if (!(item is LineElement))
                throw new InvalidItemException();
            weight = new DistributedSpanLoad();
            weight.Da = 0;
            weight.Db = 1;
            weight.Direction = LineLoad.LoadDirection.Gravity;
            weight.La = 1;
            weight.Lb = 1;
        }
    }
}
