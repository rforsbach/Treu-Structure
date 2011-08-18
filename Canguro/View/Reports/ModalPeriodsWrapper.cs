using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class ModalPeriodsWrapper : ReportData
    {
        private string rCase;
        private float period;
        private float[] values;
        
        /// <summary>
        /// Modal Periods and frequencies
        /// </summary>
        /// <param name="results"></param>
        public ModalPeriodsWrapper(Canguro.Model.Results.Results results)
        {
            rCase = results.ActiveCase.Name;
            values = new float[3];
            period = results.ModalPeriods[0];
            values[0] = results.ModalPeriods[1];
            values[1] = results.ModalPeriods[2];
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

        [Canguro.Model.ModelAttributes.GridPosition(1, 2000)]
        public string Case {
            get { return rCase; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 2000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Time)] 
        public string Period {
            get { return period.ToString("G5"); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 2000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Frequency)] 
        public string Frecuency {
            get { return string.Format("{0:G3}", values[0]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 2000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.CircFreq)] 
        public string CircFreq {
            get { return string.Format("{0:G3}", values[1]); }
            set { }
        }
    }
}