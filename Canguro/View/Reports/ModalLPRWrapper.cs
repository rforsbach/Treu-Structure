using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class ModalLPRWrapper : ReportData
    {
        private string item;
        private string itemType;
        private float[] values;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        /// <summary>
        /// Modal Load Participation Ratios
        /// </summary>
        /// <param name="results"></param>
        public ModalLPRWrapper(Canguro.Model.Results.Results results, int i)
        {
            List<Canguro.Model.Results.ModalLPRRow> list = results.ModalLPR["Modal"];
            
            item = list[i].Item;
            itemType = list[i].ItemType;

            values = new float[2];
            values[0] = list[i].DynamicVal;
            values[1] = list[i].StaticVal;
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

        [Canguro.Model.ModelAttributes.GridPosition(2, 2000)]
        public string Item {
            get { return item; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(1, 2000)]
        public string ItemType {
            get { return itemType; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        public string Static {
            get { return string.Format("{0:G3} %", values[0] * 100); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 1000)]
        public string Dynamic {
            get { return string.Format("{0:G3} %", values[1] * 100); }
            set { }
        }
    }
}