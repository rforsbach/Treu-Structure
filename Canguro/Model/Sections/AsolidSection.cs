using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class AsolidSection : AreaSection
    {
        public AsolidSection(string name, string shape, Material.Material material) : base(name, shape, material) { }

        public bool UseIncompatibleModes
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Material.Material Material
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public float MaterialAngle
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public float ThicknessAngle
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
