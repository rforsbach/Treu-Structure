using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    /// <summary>
    /// Clase abstracta que agrupa a las clases de propiedades de elementos unidimensionales: LineElement
    /// Implementa Clone porque se usa como prototipo en LineElement
    /// </summary>
    [Serializable]
    public abstract class LineProps : ICloneable
    {
        /// <summary>
        /// Método heredado de IClonable
        /// </summary>
        /// <returns>Regresa una copia superficial (completa en este caso) de sí mismo</returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}