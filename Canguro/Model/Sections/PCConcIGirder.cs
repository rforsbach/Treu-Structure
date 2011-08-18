using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    /// <summary>
    /// Precast Concrete I Girder
    /// Tipo de sección con propiedades especiales.
    /// </summary>
    [Serializable]
    public class PCConcIGirder : FrameSection
    {
        public PCConcIGirder(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, t3, t2, tf, tw, t2b, tfb, dis, area, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }

        static short[][] contourIndices;
        static PCConcIGirder()
        {
            contourIndices = new short[4][];
            //contourIndices[0] = new int[] {0, 1, 3, 4, 5, 7, 8, 9, 0};
            //contourIndices[1] = new int[] {0, 1, 5, 7, 0};
            //contourIndices[2] = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0};
        }

        public override short[][] ContourIndices
        {
            get
            {
                return contourIndices;
            }
        }

        protected override void buildHighStressCover()
        {
            coverHighStress = new short[0];
        }

        public override string Shape
        {
            get
            {
                return "I";
            }
        }
    }
}
