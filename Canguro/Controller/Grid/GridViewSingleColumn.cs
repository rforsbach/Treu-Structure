using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    class GridViewSingleColumn : DataGridViewColumn
    {
        public GridViewSingleColumn()
        {
            CellTemplate = new GridViewSingleCell();
            CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
    }
}
