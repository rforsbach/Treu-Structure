using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Canguro.Controller.Grid
{
    public class GridViewLocalizedEnumComboCell<EnumType> : DataGridViewComboBoxCell
    {
        protected static Dictionary<string, EnumType> allValues = null;
        protected string[] datasource = null;

        protected static Dictionary<string, EnumType> AllValues
        {
            get
            {
                if (allValues == null)
                {
                    allValues = new Dictionary<string, EnumType>();
                    foreach (EnumType val in Enum.GetValues(typeof(EnumType)))
                        allValues.Add(Culture.Get(val.ToString()), val);
                }
                return allValues;
            }
        }

        protected override object GetValue(int rowIndex)
        {
            object value = base.GetValue(rowIndex);
            if (value is EnumType)
                return value;
            if (value != null)
                return AllValues[value.ToString()];
            return default(EnumType);
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            //if (value is EnumType)
            //{
            //    string str = null;
            //    foreach (string key in AllValues.Keys)
            //        if (AllValues[key].Equals(value))
            //        {
            //            str = key;
            //            break;
            //        }
            //    if (!string.IsNullOrEmpty(str))
            //        return base.SetValue(rowIndex, str);
            //}
            return base.SetValue(rowIndex, value);
        }

        public override object DataSource
        {
            get
            {
                //if (datasource == null)
                //{
                //    datasource = new string[AllValues.Count];
                //    int i = 0;
                //    foreach (string key in AllValues.Keys)
                //        datasource[i++] = key;
                //}
                //return datasource;
                return Enum.GetValues(typeof(EnumType));
            }
            set
            {
//                base.DataSource = value;
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(EnumType);
            }
        }


        public override Type FormattedValueType
        {
            get
            {
                return typeof(string);
            }
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value != null && value is EnumType)
            {
                return Culture.Get(value.ToString());
            }
            return default(EnumType);
        }
    }
}
