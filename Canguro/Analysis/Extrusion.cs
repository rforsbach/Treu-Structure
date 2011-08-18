using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Analysis
{
    public class ExtrudedShape
    {
        /// <summary> Class empty constructor </summary>
        private ExtrudedShape() { }

        /// <summary> Class is a singleton </summary>
        public static readonly ExtrudedShape Instance = new ExtrudedShape();

        #region Extrusion from almost any axis curves
        public static class Extrusion
        {
            #region Counters and other variables
            private static Vector3 k = new Vector3(0, 0, 1);
            private static Vector3[] currAxes = null;
            private static Vector3[] nextAxes = null;
            private static Vector3 lineDir;

            private static int nPointSegments;
            private static int nShapeVertices;

            private static int meshVertex;
            private static int index;
            private static int baseIndex;
            private static int segment;
            private static int vertex;

            private static int shapeVertex;

            private static Vector3 temp1 = Vector3.Empty;
            private static Vector3 temp2 = Vector3.Empty;
            private static Vector3 temp3 = Vector3.Empty;
            private static Vector3 currDir = Vector3.Empty;
            private static Vector3 lastDir = Vector3.Empty;

            private static int nMeshVertices;
            private static int indexBufferSize;
            #endregion

            /// <summary> Builds a local system on the line </summary>
            /// <param name="line"> This is the direction that must be followed </param>
            /// <returns> Local system </returns>
            private static Vector3[] computeLocalSystem(Vector3 line, Vector3[] origAxes)
            {
                float zn;
                // Cortesia del hippie-comeflores y reconfigurada para la extrusion
                Vector3[] localAxes = new Vector3[3];

                // Longitudinal axis
                localAxes[2] = Vector3.Normalize(line);

                // Check if the vector and the beam axes try to follow Z axis
                zn = Vector3.Dot(localAxes[2], origAxes[2]);

                // Calculate projection of Z and n. Try to find local 2 axis following Z
                //zn = Vector3.Dot(k, localAxes[2]);

                // Axis parallel to Z (Dot product near maximum value equals parallelism)
                if (Math.Abs(zn) > 0.999999)
                {
                    // Local Axis 2 = X and 3 = +/- Y
                    localAxes[0] = origAxes[0];
                    localAxes[1] = origAxes[1];
                    localAxes[2] = origAxes[2];
                }
                else
                {
                    // Local 2 equals the difference between the projected Z at n and Z
                    localAxes[0] = Vector3.Normalize(origAxes[2] - Vector3.Scale(localAxes[2], zn));
                    localAxes[1] = Vector3.Cross(localAxes[0], localAxes[2]);
                }

                return localAxes;
            }

            /// <summary> Builds an extrusion from the shape along a curve </summary>
            /// <param name="shape"> An array indicating the shape points ordered counter_clockwise. First index is for vertex and second for normals. </param>
            /// <param name="axis"> The curve that must be followed. Array of vertixes. </param>
            /// <param name="axisDerivative"> The derivative at each vertex in the curve. </param>
            /// <param name="extrudedMesh"> A ref variable containing the extruded mesh. First index refers to vertices and the second to normals. </param>
            /// <param name="meshIB"> A ref variable containing the union rules for the extruded mesh (index buffer) </param>
            /// <param name="origAxes"></param>
            public static void Build(Vector2[][] shape, short[] shapeIndices, Vector2 cardinalPointOffset, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref short[] meshIB, Vector3[] origAxes)
            {
                // Get the number of segments in the curve
                nPointSegments = axis.GetLength(0);
                // Get the number of vertices in the shape
                nShapeVertices = shapeIndices.Length;
                // Initialize some counters
                meshVertex = 0;
                index = 0;
                currDir = Vector3.Empty;

                // How many vertices in the mesh are we going to have?
                nMeshVertices = nShapeVertices * nPointSegments;

                // Determine the number of indices for mesh tesellation
                indexBufferSize = ((nShapeVertices - 1) * 2) * (nPointSegments - 1) * 3;  // nMeshVertices * 2 = Number of faces per segment

                // Just get memory when necessary
                if (extrudedMesh == null || nMeshVertices != extrudedMesh.GetLength(0))
                    extrudedMesh = new Vector3[2, nMeshVertices];

                if (meshIB == null || indexBufferSize != meshIB.GetLength(0))
                    meshIB = new short[indexBufferSize];

                for (segment = 0; segment < nPointSegments; ++segment)
                {
                    // Compute the plane at the current point in the curve
                    if (axisDerivative != null && axisDerivative[segment] != Vector3.Empty)
                        lineDir = axisDerivative[segment];
                    // When no derivatives are computed, use instead the line direction
                    else
                    {
                        if (segment == nPointSegments - 1)
                            lineDir = axis[nPointSegments - 1] - axis[nPointSegments - 2];
                        else
                            lineDir = axis[segment + 1] - axis[segment];

                        lineDir.Normalize();
                    }

                    lastDir = lineDir;
                    lineDir += currDir;
                    lineDir.Normalize();

                    // Check if we have a 'good' value
                    checkIntegrity(ref lineDir);

                    // and compute the local axes at the current curve point
                    currAxes = computeLocalSystem(lineDir, origAxes);

                    currDir = lastDir;

                    // Fill vertex array
                    for (vertex = 0; vertex < nShapeVertices; ++vertex, ++meshVertex)
                    {
                        // Mesh vertices
                        extrudedMesh[0, meshVertex] = axis[segment] +
                                                      (shape[0][shapeIndices[vertex]].X - cardinalPointOffset.X) * currAxes[0] +
                                                      (shape[0][shapeIndices[vertex]].Y - cardinalPointOffset.Y) * currAxes[1];
                        // Mesh normals
                        extrudedMesh[1, meshVertex] = shape[1][shapeIndices[vertex]].X * currAxes[0] + shape[1][shapeIndices[vertex]].Y * currAxes[1];
                    }
                }

                // Index buffer
                for (segment = 0; segment < nPointSegments - 1; ++segment)
                {
                    baseIndex = segment * nShapeVertices;
                    for (shapeVertex = 0; shapeVertex < nShapeVertices - 1; ++shapeVertex)
                    {
                        // First triangle
                        meshIB[index++] = (short)(baseIndex + shapeVertex);
                        meshIB[index++] = (short)(baseIndex + shapeVertex + nShapeVertices);
                        meshIB[index++] = (short)(baseIndex + shapeVertex + nShapeVertices + 1);
                        // Second triangle
                        meshIB[index++] = (short)(baseIndex + shapeVertex);
                        meshIB[index++] = (short)(baseIndex + shapeVertex + nShapeVertices + 1);
                        meshIB[index++] = (short)(baseIndex + shapeVertex + 1);
                    }
                }
            }

            /// <summary> Builds an extrusion from the shape along a curve </summary>
            /// <param name="shape"> An array indicating the shape points ordered counter_clockwise. First index is for vertex and second for normals. </param>
            /// <param name="axis"> The curve that must be followed. Array of vertixes. </param>
            /// <param name="axisDerivative"> The derivative at each vertex in the curve. </param>
            /// <param name="extrudedMesh"> A ref variable containing the extruded mesh. First index refers to vertices and the second to normals. </param>
            public static void Build(Vector2[][] shape, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref int[] extrudedIndices, bool faceNormals, short[] cover, Vector3[] origAxes)
            {
                // Get the number of segments in the curve
                nPointSegments = axis.GetLength(0);
                // Get the number of vertices in the shape
                nShapeVertices = shape[0].GetLength(0);
                // Initialize some counters
                meshVertex = 0;
                //index = 0;

                // How many vertices in the mesh are we going to have?
                int coverSize = cover.Length;
                nMeshVertices = nShapeVertices * (nPointSegments - 1) * 6 + 2 * coverSize; // 6 vertices per face

                // Just get memory when necessary (vectors)
                if (extrudedMesh == null || nMeshVertices != extrudedMesh.GetLength(1))
                    extrudedMesh = new Vector3[2, nMeshVertices];
                // Just get memory when necessary (colors)
                if (extrudedIndices == null || nMeshVertices != extrudedIndices.GetLength(0))
                    extrudedIndices = new int[nMeshVertices];

                for (segment = 0; segment < nPointSegments - 1; ++segment)
                {
                    // Compute the plane at the current point in the curve
                    if (axisDerivative != null && axisDerivative[segment] != Vector3.Empty)
                        lineDir = axisDerivative[segment];
                    // When no derivatives are computed, use instead the line direction
                    else
                    {
                        if (segment == nPointSegments - 1)
                            lineDir = axis[nPointSegments - 1] - axis[nPointSegments - 2];
                        else
                            lineDir = axis[segment + 1] - axis[segment];

                        lineDir.Normalize();
                    }

                    // Check if we have a 'good' value
                    checkIntegrity(ref lineDir);

                    // and compute the local axes at the current curve point
                    currAxes = computeLocalSystem(lineDir, origAxes);

                    currDir = lineDir;

                    if (segment > 0)
                    {
                        temp1 = nextAxes[0];
                        temp2 = nextAxes[1];
                        temp3 = nextAxes[2];
                    }

                    int auxSegment = segment + 1;
                    // Compute the plane at the next point in the curve
                    if (axisDerivative != null && axisDerivative[auxSegment] != Vector3.Empty)
                        lineDir = axisDerivative[auxSegment];
                    // When no derivatives are computed, use instead the line direction
                    else
                    {
                        if (auxSegment == nPointSegments - 1)
                            lineDir = axis[nPointSegments - 1] - axis[nPointSegments - 2];
                        else
                            lineDir = axis[auxSegment + 1] - axis[auxSegment];

                        lineDir.Normalize();
                    }

                    lineDir += currDir;
                    lineDir.Normalize();

                    // Check if we have a 'good' value
                    checkIntegrity(ref lineDir);
                    nextAxes = computeLocalSystem(lineDir, origAxes);

                    if (segment > 0)
                    {
                        currAxes[0] = temp1;
                        currAxes[1] = temp2;
                        currAxes[2] = temp3;
                    }

                    // Make start cover
                    if (segment == 0)
                    {
                        for (int i = 0; i < coverSize; ++i)
                        {
                            extrudedMesh[0, meshVertex] = axis[segment] + shape[0][cover[i]].X * currAxes[0] + shape[0][cover[i]].Y * currAxes[1];
                            extrudedMesh[1, meshVertex] = -currAxes[2];
                            extrudedIndices[meshVertex] = cover[i];
                            ++meshVertex;
                        }
                    }
                    // Make end cover
                    else if (auxSegment == nPointSegments - 1)
                    {
                        for (int i = coverSize - 1; i >= 0; --i)
                        {
                            extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][cover[i]].X * nextAxes[0] + shape[0][cover[i]].Y * nextAxes[1];
                            extrudedMesh[1, meshVertex] = nextAxes[2];
                            extrudedIndices[meshVertex] = nShapeVertices * segment + cover[i];
                            ++meshVertex;
                        }
                    }

                    // Fill vertex array
                    for (vertex = 0; vertex < nShapeVertices; ++vertex)
                    {
                        // Mesh vertices - First triangle
                        extrudedMesh[0, meshVertex] = axis[segment] + shape[0][vertex].X * currAxes[0] + shape[0][vertex].Y * currAxes[1];
                        extrudedMesh[1, meshVertex] = shape[1][vertex].X * currAxes[0] + shape[1][vertex].Y * currAxes[1];
                        extrudedIndices[meshVertex] = nShapeVertices * segment + vertex;
                        ++meshVertex;

                        extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][vertex].X * nextAxes[0] + shape[0][vertex].Y * nextAxes[1];
                        extrudedMesh[1, meshVertex] = shape[1][vertex].X * nextAxes[0] + shape[1][vertex].Y * nextAxes[1];
                        extrudedIndices[meshVertex] = nShapeVertices * auxSegment + vertex;
                        ++meshVertex;

                        if (vertex + 1 < nShapeVertices)
                        {
                            extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][vertex + 1].X * nextAxes[0] + shape[0][vertex + 1].Y * nextAxes[1];
                            extrudedIndices[meshVertex] = nShapeVertices * auxSegment + vertex + 1;
                            if (faceNormals)
                                extrudedMesh[1, meshVertex] = shape[1][vertex].X * nextAxes[0] + shape[1][vertex].Y * nextAxes[1];
                            else
                                extrudedMesh[1, meshVertex] = shape[1][vertex+1].X * nextAxes[0] + shape[1][vertex+1].Y * nextAxes[1];
                        }
                        else
                        {
                            extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][0].X * nextAxes[0] + shape[0][0].Y * nextAxes[1];
                            extrudedIndices[meshVertex] = nShapeVertices * auxSegment;
                            if (faceNormals)
                                extrudedMesh[1, meshVertex] = shape[1][nShapeVertices - 1].X * nextAxes[0] + shape[1][nShapeVertices - 1].Y * nextAxes[1];
                            else
                                extrudedMesh[1, meshVertex] = shape[1][0].X * nextAxes[0] + shape[1][0].Y * nextAxes[1];
                        }
                        ++meshVertex;

                        // Mesh vertices - Second triangle
                        extrudedMesh[0, meshVertex] = extrudedMesh[0, meshVertex - 3];
                        extrudedMesh[1, meshVertex] = extrudedMesh[1, meshVertex - 3];
                        extrudedIndices[meshVertex] = extrudedIndices[meshVertex - 3];
                        ++meshVertex;

                        extrudedMesh[0, meshVertex] = extrudedMesh[0, meshVertex - 2];
                        extrudedMesh[1, meshVertex] = extrudedMesh[1, meshVertex - 2];
                        extrudedIndices[meshVertex] = extrudedIndices[meshVertex - 2];
                        ++meshVertex;

                        if (vertex + 1 < nShapeVertices)
                        {
                            extrudedMesh[0, meshVertex] = axis[segment] + shape[0][vertex + 1].X * currAxes[0] + shape[0][vertex + 1].Y * currAxes[1];
                            extrudedIndices[meshVertex] = nShapeVertices * segment + vertex + 1;
                            if (faceNormals)
                                extrudedMesh[1, meshVertex] = shape[1][vertex].X * currAxes[0] + shape[1][vertex].Y * currAxes[1];
                            else
                                extrudedMesh[1, meshVertex] = shape[1][vertex + 1].X * currAxes[0] + shape[1][vertex + 1].Y * currAxes[1];
                        }
                        else
                        {
                            extrudedMesh[0, meshVertex] = axis[segment] + shape[0][0].X * currAxes[0] + shape[0][0].Y * currAxes[1];
                            extrudedIndices[meshVertex] = nShapeVertices * segment;
                            if (faceNormals)
                                extrudedMesh[1, meshVertex] = shape[1][nShapeVertices - 1].X * currAxes[0] + shape[1][nShapeVertices - 1].Y * currAxes[1];
                            else
                                extrudedMesh[1, meshVertex] = shape[1][0].X * currAxes[0] + shape[1][0].Y * currAxes[1];
                        }
                        ++meshVertex;
                    }
                }
            }
        }
        #endregion
        
        #region Utility
        /// <summary> Checks if this vector has 'good' values, i.e. components are not very little values. </summary>
        /// <param name="vector"> Vector to check. Must be a normalized vector. </param>
        public static void checkIntegrity(ref Vector3 vector)
        {
            float threshold = 0.000001f;
            bool mustRenormalize = false;

            if (Math.Abs(vector.X) < threshold)
            {
                vector.X = 0;
                mustRenormalize = true;
            }
            if (Math.Abs(vector.Y) < threshold)
            {
                vector.Y = 0;
                mustRenormalize = true;
            }
            if (Math.Abs(vector.Z) < threshold)
            {
                vector.Z = 0;
                mustRenormalize = true;
            }

            if( mustRenormalize )
                vector.Normalize();

            if (Math.Abs(vector.X) > 0.999999)
                vector.X = Math.Sign(vector.X);

            if (Math.Abs(vector.Y) > 0.999999)
                vector.Y = Math.Sign(vector.Y);

            if (Math.Abs(vector.Z) > 0.999999)
                vector.Z = Math.Sign(vector.Z);
        }
        #endregion

        #region Callers for building the extruded mesh
        public void BuildMesh(Vector2[][] shape, short[] shapeIndices, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref short[] meshIB, Vector3[] origAxes)
        {
            Extrusion.Build(shape, shapeIndices, Vector2.Empty, axis, axisDerivative, ref extrudedMesh, ref meshIB, origAxes);
        }

        public void BuildMesh(Vector2[][] shape, short[] shapeIndices, Vector2 cardinalPointOffset, Vector3[] axis, ref Vector3[,] extrudedMesh, ref short[] meshIB, Vector3[] origAxes)
        {
            Extrusion.Build(shape, shapeIndices, cardinalPointOffset, axis, null, ref extrudedMesh, ref meshIB, origAxes);
        }

        public void BuildMesh(Vector2[][] shape, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref int[] extrudedIndices, bool faceNormals, short[] cover, Vector3[] origAxes)
        {
            Extrusion.Build(shape, axis, axisDerivative, ref extrudedMesh, ref extrudedIndices, faceNormals, cover, origAxes);
        }

        public void BuildMesh(Vector2[][] shape, Vector3[] axis, ref Vector3[,] extrudedMesh, ref int[] extrudedIndices, bool faceNormals, short[] triangIndices, Vector3[] origAxes)
        {
            Extrusion.Build(shape, axis, null, ref extrudedMesh, ref extrudedIndices, faceNormals, triangIndices, origAxes);
        }
        #endregion
    }
}


#region Legacy
//namespace Canguro.View.Renderer
//{
//    public class ExtrudedShape
//    {
//        /// <summary> The class, curvedAxis used </summary>
//        private BezierCurve curvedAxis;

//        /// <summary> Class empty constructor </summary>
//        private ExtrudedShape()
//        {
//            //curvedAxis = new BezierCurve();
//        }

//        /// <summary> Class is a singleton </summary>
//        public static readonly ExtrudedShape Instance = new ExtrudedShape();

//        #region Bezier related... [Charles River Media - Mathematics for 3D Game Programming and Computer Graphics]
//        public class BezierCurve
//        {
//            #region Class properties
//            /// <summary> Number of segments for the bezier curve </summary>
//            private int nSegments = 8;
//            /// <summary> Number of Bezier control points. Four by default. </summary>
//            private int nControlPoints = 4;
//            /// <summary> Constants defining needed constants in the Bezier expresion</summary>
//            private float[,] bezierEvaluator;
//            /// <summary> Constants for computing Bezier derivative at each point in the curve </summary>
//            private float[,] bezierDerivativeEvaluator;
//            /// <summary> The array of control points </summary>
//            private Vector3[] controlPoints;
//            /// <summary> Points in the computed Bezier curve </summary>
//            private Vector3[] bezierPoints;
//            /// <summary> Bezier derivative for each point of the curve </summary>
//            private Vector3[] bezierDerivative;
//            /// <summary> Gets the current Bezier control points </summary>
//            public Vector3[] ControlPoinst
//            {
//                get { return controlPoints; }
//            }
//            /// <summary> Gets the current Bezier curve </summary>
//            public Vector3[] CurvePoints
//            {
//                get { return bezierPoints; }
//            }
//            /// <summary> Gets the current Bezier curve derivative </summary>
//            public Vector3[] Derivative
//            {
//                get { return bezierDerivative; }
//            }
//            /// <summary> Local axes for curve points computing </summary>
//            private Vector3[] axes;
//            /// <summary> A vector pointing in Z direction </summary>
//            private Vector3 zvec = new Vector3(0, 0, 1);
//            /// <summary> Local direction for a pair of points </summary>
//            private Vector3 dir = Vector3.Empty;
//            /// <summary> Midpoint vector between two vectorial quantities </summary>
//            private Vector3 mid = Vector3.Empty;
//            private float zn = 0.0f;

//            private Vector3 startRot = Vector3.Empty;
//            private Vector3 endRot = Vector3.Empty;
//            #endregion

//            /// <summary> Empty constructor. By default a cubic interpolator is built (4 control points needed) </summary>
//            public BezierCurve()
//            {
//                float u;    // Step in the curve
//                // Build Bezier evaluator
//                bezierEvaluator = new float[nControlPoints, nSegments];
//                // Build Bezier derivative evaluator
//                bezierDerivativeEvaluator = new float[nControlPoints, nSegments];

//                for (int i = 0; i < nSegments; ++i)
//                {
//                    u = i / (float)(nSegments - 1.0f);      // Get current step

//                    // Calcule each of the Bezier constants (evaluators) for this step
//                    bezierEvaluator[0, i] = (float)Math.Pow(1 - u, 3);
//                    bezierEvaluator[1, i] = 3.0f * u * (float)Math.Pow(1 - u, 2);
//                    bezierEvaluator[2, i] = 3.0f * (float)Math.Pow(u, 2) * (1 - u);
//                    bezierEvaluator[3, i] = (float)Math.Pow(u, 3);

//                    // Calcule each of the Bezier derivative constants for this step
//                    bezierDerivativeEvaluator[0, i] = -3.0f + 6.0f * u - 3.0f * (float)Math.Pow(u, 2);
//                    bezierDerivativeEvaluator[1, i] = 3.0f - 12.0f * u + 9.0f * (float)Math.Pow(u, 2);
//                    bezierDerivativeEvaluator[2, i] = 6.0f * u - 9.0f * (float)Math.Pow(u, 2);
//                    bezierDerivativeEvaluator[3, i] = 3.0f * (float)Math.Pow(u, 2);
//                }

//                // For now, use 4 control points
//                controlPoints = new Vector3[nControlPoints];

//                // We need a Vector3 array for holding the Bezier curve points
//                bezierPoints = new Vector3[nSegments];
//                // ...and another for the derivatives
//                bezierDerivative = new Vector3[nSegments];

//                axes = new Vector3[3];
//                axes[0] = Vector3.Empty;
//                axes[1] = Vector3.Empty;
//                axes[2] = Vector3.Empty;
//            }

//            /// <summary> Builds a Bezier curve according to the interpolation grade specified </summary>
//            /// <param name="interpolGrade"> The interpolation grade, use 3 for a cubic interpolation and so on. </param>
//            public BezierCurve(int interpolGrade)
//            {
//            }

//            /// <summary> Builds a local system from a line </summary>
//            /// <param name="start"> First point of the segment </param>
//            /// <param name="end"> Last point of the segment </param>
//            /// <returns></returns>
//            private Vector3[] computeLocalAxes(Vector3 start, Vector3 end)
//            {
//                // Cortesía del Hippie Come Flores
//                axes = new Vector3[3];

//                // Longitudinal axis
//                axes[0] = Vector3.Normalize(end - start);

//                // Calculate projection of Z and n. Try to find local 2 axis following Z
//                zn = Vector3.Dot(zvec, axes[0]);

//                // Axis parallel to Z (Dot product near maximum value equals parallelism)
//                if (Math.Abs(zn) > 0.999999)
//                {
//                    // Local Axis 2 = X and 3 = +/- Y
//                    axes[1].X = 1.0f; axes[1].Y = 0.0f; axes[1].Z = 0.0f;
//                    axes[2].X = 0.0f; axes[2].Y = Math.Sign(zn); axes[2].Z = 0.0f;
//                    axes[0].X = 0.0f; axes[0].Y = 0.0f; axes[0].Z = Math.Sign(zn);
//                }
//                else
//                {
//                    // Local 2 equals the difference between the projected Z at n and Z
//                    axes[1] = Vector3.Normalize(zvec - Vector3.Scale(axes[0], zn));
//                    axes[2] = Vector3.Cross(axes[0], axes[1]);
//                }

//                return axes;
//            }

//            /// <summary> Computes the Bezier curve from a set of control points </summary>
//            /// <param name="controlPts"> Array of Vertex3 elements. At least one more element than the interpolation grade. </param>
//            /// <returns> Bezier curve </returns>
//            public Vector3[] MakeBezier(Vector3[] controlPts)
//            {
//                int i, j;

//                // Check if the number of control points is the same of the elements in the array
//                if (controlPts.Length == nControlPoints)
//                {
//                    // Copy the contents of the array
//                    controlPoints = controlPts;

//                    // Each point/derivative is computed as an MADD operation
//                    for (i = 0; i < nSegments; ++i)
//                    {
//                        bezierPoints[i] = Vector3.Empty;
//                        bezierDerivative[i] = Vector3.Empty;

//                        for (j = 0; j < nControlPoints; ++j)
//                        {
//                            bezierPoints[i] += bezierEvaluator[j, i] * controlPoints[j];

//                            bezierDerivative[i] += bezierDerivativeEvaluator[j, i] * controlPoints[j];
//                        }

//                        // We need the derivative as normalized vectors...
//                        bezierDerivative[i].Normalize();
//                        // Safe check: very little numbers can introduce errors, make them zeros
//                        checkIntegrity(ref bezierDerivative[i]);
//                    }

//                    return bezierPoints;
//                }
//                else
//                    return null;
//            }

//            /// <summary> Computes the Bezier curve from two points, but considering two other 'deformation' vectors </summary>
//            /// <param name="controlPts"> Array of 4 Vector3 elements. For using this, we must have 4 elements and a cubic interpolation. </param>
//            /// <returns> Bezier curve </returns>
//            public Vector3[] MakeBezier(Vector3 start, Vector3 startM, Vector3 end, Vector3 endM)
//            {
//                int i, j;

//                // Get line direction...
//                dir = end - start;

//                // Compute the local axes for this line segment
//                axes = computeLocalAxes(start, end);

//                /// Because we use a cubic degree Bezier curve, we must compute four control points, start extracting the linear
//                /// interpolation between deformations
//                mid = 0.5f * (startM + endM);

//                // Control points - Start at the begining of the segment
//                controlPoints[0] = start;

//                // Compute the second control point at 1/4 of the line segment and oriented according to the line local axes
//                controlPoints[1] = 0.5f * (startM + mid);
//                controlPoints[1] = start + 0.25f * dir + controlPoints[1].X * axes[0] + controlPoints[1].Y * axes[1] + controlPoints[1].Z * axes[2];

//                // Compute the third control point at 3/4 of the line segment and oriented according to the line local axes
//                controlPoints[2] = 0.5f * (mid + endM);
//                controlPoints[2] = start + 0.75f * dir + controlPoints[2].X * axes[0] + controlPoints[2].Y * axes[1] + controlPoints[2].Z * axes[2];

//                // Compute the last control point, it is simply the point at the end of the segment
//                controlPoints[3] = end;

//                // Each point/derivative is computed as an MADD operation
//                for (i = 0; i < nSegments; ++i)
//                {
//                    bezierPoints[i] = Vector3.Empty;
//                    bezierDerivative[i] = Vector3.Empty;

//                    for (j = 0; j < nControlPoints; ++j)
//                    {
//                        bezierPoints[i] += bezierEvaluator[j, i] * controlPoints[j];

//                        bezierDerivative[i] += bezierDerivativeEvaluator[j, i] * controlPoints[j];
//                    }

//                    // We need the derivative as normalized vectors...
//                    bezierDerivative[i].Normalize();
//                    // Safe check: very little numbers can introduce errors, make them zeros
//                    checkIntegrity(ref bezierDerivative[i]);
//                }

//                return bezierPoints;
//            }

//            /// <summary> Computes the Bezier curve from two points, but considering two other 'deformation' vectors </summary>
//            /// <param name="controlPts"> Array of 4 Vector3 elements. For using this, we must have 4 elements and a cubic interpolation. </param>
//            /// <returns> Bezier curve </returns>
//            public Vector3[] MakeBezier(Vector3 start, Vector3 startM, Vector3 end, Vector3 endM, Vector3[] localAxes)
//            {
//                int i, j;

//                // Get local axes
//                if (localAxes != null)
//                    axes = localAxes;
//                else
//                    axes = computeLocalAxes(start, end);

//                // Get the direction at start point
//                startRot = Vector3.Cross(startM, axes[0]);

//                // Get the direction at end point
//                endRot = Vector3.Cross(endM, -axes[0]);

//                // Get deformed line direction...
//                dir = end - start;

//                // Control points - Start at the begining of the segment
//                controlPoints[0] = start;

//                // Compute the second control point at 1/4 of the line segment and oriented according to the line local axes
//                controlPoints[1] = start + 1.0f / 16.0f * dir + startRot;

//                // Compute the third control point at 3/4 of the line segment and oriented according to the line local axes
//                controlPoints[2] = start + 15.0f / 16.0f * dir + endRot;

//                // Compute the last control point, it is simply the point at the end of the segment
//                controlPoints[3] = end;

//                // Each point/derivative is computed as an MADD operation
//                for (i = 0; i < nSegments; ++i)
//                {
//                    bezierPoints[i] = Vector3.Empty;
//                    bezierDerivative[i] = Vector3.Empty;

//                    for (j = 0; j < nControlPoints; ++j)
//                    {
//                        bezierPoints[i] += bezierEvaluator[j, i] * controlPoints[j];

//                        bezierDerivative[i] += bezierDerivativeEvaluator[j, i] * controlPoints[j];
//                    }

//                    // We need the derivative as normalized vectors...
//                    bezierDerivative[i].Normalize();
//                    // Safe check: very little numbers can introduce errors, make them zeros
//                    checkIntegrity(ref bezierDerivative[i]);
//                }

//                return bezierPoints;
//            }

//            public Vector3[] MakeCubic(Vector3 start, Vector3 startRots, Vector3 end, Vector3 endRots, Vector3 undeformedDir)
//            {
//                int i, j;

//                // Get deformed line direction...
//                dir = end - start;

//                // Get the direction at start point
//                startRot = Vector3.Normalize(undeformedDir);
//                startRot.TransformCoordinate(Matrix.RotationX((float)Math.Atan(startRots.X)));
//                startRot.TransformCoordinate(Matrix.RotationY((float)Math.Atan(startRots.Y)));
//                startRot.TransformCoordinate(Matrix.RotationZ((float)Math.Atan(startRots.Z)));
//                //startRot.Scale(0.5f);

//                startRot = 7.0f / (32.0f * Math.Abs(Vector3.Dot(undeformedDir, Vector3.Normalize(dir)))) * startRot;


//                // Get the direction at end point
//                endRot = Vector3.Normalize(-undeformedDir);
//                endRot.TransformCoordinate(Matrix.RotationX((float)Math.Atan(endRots.X)));
//                endRot.TransformCoordinate(Matrix.RotationY((float)Math.Atan(endRots.Y)));
//                endRot.TransformCoordinate(Matrix.RotationZ((float)Math.Atan(endRots.Z)));
//                //endRot *= -1.0f;
//                //endRot.Scale(0.5f);

//                endRot = 7.0f / (32.0f * Math.Abs(Vector3.Dot(undeformedDir, Vector3.Normalize(dir)))) * endRot;

//                // Control points - Start at the begining of the segment
//                controlPoints[0] = start;

//                // Compute the second control point at 1/4 of the line segment and oriented according to the line local axes
//                controlPoints[1] = start + startRot;

//                // Compute the third control point at 3/4 of the line segment and oriented according to the line local axes
//                controlPoints[2] = end + endRot;

//                // Compute the last control point, it is simply the point at the end of the segment
//                controlPoints[3] = end;

//                // Each point/derivative is computed as an MADD operation
//                for (i = 0; i < nSegments; ++i)
//                {
//                    bezierPoints[i] = Vector3.Empty;
//                    bezierDerivative[i] = Vector3.Empty;

//                    for (j = 0; j < nControlPoints; ++j)
//                    {
//                        bezierPoints[i] += bezierEvaluator[j, i] * controlPoints[j];

//                        bezierDerivative[i] += bezierDerivativeEvaluator[j, i] * controlPoints[j];
//                    }

//                    // We need the derivative as normalized vectors...
//                    bezierDerivative[i].Normalize();
//                    // Safe check: very little numbers can introduce errors, make them zeros
//                    checkIntegrity(ref bezierDerivative[i]);
//                }

//                return bezierPoints;
//            }
//        }
//        #endregion

//        #region Extrusion from Bezier and other "axis" curves
//        public static class Extrusion
//        {
//            #region Counters and other variables
//            private static Vector3 k = new Vector3(0, 0, 1);
//            //private static Vector3[] axes = null;
//            private static Vector3[] currAxes = null;
//            private static Vector3[] nextAxes = null;
//            private static Vector3 lineDir;

//            private static int nSegments;
//            private static int nShapeVertices;

//            private static int meshVertex;
//            //private static int index;
//            //private static int baseIndex;
//            private static int segment;
//            private static int vertex;

//            //private static int shapeVertex;

//            private static Vector3 temp1 = Vector3.Empty;
//            private static Vector3 temp2 = Vector3.Empty;
//            private static Vector3 temp3 = Vector3.Empty;
//            private static Vector3 currDir = Vector3.Empty;

//            private static int nMeshVertices;
//            //private static int indexBufferSize;
//            //private static int theIndex;
//            #endregion

//            /// <summary> Builds an extrusion from the shape along a curve </summary>
//            /// <param name="shape"> An array indicating the shape points ordered counter_clockwise. First index is for vertex and second for normals. </param>
//            /// <param name="axis"> The curve that must be followed. Array of vertixes. </param>
//            /// <param name="axisDerivative"> The derivative at each vertex in the curve. </param>
//            /// <param name="extrudedMesh"> A ref variable containing the extruded mesh. First index refers to vertices and the second to normals. </param>
//            /// <param name="meshIB"> A ref variable containing the union rules for the extruded mesh (index buffer) </param>
//            //public static void Build(Vector2[][] shape, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref int[] meshIB)
//            //{
//            //    // Get the number of segments in the curve
//            //    nSegments = axis.GetLength(0);
//            //    // Get the number of vertices in the shape
//            //    nShapeVertices = shape[0].GetLength(0);
//            //    // Initialize some counters
//            //    meshVertex = 0;
//            //    index = 0;

//            //    // How many vertices in the mesh are we going to have?
//            //    nMeshVertices = nShapeVertices * nSegments;

//            //    // Determine the number of indices for mesh tesellation
//            //    indexBufferSize = (nShapeVertices * 2) * (nSegments - 1) * 3;  // nMeshVertices * 2 = Number of faces per segment

//            //    // Just get memory when necessary
//            //    if (extrudedMesh == null || nMeshVertices != extrudedMesh.GetLength(1))
//            //    {
//            //        extrudedMesh = new Vector3[2, nMeshVertices];
//            //        meshIB = new int[indexBufferSize];
//            //    }

//            //    for (segment = 0; segment < nSegments; ++segment)
//            //    {
//            //        // Compute the plane at the current point in the curve
//            //        if (axisDerivative != null && axisDerivative[segment] != Vector3.Empty)
//            //            lineDir = axisDerivative[segment];
//            //        // When no derivatives are computed, use instead the line direction
//            //        else
//            //        {
//            //            if (segment == nSegments - 1)
//            //                lineDir = axis[nSegments - 1] - axis[nSegments - 2];
//            //            else
//            //                lineDir = axis[segment + 1] - axis[segment];

//            //            lineDir.Normalize();
//            //        }

//            //        // Check if we have a 'good' value
//            //        checkIntegrity(ref lineDir);

//            //        // and compute the local axes at the current curve point
//            //        axes = computeLocalSystem(lineDir);

//            //        // Fill vertex array
//            //        for (vertex = 0; vertex < nShapeVertices; ++vertex, ++meshVertex)
//            //        {
//            //            // Mesh vertices
//            //            extrudedMesh[0, meshVertex] = axis[segment] + shape[0][vertex].X * axes[0] + shape[0][vertex].Y * axes[1];
//            //            // Mesh normals
//            //            extrudedMesh[1, meshVertex] = shape[1][vertex].X * axes[0] + shape[1][vertex].Y * axes[1];
//            //        }
//            //    }

//            //    // Index buffer
//            //    for (segment = 0; segment < nSegments - 1; ++segment)
//            //    {
//            //        baseIndex = segment * nShapeVertices;
//            //        for (shapeVertex = 0; shapeVertex < nShapeVertices - 1; ++shapeVertex)
//            //        {
//            //            // First triangle
//            //            meshIB[index++] = baseIndex + shapeVertex;
//            //            meshIB[index++] = baseIndex + shapeVertex + nShapeVertices;
//            //            meshIB[index++] = baseIndex + shapeVertex + 1;
//            //            // Second triangle
//            //            meshIB[index++] = baseIndex + shapeVertex + 1;
//            //            meshIB[index++] = baseIndex + shapeVertex + nShapeVertices;
//            //            meshIB[index++] = baseIndex + shapeVertex + nShapeVertices + 1;

//            //        }
//            //        // Last face must be conformed by the first and last vertices pairs
//            //        // First triangle
//            //        meshIB[index++] = baseIndex + nShapeVertices - 1;
//            //        meshIB[index++] = baseIndex + 2 * nShapeVertices - 1;
//            //        meshIB[index++] = baseIndex;
//            //        // Second triangle
//            //        meshIB[index++] = baseIndex;
//            //        meshIB[index++] = baseIndex + 2 * nShapeVertices - 1;
//            //        meshIB[index++] = baseIndex + nShapeVertices;
//            //    }
//            //}

//            /// <summary> Builds a local system on the line </summary>
//            /// <param name="line"> This is the direction that must be followed </param>
//            /// <returns> Local system </returns>
//            private static Vector3[] computeLocalSystem(Vector3 line, Vector3[] origAxes)
//            {
//                float zn;
//                // Cortesia del hippie-comeflores y reconfigurada para la extrusion
//                Vector3[] localAxes = new Vector3[3];

//                // Longitudinal axis
//                localAxes[2] = Vector3.Normalize(line);

//                // Check if the vector and the beam axes try to follow Z axis
//                zn = Vector3.Dot(localAxes[2], origAxes[2]);

//                // Calculate projection of Z and n. Try to find local 2 axis following Z
//                //zn = Vector3.Dot(k, localAxes[2]);

//                // Axis parallel to Z (Dot product near maximum value equals parallelism)
//                if (Math.Abs(zn) > 0.999999)
//                {
//                    // Local Axis 2 = X and 3 = +/- Y
//                    localAxes[0] = origAxes[0];
//                    localAxes[1] = origAxes[1];
//                    localAxes[2] = origAxes[2];
//                }
//                else
//                {
//                    // Local 2 equals the difference between the projected Z at n and Z
//                    localAxes[0] = Vector3.Normalize(origAxes[2] - Vector3.Scale(localAxes[2], zn));
//                    localAxes[1] = Vector3.Cross(localAxes[0], localAxes[2]);
//                }

//                return localAxes;
//            }

//            /// <summary> Builds an extrusion from the shape along a curve </summary>
//            /// <param name="shape"> An array indicating the shape points ordered counter_clockwise. First index is for vertex and second for normals. </param>
//            /// <param name="axis"> The curve that must be followed. Array of vertixes. </param>
//            /// <param name="axisDerivative"> The derivative at each vertex in the curve. </param>
//            /// <param name="extrudedMesh"> A ref variable containing the extruded mesh. First index refers to vertices and the second to normals. </param>
//            public static void Build(Vector2[][] shape, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref int[] extrudedIndices, bool faceNormals, short[] triangIndices, Vector3[] origAxes)
//            {
//                // Get the number of segments in the curve
//                nSegments = axis.GetLength(0);
//                // Get the number of vertices in the shape
//                nShapeVertices = shape[0].GetLength(0);
//                // Initialize some counters
//                meshVertex = 0;
//                //index = 0;

//                // How many vertices in the mesh are we going to have?
//                int nTriangIndices = triangIndices.Length;
//                nMeshVertices = nShapeVertices * (nSegments - 1) * 6 + 2 * nTriangIndices; // 6 vertices per face

//                // Just get memory when necessary (vectors)
//                if (extrudedMesh == null || nMeshVertices != extrudedMesh.GetLength(1))
//                    extrudedMesh = new Vector3[2, nMeshVertices];
//                // Just get memory when necessary (colors)
//                if (extrudedIndices == null || nMeshVertices != extrudedIndices.GetLength(0))
//                    extrudedIndices = new int[nMeshVertices];

//                for (segment = 0; segment < nSegments - 1; ++segment)
//                {
//                    // Compute the plane at the current point in the curve
//                    if (axisDerivative != null && axisDerivative[segment] != Vector3.Empty)
//                        lineDir = axisDerivative[segment];
//                    // When no derivatives are computed, use instead the line direction
//                    else
//                    {
//                        if (segment == nSegments - 1)
//                            lineDir = axis[nSegments - 1] - axis[nSegments - 2];
//                        else
//                            lineDir = axis[segment + 1] - axis[segment];

//                        lineDir.Normalize();
//                    }

//                    // Check if we have a 'good' value
//                    checkIntegrity(ref lineDir);

//                    // and compute the local axes at the current curve point
//                    currAxes = computeLocalSystem(lineDir, origAxes);

//                    currDir = lineDir;

//                    if (segment > 0)
//                    {
//                        temp1 = nextAxes[0];
//                        temp2 = nextAxes[1];
//                        temp3 = nextAxes[2];
//                    }

//                    int auxSegment = segment + 1;
//                    // Compute the plane at the next point in the curve
//                    if (axisDerivative != null && axisDerivative[auxSegment] != Vector3.Empty)
//                        lineDir = axisDerivative[auxSegment];
//                    // When no derivatives are computed, use instead the line direction
//                    else
//                    {
//                        if (auxSegment == nSegments - 1)
//                            lineDir = axis[nSegments - 1] - axis[nSegments - 2];
//                        else
//                            lineDir = axis[auxSegment + 1] - axis[auxSegment];

//                        lineDir.Normalize();
//                    }

//                    lineDir += currDir;
//                    lineDir.Normalize();

//                    // Check if we have a 'good' value
//                    checkIntegrity(ref lineDir);
//                    nextAxes = computeLocalSystem(lineDir, origAxes);

//                    if (segment > 0)
//                    {
//                        currAxes[0] = temp1;
//                        currAxes[1] = temp2;
//                        currAxes[2] = temp3;
//                    }

//                    // Make start triangulated shape
//                    if (segment == 0)
//                    {
//                        for (int i = 0; i < nTriangIndices; ++i)
//                        {
//                            extrudedMesh[0, meshVertex] = axis[segment] + shape[0][triangIndices[i]].X * currAxes[0] + shape[0][triangIndices[i]].Y * currAxes[1];
//                            extrudedMesh[1, meshVertex] = -currAxes[2];
//                            extrudedIndices[meshVertex] = triangIndices[i];
//                            ++meshVertex;
//                        }
//                    }
//                    // Make end triangulated shape
//                    else if (auxSegment == nSegments - 1)
//                    {
//                        for (int i = nTriangIndices - 1; i >= 0; --i)
//                        {
//                            extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][triangIndices[i]].X * nextAxes[0] + shape[0][triangIndices[i]].Y * nextAxes[1];
//                            extrudedMesh[1, meshVertex] = nextAxes[2];
//                            extrudedIndices[meshVertex] = nShapeVertices * segment + triangIndices[i];
//                            ++meshVertex;
//                        }
//                    }

//                    // Fill vertex array
//                    for (vertex = 0; vertex < nShapeVertices; ++vertex)
//                    {
//                        // Mesh vertices - First triangle
//                        extrudedMesh[0, meshVertex] = axis[segment] + shape[0][vertex].X * currAxes[0] + shape[0][vertex].Y * currAxes[1];
//                        extrudedMesh[1, meshVertex] = shape[1][vertex].X * currAxes[0] + shape[1][vertex].Y * currAxes[1];
//                        extrudedIndices[meshVertex] = nShapeVertices * segment + vertex;
//                        ++meshVertex;

//                        extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][vertex].X * nextAxes[0] + shape[0][vertex].Y * nextAxes[1];
//                        extrudedMesh[1, meshVertex] = shape[1][vertex].X * nextAxes[0] + shape[1][vertex].Y * nextAxes[1];
//                        extrudedIndices[meshVertex] = nShapeVertices * auxSegment + vertex;
//                        ++meshVertex;

//                        if (vertex + 1 < nShapeVertices)
//                        {
//                            extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][vertex + 1].X * nextAxes[0] + shape[0][vertex + 1].Y * nextAxes[1];
//                            extrudedIndices[meshVertex] = nShapeVertices * auxSegment + vertex + 1;
//                            if (faceNormals)
//                                extrudedMesh[1, meshVertex] = shape[1][vertex].X * nextAxes[0] + shape[1][vertex].Y * nextAxes[1];
//                            else
//                                extrudedMesh[1, meshVertex] = shape[1][vertex + 1].X * nextAxes[0] + shape[1][vertex + 1].Y * nextAxes[1];
//                        }
//                        else
//                        {
//                            extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][0].X * nextAxes[0] + shape[0][0].Y * nextAxes[1];
//                            extrudedIndices[meshVertex] = nShapeVertices * auxSegment;
//                            if (faceNormals)
//                                extrudedMesh[1, meshVertex] = shape[1][nShapeVertices - 1].X * nextAxes[0] + shape[1][nShapeVertices - 1].Y * nextAxes[1];
//                            else
//                                extrudedMesh[1, meshVertex] = shape[1][0].X * nextAxes[0] + shape[1][0].Y * nextAxes[1];
//                        }
//                        ++meshVertex;

//                        // Mesh vertices - Second triangle
//                        extrudedMesh[0, meshVertex] = extrudedMesh[0, meshVertex - 3];
//                        extrudedMesh[1, meshVertex] = extrudedMesh[1, meshVertex - 3];
//                        extrudedIndices[meshVertex] = extrudedIndices[meshVertex - 3];
//                        ++meshVertex;

//                        extrudedMesh[0, meshVertex] = extrudedMesh[0, meshVertex - 2];
//                        extrudedMesh[1, meshVertex] = extrudedMesh[1, meshVertex - 2];
//                        extrudedIndices[meshVertex] = extrudedIndices[meshVertex - 2];
//                        ++meshVertex;

//                        if (vertex + 1 < nShapeVertices)
//                        {
//                            extrudedMesh[0, meshVertex] = axis[segment] + shape[0][vertex + 1].X * currAxes[0] + shape[0][vertex + 1].Y * currAxes[1];
//                            extrudedIndices[meshVertex] = nShapeVertices * segment + vertex + 1;
//                            if (faceNormals)
//                                extrudedMesh[1, meshVertex] = shape[1][vertex].X * currAxes[0] + shape[1][vertex].Y * currAxes[1];
//                            else
//                                extrudedMesh[1, meshVertex] = shape[1][vertex + 1].X * currAxes[0] + shape[1][vertex + 1].Y * currAxes[1];
//                        }
//                        else
//                        {
//                            extrudedMesh[0, meshVertex] = axis[segment] + shape[0][0].X * currAxes[0] + shape[0][0].Y * currAxes[1];
//                            extrudedIndices[meshVertex] = nShapeVertices * segment;
//                            if (faceNormals)
//                                extrudedMesh[1, meshVertex] = shape[1][nShapeVertices - 1].X * currAxes[0] + shape[1][nShapeVertices - 1].Y * currAxes[1];
//                            else
//                                extrudedMesh[1, meshVertex] = shape[1][0].X * currAxes[0] + shape[1][0].Y * currAxes[1];
//                        }
//                        ++meshVertex;
//                    }
//                }
//            }

//            /// <summary> Builds an extrusion from the shape along a curve </summary>
//            /// <param name="shape"> An array indicating the shape points ordered counter_clockwise. First index is for vertex and second for normals. </param>
//            /// <param name="axis"> The curve that must be followed. Array of vertixes. </param>
//            /// <param name="axisDerivative"> The derivative at each vertex in the curve. </param>
//            /// <param name="extrudedMesh"> A ref variable containing the extruded mesh. First index refers to vertices and the second to normals. </param>
//            /// <param name="meshIB"> A ref variable containing the union rules for the extruded mesh (index buffer) </param>
//            /// <param name="vbBaseIndex"> An index telling the offset for this new set of indices in the vertexBuffer </param>
//            //public static void Build(Vector2[][] shape, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref int[] meshIB, int vbBaseIndex)
//            //{
//            //    // Get the number of segments in the curve
//            //    nSegments = axis.GetLength(0);
//            //    // Get the number of vertices in the shape
//            //    nShapeVertices = shape[0].GetLength(0);
//            //    // Initialize some counters
//            //    meshVertex = 0;
//            //    index = 0;

//            //    // How many vertices in the mesh are we going to have?
//            //    nMeshVertices = nShapeVertices * nSegments;

//            //    // Determine the number of indices for mesh tesellation
//            //    indexBufferSize = (nShapeVertices * 2) * (nSegments - 1) * 3;  // nMeshVertices * 2 = Number of faces per segment

//            //    // Just get memory when necessary
//            //    if (extrudedMesh == null || nMeshVertices != extrudedMesh.GetLength(1))
//            //    {
//            //        extrudedMesh = new Vector3[2, nMeshVertices];
//            //        meshIB = new int[indexBufferSize];
//            //    }

//            //    for (segment = 0; segment < nSegments; ++segment)
//            //    {
//            //        // Compute the plane at the current point in the curve
//            //        if (axisDerivative != null && axisDerivative[segment] != Vector3.Empty)
//            //            lineDir = axisDerivative[segment];
//            //        // When no derivatives are computed, use instead the line direction
//            //        else
//            //        {
//            //            if (segment == nSegments - 1)
//            //                lineDir = axis[nSegments - 1] - axis[nSegments - 2];
//            //            else
//            //                lineDir = axis[segment + 1] - axis[segment];

//            //            lineDir.Normalize();
//            //        }

//            //        // Check if we have a 'good' value
//            //        checkIntegrity(ref lineDir);

//            //        // and compute the local axes at the current curve point
//            //        axes = computeLocalSystem(lineDir);

//            //        // Fill vertex array
//            //        for (vertex = 0; vertex < nShapeVertices; ++vertex, ++meshVertex)
//            //        {
//            //            // Mesh vertices
//            //            extrudedMesh[0, meshVertex] = axis[segment] + shape[0][vertex].X * axes[0] + shape[0][vertex].Y * axes[1];
//            //            // Mesh normals
//            //            extrudedMesh[1, meshVertex] = shape[1][vertex].X * axes[0] + shape[1][vertex].Y * axes[1];
//            //        }
//            //    }

//            //    // Index buffer
//            //    for (segment = 0; segment < nSegments - 1; ++segment)
//            //    {
//            //        baseIndex = vbBaseIndex + segment * nShapeVertices;
//            //        for (shapeVertex = 0; shapeVertex < nShapeVertices - 1; ++shapeVertex)
//            //        {
//            //            theIndex = baseIndex + shapeVertex;
//            //            // First triangle
//            //            meshIB[index++] = theIndex;
//            //            meshIB[index++] = theIndex + nShapeVertices;
//            //            meshIB[index++] = theIndex + 1;
//            //            // Second triangle
//            //            meshIB[index++] = theIndex + 1;
//            //            meshIB[index++] = theIndex + nShapeVertices;
//            //            meshIB[index++] = theIndex + nShapeVertices + 1;

//            //        }
//            //        // Last face must be conformed by the first and last vertices pairs
//            //        // First triangle
//            //        meshIB[index++] = baseIndex + nShapeVertices - 1;
//            //        meshIB[index++] = baseIndex + 2 * nShapeVertices - 1;
//            //        meshIB[index++] = baseIndex;
//            //        // Second triangle
//            //        meshIB[index++] = baseIndex;
//            //        meshIB[index++] = baseIndex + 2 * nShapeVertices - 1;
//            //        meshIB[index++] = baseIndex + nShapeVertices;
//            //    }
//            //}
//        }
//        #endregion

//        #region Utility
//        /// <summary> Checks if this vector has 'good' values, i.e. components are not very little values. </summary>
//        /// <param name="vector"> Vector to check. Must be a normalized vector. </param>
//        public static void checkIntegrity(ref Vector3 vector)
//        {
//            float threshold = 0.000001f;
//            bool mustRenormalize = false;

//            if (Math.Abs(vector.X) < threshold)
//            {
//                vector.X = 0;
//                mustRenormalize = true;
//            }
//            if (Math.Abs(vector.Y) < threshold)
//            {
//                vector.Y = 0;
//                mustRenormalize = true;
//            }
//            if (Math.Abs(vector.Z) < threshold)
//            {
//                vector.Z = 0;
//                mustRenormalize = true;
//            }

//            if (mustRenormalize)
//                vector.Normalize();

//            if (Math.Abs(vector.X) > 0.999999)
//                vector.X = Math.Sign(vector.X);

//            if (Math.Abs(vector.Y) > 0.999999)
//                vector.Y = Math.Sign(vector.Y);

//            if (Math.Abs(vector.Z) > 0.999999)
//                vector.Z = Math.Sign(vector.Z);
//        }
//        #endregion

//        #region Callers for building a curved axis. Could be using a Bezier curve or a just a cubic poly
//        /// <summary> Makes a curved axis from a set of two endpoints and two "rotations" </summary>
//        /// <param name="start"> Line start point </param>
//        /// <param name="startM"> Start "rotation vector" </param>
//        /// <param name="end"> Line end point </param>
//        /// <param name="endM"> End "rotation vector"  </param>
//        /// <returns> A Vector3 array holding the curve </returns>
//        public Vector3[] MakeExtrusionAxis(Vector3 start, Vector3 startM, Vector3 end, Vector3 endM)
//        {
//            return curvedAxis.MakeBezier(start, startM, end, endM);
//        }

//        /// <summary> Makes a curved axis from a set of two endpoints and two "rotations" and the line local axes </summary>
//        /// <param name="start"> Line start point </param>
//        /// <param name="startM"> Start "rotation vector" </param>
//        /// <param name="end"> Line end point </param>
//        /// <param name="endM"> End "rotation vector"  </param>
//        /// <param name="localAxes"></param>
//        /// <returns> A Vector3 array holding the curve </returns>
//        public Vector3[] MakeExtrusionAxis(Vector3 start, Vector3 startM, Vector3 end, Vector3 endM, Vector3[] localAxes)
//        {
//            return curvedAxis.MakeBezier(start, startM, end, endM, localAxes);
//        }

//        /// <summary> Makes a curved axis from a set of two endpoints and two "rotations" and the line direction </summary>
//        /// <param name="start"> Line start point </param>
//        /// <param name="startM"> Start "rotation vector" </param>
//        /// <param name="end"> Line end point </param>
//        /// <param name="endM"> End "rotation vector"  </param>
//        /// <param name="localAxes"></param>
//        /// <returns> A Vector3 array holding the curve </returns>
//        public Vector3[] MakeExtrusionAxis(Vector3 start, Vector3 startRots, Vector3 end, Vector3 endRots, Vector3 undeformedDir)
//        {
//            return curvedAxis.MakeCubic(start, startRots, end, endRots, undeformedDir);
//        }
//        #endregion

//        #region Callers for building the extruded mesh
//        //public void BuildMesh(Vector2[][] shape, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref int[] meshIB)
//        //{
//        //    Extrusion.Build(shape, axis, axisDerivative, ref extrudedMesh, ref meshIB);
//        //}

//        //public void BuildMesh(Vector2[][] shape, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref int[] meshIB, int vbBaseIndex)
//        //{
//        //    Extrusion.Build(shape, axis, axisDerivative, ref extrudedMesh, ref meshIB, vbBaseIndex);
//        //}

//        //public void BuildMesh(Vector2[][] shape, Vector3[] axis, ref Vector3[,] extrudedMesh, ref int[] meshIB)
//        //{
//        //    Extrusion.Build(shape, axis, null, ref extrudedMesh, ref meshIB);
//        //}

//        //public void BuildMesh(Vector2[][] shape, Vector3[] axis, ref Vector3[,] extrudedMesh, ref int[] meshIB, int vbBaseIndex)
//        //{
//        //    Extrusion.Build(shape, axis, null, ref extrudedMesh, ref meshIB, vbBaseIndex);
//        //}

//        public void BuildMesh(Vector2[][] shape, Vector3[] axis, Vector3[] axisDerivative, ref Vector3[,] extrudedMesh, ref int[] extrudedIndices, bool faceNormals, short[] triangIndices, Vector3[] origAxes)
//        {
//            Extrusion.Build(shape, axis, axisDerivative, ref extrudedMesh, ref extrudedIndices, faceNormals, triangIndices, origAxes);
//        }

//        public void BuildMesh(Vector2[][] shape, Vector3[] axis, ref Vector3[,] extrudedMesh, ref int[] extrudedIndices, bool faceNormals, short[] triangIndices, Vector3[] origAxes)
//        {
//            Extrusion.Build(shape, axis, null, ref extrudedMesh, ref extrudedIndices, faceNormals, triangIndices, origAxes);
//        }
//        #endregion
//    }
//}
#endregion

#region Another way...
/*public Vector3[] MakeBezier(Vector3 start, Vector3 startRots, Vector3 end, Vector3 endRots, Vector3[] localAxes)
{
    int i, j;

    // Get deformed line direction...
    dir = end - start;
    axes = computeLocalAxes(start, end);

    float angle1 = (float)Math.Atan(startRots.X) - (float)Math.Acos(Vector3.Dot(axes[1], localAxes[1]));
    float angle2 = (float)Math.Atan(startRots.Y) - (float)Math.Acos(Vector3.Dot(axes[2], localAxes[2]));
    float angle3 = (float)Math.Atan(startRots.Z) - (float)Math.Acos(Vector3.Dot(axes[0], localAxes[0]));

    // Get the direction at start point
    startRot = Vector3.Normalize(dir);
    startRot.TransformCoordinate(Matrix.RotationAxis(axes[0], angle1));
    startRot.TransformCoordinate(Matrix.RotationAxis(axes[1], angle2));
    startRot.TransformCoordinate(Matrix.RotationAxis(axes[2], angle3));
    startRot.Scale(0.5f);

    angle1 = (float)Math.Atan(endRots.X) - (float)Math.Acos(Vector3.Dot(axes[1], localAxes[1]));
    angle2 = (float)Math.Atan(endRots.Y) - (float)Math.Acos(Vector3.Dot(axes[2], localAxes[2]));
    angle3 = (float)Math.Atan(endRots.Z) - (float)Math.Acos(Vector3.Dot(axes[0], localAxes[0]));

    // Get the direction at end point
    endRot = Vector3.Normalize(-dir);
    endRot.TransformCoordinate(Matrix.RotationAxis(axes[0], angle1));
    endRot.TransformCoordinate(Matrix.RotationAxis(axes[1], angle2));
    endRot.TransformCoordinate(Matrix.RotationAxis(axes[2], angle3));
    //endRot *= -1.0f;
    endRot.Scale(0.5f);

    // Control points - Start at the begining of the segment
    controlPoints[0] = start;

    // Compute the second control point at 1/4 of the line segment and oriented according to the line local axes
    controlPoints[1] = start + 1.0f / 32.0f * dir + startRot;

    // Compute the third control point at 3/4 of the line segment and oriented according to the line local axes
    controlPoints[2] = start + 31.0f / 32.0f * dir + endRot;

    // Compute the last control point, it is simply the point at the end of the segment
    controlPoints[3] = end;

    // Each point/derivative is computed as an MADD operation
    for (i = 0; i < nSegments; ++i)
    {
        bezierPoints[i] = Vector3.Empty;
        bezierDerivative[i] = Vector3.Empty;

        for (j = 0; j < nControlPoints; ++j)
        {
            bezierPoints[i] += bezierEvaluator[j, i] * controlPoints[j];

            bezierDerivative[i] += bezierDerivativeEvaluator[j, i] * controlPoints[j];
        }

        // We need the derivative as normalized vectors...
        bezierDerivative[i].Normalize();
        // Safe check: very little numbers can introduce errors, make them zeros
        checkIntegrity(ref bezierDerivative[i]);
    }

    return bezierPoints;
}*/
#endregion

#region Curved normals
/*for (vertex = 0; vertex < nShapeVertices; ++vertex)
{
    // Mesh vertices - First triangle
    extrudedMesh[0, meshVertex] = axis[segment] + shape[0][vertex].X * currAxes[0] + shape[0][vertex].Y * currAxes[1];
    extrudedMesh[1, meshVertex] = shape[1][vertex].X * currAxes[0] + shape[1][vertex].Y * currAxes[1];
    ++meshVertex;

    extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][vertex].X * nextAxes[0] + shape[0][vertex].Y * nextAxes[1];
    extrudedMesh[1, meshVertex] = shape[1][vertex].X * nextAxes[0] + shape[1][vertex].Y * nextAxes[1];
    ++meshVertex;

    if (vertex + 1 < nShapeVertices)
    {
        extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][vertex + 1].X * nextAxes[0] + shape[0][vertex + 1].Y * nextAxes[1];
        extrudedMesh[1, meshVertex] = shape[1][vertex + 1].X * nextAxes[0] + shape[1][vertex + 1].Y * nextAxes[1];
    }
    else
    {
        extrudedMesh[0, meshVertex] = axis[auxSegment] + shape[0][0].X * nextAxes[0] + shape[0][0].Y * nextAxes[1];
        extrudedMesh[1, meshVertex] = shape[1][0].X * nextAxes[0] + shape[1][0].Y * nextAxes[1];
    }
    ++meshVertex;

    // Mesh vertices - Second triangle
    extrudedMesh[0, meshVertex] = extrudedMesh[0, meshVertex - 3];
    extrudedMesh[1, meshVertex] = extrudedMesh[1, meshVertex - 3];
    ++meshVertex;

    extrudedMesh[0, meshVertex] = extrudedMesh[0, meshVertex - 2];
    extrudedMesh[1, meshVertex] = extrudedMesh[1, meshVertex - 2];
    ++meshVertex;

    if (vertex + 1 < nShapeVertices)
    {
        extrudedMesh[0, meshVertex] = axis[segment] + shape[0][vertex + 1].X * currAxes[0] + shape[0][vertex + 1].Y * currAxes[1];
        extrudedMesh[1, meshVertex] = shape[1][vertex + 1].X * currAxes[0] + shape[1][vertex + 1].Y * currAxes[1];
    }
    else
    {
        extrudedMesh[0, meshVertex] = axis[segment] + shape[0][0].X * currAxes[0] + shape[0][0].Y * currAxes[1];
        extrudedMesh[1, meshVertex] = shape[1][0].X * currAxes[0] + shape[1][0].Y * currAxes[1];
    }
    ++meshVertex;
}*/
#endregion