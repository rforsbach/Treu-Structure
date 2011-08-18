using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.View
{
    public class ScenePanel : Panel
    {
        public ScenePanel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }
    }
}
