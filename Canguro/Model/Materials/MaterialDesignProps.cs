using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Propiedades de diseño de un material. Dependen del tipo de material. 
    /// Con estas propiedades se calcula si el material se rompe bajo las deformaciones calculadas.
    /// No tienen influencia sobre el análisis.
    /// </summary>
    [Serializable]
    public abstract class MaterialDesignProps : Utility.GlobalizedObject, ICloneable, INamed
    {
        /// <summary>
        /// Cada conjunto de propiedades de diseño tiene un nombre, que es el mismo para 
        /// todos los materiales del mismo tipo (eg. Concreto, Acero, Aluminio).
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Este método es necesario para que el Material funcione como Prototype.
        /// Hace una copia superficial (completa, en este caso) de las propiedades.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
