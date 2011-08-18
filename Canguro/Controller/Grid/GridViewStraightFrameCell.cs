using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Model.Section;

namespace Canguro.Controller.Grid
{
    class GridViewStraightFrameCell : GridViewPopupCell
    {
        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value == null) return "";
            return ((StraightFrameProps)value).Section.ToString();
        }
    }
}
