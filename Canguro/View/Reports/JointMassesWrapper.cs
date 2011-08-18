using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class JointMassesWrapper : ReportData
    {
        private uint id;
        private Model.Results.Results results;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public JointMassesWrapper(uint id, Model.Results.Results results)
        {
            this.id = id;
            this.results = results;
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
        public string Joint
        {
            get { return id.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string U1
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.AssembledJointMasses[id, 0], Canguro.Model.UnitSystem.Units.Mass)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string U2
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.AssembledJointMasses[id, 1], Canguro.Model.UnitSystem.Units.Mass)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string U3
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.AssembledJointMasses[id, 2], Canguro.Model.UnitSystem.Units.Mass)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string R1
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.AssembledJointMasses[id, 3], Canguro.Model.UnitSystem.Units.Mass)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string R2
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.AssembledJointMasses[id, 4], Canguro.Model.UnitSystem.Units.Mass)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string R3
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.AssembledJointMasses[id,5], Canguro.Model.UnitSystem.Units.Mass)); }
            set { }
        }
    }
}
