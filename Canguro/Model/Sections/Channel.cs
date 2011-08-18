using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class Channel : FrameSection
    {
        public Channel(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, t3, t2, tf, tw, t2b, tfb, dis, area, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }
        
        public Channel(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw)
            : base(name, shape, material, concreteProperties) 
        { 
            this.t3 = t3;
            this.t2 = t2;
            this.tf = tf;
            this.tw = 0;
            this.t2b = 0;
            this.tfb = 0;
            this.dis = 0;
            initContourAndLOD();
        }

        private void UpdateData()
        {
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
        static Channel()
        {
            contourIndices = new short[4][];
            contourIndices[0] = new short[] { 0, 2, 10, 12, 0 };
            contourIndices[1] = new short[] { 0, 2, 6, 10, 12, 0 };
            contourIndices[2] = new short[] { 0, 2, 3, 5, 7, 9, 10, 12, 0 };
            contourIndices[3] = new short[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 0 };
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
            contour[0] = new Microsoft.DirectX.Vector2[14];
            contour[1] = new Microsoft.DirectX.Vector2[14];

            contour[0][0] = new Microsoft.DirectX.Vector2(0, 0);
            contour[0][1] = new Microsoft.DirectX.Vector2(t2 / 2.0f, 0);
            contour[0][2] = new Microsoft.DirectX.Vector2(t2, 0);
            contour[0][3] = new Microsoft.DirectX.Vector2(t2, tf);
            contour[0][4] = new Microsoft.DirectX.Vector2((t2 + tw) / 2.0f, tf);
            contour[0][5] = new Microsoft.DirectX.Vector2(tw, tf);
            contour[0][6] = new Microsoft.DirectX.Vector2(tw, t3 / 2.0f);
            contour[0][7] = new Microsoft.DirectX.Vector2(tw, t3 - tf);
            contour[0][8] = new Microsoft.DirectX.Vector2((t2 + tw) / 2.0f, t3 - tf);
            contour[0][9] = new Microsoft.DirectX.Vector2(t2, t3 - tf);
            contour[0][10] = new Microsoft.DirectX.Vector2(t2, t3);
            contour[0][11] = new Microsoft.DirectX.Vector2(t2 / 2.0f, t3);
            contour[0][12] = new Microsoft.DirectX.Vector2(0, t3);
            contour[0][13] = new Microsoft.DirectX.Vector2(0, t3 / 2.0f);

            float a1 = t3 * tw;
            float a2 = 2 * (t2 - tw) * tf;
            float c3 = (a1 * (tw / 2.0F) + a2 * (tw + t2 / 2.0F)) / (a1 + a2);
            float c2 = t3 / 2.0F;

            for (int i = 0; i < 14; i++)
            {
                contour[0][i].X = contour[0][i].X - c3;
                contour[0][i].Y -= c2;
            }

            contour[1][0] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][1] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][2] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][3] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][4] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][5] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][6] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][7] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][8] = new Microsoft.DirectX.Vector2(0, -1);
            contour[1][9] = new Microsoft.DirectX.Vector2(1, 0);
            contour[1][10] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][11] = new Microsoft.DirectX.Vector2(0, 1);
            contour[1][12] = new Microsoft.DirectX.Vector2(-1, 0);
            contour[1][13] = new Microsoft.DirectX.Vector2(-1, 0);

            buildHighStressCover();
            UpdateData();
        }

        protected override void buildHighStressCover()
        {
            coverHighStress = new short[12*3];

            // First triangle
            coverHighStress[0] = 8; coverHighStress[1] = 9; coverHighStress[2] = 10;
            // Second triangle
            coverHighStress[3] = 8; coverHighStress[4] = 10; coverHighStress[5] = 11;
            // Third triangle
            coverHighStress[6] = 8; coverHighStress[7] = 11; coverHighStress[8] = 7;
            // Fourth triangle
            coverHighStress[9] = 7; coverHighStress[10] = 11; coverHighStress[11] = 12;
            // Fifth triangle
            coverHighStress[12] = 7; coverHighStress[13] = 12; coverHighStress[14] = 13;
            // Sixth triangle
            coverHighStress[15] = 7; coverHighStress[16] = 13; coverHighStress[17] = 6;
            // Seventh triangle
            coverHighStress[18] = 6; coverHighStress[19] = 13; coverHighStress[20] = 5;
            // Eight triangle
            coverHighStress[21] = 13; coverHighStress[22] = 0; coverHighStress[23] = 5;
            // Ninth triangle
            coverHighStress[24] = 0; coverHighStress[25] = 1; coverHighStress[26] = 5;
            // Tenth triangle
            coverHighStress[27] = 1; coverHighStress[28] = 4; coverHighStress[29] = 5;
            // Eleventh triangle
            coverHighStress[30] = 1; coverHighStress[31] = 2; coverHighStress[32] = 4;
            // Twelveth triangle
            coverHighStress[33] = 2; coverHighStress[34] = 3; coverHighStress[35] = 4;
        }

        protected override void buildHighLODCover()
        {
            coverHigh = new short[6 * 3];

            // First triangle
            coverHigh[0] = 0; coverHigh[1] = 1; coverHigh[2] = 3;
            // Second triangle
            coverHigh[3] = 0; coverHigh[4] = 3; coverHigh[5] = 7;
            // Third triangle
            coverHigh[6] = 1; coverHigh[7] = 2; coverHigh[8] = 3;
            // Fourth triangle
            coverHigh[9] = 3; coverHigh[10] = 4; coverHigh[11] = 7;
            // Fifth triangle
            coverHigh[12] = 4; coverHigh[13] = 5; coverHigh[14] = 7;
            // Sixth triangle
            coverHigh[15] = 5; coverHigh[16] = 6; coverHigh[17] = 7;
        }

        public override string Shape
        {
            get
            {
                return "C";
            }
        }
    }
}
