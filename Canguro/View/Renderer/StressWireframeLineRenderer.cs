using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Renderer
{
    public class StressWireframeLineRenderer : DeformedLineWireframeRenderer
    {
        private static readonly int unselectedColor = System.Drawing.Color.Gray.ToArgb();

        protected override int getLineColor(ResourceManager rc, Canguro.Model.LineElement l, bool pickingMode, RenderOptions.LineColorBy colorBy)
        {
            if (pickingMode)
                return base.getLineColor(rc, l, pickingMode, colorBy);

            return unselectedColor;
        }
    }
}
