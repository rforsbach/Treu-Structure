using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Results;

namespace Canguro.View.Reports {
    class SteelDesignWrapper : ReportData {
        private uint frameID;
        private float ratio;
        private string error;
        private string warning;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public SteelDesignWrapper(LineElement frame, SteelDesignSummary design) {
            frameID = frame.Id;
            ratio = design.Ratio;
            error = design.ErrMsg;
            warning = design.WarnMsg;
        }

        private static List<System.ComponentModel.PropertyDescriptor> myProps = null;
        [System.ComponentModel.Browsable(false)]
        public override List<System.ComponentModel.PropertyDescriptor> Properties {
            get {
                if (myProps == null)
                    myProps = InitProps();
                return myProps;
            }
        }

        [Canguro.Model.ModelAttributes.GridPosition(1, 750)]
        public string Frame {
            get { return frameID.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        public string Ratio {
            get { return string.Format("{0:G3} %", ratio * 100);  }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 3500)]
        public string Error {
            get { return error; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 3500)]
        public string Warning {
            get { return warning; }
            set { }
        }
    }
}
