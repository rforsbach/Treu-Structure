using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class BoxTube : FrameSection
    {
        public BoxTube(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, t3, t2, tf, tw, t2b, tfb, dis, area, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }

        public BoxTube(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw)
            : base(name, shape, material, concreteProperties) 
        { 
            this.t3 = t3;
            this.t2 = t2;
            this.tf = tf;
            this.tw = tw;
            this.t2b = 0;
            this.tfb = 0;
            this.dis = 0;
            UpdateData();
        }

        private void UpdateData()
        {
            this.area = 2 * (t3 * tw + t2 * tf - tf * tw);
            //this.torsConst = 0;
            this.i33 = t3 * t3 * t3 * tw + 4 * tf * tf * tf * (t2 - 2 * tw);
            this.i22 = t2 * t2 * t2 * tf + 4 * tw * tw * tw * (t3 - 2 * tf);
            this.as2 = 2f * t2 * tf;
            this.as3 = 2f * t3 * tf;
            //this.s33 = 0;
            //this.s22 = 0;
            //this.z33 = 0;
            //this.z22 = 0;
            //this.r33 = 0;
            //this.r22 = 0;
        }

        static short[][] contourIndices;
        static BoxTube()
        {
            contourIndices = new short[4][];
            contourIndices[0] = new short[] { 0, 2, 4, 6, 0};
            contourIndices[1] = new short[] { 0, 2, 4, 6, 0 };
            contourIndices[2] = new short[] { 0, 2, 4, 6, 0 };
            contourIndices[3] = new short[] { 0, 1, 2, 3, 4, 5, 6, 7, 0};
        }

        public override short[][] ContourIndices
        {
            get
            {
                return contourIndices;
            }
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
            UpdateData();
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

        public override string Shape
        {
            get
            {
                return "B";
            }
        }
    }
}
