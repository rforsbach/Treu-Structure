using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    /// <summary>
    /// Abstracción de las propiedades de concreto. Engloba las propiedades de Column y Beam
    /// </summary>
    [Serializable]
    public class ConcreteSectionProps : Utility.GlobalizedObject, ICloneable
    {
        /// <summary>
        /// Método heredado de IClonable
        /// </summary>
        /// <returns>Regresa una copia superficial (completa en este caso) de sí mismo</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
