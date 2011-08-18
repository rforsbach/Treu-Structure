using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class Tee : FrameSection
    {
        public Tee(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, t3, t2, tf, tw, t2b, tfb, dis, area, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }

        
        public Tee(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw)
            : base(name, shape, material, concreteProperties)
        {
            this.t3 = t3;
            this.t2 = t2;
            this.tf = tf;
            this.tw = tw; 
            initContourAndLOD();
        }

        protected void UpdateData()
        {
            //this.t2b = 0;
            //this.tfb = 0;
            //this.dis = 0;
            //this.area = 0;
            //this.torsConst = 0;
            //this.i33 = 0;
            //this.i22 = 0;
            //this.as2 = 0;
            //this.as3 = 0;
            //this.s33 = 0;
            //this.s22 = 0;
            //this.z33 = 0;
            //this.z22 = 0;
            //this.r33 = 0;
            //this.r22 = 0;
            CalcProps();
        }

        static short[][] contourIndices;
        static Tee()
        {
            contourIndices = new short[4][];
            contourIndices[0] = contourIndices[1] = new short[] {0, 1, 5, 7, 0};
            contourIndices[2] = new short[] { 0, 1, 3, 4, 5, 7, 8, 9, 0 };
            contourIndices[3] = new short[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0 };
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
            contour[0] = new Microsoft.DirectX.Vector2[11];
            contour[1] = new Microsoft.DirectX.Vector2[11];

            contour[0][0] = new Microsoft.DirectX.Vector2(-tw / 2.0f, 0);
            contour[0][1] = new Microsoft.DirectX.Vector2(contour[0][0].X + tw, contour[0][0].Y);
            contour[0][2] = new Microsoft.DirectX.Vector2(contour[0][1].X, (t3 - tf) / 2.0f);
            contour[0][3] = new Microsoft.DirectX.Vector2(contour[0][2].X, t3 - tf);
            contour[0][4] = new Microsoft.DirectX.Vector2(t2 / 2.0f, contour[0][3].Y);
            contour[0][5] = new Microsoft.DirectX.Vector2(contour[0][4].X, t3);
            contour[0][6] = new Microsoft.DirectX.Vector2(0, contour[0][5].Y);
            contour[0][7] = new Microsoft.DirectX.Vector2(-t2 / 2.0f, contour[0][6].Y);
            contour[0][8] = new Microsoft.DirectX.Vector2(contour[0][7].X, contour[0][3].Y);
            contour[0][9] = new Microsoft.DirectX.Vector2(contour[0][0].X, contour[0][8].Y);
            contour[0][10] = new Microsoft.DirectX.Vector2(contour[0][0].X, 0);

            float a1 = t3 * tw;
            float a2 = (t2 - tw) * tf;
            float cgy = (a1 * (t3 / 2.0f) + a2 * ((t3 + tf) / 2.0f)) / (a1 + a2);
            for (int i = 0; i < 11; i++)
                contour[0][i].Y -= cgy;

            contour[1][0] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][1] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][2] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][3] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][4] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][5] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][6] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][7] = new Microsoft.DirectX.Vector2(-1, 0);
            contour[1][8] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][9] = new Microsoft.DirectX.Vector2(-1, 0);
            contour[1][10] = new Microsoft.DirectX.Vector2(-1, 0);

            buildHighStressCover();
            UpdateData();
        }

        protected override void buildHighStressCover()
        {
            coverHighStress = new short[9*3];

            // First triangle
            coverHighStress[0] = 0; coverHighStress[1] = 1; coverHighStress[2] = 2;
            // Second triangle
            coverHighStress[3] = 2; coverHighStress[4] = 10; coverHighStress[5] = 0;
            // Third triangle
            coverHighStress[6] = 2; coverHighStress[7] = 3; coverHighStress[8] = 10;
            // Fourth triangle
            coverHighStress[9] = 3; coverHighStress[10] = 9; coverHighStress[11] = 10;
            // Fifth triangle
            coverHighStress[12] = 3; coverHighStress[13] = 4; coverHighStress[14] = 5;
            // Sixth triangle
            coverHighStress[15] = 3; coverHighStress[16] = 5; coverHighStress[17] = 6;
            // Seventh triangle
            coverHighStress[18] = 3; coverHighStress[19] = 6; coverHighStress[20] = 9;
            // Eight triangle
            coverHighStress[21] = 9; coverHighStress[22] = 6; coverHighStress[23] = 7;
            // Ninth triangle
            coverHighStress[24] = 9; coverHighStress[25] = 7; coverHighStress[26] = 8;
        }

        protected override void buildHighLODCover()
        {
            coverHigh = new short[6 * 3];

            // First triangle
            coverHigh[0] = 0; coverHigh[1] = 1; coverHigh[2] = 2;
            // Second triangle
            coverHigh[3] = 0; coverHigh[4] = 2; coverHigh[5] = 7;
            // Third triangle
            coverHigh[6] = 2; coverHigh[7] = 3; coverHigh[8] = 4;
            // Fourth triangle
            coverHigh[9] = 2; coverHigh[10] = 4; coverHigh[11] = 5;
            // Fifth triangle
            coverHigh[12] = 2; coverHigh[13] = 5; coverHigh[14] = 7;
            // Sixth triangle
            coverHigh[15] = 5; coverHigh[16] = 6; coverHigh[17] = 7;
        }

        public override string Shape
        {
            get
            {
                return "T";
            }
        }
    }
}
