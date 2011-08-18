using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Clase que representa las propiedades de diseño específicas del Acero
    /// </summary>
    [Serializable]
    public class SteelDesignProps : MaterialDesignProps
    {
        protected float fy = 248211284.023F;
        protected float fu = 399895957.591F;

        public SteelDesignProps() { }

        public SteelDesignProps(float fyInt, float fuInt)
        {
            fy = fyInt;
            fu = fuInt;
        }

        /// <summary>
        /// Minimum Yield Stress
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Stress)]
        [System.ComponentModel.Description("Minimum Yield Stress")]
        public float Fy
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(fy, Canguro.Model.UnitSystem.Units.Stress);
            }
            set
            {
                if (fy != value)
                {
                    Model.Instance.Undo.Change(this, Fy, GetType().GetProperty("Fy"));
                    fy = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Stress);
                }
            }
        }

        /// <summary>
        /// Minimum Tensile Stress
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Stress)]
        [System.ComponentModel.Description("Minimum Tensile Stress")]
        public float Fu
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(fu, Canguro.Model.UnitSystem.Units.Stress);
            }
            set
            {
                if (fu != value)
                {
                    Model.Instance.Undo.Change(this, Fu, GetType().GetProperty("Fu"));
                    fu = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Stress);
                }
            }
        }

        /// <summary>
        /// El nombre del material depende de la cultura.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public override string Name
        {
            get
            {
                return Culture.Get("steelName");
            }
        }
    }
}
