using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Controller.Grid
{
    public class GridViewPopupColumn : DataGridViewTextBoxColumn
    {
        Control popupCtrl;
        public GridViewPopupColumn(Control popupCtrl) : this(null, popupCtrl) { }
        public GridViewPopupColumn(GridViewPopupCell cellTemplate, Control popupCtrl)
        {
            if (cellTemplate == null)
                CellTemplate = new GridViewPopupCell();
            else
                CellTemplate = cellTemplate;

            this.popupCtrl = popupCtrl;
        }

        public Control PopupCtrl
        {
            get { return popupCtrl; }
        }
    }
}
