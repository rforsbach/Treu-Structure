using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Clase que representa una carga distribuida linealmente (carga trapezoidal) en un elemento lineal (ej. barra)
    /// </summary>
    [Serializable]
    public class DistributedSpanLoad : DirectionalLineLoad
    {
        private float[] distances = new float[2] { 0, 1 };
        private float[] loads = new float[2];

        /// <summary>
        /// Distancia Relativa A desde el Nodo I.
        /// 0 &lt; value &lt; 1
        /// </summary>
        public float Da
        {
            get
            {
                return distances[0];
            }
            set
            {
                if (value >= Db)
                {
                    Db = value + 0.000001f;
                    value = Db - 0.000001f;
                }
                else if (value < 0)
                    value = 0;

                Model.Instance.Undo.Change(this, distances[0], GetType().GetProperty("Da"));
                distances[0] = value;
            }
        }

        /// <summary>
        /// Distancia Relativa B desde el Nodo I
        /// </summary>
        public float Db
        {
            get
            {
                return distances[1];
            }
            set
            {
                if (value <= Da)
                {
                    Da = value - 0.000001f;
                    value = Da + 0.0000001f;
                }
                if (value > 1)
                    value = 1;

                Model.Instance.Undo.Change(this, distances[1], GetType().GetProperty("Db"));
                distances[1] = value;
            }
        }

        /// <summary>
        /// Load en el punto A
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Load1D)]
        public float La
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loads[0], Canguro.Model.UnitSystem.Units.Load1D);
            }
            set
            {
                Model.Instance.Undo.Change(this, La, GetType().GetProperty("La"));
                loads[0] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Load1D);
                if (loads[0] != 0 && Math.Sign(loads[0]) != Math.Sign(loads[1]))
                    loads[1] *= -1;
            }
        }

        /// <summary>
        /// Load en el punto B
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Load1D)]
        public float Lb
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(loads[1], Canguro.Model.UnitSystem.Units.Load1D);
            }
            set
            {
                Model.Instance.Undo.Change(this, Lb, GetType().GetProperty("Lb"));
                loads[1] = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Load1D);
                if (loads[1] != 0 && Math.Sign(loads[0]) != Math.Sign(loads[1]))
                    loads[0] *= -1;
            }
        }


        /// <summary>
        /// Gets the load magnitude in International Unit system.
        /// Used for rendering only.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        internal float LoadAInt
        {
            get { return loads[0]; }
        }

        /// <summary>
        /// Gets the load magnitude in International Unit system.
        /// Used for rendering only.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        internal float LoadBInt
        {
            get { return loads[1]; }
        }

        public override object Clone()
        {
            object o = base.Clone();
            ((DistributedSpanLoad)o).distances = (float[])distances.Clone();
            ((DistributedSpanLoad)o).loads = (float[])loads.Clone();

            return o;
        }

        public override string ToString()
        {
            return string.Format("DL ({0:F},{1:F})", La, Lb);
        }
    }
}
