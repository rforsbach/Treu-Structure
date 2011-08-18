using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Propiedades de diseño para el concreto. Son las más complejas de las propiedades de diseño,
    /// ya que incluyen más opciones y propiedades.
    /// </summary>
    [Serializable]
    public class ConcreteDesignProps : MaterialDesignProps
    {
        private float fc;
        private float rebarFy;
        private float rebarFys;
        private bool isLightweightConcrete;
        private float lightweightFactor;

        /// <summary>
        /// Constructora que recibe todas las propiedades en unidades internacionales.
        /// </summary>
        /// <param name="fc"></param>
        /// <param name="rebarFy"></param>
        /// <param name="rebarFys"></param>
        /// <param name="isLightweight"></param>
        /// <param name="lwFactor"></param>
        public ConcreteDesignProps(float fc, float rebarFy, float rebarFys, bool isLightweight, float lwFactor)
        {
            this.fc = fc;
            this.rebarFy = rebarFy;
            this.rebarFys = rebarFys;
            this.isLightweightConcrete = isLightweight;
            this.lightweightFactor = lwFactor;
        }

        [System.ComponentModel.Browsable(false)]
        public override string Name
        {
            get
            {
                return Culture.Get("concreteName");
            }
        }

        /// <summary>
        /// Specified Concrete Comp Strength
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Stress)]
        [System.ComponentModel.Description("Specified Concrete Comp Strength")]
        public float Fc
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(fc, Canguro.Model.UnitSystem.Units.Stress);
            }
            set
            {
                fc = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Stress);
            }
        }

        /// <summary>
        /// Bending Reinf. Yield Stress
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Stress)]
        [System.ComponentModel.Description("Bending Reinforcement Yield Stress")]
        public float RebarFy
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(rebarFy, Canguro.Model.UnitSystem.Units.Stress);
            }
            set
            {
                rebarFy = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Stress);
            }
        }

        /// <summary>
        /// Shear Reinf. Yield Stress
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Stress)]
        [System.ComponentModel.Description("Shear Reinforcement Yield Stress")]
        public float RebarFys
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(rebarFys, Canguro.Model.UnitSystem.Units.Stress);
            }
            set
            {
                rebarFys = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Stress);
            }
        }

        /// <summary>
        /// Is Lightweight Concrete?
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public bool IsLightweightConcrete
        {
            get
            {
                return isLightweightConcrete;
            }
            set
            {
                isLightweightConcrete = value;
            }
        }

        /// <summary>
        /// Shear Strength Reduction Factor
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public float LightweightFactor
        {
            get
            {
                return lightweightFactor;
            }
            set
            {
                lightweightFactor = value;
            }
        }
    }
}
