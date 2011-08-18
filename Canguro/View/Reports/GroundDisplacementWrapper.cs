using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class GroundDisplacementWrapper : ItemWrapper
    {
        private Canguro.Model.Load.GroundDisplacementLoad load;
        private uint jointID;
        private string loadCase;
        public GroundDisplacementWrapper(Canguro.Model.Load.GroundDisplacementLoad load, Canguro.Model.Joint joint, Canguro.Model.Load.LoadCase lCase)
        {
            this.load = load;
            jointID = joint.Id;
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
        public string LoadCase
        {
            get { return loadCase; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        public string Joint
        {
            get { return jointID.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public string Tx
        {
            get { return string.Format("{0:#,0.###}", load.Tx); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public string Ty
        {
            get { return string.Format("{0:#,0.###}", load.Ty); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public string Tz
        {
            get { return string.Format("{0:#,0.###}", load.Tz); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public string Rx
        {
            get { return string.Format("{0:#,0.###}", load.Rx); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public string Ry
        {
            get { return string.Format("{0:#,0.###}", load.Ry); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public string Rz
        {
            get { return string.Format("{0:#,0.###}", load.Rz); }
            set { }
        }
    }
}
