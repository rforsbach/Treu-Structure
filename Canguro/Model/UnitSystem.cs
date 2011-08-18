using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.UnitSystem
{
    /// <summary>
    /// Interfaz que define el comportamiento mínimo de un sistema de unidades.
    /// Básicamente, tiene que poder convertir a unidades internacionales y de unidades internacionales.
    /// Además, tiene que poder regresar el valor para la gravedad en el sistema específico.
    /// </summary>
    public abstract class UnitSystem
    {
        protected const int fromInternationalPrecision = 4;

        public abstract float Gravity
        {
            get;
        }

        public abstract string Name { get; }

        /// <summary>
        /// Firma de método abstracto para convertir valores de cualquier sistema al
        /// sistema internacional 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public abstract float ToInternational(float value, Units unit);

        /// <summary>
        /// Firma de método abstracto para convertir valores del sistema internacional 
        /// a otro sistema
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public abstract float FromInternational(float value, Units unit);

        /// <summary>
        /// Método abstracto que define el nombre de las unidades
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public abstract string UnitName(Units unit);

        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1}, {2}, {3})", Name, UnitName(Units.Force), UnitName(Units.Distance), UnitName(Units.Temperature));
        }

        public float Convert(float internationalValue, Units srcUnit, Units dstUnit)
        {
            switch (srcUnit)
            {
                case Units.Distance:
                    if (dstUnit == Units.SmallDistance)
                        return internationalValue * 100f;
                    break;
                case Units.SmallDistance:
                    if (dstUnit == Units.Distance)
                        return internationalValue / 100f;
                    break;
            }

            throw new Exception("The method or operation is not implemented. (UnitSystem).Convert");
        }

        public float Deg2Rad(float value)
        {
            return (float)(value * Math.PI / 180f);
        }

        public float Rad2Deg(float value)
        {
            return (float)Math.Round(value * 180f / Math.PI, fromInternationalPrecision);
        }
    }

    /// <summary>
    /// Enum que declara todos los tipos de unidades que se usan en el sistema.
    /// </summary>
    public enum Units
    {
        Distance,
        SmallDistance,
        Force,
        Area,
        AreaBig,
        Stress,
        Density,
        Temperature,
        TemperatureGradient,
        SpringTranslation,
        SpringRotation,
        Load0D,
        Load1D,
        Load2D,
        Moment,
        AreaInertia,
        MassInertia,
        Warping,
        SmallVolume,
        ShearModulus,
        ThermalCoefficient,
        Angle,
        Mass,
        Velocity,
        Acceleration,
        NoUnit,
        Time,
        ForceMoment,
        ParTranslation,
        Mass2D,
        Frequency,
        CircFreq,
        CircFreq2
    }
}
