using System;
using System.Collections.Generic;
using System.Text;
using dnAnalytics.LinearAlgebra;

namespace Canguro.Analysis.Sections
{
    class FemUtil   // Corresponds to femutil.f90
    {
        public static DenseMatrix JacobianMatrix(int m, Vector y, Vector z, InitFem ifem)
        {
            Vector Se = ifem.ShapeEta.GetRow(m);
            Vector Sz = ifem.ShapeZeta.GetRow(m);
            return new DenseMatrix(new double[,] { { Se.DotProduct(y), Se.DotProduct(z) }, { Sz.DotProduct(y), Sz.DotProduct(z) } });
        }

        public static double Determinant(DenseMatrix jacobian)
        {
            return jacobian[0, 0] * jacobian[1, 1] - jacobian[0, 1] * jacobian[1, 0];
        }

        public static DenseMatrix Inverse(DenseMatrix jacobian)
        {
            double determinant = Determinant(jacobian);
            return new DenseMatrix(new double[,] { { jacobian[1, 1]/determinant, -jacobian[0, 1]/determinant }, 
                                                   { -jacobian[1, 0]/determinant, jacobian[0, 0]/determinant } });
        }

        public static DenseVector ElementPoints(Vector nodalCoord, InitFem ifem)
        {
            DenseVector elementPoints = new DenseVector(9);

            for (int m = 0; m < 9; m++)
                elementPoints[m] = nodalCoord.DotProduct(ifem.ShapeFunction.GetRow(m));

            return elementPoints;
        }

        public static double ElementIntegral(Vector values, Vector y, Vector z, InitFem ifem)
        {
            DenseVector tmp = new DenseVector(9);

            for (int m = 0; m < 9; m++)
                tmp[m] = Determinant(JacobianMatrix(m, y, z, ifem)) * values[m];

            return tmp.DotProduct(ifem.GaussWeight);
        }

        public static DenseMatrix ElementBMatrix(int m, DenseMatrix jacobian, InitFem ifem)
        {
            DenseVector elementPoints = new DenseVector(9);
            DenseMatrix ts = new DenseMatrix(2, 9);
            ts.SetRow(0, ifem.ShapeEta.GetRow(m));
            ts.SetRow(1, ifem.ShapeZeta.GetRow(m));

            return Inverse(jacobian) * ts;
        }
    }
}
