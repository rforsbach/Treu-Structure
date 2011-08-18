using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    public abstract class CableSection : Section
    {
        public string Name
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

        public String Shape
        {
            get { return "W"; }
        }

        public string Description
        {
            get { return Name; }
        }
    }
}
