using System;
using System.Collections.Generic;
using System.Text;
using dnAnalytics.LinearAlgebra;

namespace Canguro.Analysis.Sections
{
    public class FemCrossSection
    {
        Meshing.Mesh mesh;

        #region Cross section properties
        double area, qy, qz, iy, iz, iyz, yC, zC;
        double iyC, izC, iyzC;
        double j, qw, iw, iyw, izw, gamma, ySC, zSC;
        double eRef, nuRef, ry, rz, ipolar, phi, phideg, ipy, ipz, ySecMod, zSecMod;
        double yExtend, zExtend, yShear, zShear, yShearC, zShearC, alphay, alphaz, alphayz;
        string fileNamePrefix = System.IO.Path.GetTempFileName();

        int numElements, numNodes;
        int[][] connect;
        int[] elementConnect;
        double wm;

        const double Tolerance = 1.0e-12;

        //SparseMatrix stiffness;
        Solver.SkylineSolver stiffness;
        //DenseVector torsionLoad, warpingFunction;
        Solver.DenseMatrix torsionLoad, warpingFunction;
        DenseVector shearLoadY, shearLoadZ, shearFunctionY, shearFunctionZ;

        DenseVector y, z, sectionMesh_E, sectionMesh_Er, sectionMesh_nu;
        DenseVector yCNodal = new DenseVector(9);
        DenseVector zCNodal = new DenseVector(9);
        DenseVector yGaussCoords = new DenseVector(9);
        DenseVector zGaussCoords = new DenseVector(9);
        DenseVector yNodal = new DenseVector(9);
        DenseVector zNodal = new DenseVector(9);
        #endregion

        public override string ToString()
        {
            return string.Format("Area: {0}, Qy: {1}, Qz: {2}\nIy: {3}, Iz: {4}, Iyz: {5}\nyC: {6}, zC: {7}\nIyC: {8}, IzC: {9}, IyzC: {10}\nzSecMod: {11}, ySecMod: {12}, Ipolar: {13}\nRy: {14}, Rz: {15}, yExtend: {16}, zExtend: {17}\nIpy: {18}, Ipz: {19}, phi: {20}, phideg: {21}\nJ: {22}\n\nProfile average:{23}",
                area, qy, qz, iy, iz, iyz, yC, zC, iyC, izC, iyzC, zSecMod, ySecMod, ipolar, ry, rz, yExtend, zExtend, ipy, ipz, phi, phideg, j, ProfileAverage);
        }

        public FemCrossSection(Meshing.Mesh mesh)
        {
            this.mesh = mesh;
        }

        public void GetSectionProperties()
        {
            InitFem iFem = new InitFem();
            beginFem();
            areaMoments(iFem);
            assembleGlobal(iFem);
            torsionBvp(iFem);
        }

        private void torsionBvp(InitFem iFem)
        {
            //printStiffness(stiffness);
            //printDense(torsionLoad, "1");

            bool solved = stiffness.solve();
            stiffness.Calc(warpingFunction, torsionLoad);
            printDense(warpingFunction, "2");
            double tmp = warpingFunction.DotProductRow(0, torsionLoad, 0);
            j = iyC + izC - tmp;
        }

        #region Print Matrices for debugging
        private void printDense(Solver.DenseMatrix dense, string suffix)
        {
            string str = dense.ToString();

            System.IO.StreamWriter sw = System.IO.File.CreateText(fileNamePrefix + suffix + ".txt");
            sw.Write(str);
            sw.Close();
        }

        private void printDense(DenseVector dense, string suffix)
        {
            string str = dense.ToString();

            System.IO.StreamWriter sw = System.IO.File.CreateText(fileNamePrefix + suffix + ".txt");
            sw.Write(str);
            sw.Close();
        }

        private void printStiffness(Solver.SkylineSolver solver)
        {
            string str = solver.ToString();

            System.IO.StreamWriter sw = System.IO.File.CreateText(fileNamePrefix + "stiffness.txt");
            sw.Write(str);
            sw.Close();
        }

        private void printStiffness(SparseMatrix mat)
        {
            string str = mat.ToString();

            System.IO.StreamWriter sw = System.IO.File.CreateText(fileNamePrefix + "stiffness.txt");
            sw.Write(str);
            sw.Close();
        }
        #endregion

        private void renumberNodes()
        {
            // Build adjacency list
            LinkedList<int>[] adjList = new LinkedList<int>[numNodes];
            for (int i = 0; i < numNodes; i++)
                adjList[i] = new LinkedList<int>();

            foreach (int[] adj in connect)
            {
                for (int i = 0; i < 8; i++)
                {
                    int j1 = adj[i] - 1;
                    for (int j = i + 1; j < 9; j++)
                    {
                        int j2 = adj[j] - 1;
                        adjList[j1].AddLast(j2);
                        adjList[j2].AddLast(j1);
                    }
                }
            }

            // Perform Reverse Cuthill-McKee
            int[] newIDs = Solver.Rcm.DoRcm(adjList);

            if (newIDs == null)
                throw new InvalidOperationException("The mesh is apparently disconnected");
            
            // Update y, z vectors (coordinates of joints)
            DenseVector yTmp = new DenseVector(y);
            DenseVector zTmp = new DenseVector(z);
            for (int i = 0; i < y.Count; i++)
            {
                y[newIDs[i]] = yTmp[i];
                z[newIDs[i]] = zTmp[i];
            }

            // Update connect (connectivity of elements)
            int[][] connectTmp = new int[connect.Length][];
            
            for (int i = 0; i < connect.Length; i++)
                connectTmp[i] = new int[] { newIDs[connect[i][0] - 1] + 1, newIDs[connect[i][1] - 1] + 1,
                                            newIDs[connect[i][2] - 1] + 1, newIDs[connect[i][3] - 1] + 1,
                                            newIDs[connect[i][4] - 1] + 1, newIDs[connect[i][5] - 1] + 1,
                                            newIDs[connect[i][6] - 1] + 1, newIDs[connect[i][7] - 1] + 1,
                                            newIDs[connect[i][8] - 1] + 1};

            for (int i = 0; i < connect.Length; i++)
                connect[i] = connectTmp[i];
        }

        private int totalProfile = 0;
        public float ProfileAverage
        {
            get { return totalProfile / (float)stiffness.NEquations; }
        }
        
        private void assembleGlobal(InitFem iFem)
        {
            DenseMatrix elementStiffness;
            DenseVector elementLoad;
            DenseVector elementShearY;
            DenseVector elementShearZ;
            
            // Create profile
            int[] profile = new int[numNodes];
            totalProfile = 0;

            foreach (int[] adj in connect)
            {
                int idMin = int.MaxValue;
                foreach (int id in adj)
                    idMin = (id < idMin) ? id : idMin;

                foreach (int id in adj)
                    if (profile[id - 1] < id - idMin)
                        profile[id - 1] = id - idMin;

                foreach (int size in profile)
                    totalProfile += size;
            }

            //stiffness = new SparseMatrix(numNodes, numNodes);
            stiffness = new Solver.SkylineSolver(profile);

            //torsionLoad = new DenseVector(numNodes, 0.0);
            //warpingFunction = new DenseVector(numNodes, 0.0);
            torsionLoad = new Solver.DenseMatrix(1, numNodes);
            warpingFunction = new Solver.DenseMatrix(1, numNodes);
            
            shearLoadY = new DenseVector(numNodes, 0.0);
            shearLoadZ = new DenseVector(numNodes, 0.0);
            shearFunctionY = new DenseVector(numNodes, 0.0);
            shearFunctionZ = new DenseVector(numNodes, 0.0);

            for (int nelem = 0; nelem < numElements; nelem++)
            {
                elementConnect = connect[nelem];
                wm = sectionMesh_Er[nelem];
                
                DenseVector mCy = new DenseVector(9);                
                getNodalPos(y, elementConnect).Subtract(yC, mCy);
                DenseVector mCz = new DenseVector(9);
                getNodalPos(z, elementConnect).Subtract(zC, mCz);

                elementStiffnessLoad(mCy, mCz, out elementStiffness, out elementLoad, out elementShearY, out elementShearZ, sectionMesh_nu[nelem], iFem);

                elementStiffness.Multiply(wm);
                elementLoad.Multiply(wm);
                elementShearY.Multiply(wm);
                elementShearZ.Multiply(wm);
                
                int m, n;
                for (int j = 0; j < 9; j++)
                {
                    n = elementConnect[j] - 1;
                    torsionLoad[0,n] += elementLoad[j];
                    shearLoadY[n] += elementShearY[j];
                    shearLoadZ[n] += elementShearZ[j];

                    for (int i = 0; i < 9; i++)
                    {
                        m = elementConnect[i] - 1;
                        if (m <= n)
                            stiffness[m, n] += elementStiffness[i, j];
                    }
                }
            }

            // Esta línea no sé qué pex (pero sin ésta la matriz es singular)
            stiffness[0, 0] = 1.0e20;
        }

        private void elementStiffnessLoad(DenseVector y, DenseVector z, out DenseMatrix elementStiffness, out DenseVector elementLoad, out DenseVector shearY, out DenseVector shearZ, double nu, InitFem iFem)
        {
            DenseMatrix B;
            DenseMatrix J;
            DenseVector S;
            DenseVector row;
            DenseVector col;
            DenseVector elcol;
            DenseVector veck;
            DenseVector vech;

            double detgw, yP, zP, pe, qe, xnu, hnu, VyFactor, VzFactor;

            elementStiffness = new DenseMatrix(9, 9, 0.0);
            elementLoad = new DenseVector(9, 0.0);
            shearY = new DenseVector(9, 0.0);
            shearZ = new DenseVector(9, 0.0);
            //hnu = nu / 2.0;
            //xnu = 2.0 * (1.0 + nu);

            for (int k = 0; k < 9; k++)
            {
                J = FemUtil.JacobianMatrix(k, y, z, iFem);
                detgw = FemUtil.Determinant(J) * iFem.GaussWeight[k];
                if (detgw <= 0.0) throw new InvalidOperationException("Negative Jacobian Determinant in elementStiffnessLoad");
                B = (DenseMatrix)FemUtil.ElementBMatrix(k, J, iFem).Transpose(); // Debe ser de tamano 9 x 2
                
                S = (DenseVector)iFem.ShapeFunction.GetRow(k);
                yP = S.DotProduct(y);
                zP = S.DotProduct(z);
                elcol = new DenseVector(new double[] { zP, -yP });
                //pe = yP * yP - zP * zP;
                //qe = 2.0 * yP * zP;
                //veck = new DenseVector(new double[] { hnu * (iyC * pe - iyzC * qe), hnu * (iyzC * pe + iyC * qe) });
                //vech = new DenseVector(new double[] { hnu * (izC * qe - iyzC * pe), hnu * (-iyzC * qe - izC * pe) });
                //VyFactor = xnu * (iyC * yP - iyzC * zP);
                //VzFactor = xnu * (izC * zP - iyzC * yP);

                for (int nrow = 0; nrow < 9; nrow++)
                {
                    row = (DenseVector)B.GetRow(nrow);   // Debe ser de tamano 2
                    for (int ncol = 0; ncol < 9; ncol++)
                    {
                        col = (DenseVector)B.GetRow(ncol);   // Debe ser de tamano 2
                        elementStiffness[nrow, ncol] += detgw * row.DotProduct(col);
                    }
                    elementLoad[nrow] += detgw * row.DotProduct(elcol);
                    //shearY[nrow] += detgw * (row.DotProduct(veck) + VyFactor * S[nrow]);
                    //shearZ[nrow] += detgw * (row.DotProduct(vech) + VzFactor * S[nrow]);
                }
            }

            for (int ncol = 0; ncol < 9; ncol++)
                for (int nrow = 0; nrow < ncol - 1; nrow++)
                    elementStiffness[nrow, ncol] = elementStiffness[ncol, nrow];
        }

        private void areaMoments(InitFem iFem)
        {
            DenseVector areaValues;

            for (int k = 0; k < numElements; k++)
            {
                yNodal = getNodalPos(y, connect[k]);
                zNodal = getNodalPos(z, connect[k]);
                wm = sectionMesh_Er[k];
                areaValues = new DenseVector(9, wm);
                area += FemUtil.ElementIntegral(areaValues, yNodal, zNodal, iFem);
                yGaussCoords = FemUtil.ElementPoints(yNodal, iFem);
                zGaussCoords = FemUtil.ElementPoints(zNodal, iFem);
                qy += FemUtil.ElementIntegral(zGaussCoords, yNodal, zNodal, iFem) * wm;
                qz += FemUtil.ElementIntegral(yGaussCoords, yNodal, zNodal, iFem) * wm;
                iy += FemUtil.ElementIntegral(zGaussCoords.PointwiseMultiply(zGaussCoords), yNodal, zNodal, iFem) * wm;
                iz += FemUtil.ElementIntegral(yGaussCoords.PointwiseMultiply(yGaussCoords), yNodal, zNodal, iFem) * wm;
                iyz += FemUtil.ElementIntegral(yGaussCoords.PointwiseMultiply(zGaussCoords), yNodal, zNodal, iFem) * wm;
            }

            qy = chop(qy);
            qz = chop(qz);
            iy = chop(iy);
            iz = chop(iz);
            iyz = chop(iyz);
            yC = qz / area;
            zC = qy / area;
            iyC = iy - zC * zC * area;
            izC = iz - yC * yC * area;
            iyzC = iyz - yC * zC * area;

            DenseVector mC = new DenseVector(y.Count);
            y.Subtract(yC, mC);
            zSecMod = izC / mC.AbsoluteMaximum();
            z.Subtract(zC, mC);
            ySecMod = iyC / mC.AbsoluteMaximum();

            ipolar = iyC + izC;

            ry = Math.Sqrt(iyC / area);
            rz = Math.Sqrt(izC / area);

            yExtend = y.Maximum() - y.Minimum();
            zExtend = z.Maximum() - z.Minimum();

            principalInertia();
        }

        private void principalInertia()
        {
            if (Math.Abs(iyzC) < Tolerance)
            {
                ipy = Math.Max(iyC, izC);
                ipz = Math.Min(iyC, izC);
                if (iyC >= izC)
                    phi = phideg = 0.0;
                else
                {
                    phi = Math.PI / 2.0;
                    phideg = 90.0;
                }
            }
            else
            {
                double delta = Math.Sqrt(0.25 * (iyC - izC) * (iyC - izC) + iyzC * iyzC);
                ipy = 0.5 * (iyC + izC);
                ipz = ipy - delta;
                ipy = ipy + delta;
                phi = Math.Atan2(iyC - ipy, iyzC);
                phideg = phi * 180.0 / Math.PI;
            }
        }

        private double chop(double value)
        {
            if (Math.Abs(value) < Tolerance)
                return 0.0;

            return value;
        }

        private DenseVector getNodalPos(DenseVector pos, int[] connect)
        {
            DenseVector nodal = new DenseVector(9);
            for (int i = 0; i < 9; i++)
                nodal[i] = pos[connect[i] - 1];
            return nodal;
        }

        private void loadMesh()
        {
            #region Mesh 100 Elements
            //numElements = 100;
            //numNodes = 441;
            //y = new DenseVector(new double[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.900011598, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.800023196, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.700034793, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.600046391, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.500057989, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.400069587, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.300081185, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.200092782, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -0.10010438, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, -1.16E-04, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 9.99E-02, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.199860826, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.299849229, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.399837631, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.499826033, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.599814435, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.699802837, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.79979124, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 0.899785642, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            //z = new DenseVector(new double[] { -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 });
            //connect = new int[][] { new int[] { 1, 2, 3, 22, 23, 24, 43, 44, 45},
            //                        new int[] { 3, 4, 5, 24, 25, 26, 45, 46, 47},
            //                        new int[] { 5, 6, 7, 26, 27, 28, 47, 48, 49},
            //                        new int[] { 7, 8, 9, 28, 29, 30, 49, 50, 51},
            //                        new int[] { 9, 10, 11, 30, 31, 32, 51, 52, 53},
            //                        new int[] { 11, 12, 13, 32, 33, 34, 53, 54, 55},
            //                        new int[] { 13, 14, 15, 34, 35, 36, 55, 56, 57},
            //                        new int[] { 15, 16, 17, 36, 37, 38, 57, 58, 59},
            //                        new int[] { 17, 18, 19, 38, 39, 40, 59, 60, 61},
            //                        new int[] { 19, 20, 21, 40, 41, 42, 61, 62, 63},
            //                        new int[] { 43, 44, 45, 64, 65, 66, 85, 86, 87},
            //                        new int[] { 45, 46, 47, 66, 67, 68, 87, 88, 89},
            //                        new int[] { 47, 48, 49, 68, 69, 70, 89, 90, 91},
            //                        new int[] { 49, 50, 51, 70, 71, 72, 91, 92, 93},
            //                        new int[] { 51, 52, 53, 72, 73, 74, 93, 94, 95},
            //                        new int[] { 53, 54, 55, 74, 75, 76, 95, 96, 97},
            //                        new int[] { 55, 56, 57, 76, 77, 78, 97, 98, 99},
            //                        new int[] { 57, 58, 59, 78, 79, 80, 99, 100, 101},
            //                        new int[] { 59, 60, 61, 80, 81, 82, 101, 102, 103},
            //                        new int[] { 61, 62, 63, 82, 83, 84, 103, 104, 105},
            //                        new int[] { 85, 86, 87, 106, 107, 108, 127, 128, 129},
            //                        new int[] { 87, 88, 89, 108, 109, 110, 129, 130, 131},
            //                        new int[] { 89, 90, 91, 110, 111, 112, 131, 132, 133},
            //                        new int[] { 91, 92, 93, 112, 113, 114, 133, 134, 135},
            //                        new int[] { 93, 94, 95, 114, 115, 116, 135, 136, 137},
            //                        new int[] { 95, 96, 97, 116, 117, 118, 137, 138, 139},
            //                        new int[] { 97, 98, 99, 118, 119, 120, 139, 140, 141},
            //                        new int[] { 99, 100, 101, 120, 121, 122, 141, 142, 143},
            //                        new int[] { 101, 102, 103, 122, 123, 124, 143, 144, 145},
            //                        new int[] { 103, 104, 105, 124, 125, 126, 145, 146, 147},
            //                        new int[] { 127, 128, 129, 148, 149, 150, 169, 170, 171},
            //                        new int[] { 129, 130, 131, 150, 151, 152, 171, 172, 173},
            //                        new int[] { 131, 132, 133, 152, 153, 154, 173, 174, 175},
            //                        new int[] { 133, 134, 135, 154, 155, 156, 175, 176, 177},
            //                        new int[] { 135, 136, 137, 156, 157, 158, 177, 178, 179},
            //                        new int[] { 137, 138, 139, 158, 159, 160, 179, 180, 181},
            //                        new int[] { 139, 140, 141, 160, 161, 162, 181, 182, 183},
            //                        new int[] { 141, 142, 143, 162, 163, 164, 183, 184, 185},
            //                        new int[] { 143, 144, 145, 164, 165, 166, 185, 186, 187},
            //                        new int[] { 145, 146, 147, 166, 167, 168, 187, 188, 189},
            //                        new int[] { 169, 170, 171, 190, 191, 192, 211, 212, 213},
            //                        new int[] { 171, 172, 173, 192, 193, 194, 213, 214, 215},
            //                        new int[] { 173, 174, 175, 194, 195, 196, 215, 216, 217},
            //                        new int[] { 175, 176, 177, 196, 197, 198, 217, 218, 219},
            //                        new int[] { 177, 178, 179, 198, 199, 200, 219, 220, 221},
            //                        new int[] { 179, 180, 181, 200, 201, 202, 221, 222, 223},
            //                        new int[] { 181, 182, 183, 202, 203, 204, 223, 224, 225},
            //                        new int[] { 183, 184, 185, 204, 205, 206, 225, 226, 227},
            //                        new int[] { 185, 186, 187, 206, 207, 208, 227, 228, 229},
            //                        new int[] { 187, 188, 189, 208, 209, 210, 229, 230, 231},
            //                        new int[] { 211, 212, 213, 232, 233, 234, 253, 254, 255},
            //                        new int[] { 213, 214, 215, 234, 235, 236, 255, 256, 257},
            //                        new int[] { 215, 216, 217, 236, 237, 238, 257, 258, 259},
            //                        new int[] { 217, 218, 219, 238, 239, 240, 259, 260, 261},
            //                        new int[] { 219, 220, 221, 240, 241, 242, 261, 262, 263},
            //                        new int[] { 221, 222, 223, 242, 243, 244, 263, 264, 265},
            //                        new int[] { 223, 224, 225, 244, 245, 246, 265, 266, 267},
            //                        new int[] { 225, 226, 227, 246, 247, 248, 267, 268, 269},
            //                        new int[] { 227, 228, 229, 248, 249, 250, 269, 270, 271},
            //                        new int[] { 229, 230, 231, 250, 251, 252, 271, 272, 273},
            //                        new int[] { 253, 254, 255, 274, 275, 276, 295, 296, 297},
            //                        new int[] { 255, 256, 257, 276, 277, 278, 297, 298, 299},
            //                        new int[] { 257, 258, 259, 278, 279, 280, 299, 300, 301},
            //                        new int[] { 259, 260, 261, 280, 281, 282, 301, 302, 303},
            //                        new int[] { 261, 262, 263, 282, 283, 284, 303, 304, 305},
            //                        new int[] { 263, 264, 265, 284, 285, 286, 305, 306, 307},
            //                        new int[] { 265, 266, 267, 286, 287, 288, 307, 308, 309},
            //                        new int[] { 267, 268, 269, 288, 289, 290, 309, 310, 311},
            //                        new int[] { 269, 270, 271, 290, 291, 292, 311, 312, 313},
            //                        new int[] { 271, 272, 273, 292, 293, 294, 313, 314, 315},
            //                        new int[] { 295, 296, 297, 316, 317, 318, 337, 338, 339},
            //                        new int[] { 297, 298, 299, 318, 319, 320, 339, 340, 341},
            //                        new int[] { 299, 300, 301, 320, 321, 322, 341, 342, 343},
            //                        new int[] { 301, 302, 303, 322, 323, 324, 343, 344, 345},
            //                        new int[] { 303, 304, 305, 324, 325, 326, 345, 346, 347},
            //                        new int[] { 305, 306, 307, 326, 327, 328, 347, 348, 349},
            //                        new int[] { 307, 308, 309, 328, 329, 330, 349, 350, 351},
            //                        new int[] { 309, 310, 311, 330, 331, 332, 351, 352, 353},
            //                        new int[] { 311, 312, 313, 332, 333, 334, 353, 354, 355},
            //                        new int[] { 313, 314, 315, 334, 335, 336, 355, 356, 357},
            //                        new int[] { 337, 338, 339, 358, 359, 360, 379, 380, 381},
            //                        new int[] { 339, 340, 341, 360, 361, 362, 381, 382, 383},
            //                        new int[] { 341, 342, 343, 362, 363, 364, 383, 384, 385},
            //                        new int[] { 343, 344, 345, 364, 365, 366, 385, 386, 387},
            //                        new int[] { 345, 346, 347, 366, 367, 368, 387, 388, 389},
            //                        new int[] { 347, 348, 349, 368, 369, 370, 389, 390, 391},
            //                        new int[] { 349, 350, 351, 370, 371, 372, 391, 392, 393},
            //                        new int[] { 351, 352, 353, 372, 373, 374, 393, 394, 395},
            //                        new int[] { 353, 354, 355, 374, 375, 376, 395, 396, 397},
            //                        new int[] { 355, 356, 357, 376, 377, 378, 397, 398, 399},
            //                        new int[] { 379, 380, 381, 400, 401, 402, 421, 422, 423},
            //                        new int[] { 381, 382, 383, 402, 403, 404, 423, 424, 425},
            //                        new int[] { 383, 384, 385, 404, 405, 406, 425, 426, 427},
            //                        new int[] { 385, 386, 387, 406, 407, 408, 427, 428, 429},
            //                        new int[] { 387, 388, 389, 408, 409, 410, 429, 430, 431},
            //                        new int[] { 389, 390, 391, 410, 411, 412, 431, 432, 433},
            //                        new int[] { 391, 392, 393, 412, 413, 414, 433, 434, 435},
            //                        new int[] { 393, 394, 395, 414, 415, 416, 435, 436, 437},
            //                        new int[] { 395, 396, 397, 416, 417, 418, 437, 438, 439},
            //                        new int[] { 397, 398, 399, 418, 419, 420, 439, 440, 441} };
            #endregion

            #region Mesh 4 Elements
            //numElements = 4;
            //numNodes = 25;
            //y = new DenseVector(new double[] { -1, -1, -1, -1, -1, -0.5, -0.5, -0.5, -0.5, -0.5, 0, 0, 0, 0, 0, 0.5, 0.5, 0.5, 0.5, 0.5, 1, 1, 1, 1, 1 });
            //z = new DenseVector(new double[] { -1, -0.5, 0, 0.5, 1, -1, -0.5, 0, 0.5, 1, -1, -0.5, 0, 0.5, 1, -1, -0.5, 0, 0.5, 1, -1, -0.5, 0, 0.5, 1 });
            //connect = new int[][] { new int[] { 1, 2, 3, 6, 7, 8, 11, 12, 13},
            //                        new int[] { 3, 4, 5, 8, 9, 10, 13, 14, 15},
            //                        new int[] { 11, 12, 13, 16, 17, 18, 21, 22, 23},
            //                        new int[] { 13, 14, 15, 18, 19, 20, 23, 24, 25} };
            #endregion

            numNodes = mesh.Vertices.Count;
            numElements = mesh.Shapes.Count;
            List<Meshing.Edge> edges = mesh.GetEdgeList(true);
            int numEdges = edges.Count;
            Dictionary<Meshing.Edge, int> edgeVertices = new Dictionary<Meshing.Edge, int>();

            y = new DenseVector(numNodes + numEdges + numElements);
            z = new DenseVector(numNodes + numEdges + numElements);

            // Quad vertices
            for (int i = 0; i < numNodes; i++)
            {
                y[i] = mesh.Vertices[i].X;
                z[i] = mesh.Vertices[i].Y;
            }

            // Edge vertices
            for (int i=0;i<numEdges;i++)
            {
                edgeVertices.Add(edges[i], numNodes + i);
                y[i + numNodes] = (edges[i].V1.X + edges[i].V2.X) / 2.0;
                z[i + numNodes] = (edges[i].V1.Y + edges[i].V2.Y) / 2.0;
            }

            connect = new int[numElements][];
            for (int i = 0; i < numElements; i++)
            {
                connect[i] = new int[9];

                Meshing.Quad q = mesh.Shapes[i] as Meshing.Quad;
                if (q != null)
                {
                    Meshing.Vertex[] vs = q.Vertices;
                    connect[i][0] = vs[0].Id + 1;
                    connect[i][6] = vs[1].Id + 1;
                    connect[i][8] = vs[2].Id + 1;
                    connect[i][2] = vs[3].Id + 1;

                    List<Meshing.Edge> orderedEdges = q.OrderedEdges;

                    connect[i][3] = edgeVertices[orderedEdges[0]] + 1;
                    connect[i][7] = edgeVertices[orderedEdges[1]] + 1;
                    connect[i][5] = edgeVertices[orderedEdges[2]] + 1;
                    connect[i][1] = edgeVertices[orderedEdges[3]] + 1;

                    // Get center vertex
                    int centerId = i + numNodes + numEdges;
                    System.Drawing.PointF pt = q.GetCentroid();
                    y[centerId] = pt.X;
                    z[centerId] = pt.Y;

                    connect[i][4] = centerId + 1;
                }
                else
                    throw new InvalidOperationException("The mesh is not correctly quadrangulated");
            }

            numNodes = numNodes + numEdges + numElements;

            // Concreto 4000Psi (KN, m)
            eRef = 2.486e7;
            nuRef = 0.2;

            // Acero A99Fy50 (KN, m)
            //eRef = 1.999e8;
            //nuRef = 0.3;
        }

        private string printNodes()
        {
            string str = "";
            for (int i = 0; i < y.Count; i++)
                str += string.Format("\npoint {0},{1}", y[i], z[i]);

            return str;
        }

        private void beginFem() // Just assign a mesh here por testing
        {
            loadMesh();
            renumberNodes();

            sectionMesh_E = new DenseVector(numElements, eRef);
            sectionMesh_nu = new DenseVector(numElements, nuRef);

            sectionMesh_Er = new DenseVector(numElements);
            for (int i = 0; i < numElements; i++)
                sectionMesh_Er[i] = sectionMesh_E[i] / eRef; // eRef == ElasticReference

            area = qy = qz = iy = iz = iyz = yC = zC = iyC = izC = iyzC = qw = iw = iyw = izw = 0.0;
        }
    }
}
