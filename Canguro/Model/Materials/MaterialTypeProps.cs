using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Clase que abstrae las propiedades de acuerdo al tipo de material (Isotrópico, anisotrópico, etc).
    /// Estas propiedades se usan principalmente en el análisis no lineal.
    /// </summary>
    [Serializable]
    public abstract class MaterialTypeProps : Utility.GlobalizedObject, ICloneable
    {
        /// <summary>
        /// Cada tipo de material tiene que tener un nombre, que será constante.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Modulus of Elasticity (Stress Units)
        /// </summary>
        public abstract float E
        {
            get;
            set;
        }

        /// <summary>
        /// La propiedades se deben poder clonar para que el material pueda ser un Prototype.
        /// Se realiza una copiasuperficial (en este caso, completa).
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
