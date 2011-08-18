using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class LoadCombinationWrapper : ReportData
    {
        Canguro.Model.Load.LoadCombination combo;
        Canguro.Model.Load.AbstractCaseFactor factor;

        public LoadCombinationWrapper(Canguro.Model.Load.LoadCombination combo, Canguro.Model.Load.AbstractCaseFactor factor)
        {
            this.combo = combo;
            this.factor = factor;
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
        public string Combination
        {
            get { return combo.Name; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 2000)]
        public string LoadCase
        {
            get { return factor.Case.Name; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        public string Factor
        {
            get { return string.Format("{0:#,0.#}", factor.Factor); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 2000)]
        public string Type
        {
            get { return combo.Type.ToString(); }
            set { }
        }
    }
}
