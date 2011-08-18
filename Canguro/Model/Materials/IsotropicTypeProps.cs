using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Clase que representa las propiedades de los materiales anisotrópicos, que serán los únicos que se
    /// utilicen en la primera versión de Canguro.
    /// </summary>
    [Serializable]
    public class IsotropicTypeProps : MaterialTypeProps
    {
        private float e;
        private float nu;
        private float alpha;

        /// <summary>
        /// Constructora que recibe todas las propiedades en unidades internacionales.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="u"></param>
        /// <param name="a"></param>
        public IsotropicTypeProps(float e, float u, float a)
        {
            this.e = e;
            this.nu = u;
            this.alpha = a;
        }

        /// <summary>
        /// Modulus of Elasticity (Stress Units)
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Stress)]
        [System.ComponentModel.Description("Modulus of Elasticity")]
        public override float E
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(e, Canguro.Model.UnitSystem.Units.Stress);
            }
            set
            {
                if (value > 0 && value != e)
                {
                    Model.Instance.Undo.Change(this, E, GetType().GetProperty("E"));
                    e = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Stress);
                }
            }
        }

        /// <summary>
        /// Poisson's Ratio (sin unidades). -1 ( U ( 0.5
        /// </summary>
        [System.ComponentModel.Description("Poisson's Ratio")]
        public float Nu
        {
            get
            {
                return nu;
            }
            set
            {
                if (value > -1 && value < 0.5 && value != nu)
                {
                    Model.Instance.Undo.Change(this, nu, GetType().GetProperty("Nu"));
                    nu = value;
                }
            }
        }

        /// <summary>
        /// Coeff. of Thermal Expansion
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ThermalCoefficient)]
        [System.ComponentModel.Description("Coeff. of Thermal Expansion")]
        public float Alpha
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(alpha, Canguro.Model.UnitSystem.Units.ThermalCoefficient);
            }
            set
            {
                if (alpha != value)
                {
                    Model.Instance.Undo.Change(this, Alpha, GetType().GetProperty("Alpha"));
                    alpha = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.ThermalCoefficient);
                }
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override string Name
        {
            get { return Culture.Get("isotropicName"); }
        }
    }
}
