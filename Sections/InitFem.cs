using System;
using System.Collections.Generic;
using System.Text;
using dnAnalytics.LinearAlgebra;

namespace Canguro.Analysis.Sections
{
    internal class InitFem   // Corresponds to initfem.f90
    {
        DenseMatrix gaussPoint = new DenseMatrix(9, 2);
        DenseVector gaussWeight = new DenseVector(9);
        DenseMatrix shapeFunction = new DenseMatrix(9, 9);
        DenseMatrix shapeEta = new DenseMatrix(9, 9);
        DenseMatrix shapeZeta = new DenseMatrix(9, 9);

        const double Factor = 3.8729833462074169, Zero = 0.0;
        const double A = -12.0, B = 5.0, C = 8.0, D = 18.0;
        const double E = 2.0 * (-5.0 - Factor), F = 3.0 * (5.0 - Factor), G = 2.0 * (-5.0 + Factor);
        const double H = -5.0 * (-4.0 + Factor), P = 5.0 * (4.0 + Factor), Q = 3.0 * (5.0 + Factor);

        DenseMatrix smoothingMatrix;

        public DenseMatrix GaussPoint
        {
            get { return gaussPoint; }
        }

        public DenseVector GaussWeight
        {
            get { return gaussWeight; }
        }

        public DenseMatrix ShapeFunction
        {
            get { return shapeFunction; }
        }

        public DenseMatrix ShapeEta
        {
            get { return shapeEta; }
        }

        public DenseMatrix ShapeZeta
        {
            get { return shapeZeta; }
        }

        public InitFem()    // InitializeFEA
        {
            int n;
            double[] pnt = new double[3];
            double[] w = new double[3];

            smoothingMatrix = new DenseMatrix(new double[,] {{C, A, C, A, D, A, C, A, C},
                                                             {G, Zero, E, F, Zero, Q, G, Zero, E}, 
                                                             {E, Zero, G, Q, Zero, F, E, Zero, G}, 
                                                             {G, F, G, Zero, Zero, Zero, E, Q, E}, 
                                                             {H, Zero, B, Zero, Zero, Zero, B, Zero, P}, 
                                                             {B, Zero, H, Zero, Zero, Zero, P, Zero, B}, 
                                                             {E, Q, E, Zero, Zero, Zero, G, F, G}, 
                                                             {B, Zero, P, Zero, Zero, Zero, H, Zero, B},
                                                             {P, Zero, B, Zero, Zero, Zero, B, Zero, H}});
            smoothingMatrix = (DenseMatrix)smoothingMatrix.Transpose();

            pnt[0] = Zero; pnt[1] = 3.0 / Factor; pnt[2] = -pnt[1];
            w[0] = 8.0 / 9.0; w[1] = 5.0 / 9.0; w[2] = w[1];

            for (int k = 0; k < 3; k++)
                for (int m = 0; m < 3; m++)
                {
                    n = 3 * k + m;
                    gaussPoint[n, 0] = pnt[k]; gaussPoint[n, 1] = pnt[m];
                    gaussWeight[n] = w[k] * w[m];
                }

            for (int m = 0; m < 9; m++)
                shapeNineNode(gaussPoint[m, 0], gaussPoint[m, 1], shapeFunction, shapeEta, shapeZeta, m);
        }

        private void shapeNineNode(double eta, double zeta, DenseMatrix shape, DenseMatrix dShapeEta, DenseMatrix dShapeZeta, int row)
        {
            double etap, etam, etap2, etam2, etapm, etazeta, zetap, zetam, zetap2, zetam2, zetapm;
            double v = 0.25, w = 0.5, One = 1.0;

            etap = One + eta;
            etam = One - eta;
            zetap = One + zeta;
            zetam = One - zeta;
            etazeta = eta * zeta;
            zetapm = zetap * zetam;
            etapm = etap * etam;
            etam2 = etam - eta;
            etap2 = etap + eta;
            zetam2 = zetam - zeta;
            zetap2 = zetap + zeta;

            shape.SetRow(row, new double[] { v*etazeta*etam*zetam, -w*eta*etam*zetapm, -v*etazeta*etam*zetap, 
                                           -w*zeta*etapm*zetam, etapm*zetapm, w*zeta*etapm*zetap, 
                                           -v*etazeta*etap*zetam, w*eta*etap*zetapm, v*etazeta*etap*zetap });
            dShapeEta.SetRow(row, new double[] { v*zeta*etam2*zetam, -w*etam2*zetapm, -v*zeta*etam2*zetap, 
                                               etazeta*zetam, -2.0*eta*zetapm, -etazeta*zetap, 
                                               -v*zeta*etap2*zetam, w*etap2*zetapm, v*zeta*etap2*zetap });
            dShapeZeta.SetRow(row, new double[] { v*eta*etam*zetam2, etazeta*etam, -v*eta*etam*zetap2, 
                                                -w*etapm*zetam2, -2.0*zeta*etapm, w*etapm*zetap2, 
                                                -v*eta*etap*zetam2, -etazeta*etap, v*eta*etap*zetap2 });
        }
    }
}
