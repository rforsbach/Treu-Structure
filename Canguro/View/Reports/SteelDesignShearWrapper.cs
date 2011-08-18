using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Results;

namespace Canguro.View.Reports {

    class SteelDesignShearWrapper : ReportData {

        private uint frameID;
        private string status;
        private float[] ratio;
        private string error;
        private string warning;

        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public SteelDesignShearWrapper(LineElement frame, SteelDesignShearDetails design) {
            frameID = frame.Id;
            status = design.Status;
            ratio = new float[2];
            ratio[0] = design.VMajorRatio;
            ratio[1] = design.VMinorRatio;
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

        [Canguro.Model.ModelAttributes.GridPosition(1, 800)]
        public string Frame {
            get { return frameID.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1200)]
        public string Status {
            get { return status.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        public string VMajorRatio {
            get { return string.Format("{0:G3} %", ratio[0] * 100); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        public string VMinorRatio {
            get { return string.Format("{0:G3} %", ratio[1] * 100); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 2000)]
        public string Error {
            get { return error; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 2000)]
        public string Warning {
            get { return warning; }
            set { }
        }
    }
}