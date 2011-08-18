using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class Rectangular : FrameSection
    {
//        public Rectangular(float depth, float width) { }
        public Rectangular(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float width, float height, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, height, width, 0, 0, 0, 0, 0, width*height, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }

        public Rectangular(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, t3, t2, tf, tw, t2b, tfb, dis, area, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }

        public Rectangular(string name, Material.Material material, ConcreteSectionProps concreteProperties, float b, float h)
            : base(name, "R", material, concreteProperties)
        {
            t3 = h;
            t2 = b;
            initContourAndLOD();
        }

        static short[][] contourIndices;
        static Rectangular()
        {
            contourIndices = new short[4][];
            contourIndices[0] = new short[] { 0, 2, 4, 6, 0};
            contourIndices[1] = new short[] { 0, 2, 4, 6, 0};
            contourIndices[2] = new short[] { 0, 2, 4, 6, 0 };
            contourIndices[3] = new short[] { 0, 1, 2, 3, 4, 5, 6, 7, 0 };
        }

        public override short[][] ContourIndices
        {
            get
            {
                return contourIndices;
            }
        }

        private void UpdataData()
        {
            float b = t2;
            float h = t3;

            float beta = 1f / 3f - 0.21f * (b / h) * (1 - b * b * b * b / (12f * h * h * h * h));

            this.area = b * h; ;
            this.torsConst = beta * h * b * b * b;
            this.i22 = h * b * b * b / 12.0f;
            this.i33 = b * h * h * h / 12.0f;
            this.as3 = 5f * b * h / 6f;
            this.as2 = 5f * b * h / 6f;
            this.s22 = 2f * i22 / b;
            this.s33 = 2f * i33 / h;
            this.z22 = b * b * h * 0.25f;
            this.z33 = b * h * h * 0.25f;
            this.r22 = 0.2887f * b;
            this.r33 = 0.2887f * h;
        }

        protected override void initContour()
        {
            contour[0] = new Microsoft.DirectX.Vector2[8];
            contour[1] = new Microsoft.DirectX.Vector2[8];

            int i = 0;
            contour[0][i++] = new Microsoft.DirectX.Vector2(0, 0);
            contour[0][i++] = new Microsoft.DirectX.Vector2(0.5f * t2, 0);
            contour[0][i++] = new Microsoft.DirectX.Vector2(t2, 0);
            contour[0][i++] = new Microsoft.DirectX.Vector2(t2, 0.5f * t3);
            contour[0][i++] = new Microsoft.DirectX.Vector2(t2, t3);
            contour[0][i++] = new Microsoft.DirectX.Vector2(0.5f * t2, t3);
            contour[0][i++] = new Microsoft.DirectX.Vector2(0, t3);
            contour[0][i++] = new Microsoft.DirectX.Vector2(0, 0.5f * t3);

            for (i = 0; i < 8; i++)
            {
                contour[0][i].X -= 0.5f * t2;
                contour[0][i].Y -= 0.5f * t3;
            }

            i = 0;
            contour[1][i++] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][i++] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][i++] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][i++] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][i++] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][i++] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][i++] = new Microsoft.DirectX.Vector2(-1, 0);
            contour[1][i++] = new Microsoft.DirectX.Vector2(-1, 0);

            buildHighStressCover();
            UpdataData();
        }

        protected override void buildHighStressCover()
        {
            coverHighStress = new short[6 * 3];

            // First triangle
            coverHighStress[0] = 0; coverHighStress[1] = 1; coverHighStress[2] = 7;
            // Second triangle
            coverHighStress[3] = 1; coverHighStress[4] = 3; coverHighStress[5] = 7;
            // Third triangle
            coverHighStress[6] = 1; coverHighStress[7] = 2; coverHighStress[8] = 3;
            // Fourth triangle
            coverHighStress[9] = 3; coverHighStress[10] = 4; coverHighStress[11] = 5;
            // Fifth triangle
            coverHighStress[12] = 5; coverHighStress[13] = 7; coverHighStress[14] = 3;
            // Sixth triangle
            coverHighStress[15] = 5; coverHighStress[16] = 6; coverHighStress[17] = 7;
        }

        protected override void buildHighLODCover()
        {
            coverHigh = new short[2 * 3];

            // First triangle
            coverHigh[0] = 0; coverHigh[1] = 1; coverHigh[2] = 2;
            // Second triangle
            coverHigh[3] = 0; coverHigh[4] = 2; coverHigh[5] = 3;
        }

        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float Depth
        {
            get
            {
                return T3;
            }
            set
            {
                T3 = value;
            }
        }

        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float Width
        {
            get
            {
                return T2;
            }
            set
            {
                T2 = value;
            }
        }

        public override string Shape
        {
            get { return "R"; }
        }
    }
}
