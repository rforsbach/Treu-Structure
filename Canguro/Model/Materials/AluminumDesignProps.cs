using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// La primera versión de Canguro no incluye diseño de estructuras de aluminio, por lo que esto no está implementado.
    /// </summary>
    [Serializable]
    public class AluminumDesignProps : MaterialDesignProps
    {
        private float alloy = 0;
        private float fcy = 0;
        private float fty = 0;
        private float ftu = 0;
        private float fsy = 0;
        private float fsu = 0;
        private AlumType type = AlumType.Wrought;
    
        public enum AlumType
        {
            Wrought, MoldCast, SandCast,
        }

        /// <summary>
        /// Aluminum Alloy Designation
        /// </summary>
        public float Alloy
        {
            get { return alloy; }
            set { }
        }

        /// <summary>
        /// Compressive Yield Strength
        /// </summary>
        public float Fcy
        {
            get { return fcy; }
            set { }
        }

        /// <summary>
        /// Tensile Yield Strength
        /// </summary>
        public float Fty
        {
            get { return fty; }
            set { }
        }

        /// <summary>
        /// Tensile Ultimate Strength
        /// </summary>
        public float Ftu
        {
            get { return ftu; }
            set { }
        }

        /// <summary>
        /// Shear Yield Strength
        /// </summary>
        public float Fsy
        {
            get { return fcy; }
            set { }
        }

        /// <summary>
        /// Shear Ultimate Strength
        /// </summary>
        public float Fsu
        {
            get { return fsu; }
            set { }
        }

        public AlumType Type
        {
            get
            {
                return AlumType.MoldCast;
            }
            set
            {
            }
        }

        public override string Name
        {
            get
            {
                return Culture.Get("aluminumName");
            }
        }
    }
}
