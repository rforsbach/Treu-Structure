using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Controller.Grid
{
    interface IPopupGridControl
    {
        object Value { get; set; }
        PopupCellEditingControl EditingControl { get; set; }
        bool HoldFocus { get; }
        // Hacer NotifyCellDirty cuando se modifique algo
    }
}
