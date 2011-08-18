using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Utility;

namespace Canguro.View.Renderer
{
    /// <summary>
    /// Class for drawing areas in wireframe mode. Must be changed
    /// </summary>
    public class WireframeAreaRenderer : AreaRenderer
    {

        public WireframeAreaRenderer()
        {
            verticesNeeded4Quads = 8;
            verticesNeeded4Triangles = 6;
        }

        /// <summary>
        /// Main method for area rendering in wireframe mode
        /// </summary>
        /// <param name="device"> DirectX active device </param>
        /// <param name="area"> The area to draw </param>
        /// <param name="options"> Render options: Ids, properties, etc. </param>
        public override void Render(Device device, AreaElement area, RenderOptions options)
        {
            throw new System.NotImplementedException();
        }

        public override void Render(Device device, Model.Model model, IEnumerable<AreaElement> areas, RenderOptions options, List<Item> itemsInView)
        {
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int vertices = 0;

            rc.ActiveStream = ResourceStreamType.Lines;

            device.RenderState.Lighting = false;

            PositionColoredPackage package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);

            Vector3 zeroVec = Vector3.Empty;
            Vector3 distVec = new Vector3(5, 0, 0);

            GraphicViewManager.Instance.ActiveView.Unproject(ref zeroVec);
            GraphicViewManager.Instance.ActiveView.Unproject(ref distVec);

            pixelDistance = Vector3.Length(distVec - zeroVec);

            if (areas != null && ((IList<AreaElement>)areas).Count > 0)
            {
                foreach (AreaElement area in areas)
                {
                    drawWireframeArea(rc, area, pickingMode, ref package, ref vertices, options);
                }
            }
            else if (itemsInView != null)
            {
                if (itemsInView.Count <= 0)
                    GetItemsInView(itemsInView);

                if (itemsInView.Count > 0)
                {
                    AreaElement area;

                    foreach (Item item in itemsInView)
                    {
                        area = item as AreaElement;
                        drawWireframeArea(rc, area, pickingMode, ref package, ref vertices, options);
                    }
                }
            }

            rc.ReleaseBuffer(vertices, 0, ResourceStreamType.Lines);
            rc.Flush(ResourceStreamType.Lines);
        }

        private void drawWireframeArea(ResourceManager rc, AreaElement area, bool pickingMode, ref PositionColoredPackage package, ref int vertices, RenderOptions options)
        {
            if (area == null || !area.IsVisible) return;

            Vector3[] localAxes = area.LocalAxes;
            List<Vector3> areaVertices = new List<Vector3>();
            List<float> areaVertexOffsets = new List<float>();
            List<int> indices = new List<int>();

            int lineColor = System.Drawing.Color.Red.ToArgb();
            int requiredVertices = 0;

            bool concave = false;

            // Put joint positions 
            requiredVertices = configureVertexList(area, localAxes, areaVertices, areaVertexOffsets, indices, ref concave);

            int length = areaVertices.Count;
            
            Vector3[] shellNormals = new Vector3[2];

            shellNormals[0] = Vector3.Normalize(Vector3.Cross(areaVertices[indices[0]] - areaVertices[indices[2]] +
                                                             (areaVertexOffsets[indices[0]] - areaVertexOffsets[indices[2]]) * localAxes[2],
                                                              areaVertices[indices[1]] - areaVertices[indices[0]] +
                                                             (areaVertexOffsets[indices[1]] - areaVertexOffsets[indices[0]]) * localAxes[2]));

            if (length > 3)
                shellNormals[1] = Vector3.Normalize(Vector3.Cross(areaVertices[indices[3]] - areaVertices[indices[5]] +
                                                                 (areaVertexOffsets[indices[3]] - areaVertexOffsets[indices[5]]) * localAxes[2],
                                                                  areaVertices[indices[4]] - areaVertices[indices[3]] +
                                                                 (areaVertexOffsets[indices[4]] - areaVertexOffsets[indices[3]]) * localAxes[2]));


            // Add four vertices when Diagonals rendering is required
            //if ((options.OptionsShown & RenderOptions.ShowOptions.ShellDiagonals) > 0 && length > 3)
            if (length > 3)
                requiredVertices += 6;

            // Check if package still have space in it
            if (vertices + requiredVertices >= package.NumVertices - 1)
            {
                rc.ReleaseBuffer(vertices, 0, package.Stream);
                package = (PositionColoredPackage)rc.CaptureBuffer(package.Stream, 0, requiredVertices, true);
                vertices = 0;
            }
            vertices += requiredVertices;

            // Inicia código NO SEGURO
            unsafe
            {
                #region Traditional filled contour
                //for (int i = 0; i < length; ++i)
                //{
                //    package.VBPointer->Position = areaVertices[i] + areaVertexOffsets[i] * localAxes[2];
                //    package.VBPointer->Color = lineColor;
                //    package.VBPointer++;

                //    package.VBPointer->Position = areaVertices[(i + 1 == length) ? 0 : i + 1] + areaVertexOffsets[(i + 1 == length) ? 0 : i + 1] * localAxes[2];
                //    package.VBPointer->Color = lineColor;
                //    package.VBPointer++;
                //}
                #endregion

                #region Draw inner contour
                // Find parallel edges for computing intersections
                Vector3[] innerVertices = findInteriorEdges(areaVertices, areaVertexOffsets, indices, length, shellNormals, localAxes);

                for (int i = 0; i < length; ++i)
                {
                    package.VBPointer->Position = innerVertices[i];
                    package.VBPointer->Color = System.Drawing.Color.Yellow.ToArgb();
                    package.VBPointer++;

                    package.VBPointer->Position = innerVertices[(i + 1 == length) ? 0 : i + 1];
                    package.VBPointer->Color = System.Drawing.Color.Yellow.ToArgb();
                    package.VBPointer++;
                }

                #endregion

                #region Display crossing diagonals
                lineColor = System.Drawing.Color.Cyan.ToArgb();
                //if ((options.OptionsShown & RenderOptions.ShowOptions.ShellDiagonals) > 0 && length > 3)
                if (length > 3)
                {
                    // Find edge that divides into two triangles the quad
                    Vector3 pseudoNormal = Vector3.Normalize(shellNormals[0] + shellNormals[1]);
                    Vector3 mainEdge = areaVertices[indices[1]] - areaVertices[indices[0]] + (areaVertexOffsets[indices[1]] - areaVertexOffsets[indices[0]]) * pseudoNormal;

                    Vector3 initVertex = areaVertices[indices[0]] + areaVertexOffsets[indices[0]] * pseudoNormal;

                    package.VBPointer->Position = initVertex;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;

                    package.VBPointer->Position = areaVertices[indices[1]] + areaVertexOffsets[indices[1]] * pseudoNormal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;

                    if (concave)
                    {
                        #region Vertices for configuring the concavity
                        package.VBPointer->Position = areaVertices[indices[2]] + areaVertexOffsets[indices[2]] * shellNormals[0];
                        package.VBPointer->Color = lineColor;
                        package.VBPointer++;

                        package.VBPointer->Position = initVertex + 0.5f * mainEdge;
                        package.VBPointer->Color = lineColor;
                        package.VBPointer++;

                        package.VBPointer->Position = areaVertices[indices[4]] + areaVertexOffsets[indices[4]] * shellNormals[1];
                        package.VBPointer->Color = lineColor;
                        package.VBPointer++;

                        package.VBPointer->Position = initVertex + 0.5f * mainEdge;
                        package.VBPointer->Color = lineColor;
                        package.VBPointer++;
                        #endregion
                    }
                    else
                    {
                        #region Vertices for configuring the other diagonal
                        float s = 0f, t = 0f;

                        Vector3 diagonal = areaVertices[indices[4]] - areaVertices[indices[2]] + 
                                           areaVertexOffsets[indices[4]] * shellNormals[1] - areaVertexOffsets[indices[2]] * shellNormals[0];
                        Vector3 normal = shellNormals[0];

                        Vector3 projectionOnPlane = diagonal - Vector3.Dot(diagonal, normal) * normal;
                        
                        GeometricUtils.FindIntersection(areaVertices[indices[2]] + areaVertexOffsets[indices[2]]*normal, projectionOnPlane, 
                                                        areaVertices[indices[0]] + areaVertexOffsets[indices[0]]*pseudoNormal, mainEdge, ref s, ref t);

                        package.VBPointer->Position = areaVertices[indices[2]] + areaVertexOffsets[indices[2]] * shellNormals[0];
                        package.VBPointer->Color = lineColor;
                        package.VBPointer++;

                        package.VBPointer->Position = areaVertices[indices[0]] + areaVertexOffsets[indices[0]] * pseudoNormal + s * mainEdge;
                        package.VBPointer->Color = lineColor;
                        package.VBPointer++;

                        package.VBPointer->Position = areaVertices[indices[0]] + areaVertexOffsets[indices[0]] * pseudoNormal + s * mainEdge;
                        package.VBPointer->Color = lineColor;
                        package.VBPointer++;

                        package.VBPointer->Position = areaVertices[indices[4]] + areaVertexOffsets[indices[4]] * shellNormals[1];
                        package.VBPointer->Color = lineColor;
                        package.VBPointer++;
                        #endregion
                    }
                }
                #endregion

            } // Termina código NO SEGURO
        }

        protected Vector3[] findInteriorEdges(List<Vector3> areaVertices, List<float> areaVertexOffsets, List<int> indices, int length, Vector3[] shellNormals, Vector3[] localAxes)
        {
            // Find parallel edges for computing intersections
            int edgeIndex = 0, startIndex, endIndex;
            Vector3[] innerVertices = new Vector3[length];
            Vector3[] pointInEdge = new Vector3[length];
            Vector3[] edges = new Vector3[length];
            Vector3[] edgeNormals = new Vector3[length];
            Vector3 shellNormal = shellNormals[0];
            Vector3 pseudoNormal = Vector3.Normalize(shellNormals[0] + shellNormals[1]);

            int nIndices = indices.Count;

            for (int i = nIndices - length; i < nIndices; ++i)
            {
                if (i == 0 || i == 1 || i == 3 || i == 5) shellNormal = pseudoNormal;
                else if (i == 2) shellNormal = shellNormals[0];
                else shellNormal = shellNormals[1];

                if (i == nIndices - 1)
                {
                    startIndex = i;
                    endIndex = nIndices - length;
                }
                else
                {
                    startIndex = i;
                    endIndex = i + 1;
                }

                edges[edgeIndex] = areaVertices[indices[endIndex]] - areaVertices[indices[startIndex]] +
                                  (areaVertexOffsets[indices[endIndex]] - areaVertexOffsets[indices[startIndex]]) * localAxes[2];

                edgeNormals[edgeIndex] = Vector3.Normalize(Vector3.Cross(shellNormal, edges[edgeIndex]));

                pointInEdge[edgeIndex] = areaVertices[indices[startIndex]] + pixelDistance * edgeNormals[edgeIndex] + areaVertexOffsets[indices[startIndex]] * localAxes[2];

                ++edgeIndex;
            }

            float t = 0.0f, s = 0.0f;
            bool doIntersect = false;
            for (int i = 0; i < length; ++i)
            {
                doIntersect = GeometricUtils.FindIntersection(pointInEdge[i], edges[i], pointInEdge[(i + 1 == length ? 0 : i + 1)], edges[(i + 1 == length ? 0 : i + 1)], ref t, ref s);
                if (doIntersect)
                    innerVertices[i] = pointInEdge[i] + s * edges[i];
            }

            return innerVertices;
        }
    }
}

