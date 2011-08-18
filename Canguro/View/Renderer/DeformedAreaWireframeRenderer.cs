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
    public class DeformedAreaWireframeRenderer : AreaRenderer
    {
        AreaDeformationCalculator calc = null;

        private Canguro.Model.Model model
        {
            get { return Canguro.Model.Model.Instance; }
        }

        public override void Render(Device device, AreaElement area, RenderOptions options)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Render(Device device, Canguro.Model.Model model, IEnumerable<AreaElement> areas, RenderOptions options, List<Item> itemsInView)
        {
            //if (!model.HasResults || model.Results.JointDisplacements == null) return;

            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            int selectedColor = Properties.Settings.Default.SelectedDefaultColor.ToArgb();

            int numVerticesInVB = 0;

            if (device.RenderState.Lighting == true)
                device.RenderState.Lighting = false;

            if (device.RenderState.CullMode != Cull.None)
                device.RenderState.CullMode = Cull.None;

            // Renderer and device resources
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            calc = new AreaDeformationCalculator();

            rc.ActiveStream = ResourceStreamType.Lines;

            PositionColoredPackage package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);

            if (areas != null && ((IList<AreaElement>)areas).Count > 0)
            {
                foreach (AreaElement area in areas)
                    drawDeformedArea(rc, area, pickingMode, options, model, ref package, ref numVerticesInVB);
            }
            else if (itemsInView != null)
            {
                // Get list of items in view (Bounding Box)
                if (itemsInView.Count <= 0)
                    GetItemsInView(itemsInView);

                if (itemsInView.Count > 0)
                {
                    AreaElement area;
                    foreach (Item item in itemsInView)
                    {
                        area = item as AreaElement;
                        drawDeformedArea(rc, area, pickingMode, options, model, ref package, ref numVerticesInVB);
                    }
                }
            }

            // Flush remaining vertices
            rc.ReleaseBuffer(numVerticesInVB, 0, ResourceStreamType.Lines);
            rc.Flush(ResourceStreamType.Lines);
        }

        public void drawDeformedArea(ResourceManager rc, AreaElement area, bool pickingMode, RenderOptions options, Model.Model model, ref PositionColoredPackage package, ref int numVerticesInVB)
        {
            if (area == null || !area.IsVisible) return;

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

            #region Paint calls
            requiredVertices = indices.Count * 2;

            if (numVerticesInVB + requiredVertices >= package.NumVertices)
            {
                rc.ReleaseBuffer(numVerticesInVB, 0, ResourceStreamType.Lines);
                package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, 0, requiredVertices, true);
                numVerticesInVB = 0;
            }
            numVerticesInVB += requiredVertices;

            unsafe
            {
                LinkedListNode<int> index = indices.First;
                for (int i = 0; i < indices.Count/3; ++i)
                {
                    package.VBPointer->Position = vertexList[index.Value];
                    package.VBPointer->Color = System.Drawing.Color.Red.ToArgb();
                    package.VBPointer++;

                    index = index.Next;

                    package.VBPointer->Position = vertexList[(index == null ? indices.First.Value : index.Value)];
                    package.VBPointer->Color = System.Drawing.Color.Red.ToArgb();
                    package.VBPointer++;

                    //////////////////

                    package.VBPointer->Position = vertexList[index.Value];
                    package.VBPointer->Color = System.Drawing.Color.Red.ToArgb();
                    package.VBPointer++;

                    index = index.Next;

                    package.VBPointer->Position = vertexList[(index == null ? indices.First.Value : index.Value)];
                    package.VBPointer->Color = System.Drawing.Color.Red.ToArgb();
                    package.VBPointer++;

                    //////////////////

                    package.VBPointer->Position = vertexList[index.Value];
                    package.VBPointer->Color = System.Drawing.Color.Red.ToArgb();
                    package.VBPointer++;

                    package.VBPointer->Position = vertexList[(index.Previous.Previous == null ? indices.First.Value : index.Previous.Previous.Value)];
                    package.VBPointer->Color = System.Drawing.Color.Red.ToArgb();
                    package.VBPointer++;

                    index = index.Next;
                }
            }
            #endregion
        }
    }
}
