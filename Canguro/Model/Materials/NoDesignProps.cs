using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Esta clase define materiales sin propiedades específicas de diseño (materiales generals).
    /// Se usa cuando se requiere de análisis pero no de diseño.
    /// </summary>
    [Serializable]
    public class NoDesignProps : MaterialDesignProps
    {
        /// <summary>
        /// La única propiedad de esta clase es el nombre, que se define para cada cultura.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public override string Name
        {
            get
            {
                return Culture.Get("noMaterialName");
            }
        }
    }
}
