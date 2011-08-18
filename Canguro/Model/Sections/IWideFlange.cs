using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class IWideFlange : FrameSection
    {
        public IWideFlange(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, t3, t2, tf, tw, t2b, tfb, dis, area, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }

        public IWideFlange(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw)
            : base(name, shape, material, concreteProperties)
        {
            this.t3 = t3;
            this.t2 = t2;
            this.tf = tf;
            this.tw = tw;
            this.t2b = t2;
            this.tfb = tf;
            this.dis = 0; 
            initContourAndLOD();
        }

        public override float T2
        {
            set
            {
                base.T2b = value;
                base.T2 = value;
            }
        }

        public override float Tf
        {
            set
            {
                base.Tf = value;
                base.Tfb = value;
            }
        }

        protected void UpdateData()
        {
            this.area = t3 * tw + 2f * t2 * tf;
            this.torsConst = (t3 * tw * tw * tw + 2f * t2 * tf * tf * tf) / 3f;
            this.i33 = t3 * t3 * (t3 * tw + 6 * t2 * tf) / 12f;
            this.i22 = t2 * t2 * t2 * tf / 6f;
//            this.as2 = tf * t3 * 5f / 3f;
//            this.as3 = t2 * tw;
//            this.s33 = i33 / (t3 / 2);
//            this.s22 = i22 / (t2 / 2);
//            this.z33 = 0;
//            this.z22 = 0;
//            this.r33 = (float)Math.Sqrt(i33 / area);
//            this.r22 = (float)Math.Sqrt(i22 / area);
        }

        static short[][] contourIndices;
        static IWideFlange()
        {
            contourIndices = new short[4][];
            contourIndices[0] = new short[] { 0, 2, 8, 10, 0 };
            contourIndices[1] = new short[] { 0, 2, 5, 8, 10, 13, 0 };
            contourIndices[2] = new short[] { 0, 2, 3, 4, 6, 7, 8, 10, 11, 12, 14, 15, 0 };
            contourIndices[3] = new short[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0};
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
            contour[0] = new Microsoft.DirectX.Vector2[16];
            contour[1] = new Microsoft.DirectX.Vector2[16];

            contour[0][0] = new Microsoft.DirectX.Vector2(-t2b / 2.0f, -t3 / 2.0f);
            contour[0][1] = new Microsoft.DirectX.Vector2(0, contour[0][0].Y);
            contour[0][2] = new Microsoft.DirectX.Vector2(-contour[0][0].X, contour[0][0].Y);
            contour[0][3] = new Microsoft.DirectX.Vector2(contour[0][2].X, contour[0][2].Y + tfb);
            contour[0][4] = new Microsoft.DirectX.Vector2(contour[0][3].X + (tw - t2b) / 2.0f, contour[0][3].Y);
            contour[0][5] = new Microsoft.DirectX.Vector2(contour[0][4].X, 0);
            contour[0][6] = new Microsoft.DirectX.Vector2(contour[0][4].X, t3/2.0f - tf);
            contour[0][7] = new Microsoft.DirectX.Vector2(t2 / 2.0f, contour[0][6].Y);
            contour[0][8] = new Microsoft.DirectX.Vector2(contour[0][7].X, contour[0][7].Y + tf);
            contour[0][9] = new Microsoft.DirectX.Vector2(0, contour[0][8].Y);
            contour[0][10] = new Microsoft.DirectX.Vector2(-contour[0][8].X, contour[0][8].Y);
            contour[0][11] = new Microsoft.DirectX.Vector2(contour[0][0].X, contour[0][6].Y);
            contour[0][12] = new Microsoft.DirectX.Vector2(contour[0][11].X + (t2 - tw) / 2.0f, contour[0][6].Y);
            contour[0][13] = new Microsoft.DirectX.Vector2(contour[0][12].X, 0);
            contour[0][14] = new Microsoft.DirectX.Vector2(contour[0][12].X, contour[0][3].Y);
            contour[0][15] = new Microsoft.DirectX.Vector2(contour[0][0].X, contour[0][3].Y);

            contour[1][0] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][1] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][2] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][3] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][4] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][5] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][6] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][7] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][8] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][9] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][10] = new Microsoft.DirectX.Vector2(-1, 0);
            contour[1][11] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][12] = new Microsoft.DirectX.Vector2(-1, 0);
            contour[1][13] = new Microsoft.DirectX.Vector2(-1, 0);
            contour[1][14] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][15] = new Microsoft.DirectX.Vector2(-1, 0);

            buildHighStressCover();
            UpdateData();
        }

        protected override void buildHighStressCover()
        {
            coverHighStress = new short[14*3];

            // First triangle
            coverHighStress[0] = 0; coverHighStress[1] = 14; coverHighStress[2] = 15;
            // Second triangle
            coverHighStress[3] = 0; coverHighStress[4] = 1; coverHighStress[5] = 14;
            // Third triangle
            coverHighStress[6] = 1; coverHighStress[7] = 4; coverHighStress[8] = 14;
            // Fourth triangle
            coverHighStress[9] = 1; coverHighStress[10] = 2; coverHighStress[11] = 4;
            // Fifth triangle
            coverHighStress[12] = 2; coverHighStress[13] = 3; coverHighStress[14] = 4;
            // Sixth triangle
            coverHighStress[15] = 4; coverHighStress[16] = 5; coverHighStress[17] = 14;
            // Seventh triangle
            coverHighStress[18] = 5; coverHighStress[19] = 13; coverHighStress[20] = 14;
            // Eight triangle
            coverHighStress[21] = 5; coverHighStress[22] = 6; coverHighStress[23] = 13;
            // Ninth triangle
            coverHighStress[24] = 6; coverHighStress[25] = 12; coverHighStress[26] = 13;
            // Tenth triangle
            coverHighStress[27] = 6; coverHighStress[28] = 7; coverHighStress[29] = 8;
            // Eleventh triangle
            coverHighStress[30] = 6; coverHighStress[31] = 8; coverHighStress[32] = 9;
            // Twelveth triangle
            coverHighStress[33] = 6; coverHighStress[34] = 9; coverHighStress[35] = 12;
            // Thirteenth triangle
            coverHighStress[36] = 9; coverHighStress[37] = 10; coverHighStress[38] = 12;
            // Fourteenth triangle
            coverHighStress[39] = 10; coverHighStress[40] = 11; coverHighStress[41] = 12;
        }

        protected override void buildHighLODCover()
        {
            coverHigh = new short[10 * 3];

            // First triangle
            coverHigh[0] = 0; coverHigh[1] = 10; coverHigh[2] = 11;
            // Second triangle
            coverHigh[3] = 0; coverHigh[4] = 1; coverHigh[5] = 10;
            // Third triangle
            coverHigh[6] = 1; coverHigh[7] = 2; coverHigh[8] = 3;
            // Fourth triangle
            coverHigh[9] = 1; coverHigh[10] = 3; coverHigh[11] = 10;
            // Fifth triangle
            coverHigh[12] = 3; coverHigh[13] = 4; coverHigh[14] = 10;
            // Sixth triangle
            coverHigh[15] = 4; coverHigh[16] = 9; coverHigh[17] = 10;
            // Seventh triangle
            coverHigh[18] = 4; coverHigh[19] = 5; coverHigh[20] = 6;
            // Eight triangle
            coverHigh[21] = 4; coverHigh[22] = 7; coverHigh[23] = 9;
            // Ninth triangle
            coverHigh[24] = 6; coverHigh[25] = 7; coverHigh[26] = 4;
            // Tenth triangle
            coverHigh[27] = 7; coverHigh[28] = 8; coverHigh[29] = 9;
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
