using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    [Serializable]
    public class GroundDisplacementLoad : JointLoad
    {
        /// <summary>
        /// Gets the 6 DoF ground displacement in International Unit system.
        /// Used for rendering only.
        /// Warning: changing this will afect the model.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        internal float[] Displacements
        {
            get { return loadComponents; }
        }

        /// <summary>
        /// Propiedad de desplazamiento en el eje X
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Tx
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[0], Canguro.Model.UnitSystem.Units.Distance);
            }
            set
            {
                Model.Instance.Undo.Change(this, Tx, GetType().GetProperty("Tx"));
                loadComponents[0] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Distance);
            }
        }

        /// <summary>
        /// Propiedad de desplazamiento en el eje Y
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Ty
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[1], Canguro.Model.UnitSystem.Units.Distance);
            }
            set
            {
                Model.Instance.Undo.Change(this, Ty, GetType().GetProperty("Ty"));
                loadComponents[1] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Distance);
            }
        }

        /// <summary>
        /// Propiedad de desplazamiento en el eje Z
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float Tz
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[2], Canguro.Model.UnitSystem.Units.Distance);
            }
            set
            {
                Model.Instance.Undo.Change(this, Tz, GetType().GetProperty("Tz"));
                loadComponents[2] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Distance);
            }
        }

        /// <summary>
        /// Propiedad de desplazamiento en el eje X
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public float Rx
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[3], Canguro.Model.UnitSystem.Units.Angle);
            }
            set
            {
                Model.Instance.Undo.Change(this, Rx, GetType().GetProperty("Rx"));
                loadComponents[3] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Angle);
            }
        }

        /// <summary>
        /// Propiedad de desplazamiento en el eje Y
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public float Ry
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[4], Canguro.Model.UnitSystem.Units.Angle);
            }
            set
            {
                Model.Instance.Undo.Change(this, Ry, GetType().GetProperty("Ry"));
                loadComponents[4] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Angle);
            }
        }

        /// <summary>
        /// Propiedad de desplazamiento en el eje Z
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public float Rz
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loadComponents[5], Canguro.Model.UnitSystem.Units.Angle);
            }
            set
            {
                Model.Instance.Undo.Change(this, Rz, GetType().GetProperty("Rz"));
                loadComponents[5] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Angle);
            }
        }

        public override string ToString()
        {
            return string.Format("GD ({0:F},{1:F},{2:F})", Tx, Ty, Tz);
        }
    }
}
