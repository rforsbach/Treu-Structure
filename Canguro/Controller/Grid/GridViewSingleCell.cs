using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Canguro.Controller.Grid
{
    class GridViewSingleCell : DataGridViewTextBoxCell
    {
        //protected override object GetValue(int rowIndex)
        //{
        //    object value = base.GetValue(rowIndex);

        //    //if (value != null)
        //    //    return string.Format("{0:G5}", value);
            
        //    //return "";
        //    return value;
        //}

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value != null)
            {
                //bool noFormatting = true;
                //try
                //{
                //    noFormatting = IsInEditMode;
                //}
                //catch (InvalidOperationException) 
                //{
                //    noFormatting = true;
                //}

                //if (noFormatting)
                //    return ((float)value).ToString("G");// string.Format("{0:G}", value);
                //else
                    return ((float)value).ToString("F3"); //return string.Format("{0:F}", value);
            }

            return "";
        }

        //protected override bool SetValue(int rowIndex, object value)
        //{
        //    //try
        //    //{
        //    //    if (value != null)
        //    //        return base.SetValue(rowIndex, float.Parse((string)value));
        //    //    return base.SetValue(rowIndex, null);
        //    //}
        //    //catch (FormatException) 
        //    //{
        //    //    return false;
        //    //}

        //    return base.SetValue(rowIndex, value);
        //}

        public override Type ValueType
        {
            get
            {
                return typeof(float);
            }
        }
    }
}
