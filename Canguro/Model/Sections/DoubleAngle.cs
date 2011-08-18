using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Model.Section
{
    [Serializable]
    public class DoubleAngle : FrameSection
    {
        public DoubleAngle(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, t3, t2, tf, tw, t2b, tfb, dis, area, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }
        
        public DoubleAngle(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw)
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
            this.as2 = 2f * t2 * tf;
            this.as3 = 2f * t3 * tf;
            //this.s33 = 0;
            //this.s22 = 0;
            //this.z33 = 0;
            //this.z22 = 0;
            //this.r33 = 0;
            //this.r22 = 0;
            //CalcProps();
        }

        static short[][] contourIndices;
        static DoubleAngle()
        {
            contourIndices = new short[4][];
            contourIndices[0] = new short[] { 1, 7, 14, 1};
            contourIndices[1] = new short[] { 1, 4, 7, 9, 12, 14, 17, 20, 1 };
            contourIndices[2] = new short[] { 0, 1, 2, 4, 6, 7, 9, 10, 11, 12, 14, 15, 17, 19, 20, 21 };
            contourIndices[3] = new short[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
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
            int nVertices = 22;

            contour[0] = new Vector2[nVertices];
            contour[1] = new Vector2[nVertices];

            float minSeparation = 0.001f;
            float lw = t2 / 2f; // L section Width

            // First L
            contour[0][0] = new Vector2(dis / 2.0f + minSeparation, t3 / 2.0f);
            contour[0][1] = new Vector2(contour[0][0].X, 0f);
            contour[0][2] = new Vector2(contour[0][1].X + tw, 0f);
            contour[0][3] = new Vector2(contour[0][2].X, (t3 - tf) / 2.0f);
            contour[0][4] = new Vector2(contour[0][3].X, t3 - tf);
            contour[0][5] = new Vector2(contour[0][2].X + lw / 2.0f, contour[0][4].Y);
            contour[0][6] = new Vector2(contour[0][2].X + lw, contour[0][5].Y);
            contour[0][7] = new Vector2(contour[0][2].X + lw, contour[0][5].Y + tf);
            contour[0][8] = new Vector2(contour[0][0].X + (tw + lw) / 2.0f, contour[0][7].Y);
            contour[0][9] = new Vector2(contour[0][0].X, contour[0][8].Y);
            contour[0][10] = contour[0][0];

            #region Legacy
            //contour[0][0] = new Microsoft.DirectX.Vector2(dis / 2.0f, 0);
            //contour[0][1] = new Microsoft.DirectX.Vector2(contour[0][0].X + tw, 0);
            //contour[0][2] = new Microsoft.DirectX.Vector2(contour[0][1].X, (t3 - tf) / 2.0f);
            //contour[0][3] = new Microsoft.DirectX.Vector2(contour[0][1].X, t3 - tf);
            //contour[0][4] = new Microsoft.DirectX.Vector2((t2 + dis / 2.0f + tw) / 2.0f, t3 - tf);
            //contour[0][5] = new Microsoft.DirectX.Vector2((t2 + dis / 2.0f), t3 - tf);
            //contour[0][6] = new Microsoft.DirectX.Vector2(t2 + dis / 2.0f, t3);
            //contour[0][7] = new Microsoft.DirectX.Vector2((t2 + dis / 2.0f) / 2.0f, t3);
            //contour[0][8] = new Microsoft.DirectX.Vector2(dis / 2.0f, t3);
            //contour[0][9] = new Microsoft.DirectX.Vector2(dis / 2.0f, t3 / 2.0f);
            #endregion

            // Second L
            contour[0][11] = new Vector2(-contour[0][10].X, contour[0][10].Y);
            contour[0][12] = new Vector2(-contour[0][9].X, contour[0][9].Y);
            contour[0][13] = new Vector2(-contour[0][8].X, contour[0][8].Y);
            contour[0][14] = new Vector2(-contour[0][7].X, contour[0][7].Y);
            contour[0][15] = new Vector2(-contour[0][6].X, contour[0][6].Y);
            contour[0][16] = new Vector2(-contour[0][5].X, contour[0][5].Y);
            contour[0][17] = new Vector2(-contour[0][4].X, contour[0][4].Y);
            contour[0][18] = new Vector2(-contour[0][3].X, contour[0][3].Y);
            contour[0][19] = new Vector2(-contour[0][2].X, contour[0][2].Y);
            contour[0][20] = new Vector2(-contour[0][1].X, contour[0][1].Y);
            contour[0][21] = new Vector2(-contour[0][0].X, contour[0][0].Y);

            float a1 = t3 * tw;
            float a2 = (t2 - tw) * tf;
            float cgy = (a1 * (t3 / 2.0f) + a2 * ((t3 + tf) / 2.0f)) / (a1 + a2);
            for (int i = 0; i < nVertices; i++)
                contour[0][i].Y -= cgy;

            // Start first L
            contour[1][0] = new Vector2(-1, 0);
            contour[1][1] = new Vector2(0, -1);
            contour[1][2] = new Vector2(1, 0);
            contour[1][3] = new Vector2(1, 0);
            contour[1][4] = new Vector2(0, -1);
            contour[1][5] = new Vector2(0, -1);
            contour[1][6] = new Vector2(1, 0);
            contour[1][7] = new Vector2(0, 1);
            contour[1][8] = new Vector2(0, 1);
            contour[1][9] = new Vector2(-1, 0);
            contour[1][10] = new Vector2(-1, 0);

            // Start second L
            contour[1][11] = new Vector2(1, 0);
            contour[1][12] = new Vector2(0, 1);
            contour[1][13] = new Vector2(0, 1);
            contour[1][14] = new Vector2(-1, 0);
            contour[1][15] = new Vector2(0, -1);
            contour[1][16] = new Vector2(0, -1);
            contour[1][17] = new Vector2(-1, 0);
            contour[1][18] = new Vector2(-1, 0);
            contour[1][19] = new Vector2(0, -1);
            contour[1][20] = new Vector2(1, 0);
            contour[1][21] = new Vector2(1, 0);
            
            buildHighStressCover();
            UpdateData();
        }

        protected override void buildHighStressCover()
        {
            coverHighStress = new short[16*3];

            // First triangle
            coverHighStress[0] = 1; coverHighStress[1] = 2; coverHighStress[2] = 3;
            // Second triangle
            coverHighStress[3] = 1; coverHighStress[4] = 3; coverHighStress[5] = 0;
            // Third triangle
            coverHighStress[6] = 3; coverHighStress[7] = 4; coverHighStress[8] = 0;
            // Fourth triangle
            coverHighStress[9] = 0; coverHighStress[10] = 4; coverHighStress[11] = 9;
            // Fifth triangle
            coverHighStress[12] = 4; coverHighStress[13] = 8; coverHighStress[14] = 9;
            // Sixth triangle
            coverHighStress[15] = 4; coverHighStress[16] = 5; coverHighStress[17] = 8;
            // Seventh triangle
            coverHighStress[18] = 5; coverHighStress[19] = 7; coverHighStress[20] = 8;
            // Eight triangle
            coverHighStress[21] = 5; coverHighStress[22] = 6; coverHighStress[23] = 7;

            // Ninth triangle
            coverHighStress[24] = 19; coverHighStress[25] = 20; coverHighStress[26] = 18;
            // Tenth triangle
            coverHighStress[27] = 20; coverHighStress[28] = 11; coverHighStress[29] = 18;
            // Eleventh triangle
            coverHighStress[30] = 11; coverHighStress[31] = 17; coverHighStress[32] = 18;
            // Twelveth triangle
            coverHighStress[33] = 11; coverHighStress[34] = 12; coverHighStress[35] = 17;
            // Thirteenth triangle
            coverHighStress[36] = 12; coverHighStress[37] = 13; coverHighStress[38] = 17;
            // Fourteenth triangle
            coverHighStress[39] = 13; coverHighStress[40] = 16; coverHighStress[41] = 17;
            // Fifteenth triangle
            coverHighStress[42] = 13; coverHighStress[43] = 14; coverHighStress[44] = 16;
            // Sixteenth triangle
            coverHighStress[45] = 14; coverHighStress[46] = 15; coverHighStress[47] = 16;
        }

        protected override void buildHighLODCover()
        {
            coverHigh = new short[10 * 3];

            // First triangle
            coverHigh[0] = 0; coverHigh[1] = 1; coverHigh[2] = 2;
            // Second triangle
            coverHigh[3] = 0; coverHigh[4] = 3; coverHigh[5] = 6;
            // Third triangle
            coverHigh[6] = 0; coverHigh[7] = 2; coverHigh[8] = 3;
            // Fourth triangle
            coverHigh[9] = 3; coverHigh[10] = 4; coverHigh[11] = 5;
            // Fifth triangle
            coverHigh[12] = 3; coverHigh[13] = 5; coverHigh[14] = 6;

            // Sixth triangle
            coverHigh[15] = 8; coverHigh[16] = 13; coverHigh[17] = 14;
            // Seventh triangle
            coverHigh[18] = 8; coverHigh[19] = 12; coverHigh[20] = 13;
            // Eight triangle
            coverHigh[21] = 8; coverHigh[22] = 9; coverHigh[23] = 12;
            // Ninth triangle
            coverHigh[24] = 9; coverHigh[25] = 10; coverHigh[26] = 12;
            // Tenth triangle
            coverHigh[27] = 10; coverHigh[28] = 11; coverHigh[29] = 12;
        }

        public override string Shape
        {
            get
            {
                return "2L";
            }
        }
    }
}
