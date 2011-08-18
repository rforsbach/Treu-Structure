using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class PlaneSection : AreaSection
    {
        public PlaneSection(string name, string shape, Material.Material material) : base(name, shape, material) { }

        public PlaneType Type
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

        public float Thickness
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

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
    }
}
