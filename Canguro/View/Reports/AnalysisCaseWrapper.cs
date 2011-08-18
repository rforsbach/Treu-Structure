using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class AnalysisCaseWrapper : ItemWrapper
    {

        private static List<System.ComponentModel.PropertyDescriptor> myProps = null;
        [System.ComponentModel.Browsable(false)]
        public override List<System.ComponentModel.PropertyDescriptor> Properties
        {
            get
            {
                if (myProps == null)
                    myProps = InitProps();
                return myProps;
            }
        }

        [Canguro.Model.ModelAttributes.GridPosition(1, 2000)]
        public string Name
        {
            get { return "1"; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 2000)]
        public string Type
        {
            get { return "1"; }
            set { }
        }
    }
}
