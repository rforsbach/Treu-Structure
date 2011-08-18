using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Analysis;

namespace Canguro.View.Renderer
{
    public class ShadedAreaRenderer : AreaRenderer
    {
        public ShadedAreaRenderer()
        {
            // Required vertices: 3 + 3 + 18 (3 for top cover, 3 for bottom cover, 18 for the sides)
            verticesNeeded4Triangles = 24;

            // Required vertices: 6 + 6 + 24 vertices (6 for top cover, 6 more for bottom cover and 24 for the sides)
            verticesNeeded4Quads = 36;
        }

        public override void Render(Device device, AreaElement area, RenderOptions options)
        {
            throw new System.NotImplementedException();
        }

        public override void Render(Device device, Model.Model model, IEnumerable<AreaElement> areas, RenderOptions options, List<Item> itemsInView)
        {
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int vertices = 0;
            bool alphaEnable = false;

            rc.ActiveStream = ResourceStreamType.TriangleListPositionNormalColored;

            if ((options.OptionsShown & RenderOptions.ShowOptions.ShellTransparency) > 0)
            {
                alphaEnable = device.RenderState.AlphaBlendEnable;
                device.RenderState.AlphaBlendEnable = true;

                device.RenderState.SourceBlend = Blend.BothSourceAlpha;
                device.RenderState.DestinationBlend = Blend.DestinationColor;
            }

            System.Drawing.Color ambientColor = device.RenderState.Ambient;
            device.RenderState.ShadeMode = ShadeMode.Flat;
            device.RenderState.CullMode = Cull.Clockwise;
            
            device.RenderState.Lighting = true;
            device.Lights[0].Enabled = true;
            device.RenderState.ColorVertex = true;

            PositionNormalColoredPackage package = (PositionNormalColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionNormalColored, false, true);

            if (areas != null && ((IList<AreaElement>)areas).Count > 0)
            {
                foreach (AreaElement area in areas)
                {
                    drawShadedArea(rc, area, pickingMode, ref package, ref vertices, options);
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
                        drawShadedArea(rc, area, pickingMode, ref package, ref vertices, options);
                    }
                }
            }

            rc.ReleaseBuffer(vertices, 0, ResourceStreamType.TriangleListPositionNormalColored);
            rc.Flush(ResourceStreamType.TriangleListPositionNormalColored);

            if ((options.OptionsShown & RenderOptions.ShowOptions.ShellTransparency) > 0)
                device.RenderState.AlphaBlendEnable = alphaEnable;
        }

        private void drawShadedArea(ResourceManager rc, AreaElement area, bool pickingMode, ref PositionNormalColoredPackage package, ref int vertices, RenderOptions options)
        {
            if (area == null || !area.IsVisible) return;

            Vector3[] localAxes = area.LocalAxes;
            List<Vector3> areaVertices = new List<Vector3>();
            List<float> areaVertexOffsets = new List<float>();
            List<int> indices = new List<int>();

            int requiredVertices = 0;
            bool concave = false;
            int lineColor = System.Drawing.Color.FromArgb(159, System.Drawing.Color.Red).ToArgb();

            Canguro.Model.Section.ShellSection section = area.Properties.Section as Canguro.Model.Section.ShellSection;
            float areaThickness = section.ThicknessMembrane;

            requiredVertices = configureVertexList(area, localAxes, areaVertices, areaVertexOffsets, indices, ref concave);

            // Check space in package's VB
            if (vertices + requiredVertices >= package.NumVertices - 1)
            {
                rc.ReleaseBuffer(vertices, 0, package.Stream);
                package = (PositionNormalColoredPackage)rc.CaptureBuffer(package.Stream, 0, requiredVertices, true);
                vertices = 0;
            }
            vertices += requiredVertices;

            // Compute Vertex normals
            int length = areaVertices.Count, leftIndex, rightIndex;
            Vector3[] vertexNormals = new Vector3[length];

            for (int i = 0; i < length; ++i)
            {
                leftIndex = (i - 1 < 0 ? length - 1 : i - 1);
                rightIndex = (i + 1 > length - 1 ? 0 : i + 1);

                vertexNormals[i] = Vector3.Normalize(Vector3.Cross(areaVertices[i] - areaVertices[leftIndex], areaVertices[rightIndex] - areaVertices[i]));
            }

            // Inicia código NO SEGURO
            int indicesLength = indices.Count;
            int shapeVertices = areaVertices.Count;

            unsafe
            {
                Vector3[] shellNormals = new Vector3[2];
                Vector3 normal, vertexNormal;

                // Calculate first normal based in the first two edges
                shellNormals[0] = Vector3.Normalize(Vector3.Cross(areaVertices[indices[0]] - areaVertices[indices[2]] +
                                                                  (areaVertexOffsets[indices[0]] - areaVertexOffsets[indices[2]]) * localAxes[2],
                                                                  areaVertices[indices[1]] - areaVertices[indices[0]] +
                                                                  (areaVertexOffsets[indices[1]] - areaVertexOffsets[indices[0]]) * localAxes[2]));

                if (indicesLength > 3)
                    shellNormals[1] = Vector3.Normalize(Vector3.Cross(areaVertices[indices[3]] - areaVertices[indices[5]] +
                                                                 (areaVertexOffsets[indices[3]] - areaVertexOffsets[indices[5]]) * localAxes[2],
                                                                 areaVertices[indices[4]] - areaVertices[indices[3]] +
                                                                 (areaVertexOffsets[indices[4]] - areaVertexOffsets[indices[3]]) * localAxes[2]));

                Vector3 pseudoNormal = Vector3.Normalize(shellNormals[0] + shellNormals[1]);

                normal = shellNormals[0];

                #region Create top cover
                for (int i = 0; i < indicesLength; ++i)
                {
                    if (i == 0 || i == 1 || i == 3 || i == 5) vertexNormal = pseudoNormal;
                    else if (i == 2) vertexNormal = shellNormals[0];
                    else vertexNormal = shellNormals[1];

                    if (i == 3)
                        normal = shellNormals[1];

                    package.VBPointer->Position = areaVertices[indices[i]] + areaVertexOffsets[indices[i]]*localAxes[2] + 0.5f * areaThickness * vertexNormal;
                    package.VBPointer->Normal = normal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;
                }
                #endregion

                #region Create side triangles
                int nextIndex;
                Vector3 nextVertexNormal = Vector3.Empty;
                Vector3 sideNormal;

                normal = shellNormals[0];

                for (int i = indicesLength - length; i < indicesLength; ++i)
                {
                    if (i == 0 || i == 1 || i == 3 || i == 5) vertexNormal = pseudoNormal;
                    else if (i == 2) vertexNormal = shellNormals[0];
                    else vertexNormal = shellNormals[1];

                    // Get index based in shape vertex
                    nextIndex = (i + 1 == indicesLength) ? indicesLength - length : i + 1;

                    if (nextIndex == 0 || nextIndex == 1 || nextIndex == 3 || nextIndex == 5) nextVertexNormal = pseudoNormal;
                    else if (nextIndex == 2) nextVertexNormal = shellNormals[0];
                    else nextVertexNormal = shellNormals[1];

                    if (i == 3)
                        normal = shellNormals[1];
                    else if (i == indicesLength - 1)
                        normal = shellNormals[0];
                   
                    // Get normal for each side bases on local axis 2 and the edge we are representing
                    sideNormal = Vector3.Normalize(Vector3.Cross(areaVertices[indices[nextIndex]] - areaVertices[indices[i]] +
                                                                (areaVertexOffsets[indices[nextIndex]] - areaVertexOffsets[indices[i]]) * localAxes[2], normal));

                    lineColor = System.Drawing.Color.Cyan.ToArgb();

                    #region First triangle
                    package.VBPointer->Position = areaVertices[indices[i]] + areaVertexOffsets[indices[i]] * localAxes[2] - 0.5f * areaThickness * vertexNormal;
                    package.VBPointer->Normal = sideNormal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;

                    package.VBPointer->Position = areaVertices[indices[nextIndex]] + areaVertexOffsets[indices[nextIndex]] * localAxes[2] - 0.5f * areaThickness * nextVertexNormal;
                    package.VBPointer->Normal = sideNormal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;

                    package.VBPointer->Position = areaVertices[indices[nextIndex]] + areaVertexOffsets[indices[nextIndex]] * localAxes[2] + 0.5f * areaThickness * nextVertexNormal;
                    package.VBPointer->Normal = sideNormal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;
                    #endregion

                    lineColor = System.Drawing.Color.Yellow.ToArgb();
                    #region Second triangle
                    package.VBPointer->Position = areaVertices[indices[i]] + areaVertexOffsets[indices[i]] * localAxes[2] - 0.5f * areaThickness * vertexNormal;
                    package.VBPointer->Normal = sideNormal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;

                    package.VBPointer->Position = areaVertices[indices[nextIndex]] + areaVertexOffsets[indices[nextIndex]] * localAxes[2] + 0.5f * areaThickness * nextVertexNormal;
                    package.VBPointer->Normal = sideNormal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;

                    package.VBPointer->Position = areaVertices[indices[i]] + areaVertexOffsets[indices[i]] * localAxes[2] + 0.5f * areaThickness * vertexNormal;
                    package.VBPointer->Normal = sideNormal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;
                    #endregion
                }
                #endregion

                #region Create bottom cover
                lineColor = System.Drawing.Color.Cyan.ToArgb();
                normal = -shellNormals[1];

                for (int i = indicesLength - 1; i >= 0; --i)
                {
                    if (i == 0 || i == 1 || i == 3 || i == 5) vertexNormal = -pseudoNormal;
                    else if (i == 2) vertexNormal = -shellNormals[0];
                    else vertexNormal = -shellNormals[1];

                    if (i == 3) // We have a quad
                        normal = -shellNormals[0];

                    package.VBPointer->Position = areaVertices[indices[i]] + areaVertexOffsets[indices[i]] * localAxes[2] - 0.5f * areaThickness * vertexNormal;
                    package.VBPointer->Normal = normal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;
                }
                #endregion
            } // Termina código NO SEGURO
        }
    }
}
