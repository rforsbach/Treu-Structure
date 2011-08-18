using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model;
using Canguro.Model.Results;

namespace Canguro.View.Reports {

    class SteelDesignPMMWrapper: ReportData {

        private uint frameID;
        private string status;
        private float[] ratio;
        private string error;
        private string warning;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        public SteelDesignPMMWrapper(LineElement frame, SteelDesignPMMDetails design) {
            frameID = frame.Id;
            status = design.Status;
            ratio = new float[4];
            ratio[0] = design.TotalRatio;
            ratio[1] = design.PRatio;
            ratio[2] = design.MMajRatio;
            ratio[3] =  design.MMinRatio;
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

        [Canguro.Model.ModelAttributes.GridPosition(2, 1100)]
        public string Status {
            get { return status.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        public string TotalRatio {
            get { return string.Format("{0:G3} %", ratio[0] * 100); }
            set { }
        }
        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        public string PRatio {
            get { return string.Format("{0:G3} %", ratio[1] * 100); }
            set { }
        }
        [Canguro.Model.ModelAttributes.GridPosition(5, 1000)]
        public string MMajRatio {
            get { return string.Format("{0:G3} %", ratio[2] * 100); }
            set { }
        }
        [Canguro.Model.ModelAttributes.GridPosition(6, 1000)]
        public string MMinRatio {
            get { return string.Format("{0:G3} %", ratio[3] * 100); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 2000)]
        public string Error {
            get { return error; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 1800)]
        public string Warning {
            get { return warning; }
            set { }
        }
    }
}