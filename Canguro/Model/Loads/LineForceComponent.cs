using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Enumeración de las 6 componentes en fuerzas de un elemento lineal.
    /// </summary>
    public enum LineForceComponent : byte
    {
        Axial       = 0, 
        Shear22     = 1,
        Shear33     = 2,
        Torsion     = 3,
        Moment22    = 4, 
        Moment33    = 5
    }

}
