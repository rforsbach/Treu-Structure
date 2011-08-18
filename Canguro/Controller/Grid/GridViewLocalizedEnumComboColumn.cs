using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    class GridViewLocalizedEnumComboColumn<EnumType> : DataGridViewColumn
    {
        public GridViewLocalizedEnumComboColumn()
        {
            CellTemplate = new GridViewLocalizedEnumComboCell<EnumType>();
        }
    }
}
