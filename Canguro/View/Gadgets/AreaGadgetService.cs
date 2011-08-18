using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;

namespace Canguro.View.Gadgets
{
    public class AreaGadgetService : GadgetService
    {
        GadgetManager gadgetManager;
        public AreaGadgetService(GadgetManager gm)
        {
            gadgetManager = gm;
        }

        #region GadgetService Members

        public void ClearLocators()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
