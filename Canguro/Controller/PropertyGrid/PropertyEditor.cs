using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Canguro.Controller.PropertyGrid
{
    class PropertyEditor : System.Windows.Forms.PropertyGrid
    {
        protected override void OnLostFocus(EventArgs e)
        {
            if (Canguro.Model.Model.Instance.IsLocked)
                ResetSelectedProperty();
            base.OnLostFocus(e);
        }
    }
}
