using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.View.Reports
{
    class ElementJointForcesWrapper : ItemWrapper
    {
        private uint lineID;
        private uint jointID;
        private string rCase;
        private float[] forces;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public ElementJointForcesWrapper(LineElement line, Joint joint, Model.Results.Results results)
        {
            lineID = line.Id;
            jointID = joint.Id;
            rCase = results.ActiveCase.Name;
            int jIndex = (jointID == line.I.Id) ? 0 : 1;
            
            forces = new float[6];
            for (int i = 0; i < 6; i++)
                forces[i] = results.ElementJointForces[lineID, 0, i];
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

        [Canguro.Model.ModelAttributes.GridPosition(1, 1600)]
        public string Case
        {
            get { return rCase; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 900)]
        public string Element
        {
            get { return lineID.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 500)]
        public string Joint
        {
            get { return jointID.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public string Fx
        {
            get { return string.Format("{0:G3}", us.FromInternational(forces[0], Canguro.Model.UnitSystem.Units.Force)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public string Fy
        {
            get { return string.Format("{0:G3}", us.FromInternational(forces[1], Canguro.Model.UnitSystem.Units.Force)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Force)]
        public string Fz
        {
            get { return string.Format("{0:G3}", us.FromInternational(forces[2], Canguro.Model.UnitSystem.Units.Force)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public string Mx
        {
            get { return string.Format("{0:G3}", us.FromInternational(forces[3], Canguro.Model.UnitSystem.Units.Moment)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public string My
        {
            get { return string.Format("{0:G3}", us.FromInternational(forces[4], Canguro.Model.UnitSystem.Units.Moment)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(9, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Moment)]
        public string Mz
        {
            get { return string.Format("{0:G3}", us.FromInternational(forces[5], Canguro.Model.UnitSystem.Units.Moment)); }
            set { }
        }
    }
}
