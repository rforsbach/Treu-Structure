using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;

namespace Canguro.View.Reports
{
    class JointWrapper : ItemWrapper
    {
        private Joint joint;

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

        public JointWrapper(Joint j)
        {
            joint = j;
        }

        [Canguro.Model.ModelAttributes.GridPosition(1, 1000)]
        public string ID
        {
            get { return joint.Id.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public string X
        {
            get { return string.Format("{0:#,0.#}", joint.X); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public string Y
        {
            get { return string.Format("{0:#,0.#}", joint.Y); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public string Z
        {
            get { return string.Format("{0:#,0.#}", joint.Z); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 4000)]
        public string DoF
        {
            get { return joint.DoF.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 2000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Mass)]
        [System.ComponentModel.Browsable(false)]
        public string Masses
        {
            get 
            {
                float[] m = joint.Masses;
                return string.Format("{0:#,0.#}, {1:#,0.#}, {2:#,0.#}, {3:#,0.#}, {4:#,0.#}, {5:#,0.#}", m[0], m[1], m[2], m[3], m[4], m[5]);
            }
            set { }
        }
    }
}
