using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Design;

namespace Canguro.View.Reports
{
    class DesignOptionsWrapper : ReportData
    {
        DesignOptions options;
        string variable;
        string value;

        public DesignOptionsWrapper(DesignOptions design, string var, string val)
        {
            options = design;
            variable = var;
            value = val;
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

        [Canguro.Model.ModelAttributes.GridPosition(1, 2200)]
        public string Code
        {
            get 
            { 
                string mat = (options is SteelDesignOptions) ? Culture.Get("steel") : 
                    (options is ConcreteDesignOptions) ? Culture.Get("concrete") : Culture.Get("Other"); 
                return string.Format("{0} ({1})", options.ToString(), mat);
            }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 4000)]
        public string Variable
        {
            get { return variable; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 2000)]
        public string Value
        {
            get { return value; }
            set { }
        }
    }
}
