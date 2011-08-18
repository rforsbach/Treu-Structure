using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.UnitSystem
{
    /// <summary>
    /// From http://www.tecgraf.puc-rio.br/ftool/unitsinftooleng.html
    /// Second revision: http://www.wsdot.wa.gov/Reference/metrics/factors.htm
    /// Clase que representa el sistema de Inglés de Unidades.
    /// </summary>
    sealed public class EnglishSystem : UnitSystem
    {
        /// <summary>
        /// Constructor default
        /// </summary>
        private EnglishSystem() { }

        /// <summary>
        /// La gravedad en sistema inglés es 32 ft/s^2
        /// </summary>
        public override float Gravity
        {
            get
            {
                return 32f;
            }
        }

        public override string Name
        {
            get { return Culture.Get("EnglishSystem"); }
        }

        public static readonly EnglishSystem Instance = new EnglishSystem();

        #region UnitSystem Members


        private const float ft = 0.3048f;
        private const float inch = 0.0254f;
        private const float kip = 4448.222f;
        private const float lb = 0.4535924f;

        /// <summary>
        /// Realiza la conversión de sistema inglés a sistema internacional
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
                case Units.Distance: return value * ft; // "ft";
                case Units.SmallDistance: return value * inch; // "in";
                case Units.Force: return value * kip; // "Kip";
                case Units.Area: return value * (inch * inch); // "in2";
                case Units.AreaBig: return value * (ft * ft); // "ft2";
                case Units.Stress: return value * (kip / (inch * inch)); // "Kip/in2";
                case Units.Density: return value * (lb / (inch * inch * inch)); // "lb/in3";
                case Units.Temperature: return (value - 32f) / 1.8f; // "F";
                case Units.TemperatureGradient: return (value * inch - 32f) / 1.8f; // "F/in"
                case Units.SpringTranslation: return value * (kip / inch); // "Kip/in";
                case Units.SpringRotation: return value * (kip * ft); //"Kip*ft/deg";
                case Units.Load0D: return value * kip;  //"Kip";
                case Units.Load1D: return value * (kip / ft); //"Kip/ft";
                case Units.Load2D: return value * (kip / (ft * ft)); //"Kip/in2"
                case Units.Moment: return value * (kip * inch); //"Kip*in";
                case Units.AreaInertia: return value * (inch * inch * inch * inch); // "in4";
                case Units.Warping: return value * (inch * inch * inch * inch * inch * inch); // "in6";
                case Units.SmallVolume: return value * (inch * inch * inch); //"in3";
                case Units.ShearModulus: return value / (inch * inch); // "1/in2";
                case Units.ThermalCoefficient: return value / 1.8f; //"1/°F";
                case Units.Angle: return value; // "Deg";
                case Units.Mass: return value * lb; // "Lb";
                case Units.Mass2D: return value * lb / (ft * ft);//Lb/ft2
                case Units.Velocity: return value * inch; // "in/s";
                case Units.Acceleration: return value * inch; // "in/s2";
                case Units.NoUnit: return value;
                case Units.Time: return value;
                case Units.MassInertia: return value * kip * inch; // Kip*in*s2
                case Units.ParTranslation: return value * kip; // Kip*s2
                case Units.ForceMoment: return value * kip; // "Kip*in";
                case Units.Frequency: return value; // "1/s";
                case Units.CircFreq: return value; // "rad/s";
                case Units.CircFreq2: return value; // "rad2/s2";
                default: return value;
            }
        }
        /// <summary>
        /// Realiza la conversión de sistema internacional a sistema inglés  
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
                case Units.Distance: return (float)Math.Round(value / ft, fromInternationalPrecision); // "ft";
                case Units.SmallDistance: return (float)Math.Round(value / inch, fromInternationalPrecision); // "in";
                case Units.Force: return (float)Math.Round(value / kip, fromInternationalPrecision); // "Kip";
                case Units.Area: return (float)Math.Round(value / (inch * inch), fromInternationalPrecision); // "in2";
                case Units.AreaBig: return (float)Math.Round(value / (ft * ft)); //ft2
                case Units.Stress: return (float)Math.Round(value / (kip / (inch * inch)), fromInternationalPrecision); // "Kip/in2";
                case Units.Density: return (float)Math.Round(value / (lb / (inch * inch * inch)), fromInternationalPrecision); // "lb/ft3";
                case Units.Temperature: return (value * 1.8f + 32f); // "F";
                case Units.TemperatureGradient: return (value * 1.8f + 32f) / inch; // "F/in";
                case Units.SpringTranslation: return (float)Math.Round(value / (kip / inch), fromInternationalPrecision); // "Kip/in";
                case Units.SpringRotation: return (float)Math.Round(value / (kip * ft), fromInternationalPrecision); //"Kip*ft/deg";
                case Units.Load0D: return (float)Math.Round(value / kip, fromInternationalPrecision);  //"Kip";
                case Units.Load1D: return (float)Math.Round(value / (kip / ft), fromInternationalPrecision); //"Kip/ft";
                case Units.Load2D: return (float)Math.Round(value / (kip / (ft * ft)), fromInternationalPrecision); //"Kip/ft^2"
                case Units.Moment: return (float)Math.Round(value / (kip * inch), fromInternationalPrecision); //"Kip*in";
                case Units.AreaInertia: return (float)Math.Round(value / (inch * inch * inch * inch), fromInternationalPrecision); // "in4";
                case Units.Warping: return (float)Math.Round(value / (inch * inch * inch * inch * inch * inch), fromInternationalPrecision); // "in6";
                case Units.SmallVolume: return (float)Math.Round(value / (inch * inch * inch), fromInternationalPrecision); //"in3";
                case Units.ShearModulus: return (float)Math.Round(value * (inch * inch), fromInternationalPrecision); // "1/in2";
                case Units.ThermalCoefficient: return value * 1.8f; //"1/°F";
                case Units.Angle: return value; // "Deg";
                case Units.Mass: return (float)Math.Round(value / lb, fromInternationalPrecision); // "Lb";
                case Units.Mass2D: return (float)Math.Round(value * ft * ft / lb);//Lb/ft2
                case Units.Velocity: return (float)Math.Round(value / inch, fromInternationalPrecision); // "in/s";
                case Units.Acceleration: return (float)Math.Round(value / inch, fromInternationalPrecision); // "in/s2";
                case Units.NoUnit: return value;
                case Units.Time: return value; // s
                case Units.MassInertia: return (float)Math.Round(value / (kip * inch), fromInternationalPrecision); // kip*in*s2
                case Units.ParTranslation: return (float)Math.Round(value / kip, fromInternationalPrecision); // Kip*s2
                case Units.ForceMoment: return (float)Math.Round(value / kip, fromInternationalPrecision);// "Kip*in";
                case Units.Frequency: return value; // "1/s";
                case Units.CircFreq: return value; //  "rad/s";
                case Units.CircFreq2: return value; //  "rad2/s2";
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
                case Units.Distance: return "ft";
                case Units.SmallDistance: return "in";
                case Units.Force: return "Kip";
                case Units.Area: return "in2";
                case Units.AreaBig: return "ft2";
                case Units.Stress: return "Kip/in2";
                case Units.Density: return "Kip/ft3";
                case Units.Temperature: return "°F";
                case Units.TemperatureGradient: return "°F/in";
                case Units.SpringTranslation: return "Kip/in";
                case Units.SpringRotation: return "Kip*ft/deg";
                case Units.Load0D: return "Kip";
                case Units.Load1D: return "Kip/ft";
                case Units.Load2D: return "Kip/ft2";
                case Units.Moment: return "Kip*in";
                case Units.AreaInertia: return "in4";
                case Units.Warping: return "in6";
                case Units.SmallVolume: return "in3";
                case Units.ShearModulus: return "1/in2";
                case Units.ThermalCoefficient: return "1/F";
                case Units.Angle: return "Deg";
                case Units.Mass: return "Lb";
                case Units.Mass2D: return "Lb/ft2";
                case Units.Velocity: return "in/s";
                case Units.Acceleration: return "in/s2";
                case Units.Time: return "s";
                case Units.MassInertia: return "Kip*in*s2";
                case Units.ParTranslation: return "Kip*s2";
                case Units.ForceMoment: return "Kip*in";
                case Units.NoUnit: return "";
                case Units.Frequency: return "1/s";
                case Units.CircFreq: return "rad/s";
                case Units.CircFreq2: return "rad2/s2";
                default: return "";
            }
        }

        #endregion
    }
}