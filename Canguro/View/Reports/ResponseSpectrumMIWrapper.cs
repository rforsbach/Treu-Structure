using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class ResponseSpectrumMIWrapper : ReportData
    {
        private string rCase;
        private float[,] info;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public ResponseSpectrumMIWrapper(Canguro.Model.Results.Results results)
        {
            rCase = results.ActiveCase.Name;
            info = results.ResponseSpectrumMI;
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
        public string Period
        {
            get { return string.Format("{0:G3}", info[0,0]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 2000)]
        public string DampRatio
        {
            get { return string.Format("{0:G3}", info[0,1]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public string Ux
        {
            get { return string.Format("{0:G3}", us.FromInternational(info[0,5], Canguro.Model.UnitSystem.Units.SmallDistance)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public string Uy
        {
            get { return string.Format("{0:G3}", us.FromInternational(info[0,6], Canguro.Model.UnitSystem.Units.SmallDistance)); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public string Uz
        {
            get { return string.Format("{0:G3}", us.FromInternational(info[0,7], Canguro.Model.UnitSystem.Units.SmallDistance)); }
            set { }
        }
    }
}
