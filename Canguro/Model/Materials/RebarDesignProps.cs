using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Las varillas tienen las mismas propiedades de diseño que el acero cumún (son de acero).
    /// </summary>
    [Serializable]
    public class RebarDesignProps : SteelDesignProps
    {
        public RebarDesignProps()
        {
            fy = 413685473.370947F;
            fu = 620528210.05642F;
        }

        /// <summary>
        /// El nombre del material depende de la cultura.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public override string Name
        {
            get
            {
                return Culture.Get("rebarName");
            }
        }
    }
}
