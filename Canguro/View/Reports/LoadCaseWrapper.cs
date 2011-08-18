using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class LoadCaseWrapper : ItemWrapper
    {
        Canguro.Model.Load.LoadCase lCase;
        public LoadCaseWrapper(Canguro.Model.Load.LoadCase lCase)
        {
            this.lCase = lCase;
        }

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
            get { return lCase.Name; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 2000)]
        public string Type
        {
            get { return Culture.Get(lCase.CaseType.ToString()); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 2000)]
        public string SelfWeightFactor
        {
            get { return string.Format("{0:#,0.#}", lCase.SelfWeight); }
            set { }
        }
    }
}
