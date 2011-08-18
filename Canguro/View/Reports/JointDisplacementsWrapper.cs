using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class JointDisplacementsWrapper : ItemWrapper
    {
        private uint id;
        private float tx, ty, tz, rx, ry, rz;
        private string rCase;

        public JointDisplacementsWrapper(uint id, Model.Results.Results results)
        {
            float[,] d = results.JointDisplacements;
            Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;
            
            this.id = id;
            this.tx = us.FromInternational(d[id, 0], Canguro.Model.UnitSystem.Units.SmallDistance);
            this.ty = us.FromInternational(d[id, 1], Canguro.Model.UnitSystem.Units.SmallDistance);
            this.tz = us.FromInternational(d[id, 2], Canguro.Model.UnitSystem.Units.SmallDistance);
            this.rx = d[id, 3];
            this.ry = d[id, 4];
            this.rz = d[id, 5];
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
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public string Tx
        {
            get { return string.Format("{0:G3}",tx); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public string Ty
        {
            get { return string.Format("{0:G3}", ty); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public string Tz
        {
            get { return string.Format("{0:G3}", tz); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public string Rx
        {
            get { return string.Format("{0:G3}", rx); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public string Ry
        {
            get { return string.Format("{0:G3}", ry); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public string Rz
        {
            get { return string.Format("{0:G3}", rz); }
            set { }
        }
    }
}
