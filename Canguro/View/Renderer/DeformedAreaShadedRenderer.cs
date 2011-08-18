using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Model.Section;
using Canguro.Model.Load;

using Canguro.Analysis;

namespace Canguro.View.Renderer
{
    public class DeformedAreaShadedRenderer:AreaRenderer
    {
        AreaDeformationCalculator calc = null;

        private Canguro.Model.Model model
        {
            get { return Canguro.Model.Model.Instance; }
        }

        public DeformedAreaShadedRenderer(DeformedAreaWireframeRenderer dwa)
        {
        }

        public override void Render(Device device, AreaElement area, RenderOptions options)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Render(Device device, Canguro.Model.Model model, IEnumerable<AreaElement> areas, RenderOptions options, List<Item> itemsInView)
        {
            //if (!model.HasResults || model.Results.JointDisplacements == null) return;

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

            calc = new AreaDeformationCalculator();

            PositionNormalColoredPackage package = (PositionNormalColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionNormalColored, false, true);

            if (areas != null && ((IList<AreaElement>)areas).Count > 0)
            {
                foreach (AreaElement area in areas)
                {
                    drawDeformedArea(rc, area, pickingMode, ref package, ref vertices, options);
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
                        drawDeformedArea(rc, area, pickingMode, ref package, ref vertices, options);
                    }
                }
            }

            rc.ReleaseBuffer(vertices, 0, ResourceStreamType.TriangleListPositionNormalColored);
            rc.Flush(ResourceStreamType.TriangleListPositionNormalColored);

            if ((options.OptionsShown & RenderOptions.ShowOptions.ShellTransparency) > 0)
                device.RenderState.AlphaBlendEnable = alphaEnable;
        }

        private void drawDeformedArea(ResourceManager rc, AreaElement area, bool pickingMode, ref PositionNormalColoredPackage package, ref int numVerticesInVB, RenderOptions options)
        {
            if (area == null || !area.IsVisible) return;

            Canguro.Model.Section.ShellSection section = area.Properties.Section as Canguro.Model.Section.ShellSection;
            float areaThickness = section.ThicknessMembrane;

            int lineColor = System.Drawing.Color.FromArgb(159, System.Drawing.Color.Red).ToArgb();

            List<Vector3> vertexList = new List<Vector3>();
            List<int> auxIndices = new List<int>();
            List<float> vertexOffsets = new List<float>();
            LinkedList<int> indices = new LinkedList<int>();
            LinkedListNode<int> initNode;

            Vector3[] localAxes = area.LocalAxes;

            bool concavity = false;
            int requiredVertices, verticesInList;

            requiredVertices = configureVertexList(area, localAxes, vertexList, vertexOffsets, auxIndices, ref concavity);

            verticesInList = vertexList.Count;

            for (int i = 0; i < verticesInList; ++i)
                vertexList[i] += vertexOffsets[i] * localAxes[2];

            for (int i = 0; i < auxIndices.Count; ++i)
                indices.AddLast(auxIndices[i]);

            auxIndices.Clear();

            if (verticesInList > 3)
            {
                // Get a pointer to the second triangle
                LinkedListNode<int> secondTriangle = indices.First.Next.Next.Next;

                triangulateTriangle(vertexList, indices.First, vertexList[indices.First.Value],
                                                               vertexList[indices.First.Next.Value],
                                                               vertexList[indices.First.Next.Next.Value], 2);

                triangulateTriangle(vertexList, secondTriangle, vertexList[secondTriangle.Value],
                                                                vertexList[secondTriangle.Next.Value],
                                                                vertexList[secondTriangle.Next.Next.Value], 2);
            }
            else
            {
                // Point to the first node in the list
                initNode = indices.First;

                // Triangulate the triangle
                triangulateTriangle(vertexList, initNode, vertexList[0], vertexList[1], vertexList[2], 2);
            }

            #region Deformations

            Vector3[] ctrlPoints = new Vector3[verticesInList];
            Vector3[] deformedVertices = new Vector3[verticesInList];
            Matrix m;

            area.RotationMatrix(out m);

            m.M41 = -vertexList[0].X;
            m.M42 = -vertexList[0].Y;
            m.M43 = -vertexList[0].Z;

            for (int i = 0; i < verticesInList; ++i)
            {
                ctrlPoints[i] = vertexList[i];

                ctrlPoints[i].TransformCoordinate(m);
            }

            calc.GetDeformationVectors(area, localAxes, null, ctrlPoints, deformedVertices);

            for (int i = 0; i < verticesInList; ++i)
                vertexList[i] += (deformedVertices[i].X * localAxes[0] + deformedVertices[i].Y * localAxes[1] + deformedVertices[i].Z * localAxes[2]);

            #endregion

            int generatedVertices = indices.Count;

            // The required vertices are two times the number of generated vertices for building the covers plus 6 times the original verticesInList for the sides
            //requiredVertices = 2 * generatedVertices + 6 * verticesInList;
            requiredVertices = 2 * generatedVertices;

            if (numVerticesInVB + requiredVertices >= package.NumVertices)
            {
                rc.ReleaseBuffer(numVerticesInVB, 0, ResourceStreamType.Lines);
                package = (PositionNormalColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, 0, requiredVertices, true);
                numVerticesInVB = 0;
            }
            numVerticesInVB += requiredVertices;

            unsafe
            {
                LinkedListNode<int> index;
                Vector3 normal;
                int[] triangleIndices = new int[3];

                #region Top cover
                index = indices.First;

                triangleIndices[0] = index.Value;
                triangleIndices[1] = index.Next.Value;
                triangleIndices[2] = index.Next.Next.Value;

                normal = Vector3.Normalize(Vector3.Cross(vertexList[triangleIndices[1]] - vertexList[triangleIndices[0]],
                                                         vertexList[triangleIndices[2]] - vertexList[triangleIndices[1]]));

                for (int i = 0; i < generatedVertices; ++i)
                {
                    if ((i%3) == 0)
                    {
                        triangleIndices[0] = index.Value;
                        triangleIndices[1] = index.Next.Value;
                        triangleIndices[2] = index.Next.Next.Value;

                        normal = Vector3.Normalize(Vector3.Cross(vertexList[triangleIndices[1]] - vertexList[triangleIndices[0]],
                                                                 vertexList[triangleIndices[2]] - vertexList[triangleIndices[1]]));
                    }

                    package.VBPointer->Position = vertexList[index.Value] + 0.5f * areaThickness * localAxes[2];
                    package.VBPointer->Normal = normal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;

                    index = index.Next;
                }
                #endregion

                #region Bottom Cover
                index = indices.Last;

                triangleIndices[0] = index.Value;
                triangleIndices[1] = index.Previous.Value;
                triangleIndices[2] = index.Previous.Previous.Value;

                normal = Vector3.Normalize(Vector3.Cross(vertexList[triangleIndices[1]] - vertexList[triangleIndices[0]],
                                                         vertexList[triangleIndices[2]] - vertexList[triangleIndices[1]]));

                for (int i = 0; i < generatedVertices; ++i)
                {
                    if ((i % 3) == 0)
                    {
                        triangleIndices[0] = index.Value;
                        triangleIndices[1] = index.Previous.Value;
                        triangleIndices[2] = index.Previous.Previous.Value;

                        normal = Vector3.Normalize(Vector3.Cross(vertexList[triangleIndices[1]] - vertexList[triangleIndices[0]],
                                                                 vertexList[triangleIndices[2]] - vertexList[triangleIndices[1]]));
                    }

                    package.VBPointer->Position = vertexList[index.Value] - 0.5f*areaThickness * localAxes[2];
                    package.VBPointer->Normal = normal;
                    package.VBPointer->Color = lineColor;
                    package.VBPointer++;

                    index = index.Previous;
                }
                #endregion
            }
        }
    }
}
