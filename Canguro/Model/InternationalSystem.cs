using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.UnitSystem
{
    /// <summary>
    /// Clase que representa el sistema de unidades internacional.
    /// Dado que es el sistema base, esta clase no hace nada para las conversiones, 
    /// sólo devuelve el valor dado.
    /// </summary>
    sealed public class InternationalSystem : UnitSystem
    {
        /// <summary>
        /// Constructor default
        /// </summary>
        private InternationalSystem() { }

        /// <summary>
        /// La gravedad, en sistema internacional, es de 9.81 m/s^2
        /// </summary>
        public override float Gravity
        {
            get
            {
                return 9.81F;
            }
        }

        public override string Name
        {
            get { return Culture.Get("InternationalSystem"); }
        }

        public static readonly InternationalSystem Instance = new InternationalSystem();

        #region UnitSystem Members

        /// <summary>
        /// Como el valor, independientemente de las unidades, 
        /// ya está en sistema internacional, no hace nada.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public override float ToInternational(float value, Units unit)
        {
            return (float.IsNaN(value) || float.IsInfinity(value)) ? 0 : value;
        }

        /// <summary>
        /// Como el valor, independientemente de las unidades,
        /// ya está en sistema internacional, no hace nada.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public override float FromInternational(float value, Units unit)
        {
            return (float.IsNaN(value) || float.IsInfinity(value)) ? 0 : value;
        }

        /// <summary>
        /// Devuelve el nombre de las unidades para este sistema.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public override string UnitName(Units unit)
        {
            switch (unit)
            {
                case Units.Distance: return "m";
                case Units.SmallDistance: return "m";
                case Units.Force: return "N";
                case Units.Area: return "m2";
                case Units.AreaBig: return "m2";
                case Units.Stress: return "N/m2";
                case Units.Density: return "Kg/m3";
                case Units.Temperature: return "°C";
                case Units.TemperatureGradient: return "°C/m";
                case Units.SpringTranslation: return "m";
                case Units.SpringRotation: return "m";
                case Units.Load0D: return "N";
                case Units.Load1D: return "N/m";
                case Units.Load2D: return "N/m2";
                case Units.Moment: return "Nm";
                case Units.AreaInertia: return "m4";
                case Units.Warping: return "m6";
                case Units.SmallVolume: return "m3";
                case Units.ShearModulus: return "m3";
                case Units.ThermalCoefficient: return "1/°C";
                case Units.Angle: return "Deg";
                case Units.Mass: return "Kg";
                case Units.Mass2D: return "Kg/m2";
                case Units.Velocity: return "m/s";
                case Units.Acceleration: return "m/s2";
                case Units.Time: return "s";
                case Units.NoUnit: return "";
                case Units.MassInertia: return "N*m*s2";
                case Units.ParTranslation: return "N*s2";
                case Units.ForceMoment: return "N*m";
                case Units.Frequency: return "1/s";
                case Units.CircFreq: return "rad/s";
                case Units.CircFreq2: return "rad2/s2";
                default: return "";
            }
        }

        #endregion
    }
}