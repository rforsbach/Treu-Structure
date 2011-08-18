using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Model.Load;
using Canguro.Utility;


namespace Canguro.Analysis
{
    class AreaDeformationCalculator : ModelCalculator
    {
        private DistributedSpanLoad selfWeight;

        public AreaDeformationCalculator()
        {
            
        }

        public enum DeformationAxis { Local2, Local3};

        public bool GetDeformationVectors(AreaElement area, Vector3[] localAxes, AbstractCase abstractCase, Vector3[] ctrlPoints, Vector3[] deformations)
        {
            // Dummy routine

            int verticesInList= ctrlPoints.Length;

            for (int i = 0; i < verticesInList; ++i)
                getDeformationAt(area, abstractCase, ctrlPoints[i], ref deformations[i]);

            return true;
        }

        private void getDeformationAt(AreaElement area, AbstractCase abstractCase, Vector3 request, ref Vector3 deformation)
        {
            Random myRandomizer = new Random(0);

            float max = 0.5f;

            deformation.X += max * (float)myRandomizer.NextDouble();
            deformation.Y += max * (float)myRandomizer.NextDouble();
            deformation.Z += max * (float)myRandomizer.NextDouble();
        }


        #region Estas son copias de las lineas

        public Vector3[] GetCurve(AreaElement area, AbstractCase ac, int numPoints, float deformationScale, float paintScaleFactorTranslation, out float[] xPos)
        {
            xPos = null;
            return null;
        }

        private void getCurvedAxis(AreaElement area, AbstractCase ac, DeformationAxis component, float[,] controlPoints)
        {
            
        }

        public float[,] GetCurvedAxis(AreaElement area, AbstractCase ac, DeformationAxis component, int numPoints)
        {
            return null;
        }

        public float[] GetCurvedPoint(AreaElement area, AbstractCase ac, DeformationAxis component, float xPos)
        {
            return null;
        }
        #endregion
    }
}
