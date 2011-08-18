using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class JointReactionsWrapper : ReportData
    {
        private uint jointID;
        private string rCase;
        private float[] reactions;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public JointReactionsWrapper(Canguro.Model.Joint joint, Canguro.Model.Results.Results results)
        {
            rCase = results.ActiveCase.Name;
            jointID = joint.Id;
            reactions = new float[6];
            for (int i = 0; i < 6; i++)
                reactions[i] = results.JointReactions[jointID, i];
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
        public string Case
        {
            get { return rCase; }
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
            get { return string.Format("{0:G3}", us.FromInternational(reactions[0], Canguro.Model.UnitSystem.Units.Force)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public string Fy
        {
            get { return string.Format("{0:G3}", us.FromInternational(reactions[1], Canguro.Model.UnitSystem.Units.Force)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public string Fz
        {
            get { return string.Format("{0:G3}", us.FromInternational(reactions[2], Canguro.Model.UnitSystem.Units.Force)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public string Mx
        {
            get { return string.Format("{0:G3}", us.FromInternational(reactions[3], Canguro.Model.UnitSystem.Units.Moment)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public string My
        {
            get { return string.Format("{0:G3}", us.FromInternational(reactions[4], Canguro.Model.UnitSystem.Units.Moment)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public string Mz
        {
            get { return string.Format("{0:G3}", us.FromInternational(reactions[5], Canguro.Model.UnitSystem.Units.Moment)); }
            set { }
        }
    }
}
