using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    public class GridViewSingleArrayColumn : DataGridViewColumn
    {
        public GridViewSingleArrayColumn()
        {
            CellTemplate = new GridViewSingleArrayCell();
        }
    }
}
