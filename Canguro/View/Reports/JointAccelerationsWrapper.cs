using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class JointAccelerationsWrapper : ReportData
    {        
        private uint id;
        private Model.Results.Results results;
        private string rCase;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public JointAccelerationsWrapper(uint id, Model.Results.Results results)
        {
            this.id = id;
            this.results = results;
            rCase = results.ActiveCase.Name;
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

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        public string Joint
        {
            get { return id.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(1, 2000)]
        public string Case
        {
            get { return rCase; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Acceleration)]
        public string U1
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.JointAccelerations[id, 0], Canguro.Model.UnitSystem.Units.Acceleration)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Acceleration)]
        public string U2
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.JointAccelerations[id, 1], Canguro.Model.UnitSystem.Units.Acceleration)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Acceleration)]
        public string U3
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.JointAccelerations[id, 2], Canguro.Model.UnitSystem.Units.Acceleration)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Acceleration)]
        public string R1
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.JointAccelerations[id, 3], Canguro.Model.UnitSystem.Units.Acceleration)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Acceleration)]
        public string R2
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.JointAccelerations[id, 4], Canguro.Model.UnitSystem.Units.Acceleration)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Acceleration)]
        public string R3
        {
            get { return string.Format("{0:G3}", us.FromInternational(results.JointAccelerations[id, 5], Canguro.Model.UnitSystem.Units.Acceleration)); }
            set { }
        }
    }
}
