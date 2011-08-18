using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class LineDistributedForcesWrapper : ItemWrapper
    {
        private Canguro.Model.Load.DistributedSpanLoad load;
        private uint elementID;
        private string loadCase;
        public LineDistributedForcesWrapper(Canguro.Model.Load.DistributedSpanLoad load, Canguro.Model.LineElement line, Canguro.Model.Load.LoadCase lCase)
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
        public string Da
        {
            get { return string.Format("{0:#,0.###}", load.Da); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        public string Db
        {
            get { return string.Format("{0:#,0.###}", load.Db); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        public string Direction
        {
            get { return Culture.Get(load.Direction.ToString()); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        public string Type
        {
            get { return Culture.Get(load.Type.ToString()); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Load1D)]
        public string La
        {
            get { return string.Format("{0:#,0.###}", load.La); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Load1D)]
        public string Lb
        {
            get { return string.Format("{0:#,0.###}", load.Lb); }
            set { }
        }
    }
}
