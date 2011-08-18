using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Canguro.Controller.Grid
{
    public class GridViewPopupCell : DataGridViewTextBoxCell
    {
        public override Type EditType
        {
            get
            {
                return typeof(PopupCellEditingControl);
            }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            PopupCellEditingControl ctl = (PopupCellEditingControl)DataGridView.EditingControl;
            ctl.Value = Value;
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return null;
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return base.ValueType;
            }
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value is Enum)
                return Culture.Get(value.ToString());
            else if (value != null)
                return value.ToString();
            else
                return "";
        }
    }
}
