using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class JointForcesWrapper : ItemWrapper
    {
        private Canguro.Model.Load.ForceLoad load;
        private uint jointID;
        private string loadCase;
        public JointForcesWrapper(Canguro.Model.Load.ForceLoad load, Canguro.Model.Joint joint, Canguro.Model.Load.LoadCase lCase)
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

        [Canguro.Model.ModelAttributes.GridPosition(1, 2000)]
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
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public string Fx
        {
            get { return string.Format("{0:#,0.###}", load.Fx); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public string Fy
        {
            get { return string.Format("{0:#,0.###}", load.Fy); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public string Fz
        {
            get { return string.Format("{0:#,0.###}", load.Fz); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public string Mx
        {
            get { return string.Format("{0:#,0.###}", load.Mx); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public string My
        {
            get { return string.Format("{0:#,0.###}", load.My); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public string Mz
        {
            get { return string.Format("{0:#,0.###}", load.Mz); }
            set { }
        }
    }
}
