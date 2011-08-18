using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    class GridViewConstraintComboColumn : DataGridViewColumn
    {
        public GridViewConstraintComboColumn()
        {
            CellTemplate = new GridViewConstraintComboCell();
        }
    }
}
