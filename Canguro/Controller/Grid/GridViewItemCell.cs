using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Canguro.Controller.Grid
{
    public class GridViewItemCell : DataGridViewTextBoxCell
    {
        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value == null) return "";

            return ((Canguro.Model.Item)value).Id.ToString();
        }

        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            try
            {
                System.Collections.IList list = ((GridViewTextBoxCellTemplateColumn)DataGridView.Columns[ColumnIndex]).List;
                return list[int.Parse((string)formattedValue)];
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            try
            {
                return base.SetValue(rowIndex, value);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
