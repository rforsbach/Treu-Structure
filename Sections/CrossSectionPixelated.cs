using System;
using System.Collections.Generic;
using System.Text;
using dnAnalytics.LinearAlgebra;

namespace Canguro.Analysis.Sections
{
    public class CrossSectionPixelated
    {
        int a;
        bool[,] pixels;
        double pixelSize;
        double c2, c3, j, i2, i3, as2, as3, s2, s3, z2, z3, r2, r3;

        #region Properties
        public float Centroid3
        {
            get { return (float)(c3 * pixelSize); }
        }

        public float Centroid2
        {
            get { return (float)(c2 * pixelSize); }
        }

        public float R3
        {
            get { return (float)(r3 * pixelSize); }
        }

        public float R2
        {
            get { return (float)(r2 * pixelSize); }
        }

        public float Z3
        {
            get { return (float)(z3 * Math.Pow(pixelSize, 3)); }
        }

        public float Z2
        {
            get { return (float)(z2 * Math.Pow(pixelSize, 3)); }
        }

        public float S3
        {
            get { return (float)(s3 * Math.Pow(pixelSize, 3)); }
        }

        public float S2
        {
            get { return (float)(s2 * Math.Pow(pixelSize, 3)); }
        }

        public float As3
        {
            get { return (float)(as3 * pixelSize * pixelSize); }
        }

        public float I3
        {
            get { return (float)(i3 * Math.Pow(pixelSize, 4)); }
        }

        public float As2
        {
            get { return (float)(as2 * pixelSize * pixelSize); }
        }

        public float I2
        {
            get { return (float)(i2 * Math.Pow(pixelSize, 4)); }
        }

        public float J
        {
            get { return (float)(j); }
        }

        public float A
        {
            get { return (float)(a * (pixelSize * pixelSize)); }
        }
        #endregion

        public override string ToString()
        {
            return string.Format("A:\t{0}\nJ:\t{1}\nI2:\t{2}\nI3:\t{3}\nAs2:\t{4}\nAs3:\t{5}\nS2:\t{6}\nS3:\t{7}\nZ2:\t{8}\nZ3:\t{9}\nR2:\t{10}\nR3:\t{11}\nC2:\t{12}\nC3:\t{13}",
                A, J, I2, I3, As2, As3, S2, S3, Z2, Z3, R2, R3, Centroid2, Centroid3);
        }

        public CrossSectionPixelated(bool[,] pixelatedSection, double pixelSize)
        {
            int i, j;
            int boundaryX1, boundaryX2, boundaryY1, boundaryY2;
            this.pixelSize = pixelSize;
            pixels = pixelatedSection;
            int px = pixels.GetLength(0);
            int py = pixels.GetLength(1);
            int[] colAreas = new int[px];
            int[] rowAreas = new int[py];
            int[] colBoundaryMin = new int[px];
            int[] rowBoundaryMin = new int[py];
            int[] colBoundaryMax = new int[px];
            int[] rowBoundaryMax = new int[py];
            for (i = 0; i < px; i++)
                colBoundaryMin[i] = px;
            for (j = 0; j < py; j++)
                rowBoundaryMin[j] = py;
            
            // Calculate section properties:
            ///////////////////////////////////

            boundaryX1 = boundaryY1 = Math.Max(px, py);
            boundaryX2 = boundaryY2 = 0;
            int ic2 = 0, ic3 = 0;
            c2 = c3 = i2 = i3 = a = 0;

            #region Area and centroid
            for (i=0; i<px; i++)
                for (j = 0; j < py; j++)
                {
                    if (pixels[i, j])
                    {
                        // Boundary
                        boundaryX1 = Math.Min(boundaryX1, i);
                        boundaryX2 = Math.Max(boundaryX2, i);
                        boundaryY1 = Math.Min(boundaryY1, j);
                        boundaryY2 = Math.Max(boundaryY2, j);

                        // Boundaryies per row/col
                        colBoundaryMin[i] = Math.Min(colBoundaryMin[i], j);
                        colBoundaryMax[i] = Math.Max(colBoundaryMax[i], j);
                        rowBoundaryMin[j] = Math.Min(rowBoundaryMin[j], i);
                        rowBoundaryMax[j] = Math.Max(rowBoundaryMax[j], i);

                        // Cols and Rows count
                        colAreas[i]++; rowAreas[j]++;

                        // Cross Sectional Area A
                        ++a;

                        // Centroid C
                        ic3 += i;
                        ic2 += j;                        
                    }
                }

            // Centroid C
            c3 = ic3 / (double)a;
            c2 = ic2 / (double)a;

            #endregion

            #region Moments of Inertia
            for (i = 0; i < px; i++)
                for (j = 0; j < py; j++)
                {
                    if (pixels[i, j])
                    {
                        // Moments of Inertia I2, I3
                        i3 += ((j - c2) * (j - c2));
                        i2 += ((i - c3) * (i - c3));
                    }
                }
            #endregion

            #region Radii of Gyration
            // Radii of Gyration R2, R3
            r3 = Math.Sqrt(i3 / a);
            r2 = Math.Sqrt(i2 / a);
            #endregion

            #region Section Moduli
            // Section Moduli S2, S3
            s3 = i3 / Math.Max(c2 - boundaryY1, boundaryY2 - c2);
            s2 = i2 / Math.Max(c3 - boundaryX1, boundaryX2 - c3);
            #endregion

            #region Plastic Moduli
            // Plastic Moduli Z2, Z3
            int sumArea = 0;
            double halfArea = (double)a / 2.0;
            double zc2, zc3;

            for (i = 0; i < px && sumArea < halfArea; ++i)
                sumArea += colAreas[i];
            --i;
            zc3 = i - (sumArea - halfArea) / colAreas[i];

            sumArea = 0;
            for (j = 0; j < py && sumArea < halfArea; ++j)
                sumArea += rowAreas[j];
            --j;
            zc2 = j - (sumArea - halfArea) / rowAreas[j];

            double zVertical1 =0, zVertical2=0, zHorizontal1=0, zHorizontal2=0;
            for (j = 0; j <= zc2; j++)
                zVertical1 += rowAreas[j] * j;
            zVertical1 += rowAreas[j] * (zc2 - Math.Floor(zc2)) * j;

            zVertical2 = rowAreas[j] * (1 - zc2 + Math.Floor(zc2)) * j;
            for (j = (int)zc2 + 2; j < py; j++)
                zVertical2 += rowAreas[j] * j;

            for (i = 0; i <= zc3; i++)
                zHorizontal1 += colAreas[i] * i;
            zHorizontal1 += colAreas[i] * (zc3 - Math.Floor(zc3)) * i;

            zHorizontal2 = colAreas[i] * (1 - zc3 + Math.Floor(zc3)) * i;
            for (i = (int)zc3 + 2; i < px; i++)
                zHorizontal2 += colAreas[i] * i;

            zVertical1 /= halfArea;
            zVertical2 /= halfArea;
            zHorizontal1 /= halfArea;
            zHorizontal2 /= halfArea;

            z2 = a * ((zc3 - zHorizontal1) + (zHorizontal2 - zc3)) / 2f;
            z3 = a * ((zc2 - zVertical1) + (zVertical2 - zc2)) / 2f;
            #endregion

            #region Shear Area
            // Shear Areas As2, As3
            double[] q3 = new double[px], q2 = new double[py];
            double asTmp2=0, asTmp3=0, asTmp;
            as2 = as3 = 0;

            // Get first moment of area
            //for (j = py - 1; j >= 0; j--)
            //{
            //    q2[j] = (j != (py - 1)) ? q2[j + 1] : 0;
            //    q2[j] += (c2 - j) * rowAreas[j];
            //}

            //for (i = px - 1; i >= 0; i--)
            //{
            //    q3[i] = (i != (px - 1)) ? q3[i + 1] : 0;
            //    q3[i] += (c3 - i) * colAreas[i];
            //}


            //for (i = 0; i < px; i++)
            //    for (j = 0; j < py; j++)
            //    {
            //        if (pixels[i, j])
            //        {
            //            asTmp = q2[j] / (double)(rowBoundaryMax[j] - rowBoundaryMin[j] + 1);
            //            asTmp3 += asTmp * asTmp;

            //            asTmp = q3[i] / (double)(colBoundaryMax[i] - colBoundaryMin[i] + 1);
            //            asTmp2 += asTmp * asTmp;
            //        }
            //    }

            //as2 = i2 * i2 / asTmp2;
            //as3 = i3 * i3 / asTmp3;

            q2[0] = c2 * rowAreas[0];
            for (j = 1; j < py; j++)
                q2[j] = q2[j - 1] + (c2 - j) * rowAreas[j];

            q3[0] = c3 * colAreas[0];
            for (j = 1; j < px; j++)
                q3[j] = q3[j - 1] + (c3 - j) * colAreas[j];


            asTmp2 = 0;
            for (j = 0; j < py; j++)
                if (rowAreas[j] > 0)
                    asTmp2 += q2[j] * q2[j] / (double)rowAreas[j];

            asTmp3 = 0;
            for (j = 0; j < px; j++)
                if (colAreas[j] > 0)
                    asTmp3 += q3[j] * q3[j] / (double)colAreas[j];

            as2 = i3 * i3 / asTmp2;
            as3 = i2 * i2 / asTmp3;
            #endregion
        }

        public float getJFromRect(double h, double b)
        {
            DenseVector Pe;
            DenseMatrix Ke;

            DenseVector jy = new DenseVector(new double[] { -b/2, -b/2, -b/2, 0, 0, 0, b/2, b/2, b/2 });
            DenseVector jz = new DenseVector(new double[] { -h/2, 0, h/2, -h/2, 0, h/2, -h/2, 0, h/2 });

            int[] C0 = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            torsionElementKeAndPe(jy, jz, out Ke, out Pe);

            int nodes = 9;
            DenseVector P = new DenseVector(nodes);
            DenseMatrix K = new DenseMatrix(nodes, nodes);

            torsionAssembleKandP(K, Ke, P, Pe, C0);

            dnAnalytics.LinearAlgebra.DenseMatrix Kdn = new dnAnalytics.LinearAlgebra.DenseMatrix(K);
            dnAnalytics.LinearAlgebra.DenseVector Pdn = new dnAnalytics.LinearAlgebra.DenseVector(P);
            dnAnalytics.LinearAlgebra.Solvers.Direct.LUSolver solver = new dnAnalytics.LinearAlgebra.Solvers.Direct.LUSolver();
            dnAnalytics.LinearAlgebra.DenseVector w = (dnAnalytics.LinearAlgebra.DenseVector)solver.Solve(Kdn, Pdn);

            dnAnalytics.LinearAlgebra.DenseVector wk = new dnAnalytics.LinearAlgebra.DenseVector(9);
            wk = (dnAnalytics.LinearAlgebra.DenseVector)Kdn.LeftMultiply(w);

            double i22 = h * b * b * b / 12.0f;
            double i33 = b * h * h * h / 12.0f;
            double wKw = wk.DotProduct(w);
            this.j = i22 + i33 - wKw;

            return (float)this.j;
        }
        
        #region Torsional Constant Functions
        private double torsionShapeFunc(int func, float eta, float zeta)
        {
            switch (func)
            {
                case 1:
                    return 0.25 * eta * zeta * (1 - eta) * (1 - zeta);
                case 2:
                    return -0.5 * eta * (1 - eta) * (1 - zeta * zeta);
                case 3:
                    return -0.25 * eta * zeta * (1 - eta) * (1 + zeta);
                case 4:
                    return -0.5 * zeta * (1 - eta * eta) * (1 - zeta);
                case 5:
                    return (1 - eta * eta) * (1 - zeta * zeta);
                case 6:
                    return 0.5 * zeta * (1 - eta * eta) * (1 + zeta);
                case 7:
                    return -0.25 * eta * zeta * (1 + eta) * (1 - zeta);
                case 8:
                    return 0.5 * eta * (1 + eta) * (1 - zeta * zeta);
                case 9:
                    return 0.25* eta * zeta * (1 + eta) * (1 + zeta);
                default:
                    throw new ArgumentOutOfRangeException("Cannot find shape function #" + func.ToString());
            }
        }

        private double torsionShapeFunc_d_eta(int func, float eta, float zeta)
        {
            switch (func)
            {
                case 1:
                    return 0.25 * zeta * (1 - 2 * eta) * (1 - zeta);
                case 2:
                    return -0.5 * (1 - 2 * eta) * (1 - zeta * zeta);
                case 3:
                    return -zeta * 0.25 * (1 - 2 * eta) * (1 + zeta);
                case 4:
                    return eta * zeta * (1 - zeta);
                case 5:
                    return -2 * eta * (1 - zeta * zeta);
                case 6:
                    return -eta * zeta * (1 + zeta);
                case 7:
                    return -zeta * 0.25 * (1 + 2 * eta) * (1 - zeta);
                case 8:
                    return 0.5 * (1 + 2 * eta) * (1 - zeta * zeta);
                case 9:
                    return zeta * 0.25 * (1 + 2 * eta) * (1 + zeta);
                default:
                    throw new ArgumentOutOfRangeException("Cannot find shape function #" + func.ToString());
            }
        }

        private double torsionShapeFunc_d_zeta(int func, float eta, float zeta)
        {
            switch (func)
            {
                case 1:
                    return 0.25 * eta * (1 - eta) * (1 - 2 * zeta);
                case 2:
                    return eta * zeta * (1 - eta);
                case 3:
                    return -eta * 0.25 * (1 - eta) * (1 + 2 * zeta);
                case 4:
                    return -0.5 * (1 - eta * eta) * (1 - 2 * zeta);
                case 5:
                    return -2 * zeta * (1 - eta * eta);
                case 6:
                    return 0.5 * (1 - eta * eta) * (1 + 2 * zeta);
                case 7:
                    return -eta * 0.25 * (1 + eta) * (1 - 2 * zeta);
                case 8:
                    return -eta * zeta * (1 + eta);
                case 9:
                    return 0.25 * eta * (1 + eta) * (1 + 2 * zeta);
                default:
                    throw new ArgumentOutOfRangeException("Cannot find shape function #" + func.ToString());
            }
        }

        /// <summary>
        /// Returns the Jacobian of the isoparametric element
        /// </summary>
        /// <param name="eta">y coordinate where this Jacobian is being evaluated in the reference domain</param>
        /// <param name="zeta">z coordinate where this Jacobian is being evaluated in the reference domain</param>
        /// <param name="y">y coordinates of the nodes of the element in the real domain</param>
        /// <param name="z">z coordinates of the nodes of the element in the real domain</param>
        /// <returns>A 2 x 2 Jacobian matrix</returns>
        private DenseMatrix torsionJacobian_atPoint(float eta, float zeta, DenseVector y, DenseVector z)
        {
            int i;
            DenseMatrix jac = new DenseMatrix(2, 2);

            jac[0, 0] = 0;
            for (i = 0; i < 9; i++)
                jac[0, 0] += torsionShapeFunc_d_eta(i + 1, eta, zeta) * y[i];

            jac[0, 1] = 0;
            for (i = 0; i < 9; i++)
                jac[0, 1] += torsionShapeFunc_d_eta(i + 1, eta, zeta) * z[i];

            jac[1, 0] = 0;
            for (i = 0; i < 9; i++)
                jac[1, 0] += torsionShapeFunc_d_zeta(i + 1, eta, zeta) * y[i];

            jac[1, 1] = 0;
            for (i = 0; i < 9; i++)
                jac[1, 1] += torsionShapeFunc_d_zeta(i + 1, eta, zeta) * z[i];

            return jac;
        }

        private DenseMatrix torsionB_atPoint(DenseMatrix jacobian, float eta, float zeta)
        {
            DenseMatrix B = new DenseMatrix(2, 9);
            DenseMatrix Ji = new DenseMatrix(2, 2);
            double invDet = 1.0 / jacobian.Determinant();
            Ji[0, 0] = invDet * jacobian[1, 1];
            Ji[0, 1] = invDet * (-jacobian[0, 1]);
            Ji[1, 0] = invDet * (-jacobian[1, 0]);
            Ji[1, 1] = invDet * jacobian[0, 0];

            for (int i = 0; i < 9; i++)
            {
                B[0, i] = Ji[0, 0] * torsionShapeFunc_d_eta(i + 1, eta, zeta) + Ji[0, 1] * torsionShapeFunc_d_zeta(i + 1, eta, zeta);
                B[1, i] = Ji[1, 0] * torsionShapeFunc_d_eta(i + 1, eta, zeta) + Ji[1, 1] * torsionShapeFunc_d_zeta(i + 1, eta, zeta);
            }

            return B;
        }

        private void torsionKandP_atPoint(float eta, float zeta, DenseVector y, DenseVector z, out DenseMatrix K, out DenseVector P)
        {
            DenseMatrix jacobian = torsionJacobian_atPoint(eta, zeta, y, z);
            double det = jacobian.Determinant();

            if (det <= 0.0)
                throw new Exception("Error calculating the torsion constant");

            DenseMatrix B = torsionB_atPoint(jacobian, eta, zeta);
            K = (DenseMatrix)((B.Transpose() * B) * det);            
            DenseVector N = new DenseVector(2);
            
            for (int i = 0; i < 9; i++)
            {
                N[0] += torsionShapeFunc(i + 1, eta, zeta) * z[i];
                N[1] -= torsionShapeFunc(i + 1, eta, zeta) * y[i];
            }

            P = (DenseVector)(B.Transpose() * N * det);
        }

        private void torsionAssembleKandP(DenseMatrix K, DenseMatrix Ke, DenseVector P, DenseVector Pe, int[] C)
        {
            int n = C.Length;

            // Assemble K
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    K[C[i], C[j]] += Ke[i, j];
                }

            // Assemble P
            for (int i = 0; i < n; i++)
            {
                P[C[i]] += Pe[i];
            }
        }
                
        private void torsionElementKeAndPe(DenseVector y, DenseVector z, out DenseMatrix Ke, out DenseVector Pe)
        {
            DenseVector P;
            Pe = new DenseVector(9);
            DenseMatrix K;
            Ke = new DenseMatrix(9, 9);

            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    torsionKandP_atPoint(i / 100.0f - 1.0f + 0.005f, j / 100.0f - 1.0f + 0.005f, y, z, out K, out P);
                    Ke += (K * 0.01);
                    Pe += (P * 0.01);
                }
                if (i % 100 == 0) Console.Write(".");
            }

            double det = Ke.Determinant();
            if (det > -0.00001 && det < 0.00001)
                throw new Exception("Ill Conditioned Matrix");
        }
        #endregion
    }
}
