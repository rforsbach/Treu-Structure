using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class Pipe : FrameSection
    {
        public Pipe(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, t3, t2, tf, tw, t2b, tfb, dis, area, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }

        public Pipe(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float tw)
            : base(name, shape, material, concreteProperties) 
        { 
            this.t3 = t3;
            this.t2 = 0;
            this.tf = 0;
            this.tw = tw;
            this.t2b = 0;
            this.tfb = 0;
            this.dis = 0;
            initContourAndLOD();
        }

        private void UpdateData()
        {
            float r = t3 / 2f;
            float di = t3 - 2 * tw;
            float ri = di / 2f;

            this.area = (float)Math.PI * (r*r - ri*ri);
            //this.torsConst = 0;

            this.i33 = (float)(Math.PI * (t3*t3*t3*t3 - di*di*di*di) / 64f);
            this.i22 = i33;
            this.as2 = (float)Math.PI * t3 * tw;
            this.as3 = as2;
            //this.s33 = 0;
            //this.s22 = 0;
            //this.z33 = 0;
            //this.z22 = 0;
            //this.r33 = 0;
            //this.r22 = 0;
        }

        protected const int segments = 8;

        static short[][] contourIndices;
        static Pipe()
        {
            contourIndices = new short[4][];
            contourIndices[3] = new short[segments + 1];
            for (int i = 0; i < segments; i++)
                contourIndices[3][i] = (short)i;
            
            // Close the shape
            contourIndices[3][segments] = 0;

            // LOD Shapes
            contourIndices[0] = contourIndices[1] = new short[] { 0, 2, 4, 6, 0 };
            contourIndices[2] = contourIndices[3];

            // If want to use 4 vertices instead of 8 when having a LOD in High
            //contourIndices[0] = contourIndices[1] = contourIndices[2] =new short[] { 0, 2, 4, 6, 0 };
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
            contour[0] = new Microsoft.DirectX.Vector2[segments];
            contour[1] = new Microsoft.DirectX.Vector2[segments];
            int i;
            float angle, delta = 2.0f * (float)Math.PI / (float)segments;
            float r = t3 / 2.0f;

            for (i = 0, angle = 0; i < segments; angle += delta, i++)
            {
                contour[0][i] = new Microsoft.DirectX.Vector2((float)Math.Cos(angle) * r, (float)Math.Sin(angle) * r);
                contour[1][i] = Microsoft.DirectX.Vector2.Normalize(contour[0][i]);
            }

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
            coverHigh = coverHighStress;
            //coverHigh = new short[2 * 3];

            //// First triangle
            //coverHigh[0] = 0; coverHigh[1] = 1; coverHigh[2] = 2;
            //// Second triangle
            //coverHigh[3] = 0; coverHigh[4] = 2; coverHigh[5] = 3;
        }

        public override string Shape
        {
            get
            {
                return "P";
            }
        }

        public override bool FaceNormals
        {
            get { return false; }
        }
    }
}
