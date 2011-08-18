using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    /// <summary>
    /// Represents a Full round section.
    /// </summary>
    [Serializable]
    public class Circle : FrameSection
    {
        public Circle(string name, string shape, Material.Material material, ConcreteSectionProps concreteProperties, float t3, float t2, float tf, float tw, float t2b, float tfb, float dis, float area, float torsConst, float i33, float i22, float as2, float as3, float s33, float s22, float z33, float z22, float r33, float r22)
            : base(name, shape, material, concreteProperties, t3, t2, tf, tw, t2b, tfb, dis, area, torsConst, i33, i22, as2, as3, s33, s22, z33, z22, r33, r22) { }

        public Circle(string name, Material.Material material, ConcreteSectionProps concreteProperties, float d)
            : base(name, "RN", material, concreteProperties)
        {
            t3 = d;
            initContourAndLOD();
        }

        protected const int segments = 8;

        static short[][] contourIndices;
        static Circle()
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
        }

        public override short[][] ContourIndices
        {
            get
            {
                return contourIndices;
            }
        }

        protected void UpdateData()
        {
            float d = t3;
            float r = d / 2f;
            float PI = (float)Math.PI;

            this.area = PI * r * r;
            this.torsConst = PI * r * r * r * r / 2f;
            this.i33 = PI * r * r * r * r / 4f;
            this.i22 = i33;
            this.as2 = 0.9f * PI * r * r;
            this.as3 = as2;
            this.s33 = i33 / r;
            this.s22 = s33;
            this.z33 = d * d * d / 6f;
            this.z22 = z33;
            this.r33 = r / 2f;
            this.r22 = r33;
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
                contour[1][i] = contour[0][i];
            }

            buildHighStressCover();
            UpdateData();
        }

        protected override void buildHighStressCover()
        {
            coverHighStress = new short[6 * 3];

            // First triangle
            coverHighStress[0] = 0; coverHighStress[1] = 1; coverHighStress[2] = 2;
            // Second triangle
            coverHighStress[3] = 2; coverHighStress[4] = 3; coverHighStress[5] = 4;
            // Third triangle
            coverHighStress[6] = 4; coverHighStress[7] = 5; coverHighStress[8] = 6;
            // Fourth triangle
            coverHighStress[9] = 6; coverHighStress[10] = 7; coverHighStress[11] = 0;
            // Fifth triangle
            coverHighStress[12] = 0; coverHighStress[13] = 4; coverHighStress[14] = 6;
            // Sixth triangle
            coverHighStress[15] = 0; coverHighStress[16] = 2; coverHighStress[17] = 4;
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
        public float D
        {
            get { return T3; }
            set 
            { 
                T3 = value; 
            }
        }

        public override string Shape
        {
            get { return "RN"; }
        }

        public override bool FaceNormals
        {
            get { return false; }
        }
    }
}
