using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.UnitSystem
{
    /// <summary>
    /// Clase que representa el sistema de métrico
    /// </summary>
    sealed public class MetricSystem : UnitSystem
    {
        /// <summary>
        /// Constructor default
        /// </summary>
        private MetricSystem() { }

        /// <summary>
        /// La gravedad, en sistema métrico, es de 9.81 m/s^2
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
            get { return Culture.Get("MetricSystem"); }
        }
    
        public static readonly MetricSystem Instance = new MetricSystem();

        #region UnitSystem Members

        /// <summary>
        /// Realiza la conversión de sistema métrico a sistema internacional
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public override float ToInternational(float value, Units unit)
        {
            if ((float.IsNaN(value) || float.IsInfinity(value)))
                return 0;
            switch (unit)
            {
                case Units.Distance: return value; // m
                case Units.SmallDistance: return value / 100f; // cm
                case Units.Force: return value * 1000f * Gravity; // Ton
                case Units.Area: return value / 10000f; // cm2
                case Units.AreaBig: return value; // m2
                case Units.Stress: return value * 10000f * Gravity; // "Ton/m2" changed to Kg/cm2
                case Units.Density: return value * 1000f; // "Ton/m3";
                case Units.Temperature: return value; // "C";
                case Units.TemperatureGradient: return value * 100f; // "C/cm";
                case Units.SpringTranslation: return value * 1000f * Gravity; // "Ton/m";
                case Units.SpringRotation: return value * 1000f * Gravity; // "Ton/Deg";
                case Units.Load0D: return value * 1000f * Gravity; // "Ton";
                case Units.Load1D: return value * 1000f * Gravity; // "Ton/m";
                case Units.Load2D: return value * 1000f * Gravity; // "Ton/m2";
                case Units.Moment: return value * 1000f * Gravity; // "Ton*m";
                case Units.AreaInertia: return value * (100000000f); // "cm4";
                case Units.Warping: return value * (1000000000000f); // "cm6";
                case Units.SmallVolume: return value / (1000000f); // "cm3";
                case Units.ShearModulus: return value / (1000000f); // "cm3";
                case Units.ThermalCoefficient: return value; // "1/C";
                case Units.Angle: return value; // "Deg";
                case Units.Mass: return value; // "Kg";
                case Units.Mass2D: return value / 1000f; // Ton/m2
                case Units.Velocity: return value; // "m/s";
                case Units.Acceleration: return value; // "m/s2";
                case Units.NoUnit: return value;
                case Units.Time: return value; // s
                case Units.MassInertia: return value * Gravity; // "Kg*m*s2";
                case Units.ParTranslation: return value * Gravity; // "Kg*s2";
                case Units.ForceMoment: return value * Gravity; // "Kg*m";
                case Units.Frequency: return value; // "1/s";
                case Units.CircFreq: return value; //  "rad/s";
                case Units.CircFreq2: return value; // "rad2/s2"; 
                default: return value;
            }
        }

        /// <summary>
        /// Realiza la conversión de sistema internacional al sistema métrico  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public override float FromInternational(float value, Units unit)
        {
            if ((float.IsNaN(value) || float.IsInfinity(value)))
                return 0;
            switch (unit)
            {
                case Units.Distance: return value; // m
                case Units.SmallDistance: return value * 100f; // cm
                case Units.Force: return (float)Math.Round(value / (1000f * Gravity), fromInternationalPrecision); // Tonf
                case Units.Area: return value * 10000f; // cm2
                case Units.AreaBig: return value; // m2
                case Units.Stress: return (float)Math.Round(value / (10000f * Gravity), fromInternationalPrecision); // "Tonf/m2" changed to Kgf/cm2
                case Units.Density: return value / (1000f); // "Ton/m3";
                case Units.Temperature: return value; // "C";
                case Units.TemperatureGradient: return value / 100f; // "C/cm";
                case Units.SpringTranslation: return (float)Math.Round(value / (1000f * Gravity), fromInternationalPrecision); // "Tonf/m";
                case Units.SpringRotation: return (float)Math.Round(value / (1000f * Gravity), fromInternationalPrecision); // "Tonf*m/Deg";
                case Units.Load0D: return (float)Math.Round(value / (1000f * Gravity), fromInternationalPrecision); // "Tonf";
                case Units.Load1D: return (float)Math.Round(value / (1000f * Gravity), fromInternationalPrecision); // "Tonf/m";
                case Units.Load2D: return (float)Math.Round(value / (1000f * Gravity), fromInternationalPrecision); // "Tonf/m2";
                case Units.Moment: return (float)Math.Round(value / (1000f * Gravity), fromInternationalPrecision); // "Tonf*m";
                case Units.AreaInertia: return value / (100000000f); // "cm4";
                case Units.Warping: return value / (1000000000000f); // "cm6";
                case Units.SmallVolume: return value * (1000000f); // "cm3";
                case Units.ShearModulus: return value * (1000000f); // "cm3";
                case Units.ThermalCoefficient: return value; // "1/C";
                case Units.Angle: return value; // "Deg";
                case Units.Mass: return value; // "Kg";
                case Units.Velocity: return value; // "m/s";
                case Units.Acceleration: return value; // "m/s2";
                case Units.NoUnit: return value;
                case Units.Time: return value; // s
                case Units.MassInertia: return (float)Math.Round(value / Gravity, fromInternationalPrecision);// "Kgf*m*s2";
                case Units.ParTranslation: return (float)Math.Round(value / Gravity, fromInternationalPrecision); //"Kgf*s2";
                case Units.Mass2D: return (float)Math.Round(value / 1000f, fromInternationalPrecision); // "Ton/m2";
                case Units.ForceMoment: return (float)Math.Round(value / Gravity, fromInternationalPrecision); // "Kgf*m";
                case Units.Frequency: return value; // "1/s";
                case Units.CircFreq: return value; // "rad/s";
                case Units.CircFreq2: return value; // "rad2/s2"; 
                default: return value;
            }
        }


        /// <summary>
        /// Returns the name of the unit for this system.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public override string UnitName(Units unit)
        {
            switch (unit)
            {
                case Units.Distance: return "m";
                case Units.SmallDistance: return "cm";
                case Units.Force: return "Ton";
                case Units.Area: return "cm2";
                case Units.AreaBig: return "m2";
                case Units.Stress: return "Kg/cm2";// "Ton/m2";
                case Units.Density: return "Ton/m3";
                case Units.Temperature: return "°C";
                case Units.TemperatureGradient: return "°C/cm";
                case Units.SpringTranslation: return "Ton/m";
                case Units.SpringRotation: return "Ton*m/Deg";
                case Units.Load0D: return "Ton";
                case Units.Load1D: return "Ton/m";
                case Units.Load2D: return "Ton/m2";
                case Units.Moment: return "Ton*m";
                case Units.AreaInertia: return "cm4";
                case Units.Warping: return "cm6";
                case Units.SmallVolume: return "cm3";
                case Units.ShearModulus: return "cm3";
                case Units.ThermalCoefficient: return "1/C";
                case Units.Angle: return "Deg";
                case Units.Mass: return "Kg";
                case Units.Mass2D: return "Ton/m2";
                case Units.Velocity: return "m/s";
                case Units.Acceleration: return "m/s2";
                case Units.Time: return "s";
                case Units.NoUnit: return "";
                case Units.MassInertia:  return "Kg*m*s2";
                case Units.ParTranslation: return "Kg*s2";
                case Units.ForceMoment: return "Kg*m";
                case Units.Frequency: return "1/s";
                case Units.CircFreq: return "rad/s";
                case Units.CircFreq2: return "rad2/s2"; 
                default: return "";
            }
        }

        #endregion
    }
}