using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;

namespace Canguro.View.Gadgets
{
    public class PointGadgetService : GadgetService
    {
        #region New Fields
        
        GadgetManager gadgetManager;
        Dictionary<short, GadgetLocator> restraints;
        
        #endregion

        #region Class properties and fields
        private CustomVertex.PositionColored[] symVertices;
        private int[][][] symIndices;
        private int[][][] symbolIndices;

        public CustomVertex.PositionColored[] SymVertices
        {
            get { return symVertices; }
            set { symVertices = value; }
        }

        private int numOptions = 3;         // three planes or three axes
        private int factorySymbols = 5;
        private int possibleSymbols = 8;    // Seven: Five symbols on planes, and two more on any axis

        private float triangleSideSize = 0.5f;
        private float baseLineSize = 0.6f;
        private float circleRadius = 0.08f;
        private int groundLines = 3;

        private int triangleVertices = 4;
        private int baseLineVertices = 2;
        private int perpLineVertices = 2;
        private int groundLineVertices;
        private int ground2LineVertices = 6;
        private int circleSegments = 6;
        private int circlesInSymbol = 3;
        private int circlesVertices;

        private float translationSpringSegmentSize = 0.2f;
        private int translationSpringSegments = 8;
        private int translationSpringVertices;

        private float rotationSpringRadius = 0.1f;
        private float rotationSpringHeight = 0.01f;
        private int rotationSpringSegments = 20;
        private int rotationSpringVertices = 0;

        private int bunchSize;

        #endregion

        public PointGadgetService(GadgetManager gm)
        {
            gadgetManager = gm;
            restraints = new Dictionary<short, GadgetLocator>();

            // Get default vertex color
            int symColor = System.Drawing.Color.Aquamarine.ToArgb();
            int translationSpringColor = System.Drawing.Color.Aquamarine.ToArgb();
            int rotationSpringColor = System.Drawing.Color.Yellow.ToArgb();

            #region Update size delimiters
            // 3 lines and 2 vertices for each
            groundLineVertices = 2 * groundLines;

            // 6 circles with 6 segments each. 'Cause we are using LineList as primitive, the number of segments must be doubled
            circlesVertices = circlesInSymbol * circleSegments * 2;

            translationSpringVertices = translationSpringSegments * 2;
            rotationSpringVertices = rotationSpringSegments * 2;

            // How many vertices are there per plane and non transformed
            bunchSize = triangleVertices + baseLineVertices + perpLineVertices + groundLineVertices + circlesVertices + ground2LineVertices + translationSpringVertices + rotationSpringVertices;
            #endregion

            #region Memory Allocation for vertices and indices
            symVertices = new CustomVertex.PositionColored[
                                      (triangleVertices +           // Triangle vertices... LineList instead of LineStrip for sending bunches of vertices  
                                       baseLineVertices +           // Line as base vertices... Just 2
                                       perpLineVertices +           // Perpendicular line to the base... Just 2
                                       groundLineVertices +         // "Ground" lines... Six of them
                                       circlesVertices +            // Circles bellow base line
                                       ground2LineVertices +        // Horizontal Lines...
                                       translationSpringVertices +  // Translations spring vertices
                                       rotationSpringVertices)      // Rotation spring vertices
                                       * 3];                        // We multiply by 3, because there are three planes

            // Indices
            symbolIndices = new int[numOptions][][];
            for (int i = 0; i < numOptions; ++i)
                symbolIndices[i] = new int[possibleSymbols][];
            #endregion

            #region Geometry and first indices generation
            // First bunch - XY
            int currIndex = 0, baseIndex;
            int vertices = 0, indices = 0;

            #region Triangle
            symVertices[vertices++] = new CustomVertex.PositionColored(-triangleSideSize / 2.0f, -triangleSideSize * 0.6f, 0.0f, symColor);
            symVertices[vertices++] = new CustomVertex.PositionColored(0.0f, 0.0f, 0.0f, symColor);
            symVertices[vertices++] = new CustomVertex.PositionColored(0.0f, 0.0f, 0.0f, symColor);
            symVertices[vertices++] = new CustomVertex.PositionColored(triangleSideSize / 2.0f, -triangleSideSize * 0.6f, 0.0f, symColor);

            symbolIndices[0][0] = new int[triangleVertices];
            for (int i = 0; i < triangleVertices; ++i)
                symbolIndices[0][0][i] = indices++;
            #endregion

            #region BaseLine
            symVertices[vertices++] = new CustomVertex.PositionColored(-baseLineSize / 2.0f, -triangleSideSize * 0.6f, 0.0f, symColor);
            symVertices[vertices++] = new CustomVertex.PositionColored(baseLineSize / 2.0f, -triangleSideSize * 0.6f, 0.0f, symColor);

            symbolIndices[0][1] = new int[baseLineVertices];
            for (int i = 0; i < baseLineVertices; ++i)
                symbolIndices[0][1][i] = indices++;
            #endregion

            #region PerpendicularLine
            symVertices[vertices++] = new CustomVertex.PositionColored(0.0f, -triangleSideSize * 0.6f, 0.0f, symColor);
            symVertices[vertices++] = new CustomVertex.PositionColored(0.0f, 0.0f, 0.0f, symColor);

            symbolIndices[0][2] = new int[perpLineVertices];
            for (int i = 0; i < perpLineVertices; ++i)
                symbolIndices[0][2][i] = indices++;
            #endregion

            #region groundLineVertices
            symbolIndices[0][3] = new int[groundLineVertices];
            for (int i = 0; i < groundLineVertices; i += 2)
            {
                symVertices[vertices++] = new CustomVertex.PositionColored(-baseLineSize / 2.0f + (baseLineSize / groundLines) * (i / 2.0f + 1.0f), -triangleSideSize * 0.6f, 0.0f, symColor);
                symVertices[vertices++] = new CustomVertex.PositionColored(-baseLineSize / 2.0f + (baseLineSize / groundLines) * (i / 2.0f), -triangleSideSize * 0.7f, 0.0f, symColor);

                symbolIndices[0][3][i] = indices++;
                symbolIndices[0][3][i + 1] = indices++;
            }
            #endregion

            #region circlesVertices
            currIndex = vertices;
            float angleStep = 2.0f * (float)Math.PI / circleSegments;

            symbolIndices[0][4] = new int[circlesVertices];

            for (int i = 0; i < circleSegments * 2; i += 2)
            {
                symVertices[vertices++] = new CustomVertex.PositionColored(
                                                -baseLineSize / 2.0f + baseLineSize / (2.0f * circlesInSymbol) + circleRadius * (float)Math.Cos(angleStep * (i / 2)),
                                                -triangleSideSize * 0.6f - circleRadius + circleRadius * (float)Math.Sin(angleStep * (i / 2)), 0.0f, symColor);

                symVertices[vertices++] = new CustomVertex.PositionColored(
                                                -baseLineSize / 2.0f + baseLineSize / (2.0f * circlesInSymbol) + circleRadius * (float)Math.Cos(angleStep * (i / 2 + 1)),
                                                -triangleSideSize * 0.6f - circleRadius + circleRadius * (float)Math.Sin(angleStep * (i / 2 + 1)), 0.0f, symColor);

                symbolIndices[0][4][i] = indices++;
                symbolIndices[0][4][i + 1] = indices++;
            }

            baseIndex = currIndex;

            for (int i = 1; i < circlesInSymbol; ++i)
            {
                currIndex = circleSegments * 2 * i;
                CustomVertex.PositionColored centerOffset = new CustomVertex.PositionColored(i * baseLineSize / circlesInSymbol, 0.0f, 0.0f, symColor);
                for (int j = 0; j < circleSegments * 2; ++j)
                {
                    symVertices[vertices].Position = centerOffset.Position + symVertices[baseIndex + j].Position;
                    symVertices[vertices++].Color = centerOffset.Color;
                    symbolIndices[0][4][currIndex + j] = indices++;
                }
            }
            #endregion

            #region ground2LineVertices
            symbolIndices[0][5] = new int[ground2LineVertices];
            for (int i = 0; i < ground2LineVertices; i += 2)
            {
                symVertices[vertices++] = new CustomVertex.PositionColored(baseLineSize * (-0.5f + 0.05f * (i + 1)), triangleSideSize * (-0.6f - 0.05f * (i + 1)), 0.0f, symColor);

                symVertices[vertices++] = new CustomVertex.PositionColored(baseLineSize * (0.5f - 0.05f * (i + 1)), triangleSideSize * (-0.6f - 0.05f * (i + 1)), 0.0f, symColor);

                symbolIndices[0][5][i] = indices++;
                symbolIndices[0][5][i + 1] = indices++;
            }
            #endregion

            #region translationSpringVertices
            float lastX = 0.0f, lastY = 0.0f, x = 0.0f, y = 0.0f;
            float z = 0.0f, angle = 0.0f;

            symbolIndices[0][6] = new int[translationSpringVertices];

            //symVertices[vertices++] = new Vector3(0.0f, 0.0f, 0.0f);
            //symVertices[vertices++] = new Vector3(translationSpringSegmentSize, 0.0f, 0.0f);

            //symbolIndices[0][6][0] = indices++;
            //symbolIndices[0][6][1] = indices++;

            //lastX = translationSpringSegmentSize; lastY = 0.0f;

            int dir = 1, step = 1;

            for (int i = 0; i < translationSpringSegments; ++i)
            {
                symVertices[vertices++] = new CustomVertex.PositionColored(lastX, lastY, 0.0f, translationSpringColor);

                if (step % 2 == 0)
                    dir *= -1;

                x = lastX + translationSpringSegmentSize * (float)Math.Cos(dir * 3.0 * Math.PI / 8.0f);
                y = lastY + translationSpringSegmentSize * (float)Math.Sin(dir * 3.0 * Math.PI / 8.0f);

                symVertices[vertices++] = new CustomVertex.PositionColored(x, y, 0.0f, translationSpringColor);

                symbolIndices[0][6][2 * i] = indices++;
                symbolIndices[0][6][2 * i + 1] = indices++;

                lastX = x; lastY = y;

                step += dir;
            }

            //symVertices[vertices++] = new Vector3(lastX, lastY, 0.0f);
            //symVertices[vertices++] = new Vector3(lastX + triangleSideSize, 0.0f, 0.0f);

            //symbolIndices[0][6][translationSpringVertices - 2] = indices++;
            //symbolIndices[0][6][translationSpringVertices - 1] = indices++;

            // Orient segments... Rotation about draw axis
            for (int i = 0, index = vertices - translationSpringVertices; i < translationSpringVertices; ++i)
            {
                x = -symVertices[index].X;
                y = -symVertices[index].Y;
                z = symVertices[index].Z;

                symVertices[index].X = x;
                symVertices[index].Y = y;
                symVertices[index].Z = z;

                x = symVertices[index].X;
                y = symVertices[index].Y * (float)Math.Cos(-Math.PI / 4.0f) - symVertices[index].Z * (float)Math.Sin(-Math.PI / 4.0f);
                z = symVertices[index].Y * (float)Math.Sin(-Math.PI / 4.0f) + symVertices[index].Z * (float)Math.Cos(-Math.PI / 4.0f);

                symVertices[index].X = x;
                symVertices[index].Y = y;
                symVertices[index].Z = z;

                ++index;
            }

            #endregion

            #region rotationSpringVertices
            symbolIndices[0][7] = new int[rotationSpringVertices];

            //symVertices[vertices++] = new Vector3(0.0f, 0.0f, 0.0f);
            //symVertices[vertices++] = new Vector3(0.0f, 0.0f, translationSpringSegmentSize / 2.0f);

            //symbolIndices[0][7][0] = indices++;
            //symbolIndices[0][7][1] = indices++;

            //symVertices[vertices++] = new Vector3(0.0f, 0.0f, translationSpringSegmentSize / 2.0f);
            //symVertices[vertices++] = new Vector3(0.0f, translationSpringSegmentSize / 2.0f, translationSpringSegmentSize / 2.0f);

            //symbolIndices[0][7][2] = indices++;
            //symbolIndices[0][7][3] = indices++;

            angleStep = 3.0f * (float)Math.PI / rotationSpringSegments;
            angle = 0.0f;

            lastX = 0.0f; lastY = 0.0f;

            x = y = z = 0.0f;

            for (int i = 0; i < rotationSpringSegments; ++i)
            {
                symVertices[vertices++] = new CustomVertex.PositionColored(x, y, z, rotationSpringColor);

                y = rotationSpringRadius * (float)Math.Cos(angle);
                z = rotationSpringRadius * (float)Math.Sin(angle);
                x = rotationSpringHeight * (float)Math.PI * angle * 2.0f;

                symVertices[vertices++] = new CustomVertex.PositionColored(x, y, z, rotationSpringColor);

                symbolIndices[0][7][2 * i] = indices++;
                symbolIndices[0][7][2 * i + 1] = indices++;

                angle += angleStep;
            }

            //symVertices[vertices++] = new Vector3(lastX + x, lastY + y, lastZ + z);
            //symVertices[vertices++] = new Vector3(lastX + x, lastY + y, lastZ + z + translationSpringSegmentSize);

            //symbolIndices[0][7][rotationSpringVertices - 2] = indices++;
            //symbolIndices[0][7][rotationSpringVertices - 1] = indices++;

            // Orient segments... Rotation about draw axis
            for (int i = 0, index = vertices - rotationSpringVertices; i < rotationSpringVertices; ++i)
            {
                x = -symVertices[index].X;
                y = -symVertices[index].Y;
                z = symVertices[index].Z;

                symVertices[index].X = x;
                symVertices[index].Y = y;
                symVertices[index].Z = z;

                ++index;
            }
            #endregion

            #endregion

            #region Copy contents for each plane
            int counter = 0, symbol = 0;
            int symbolBunch = triangleVertices + baseLineVertices + perpLineVertices + groundLineVertices + circlesVertices + ground2LineVertices;

            // If there's nothing else to draw, then put the rest of the contents in the vertex array
            for (int k = 1; k < numOptions; ++k)
            {
                symbol = 0;
                for (int j = 0; j < symbolBunch; ++j)
                {
                    if (k == 1)
                        symVertices[vertices++] = new CustomVertex.PositionColored(symVertices[j].X, symVertices[j].Z, symVertices[j].Y, symVertices[j].Color);
                    else
                        symVertices[vertices++] = new CustomVertex.PositionColored(symVertices[j].Z, symVertices[j].X, symVertices[j].Y, symVertices[j].Color);

                    if (j == triangleVertices)
                        counter = triangleVertices;
                    else if (j == triangleVertices + baseLineVertices)
                        counter = baseLineVertices;
                    else if (j == triangleVertices + baseLineVertices + perpLineVertices)
                        counter = perpLineVertices;
                    else if (j == triangleVertices + baseLineVertices + perpLineVertices + groundLineVertices)
                        counter = groundLineVertices;
                    else if (j == triangleVertices + baseLineVertices + perpLineVertices + groundLineVertices + circlesVertices)
                        counter = circlesVertices;
                    else if (j + 1 == triangleVertices + baseLineVertices + perpLineVertices + groundLineVertices + circlesVertices + ground2LineVertices)
                        counter = ground2LineVertices;

                    if (counter != 0)
                    {
                        if (symbolIndices[k][symbol] == null)
                            symbolIndices[k][symbol] = new int[counter];
                        for (int i = 0; i < counter; ++i)
                            symbolIndices[k][symbol][i] = indices++;

                        counter = 0;
                        ++symbol;
                    }
                }
            }

            for (int k = 1; k < numOptions; ++k)
            {
                symbol = 6;
                for (int j = symbolBunch; j < bunchSize; ++j)
                {
                    if (k == 1)
                        symVertices[vertices++] = new CustomVertex.PositionColored(symVertices[j].Z, symVertices[j].X, symVertices[j].Y, symVertices[j].Color);
                    else
                        symVertices[vertices++] = new CustomVertex.PositionColored(symVertices[j].Y, symVertices[j].Z, symVertices[j].X, symVertices[j].Color);

                    if (j == triangleVertices + baseLineVertices + perpLineVertices + groundLineVertices + circlesVertices + ground2LineVertices + translationSpringVertices)
                        counter = translationSpringVertices;
                    else if (j + 1 == triangleVertices + baseLineVertices + perpLineVertices + groundLineVertices + circlesVertices + ground2LineVertices + translationSpringVertices + rotationSpringVertices)
                        counter = rotationSpringVertices;

                    if (counter != 0)
                    {
                        if (symbolIndices[k][symbol] == null)
                            symbolIndices[k][symbol] = new int[counter];
                        for (int i = 0; i < counter; ++i)
                            symbolIndices[k][symbol][i] = indices++;

                        counter = 0;
                        ++symbol;
                    }
                }
            }
            #endregion

            // Generate complete symbol indices
            symIndices = new int[6][][];
            #region Plane symbols
            for (int i = 0; i < 3; ++i) // i == 0 => XY Plane; i == 1 => XZ Plane; i == 2 => Plane YZ
            {
                symIndices[i] = new int[factorySymbols][];
                // Symbol 1: Triangle + Base
                symIndices[i][0] = new int[symbolIndices[i][0].Length + symbolIndices[i][1].Length]; // + symbolIndices[i][3].Length];
                Array.Copy(symbolIndices[i][0], 0, symIndices[i][0], 0, symbolIndices[i][0].Length);
                Array.Copy(symbolIndices[i][1], 0, symIndices[i][0], symbolIndices[i][0].Length, symbolIndices[i][1].Length);
                //Array.Copy(symbolIndices[i][3], 0, symIndices[i][0], symbolIndices[i][0].Length + symbolIndices[i][1].Length, symbolIndices[i][3].Length);

                // Symbol 2: PerpendicularLine + Base
                symIndices[i][1] = new int[symbolIndices[i][2].Length + symbolIndices[i][1].Length]; // + symbolIndices[i][3].Length];
                Array.Copy(symbolIndices[i][2], 0, symIndices[i][1], 0, symbolIndices[i][2].Length);
                Array.Copy(symbolIndices[i][1], 0, symIndices[i][1], symbolIndices[i][2].Length, symbolIndices[i][1].Length);
                //Array.Copy(symbolIndices[i][3], 0, symIndices[i][1], symbolIndices[i][2].Length + symbolIndices[i][1].Length, symbolIndices[i][3].Length);

                // Symbol 3: Triangle + Base + Circles
                symIndices[i][2] = new int[symbolIndices[i][0].Length + symbolIndices[i][1].Length + symbolIndices[i][4].Length];
                Array.Copy(symbolIndices[i][0], 0, symIndices[i][2], 0, symbolIndices[i][0].Length);
                Array.Copy(symbolIndices[i][1], 0, symIndices[i][2], symbolIndices[i][0].Length, symbolIndices[i][1].Length);
                Array.Copy(symbolIndices[i][4], 0, symIndices[i][2], symbolIndices[i][0].Length + symbolIndices[i][1].Length, symbolIndices[i][4].Length);

                // Symbol 4: PerpendicularLine + Base + Circles
                symIndices[i][3] = new int[symbolIndices[i][2].Length + symbolIndices[i][1].Length + symbolIndices[i][4].Length];
                Array.Copy(symbolIndices[i][2], 0, symIndices[i][3], 0, symbolIndices[i][2].Length);
                Array.Copy(symbolIndices[i][1], 0, symIndices[i][3], symbolIndices[i][2].Length, symbolIndices[i][1].Length);
                Array.Copy(symbolIndices[i][4], 0, symIndices[i][3], symbolIndices[i][2].Length + symbolIndices[i][1].Length, symbolIndices[i][4].Length);

                // Symbol 5: PerpendicularLine + Base + GroundLines
                symIndices[i][4] = new int[symbolIndices[i][2].Length + symbolIndices[i][1].Length + symbolIndices[i][5].Length];
                Array.Copy(symbolIndices[i][2], 0, symIndices[i][4], 0, symbolIndices[i][2].Length);
                Array.Copy(symbolIndices[i][1], 0, symIndices[i][4], symbolIndices[i][2].Length, symbolIndices[i][1].Length);
                Array.Copy(symbolIndices[i][5], 0, symIndices[i][4], symbolIndices[i][2].Length + symbolIndices[i][1].Length, symbolIndices[i][5].Length);
            }
            #endregion

            #region Axes symbols
            for (int i = 3; i < 6; ++i) // i == 3 => XY Plane; i == 4 => XZ Plane; i == 5 => YZ Plane
            {
                symIndices[i] = new int[3][];

                // Symbol 6: Translation spring
                symIndices[i][0] = symbolIndices[i - 3][6];
                
                // Symbol 7: Rotation spring
                symIndices[i][1] = symbolIndices[i - 3][7];

                // Symbol 8: Translation and Rotation springs
                symIndices[i][2] = new int[symbolIndices[i - 3][6].Length + symbolIndices[i - 3][7].Length];
                Array.Copy(symbolIndices[i - 3][6], 0, symIndices[i][2], 0, symbolIndices[i - 3][6].Length);
                Array.Copy(symbolIndices[i - 3][7], 0, symIndices[i][2], symbolIndices[i - 3][6].Length, symbolIndices[i - 3][7].Length);
            }
            #endregion
        }

        private int[][] getConstraints(Model.JointDOF jointDoF, ref bool[] orientation)
        {
            int[][] symVertices;
            bool[] symPossibilities;

            symVertices = new int[6][];
            orientation = new bool[6];
            symPossibilities = new bool[6];

            for (int i = 0; i < 6; ++i)
            {
                symVertices[i] = new int[0];
                symPossibilities[i] = true;
            }

            byte R = jointDoF.AllRestraints;
            byte S = jointDoF.AllSprings;

            // When springs on any translation are active...
            if ((S & 1) != 0) // X
            {
                symVertices[(int)AxesOrPlane.X] = symIndices[(int)AxesOrPlane.X][0];
                symPossibilities[(int)AxesOrPlane.XY] = symPossibilities[(int)AxesOrPlane.XZ] = false;
            }
            if ((S & 2) != 0) // Y
            {
                symVertices[(int)AxesOrPlane.Y] = symIndices[(int)AxesOrPlane.Y][0];
                symPossibilities[(int)AxesOrPlane.XY] = symPossibilities[(int)AxesOrPlane.YZ] = false;
            }
            if ((S & 4) != 0) // Z
            {
                symVertices[(int)AxesOrPlane.Z] = symIndices[(int)AxesOrPlane.Z][0]; ;
                symPossibilities[(int)AxesOrPlane.XZ] = symPossibilities[(int)AxesOrPlane.XY] = false;
            }
            // When springs on any moment are active...
            if ((S & 8) != 0) // X
            {
                if (symVertices[(int)AxesOrPlane.X] != null && symVertices[(int)AxesOrPlane.X].Length != 0)
                    symVertices[(int)AxesOrPlane.X] = symIndices[(int)AxesOrPlane.X][2];
                else
                    symVertices[(int)AxesOrPlane.X] = symIndices[(int)AxesOrPlane.X][1];

                symPossibilities[(int)AxesOrPlane.YZ] = false;
            }
            if ((S & 16) != 0) // Y
            {
                if (symVertices[(int)AxesOrPlane.Y] != null && symVertices[(int)AxesOrPlane.Y].Length != 0)
                    symVertices[(int)AxesOrPlane.Y] = symIndices[(int)AxesOrPlane.Y][2];
                else
                    symVertices[(int)AxesOrPlane.Y] = symIndices[(int)AxesOrPlane.Y][1];

                symPossibilities[(int)AxesOrPlane.XZ] = false;
            }
            if ((S & 32) != 0) // Z
            {
                if (symVertices[(int)AxesOrPlane.Z] != null && symVertices[(int)AxesOrPlane.Z].Length != 0)
                    symVertices[(int)AxesOrPlane.Z] = symIndices[(int)AxesOrPlane.Z][2];
                else
                    symVertices[(int)AxesOrPlane.Z] = symIndices[(int)AxesOrPlane.Z][1];

                symPossibilities[(int)AxesOrPlane.XY] = false;
            }

            // Now verify restrictions...
            for (int i = 0; i < 3; ++i)
            {
                if (symPossibilities[i] == true)
                    symVertices[i] = getConstraintSymAtPlane((AxesOrPlane)i, R, ref orientation[i]);
            }

            return symVertices;
        }

        private int[] getConstraintSymAtPlane(AxesOrPlane plane, byte R, ref bool orientation)
        {
            int result = 0;

            if (plane == AxesOrPlane.XY)
                result = ((R & (byte)1) != 0 ? 1 : 0) + ((R & (byte)2) != 0 ? 1 : 0) * 2 + ((R & (byte)32) != 0 ? 1 : 0) * 4;
            else if (plane == AxesOrPlane.XZ)
                result = ((R & (byte)1) != 0 ? 1 : 0) + ((R & (byte)4) != 0 ? 1 : 0) * 2 + ((R & (byte)16) != 0 ? 1 : 0) * 4;
            else if (plane == AxesOrPlane.YZ)
                result = ((R & (byte)2) != 0 ? 1 : 0) + ((R & (byte)4) != 0 ? 1 : 0) * 2 + ((R & (byte)8) != 0 ? 1 : 0) * 4;

            return getConstraintShape(result, plane, ref orientation);
        }

        private int[] getConstraintShape(int index, AxesOrPlane plane, ref bool orientation)
        {
            int[] shapeVertices;
            orientation = false;

            switch (index)
            {
                case 0:
                    shapeVertices = new int[0];
                    break;
                case 1: // Triangle + Base + Circles
                    shapeVertices = symIndices[(int)plane][2];
                    orientation = true;
                    break;
                case 2: // Triangle + Base + Circles
                    shapeVertices = symIndices[(int)plane][2];
                    break;
                case 3: // Triangle + Base
                    shapeVertices = symIndices[(int)plane][0];
                    break;
                case 4: // PerpendicularLines + Base + GroundLines
                    shapeVertices = symIndices[(int)plane][4];
                    break;
                case 5: // PerpendicularLine + Base + Circles
                    shapeVertices = symIndices[(int)plane][3];
                    orientation = true;
                    break;
                case 6: // PerpendicularLine + Base + Circles
                    shapeVertices = symIndices[(int)plane][3];
                    break;
                case 7: // PerpendicularLine + Base
                    shapeVertices = symIndices[(int)plane][1];
                    break;
                default:
                    shapeVertices = new int[0];
                    break;
            }

            return shapeVertices;
        }

        private int[] getAxialSpring(AxesOrPlane axis)
        {
            int[] vertices = new int[0];

            //if (axis == AxesOrPlane.X)
            //    vertices = symIndices[(int)axis][3];
            //else if (axis == AxesOrPlane.Y)
            //    vertices = symIndices[(int)axis][4];
            //else if (axis == AxesOrPlane.Z)
            //    vertices = symIndices[(int)axis][5];

            return vertices;
        }

        private int[] getAxialHelicSpring(AxesOrPlane val)
        {
            int[] vertices = new int[0];

            return vertices;
        }

        private enum AxesOrPlane : int
        {
            XY, XZ, YZ, X, Y, Z
        };

        #region GadgetService Members

        public void ClearLocators()
        {
            restraints.Clear();
        }

        #endregion

        public void DrawCanonicalConstraints(Device device, JointDOF DoF, Vector3 pos)
        {

            gadgetManager.SetActiveStream(ResourceStreamType.Lines);

            if (DoF.AllDOF != 0 && !restraints.ContainsKey(DoF.AllDOF))
            {
                bool[] orientations = null;
                int[][] constraintIndices = getConstraints(DoF, ref orientations);
                int rows, cols;
                CustomVertex.PositionColored currSymVertex;

                int totalVertices = 0;

                rows = constraintIndices.Length;

                for (int i = 0; i < rows; ++i)
                    totalVertices += constraintIndices[i].Length;

                PositionColoredPackage package = (PositionColoredPackage)gadgetManager.CaptureBuffer(ResourceStreamType.Lines, 0, totalVertices);

                unsafe
                {

                    if (constraintIndices != null && orientations != null)
                    {
                        for (int i = 0; i < rows; ++i)
                        {
                            if (constraintIndices[i] != null && constraintIndices[i].Length != 0)
                            {
                                cols = constraintIndices[i].Length;
                                for (int k = 0; k < cols; ++k)
                                {
                                    if (orientations[i])
                                    {
                                        if (i == 0) // Plane XY... X = Y, Y = -X
                                        {
                                            currSymVertex.X = SymVertices[constraintIndices[i][k]].Y;
                                            currSymVertex.Y = -SymVertices[constraintIndices[i][k]].X;
                                            currSymVertex.Z = SymVertices[constraintIndices[i][k]].Z;
                                            currSymVertex.Color = SymVertices[constraintIndices[i][k]].Color;
                                        }
                                        else if (i == 1)
                                        {
                                            currSymVertex.X = SymVertices[constraintIndices[i][k]].Z;
                                            currSymVertex.Y = SymVertices[constraintIndices[i][k]].Y;
                                            currSymVertex.Z = -SymVertices[constraintIndices[i][k]].X;
                                            currSymVertex.Color = SymVertices[constraintIndices[i][k]].Color;
                                        }
                                        else if (i == 2)
                                        {
                                            currSymVertex.X = SymVertices[constraintIndices[i][k]].X;
                                            currSymVertex.Y = -SymVertices[constraintIndices[i][k]].Z;
                                            currSymVertex.Z = SymVertices[constraintIndices[i][k]].Y;
                                            currSymVertex.Color = SymVertices[constraintIndices[i][k]].Color;
                                        }
                                        else
                                            currSymVertex = SymVertices[constraintIndices[i][k]];
                                    }
                                    else
                                        currSymVertex = SymVertices[constraintIndices[i][k]];

                                    package.VBPointer->Position = currSymVertex.Position;
                                    package.VBPointer->Color = currSymVertex.Color;

                                    package.VBPointer++;
                                }
                            }
                        }
                    }
                }

                gadgetManager.ReleaseBuffer(totalVertices, 0, ResourceStreamType.Lines);

                GadgetLocator locator;
                locator.Offset = package.Offset;
                locator.Size = totalVertices;
                restraints.Add(DoF.AllDOF, locator);
            }

            if (DoF.AllDOF != 0)
            {
                device.Transform.World = Matrix.Translation(pos);

                device.DrawPrimitives(PrimitiveType.LineList, restraints[DoF.AllDOF].Offset, restraints[DoF.AllDOF].Size / 2);
            }
        }
    }
}
