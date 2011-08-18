using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Analysis.Sections.Meshing
{
    public class Triangle : Shape
    {
        protected override int getNumEdges()
        {
            return 3;
        }

        public override System.Drawing.PointF GetCentroid()
        {
            return new System.Drawing.PointF((float)(
                edges[0].V1.X + edges[0].V2.X + edges[1].V1.X + edges[1].V2.X + 
                edges[2].V1.X + edges[2].V2.X) / 6.0f, (float)(
                edges[0].V1.Y + edges[0].V2.Y + edges[1].V1.Y + edges[1].V2.Y + 
                edges[2].V1.Y + edges[2].V2.Y) / 6.0f);
        }
    }
}
