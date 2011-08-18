using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View
{
    /// <summary>
    /// Interfaz para los objetos que se suscribirán al evento Model.ModelChanged
    /// siguiendo el patrón Observer.
    /// 
    /// Por convención se debe modificar la suscripción al evento 
    /// Model.ModelChanged al cambiar el estado de Enabled
    /// </summary>
    public interface ModelObserver
    {        
        /// <summary>
        /// Propiedad de lectura escritura para cambiar el estado del observador,
        /// así como su suscripción en Model.ModelChanged
        /// </summary>
        bool Enabled
        {
            get;
            set;
        }
    }
}
