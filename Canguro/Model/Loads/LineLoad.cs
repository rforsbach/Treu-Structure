using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que representa cualquier carga que se le aplique a un elemento unidimensional (ej. barra).
    /// </summary>
    [Serializable]
    public class LineLoad : Load
    {
    
        /// <summary>
        /// Tipos de carga: Fuerza, Momento.
        /// </summary>
        public enum LoadType : byte
        {
            Force, Moment,
        }

        /// <summary>
        /// Dirección de la carga (eje local o global)
        /// </summary>
        public enum LoadDirection : byte
        {
            GlobalX, GlobalY, GlobalZ, Gravity, Local1, Local2, Local3,
        }
    }
}
