using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class ModalPMRWrapper : ReportData
    {
        private string rCase;
        private float[] ratios;
        private float period;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        /// <summary>
        /// Modal Participation Mass Ratios
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="results"></param>
        public ModalPMRWrapper(Canguro.Model.Results.Results results)
        {
            rCase = results.ActiveCase.Name;
            ratios = new float[6];
            period = results.ModalPMR[0];
            ratios[0] = results.ModalPMR[1];
            ratios[1] = results.ModalPMR[2];
            ratios[2] = results.ModalPMR[3];
            ratios[3] = results.ModalPMR[7];
            ratios[4] = results.ModalPMR[8];
            ratios[5] = results.ModalPMR[9];
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

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Time)]
        public string Period
        {
            get { return period.ToString("G5"); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        public string Ux
        {
            get { return string.Format("{0:G3} %", ratios[0]*100); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        public string Uy
        {
            get { return string.Format("{0:G3} %", ratios[1]*100); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        public string Uz
        {
            get { return string.Format("{0:G3} %", ratios[2]*100); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        public string Rx
        {
            get { return string.Format("{0:G3} %", ratios[3]*100); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 1000)]
        public string Ry
        {
            get { return string.Format("{0:G3} %", ratios[4]*100); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 1000)]
        public string Rz
        {
            get { return string.Format("{0:G3} %", ratios[5]*100); }
            set { }
        }
    }
}