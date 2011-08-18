using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class LineConcentratedForcesWrapper : ItemWrapper
    {
        private Canguro.Model.Load.ConcentratedSpanLoad load;
        private uint elementID;
        private string loadCase;
        public LineConcentratedForcesWrapper(Canguro.Model.Load.ConcentratedSpanLoad load, Canguro.Model.LineElement line, Canguro.Model.Load.LoadCase lCase)
        {
            this.load = load;
            elementID = line.Id;
            loadCase = lCase.Name;
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

        [Canguro.Model.ModelAttributes.GridPosition(1, 1000)]
        public string Case
        {
            get { return loadCase; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        public string Element
        {
            get { return elementID.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        public string D
        {
            get { return string.Format("{0:#,0.###}", load.D); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        public string Type
        {
            get { return load.Type.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Load1D)]
        public string L
        {
            get { return string.Format("{0:#,0.###}", load.L); }
            set { }
        }
    }
}
