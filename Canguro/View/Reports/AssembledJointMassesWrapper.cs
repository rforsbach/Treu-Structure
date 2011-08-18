using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.View.Reports
{
    class AssembledJointMassesWrapper : ReportData
    {
        private uint jointID;
        private float[] masses;

        public AssembledJointMassesWrapper(Joint joint, Model.Results.Results results)
        {
            jointID = joint.Id;
            float[,] tmp = results.AssembledJointMasses;
            masses = new float[6];
            if (tmp != null && tmp.GetLength(0) > jointID && tmp.GetLength(1) >= 6)
                for (int i = 0; i < 6; i++)
                    masses[i] = tmp[jointID, i];
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
            get { return string.Format("{0}", jointID); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string U1
        {
            get { return string.Format("{0:G3}", masses[0]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string U2
        {
            get { return string.Format("{0:G3}", masses[1]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string U3
        {
            get { return string.Format("{0:G3}", masses[2]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string R1
        {
            get { return string.Format("{0:G3}", masses[3]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string R2
        {
            get { return string.Format("{0:G3}", masses[4]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        public string R3
        {
            get { return string.Format("{0:G3}", masses[5]); }
            set { }
        }
    }
}
