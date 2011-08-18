using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que representa la relación entre Joints, JointLoads y LoadCases
    /// </summary>
    [Serializable]
    public class AssignedJointLoads : AssignedLoads
    {
        /// <summary>
        /// Constructora que revisa que el item recibido sea un Joint. 
        /// Lanza una excepción si esto no ocurre.
        /// </summary>
        /// <param name="item"></param>
        public AssignedJointLoads(Item item) 
            : base(item)
        {
            if (!(item is Joint))
                throw new InvalidItemException();
        }
    }
}
