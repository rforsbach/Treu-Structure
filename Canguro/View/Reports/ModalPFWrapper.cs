using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class ModalPFWrapper : ReportData
    {
        private string rCase;
        //private float period;
        private float[] values;

        /// <summary>
        /// Modal Participation Factors
        /// </summary>
        /// <param name="results"></param>
        public ModalPFWrapper(Canguro.Model.Results.Results results)
        {
            rCase = results.ActiveCase.Name;
            values = new float[9];
            //period = results.ModalPF[0];
            values[0] = results.ModalPF[1];
            values[1] = results.ModalPF[2];
            values[2] = results.ModalPF[3];
            values[3] = results.ModalPF[4];
            values[4] = results.ModalPF[5];
            values[5] = results.ModalPF[6];
            values[6] = results.ModalPF[7];
            values[7] = results.ModalPF[8];
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

        [Canguro.Model.ModelAttributes.GridPosition(1, 1600)]
        public string Case {
            get { return rCase; }
            set { }
        }

        //[Canguro.Model.ModelAttributes.GridPosition(2, 900)]
        //public string Period {
        //    get { return period.ToString("G5"); }
        //    set { }
        //}

        [Canguro.Model.ModelAttributes.GridPosition(3, 900)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ParTranslation)]
        public string Ux {
            get { return string.Format("{0:G3}", values[0]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 900)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ParTranslation)]
        public string Uy {
            get { return string.Format("{0:G3}", values[1]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 900)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ParTranslation)]
        public string Uz {
            get { return string.Format("{0:G3}", values[2]); }
            set { }
        }
        [Canguro.Model.ModelAttributes.GridPosition(6, 900)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.MassInertia)]
        public string Rx {
            get { return string.Format("{0:G3}", values[3]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(7, 900)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.MassInertia)] 
        public string Ry {
            get { return string.Format("{0:G3}", values[4]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(8, 900)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.MassInertia)] 
        public string Rz {
            get { return string.Format("{0:G3}", values[5]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(9,900)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.MassInertia)] 
        public string ModalMass {
            get { return string.Format("{0:G3}", values[6]); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(10, 900)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.ForceMoment)]  
        public string ModalStiff {
            get { return string.Format("{0:G3}", values[7]); }
            set { }
        }
    }
}