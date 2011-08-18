using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Las piezas de acero doblado en frío tienen las mismas propiedades que el acero común.
    /// </summary>
    [Serializable]
    public class ColdFormedDesignProps : SteelDesignProps
    {

        public override string Name
        {
            get
            {
                return Culture.Get("coldFormedName");
            }
        }
    }
}
