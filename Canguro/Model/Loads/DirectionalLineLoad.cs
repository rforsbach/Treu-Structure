using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    [Serializable]
    public class DirectionalLineLoad : LineLoad
    {
        private LoadType type = LoadType.Force;
        private LoadDirection direction = LoadDirection.Gravity;

        /// <summary>
        /// Propiedad que representa el tipo de carga: Fuerza o Momento
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public LoadType Type
        {
            get
            {
                return type;
            }
            set
            {
                Model.Instance.Undo.Change(this, type, GetType().GetProperty("Type"));
                type = value;
            }
        }

        /// <summary>
        /// Propiedad que representa la dirección de la carga: X, Y, Z, -Z, local, global.
        /// </summary>
        public LoadDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                Model.Instance.Undo.Change(this, direction, GetType().GetProperty("Direction"));
                direction = value;
            }
        }
    }
}
