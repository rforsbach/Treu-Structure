using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    [Serializable]
    public class UniaxialTypeProps : MaterialTypeProps
    {
        private float e;
        private float a;

        public UniaxialTypeProps(float e, float a)
        {
            this.e = e;
            this.a = a;
        }

        /// <summary>
        /// Modulus of Elasticity (Stress Units)
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Stress)]
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
        /// Coeff. of Thermal Expansion
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ThermalCoefficient)]
        public float A
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(a, Canguro.Model.UnitSystem.Units.ThermalCoefficient);
            }
            set
            {
                if (a != value)
                {
                    Model.Instance.Undo.Change(this, A, GetType().GetProperty("A"));
                    a = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.ThermalCoefficient);
                }
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override string Name
        {
            get { return Culture.Get("uniaxialName"); }
        }
    }
}
