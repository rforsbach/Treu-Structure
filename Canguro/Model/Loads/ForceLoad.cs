using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que implementa la carga sobre nodos.
    /// </summary>
    [Serializable]
    public class ForceLoad : JointLoad
    {
        /// <summary>
        /// Gets the 6 DoF force in International Unit system.
        /// Used for rendering only.
        /// Warning: changing this will afect the model.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        internal float[] Force
        {
            get { return loadComponents; }
        }

        /// <summary>
        /// Propiedad de fuerza en el eje X
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public float Fx
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[0], Canguro.Model.UnitSystem.Units.Force);
            }
            set
            {
                Model.Instance.Undo.Change(this, Fx, GetType().GetProperty("Fx"));
                loadComponents[0] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Force);
            }
        }

        /// <summary>
        /// Propiedad de fuerza en el eje Y
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public float Fy
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[1], Canguro.Model.UnitSystem.Units.Force);
            }
            set
            {
                Model.Instance.Undo.Change(this, Fy, GetType().GetProperty("Fy"));
                loadComponents[1] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Force);
            }
        }

        /// <summary>
        /// Propiedad de fuerza en el eje Z
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public float Fz
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[2], Canguro.Model.UnitSystem.Units.Force);
            }
            set
            {
                Model.Instance.Undo.Change(this, Fz, GetType().GetProperty("Fz"));
                loadComponents[2] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Force);
            }
        }

        /// <summary>
        /// Propiedad de momento sobre el eje X
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public float Mx
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[3], Canguro.Model.UnitSystem.Units.Moment);
            }
            set
            {
                Model.Instance.Undo.Change(this, Mx, GetType().GetProperty("Mx"));
                loadComponents[3] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Moment);
            }
        }

        /// <summary>
        /// Propiedad de momento sobre el eje Y
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public float My
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[4], Canguro.Model.UnitSystem.Units.Moment);
            }
            set
            {
                Model.Instance.Undo.Change(this, My, GetType().GetProperty("My"));
                loadComponents[4] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Moment);
            }
        }

        /// <summary>
        /// Propiedad de momento sobre el eje Z
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public float Mz
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[5], Canguro.Model.UnitSystem.Units.Moment);
            }
            set
            {
                Model.Instance.Undo.Change(this, Mz, GetType().GetProperty("Mz"));
                loadComponents[5] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Moment);
            }
        }

        public override string ToString()
        {
            return string.Format("FL ({0:F},{1:F},{2:F})", Fx, Fy, Fz);
        }
    }
}
