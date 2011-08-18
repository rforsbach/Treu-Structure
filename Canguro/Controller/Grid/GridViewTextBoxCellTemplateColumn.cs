using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    public class GridViewTextBoxCellTemplateColumn : DataGridViewTextBoxColumn
    {
        System.Collections.IList list;
        public GridViewTextBoxCellTemplateColumn(DataGridViewTextBoxCell cellTemplate, System.Collections.IList list)
        {
            this.list = list;
            if (cellTemplate != null)
                CellTemplate = cellTemplate;
        }

        public System.Collections.IList List
        {
            get { return list; }
        }
    }
}
