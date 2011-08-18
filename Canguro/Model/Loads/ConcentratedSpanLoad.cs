using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que implementa la carga puntual sobre un elemento unidimensional (barra, cable)
    /// </summary>
    [Serializable]
    public class ConcentratedSpanLoad : DirectionalLineLoad
    {
        private float distance = 0.5F;
        private float load = 0;

        /// <summary>
        /// Distancia relativa desde el nodo I. El valor debe estar entre 0 y 1 y no tiene unidades.
        /// </summary>
        public float D
        {
            get
            {
                return distance;
            }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    Model.Instance.Undo.Change(this, distance, GetType().GetProperty("D"));
                    distance = value;
                }
            }
        }

        /// <summary>
        /// Load en el punto
        /// La carga debe estar en unidades de fuerza.
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public float L
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(load, Canguro.Model.UnitSystem.Units.Force);
            }
            set
            {
                Model.Instance.Undo.Change(this, L, GetType().GetProperty("L"));
                load = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Force);
            }
        }

        /// <summary>
        /// Gets the load magnitude in International Unit system.
        /// Used for rendering only.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        internal float LoadInt
        {
            get { return load; }
        }

        public override string ToString()
        {
            return string.Format("CL ({0:F},{1:F})", D, L);
        }
    }
}
