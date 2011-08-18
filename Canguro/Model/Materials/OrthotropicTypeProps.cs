using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    [Serializable]
    public class OrthotropicTypeProps : MaterialTypeProps
    {
        [System.ComponentModel.Browsable(false)]
        public override string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override float E
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
    }
}
