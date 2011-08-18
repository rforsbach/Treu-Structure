using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;

namespace Canguro.View.Renderer
{
    /// <summary>
    /// Asbtract class for drawing areas. Must be changed
    /// </summary>
    public abstract class AreaRenderer : ItemRenderer
    {

        protected float pixelDistance = 0f;
        protected int verticesNeeded4Triangles = 0;
        protected int verticesNeeded4Quads = 0;

        /// <summary>
        /// Main abstract method for area rendering
        /// </summary>
        /// <param name="device"> DirectX active device </param>
        /// <param name="area"> The area to draw </param>
        /// <param name="options"> Render options: Ids, properties, etc. </param>
        public abstract void Render(Microsoft.DirectX.Direct3D.Device device, Model.AreaElement area, RenderOptions options);

        public abstract void Render(Microsoft.DirectX.Direct3D.Device device, Model.Model model, IEnumerable<Model.AreaElement> areas, RenderOptions options, List<Model.Item> itemsInView);

        #region Triangulate triangle as the Sierpinsky Gasket fractal
        protected void triangulateTriangle(List<Vector3> vertexList, LinkedListNode<int> initNode, Vector3 v1, Vector3 v2, Vector3 v3, int level)
        {
            if (level == 0) return;

            Vector3 mid1 = 0.5f * (v1 + v2);
            Vector3 mid2 = 0.5f * (v2 + v3);
            Vector3 mid3 = 0.5f * (v3 + v1);
             
            vertexList.Add(mid1);
            vertexList.Add(mid2);
            vertexList.Add(mid3);

            int nextIndex = vertexList.Count - 3;

            // Generate three nodes for storing the three references of interest
            LinkedListNode<int> first = initNode;
            LinkedListNode<int> second = first.Next;
            LinkedListNode<int> third = second.Next;

            LinkedListNode<int> fourthElement = new LinkedListNode<int>(nextIndex);
            third.List.AddAfter(third, fourthElement);

            LinkedListNode<int> fourth = third.Next;

            // First generated triangle and its triangulation
            addTriangleAndTriangulate(vertexList, first, nextIndex, nextIndex + 2, level - 1);

            // Second generated triangle and its triangulation
            addTriangleAndTriangulate(vertexList, second, nextIndex + 1, nextIndex, level - 1);

            // Third generated triangle and its triangulation
            addTriangleAndTriangulate(vertexList, third, nextIndex + 2, nextIndex + 1, level - 1);

            // Fourth generated triangle and its triangulation
            addTriangleAndTriangulate(vertexList, fourth, nextIndex + 1, nextIndex + 2, level - 1);
        }

        private void addTriangleAndTriangulate(List<Vector3> vertexList, LinkedListNode<int> initNode, int nextIndex, int lastIndex, int level)
        {
            LinkedListNode<int> firstVertex2Add = initNode;
            LinkedListNode<int> secondVertex2Add = new LinkedListNode<int>(nextIndex);
            LinkedListNode<int> thirdVertex2Add = new LinkedListNode<int>(lastIndex);

            firstVertex2Add.List.AddAfter(firstVertex2Add, secondVertex2Add);
            secondVertex2Add.List.AddAfter(secondVertex2Add, thirdVertex2Add);

            // Triangulate the created triangle
            triangulateTriangle(vertexList, initNode, vertexList[firstVertex2Add.Value], vertexList[secondVertex2Add.Value], vertexList[thirdVertex2Add.Value], level);
        }
        #endregion

        #region Triangulate using the centroid of the triangle for generating next vertices. Not used in the program.
        //protected void triangulateTriangle(List<Vector3> vertexList, LinkedListNode<int> initNode, Vector3 v1, Vector3 v2, Vector3 v3, int level)
        //{
        //    if (level == 0) return;

        //    vertexList.Add(1f / 3f * (v1 + v2 + v3));

        //    int newIndex = vertexList.Count - 1;

        //    // Generate three nodes for storing the three interest references
        //    LinkedListNode<int> first = initNode;
        //    LinkedListNode<int> second = first.Next;
        //    LinkedListNode<int> third = second.Next;

        //    // First generated triangle and its triangulation
        //    addTriangleAndTriangulate(vertexList, first, second, newIndex, level - 1);

        //    // Second generated triangle and its triangulation
        //    addTriangleAndTriangulate(vertexList, second, third, newIndex, level - 1);

        //    // Third generated triangle and its triangulation
        //    addTriangleAndTriangulate(vertexList, third, first, newIndex, level - 1);
        //}

        //private void addTriangleAndTriangulate(List<Vector3> vertexList, LinkedListNode<int> initNode, LinkedListNode<int> lastNode, int newIndex, int level)
        //{
        //    LinkedListNode<int> firstVertex2Add = initNode;
        //    LinkedListNode<int> secondVertex2Add = new LinkedListNode<int>(lastNode.Value);
        //    LinkedListNode<int> thirdVertex2Add = new LinkedListNode<int>(newIndex);

        //    firstVertex2Add.List.AddAfter(firstVertex2Add, secondVertex2Add);
        //    secondVertex2Add.List.AddAfter(secondVertex2Add, thirdVertex2Add);

        //    // Triangulate the created triangle
        //    triangulateTriangle(vertexList, initNode, vertexList[firstVertex2Add.Value], vertexList[secondVertex2Add.Value], vertexList[thirdVertex2Add.Value], level);
        //}
        #endregion

        protected bool checkIfConcave(Vector3 vLeft, Vector3 vRight, Vector3 areaNormal)
        {
            if (Vector3.Dot(Vector3.Cross(vLeft, vRight), areaNormal) < 0) // We found the vertex that make this quad a concave polygon
                return true;

            return false;
        }

        protected bool rearrangeIfConcavities(List<Vector3> areaVertices, List<int> indices, Vector3 shellNormal)
        {
            // Test if quad is not concave
            Vector3 vLeft, vRight;
            bool concave = false;
            int index = 0;

            // Check for each vertex if the normal at that vertex points in the same direction as the plane normal. If not, then the vertex forms a concavitie
            for (index = 0; !concave && index < 4; ++index)
            {
                vLeft = areaVertices[index] - areaVertices[index - 1 < 0 ? 3 : index - 1];
                vRight = areaVertices[index + 1 > 3 ? 0 : index + 1] - areaVertices[index];

                concave = checkIfConcave(vLeft, vRight, shellNormal);
            }

            // Re-order vertices when a concavitie is found
            if (concave)
            {
                --index;
                indices[0] = index; indices[1] = (index + 2 > 3) ? index - 2 : index + 2;   indices[2] = (index - 1 < 0) ? 3 : index - 1;
                indices[3] = index; indices[4] = (index + 1 > 3) ? 0 : index + 1;           indices[5] = (index + 2 > 3) ? index - 2 : index + 2;
            }

            return concave;
        }

        protected int configureVertexList(AreaElement area, Vector3[] localAxes, List<Vector3> areaVertices, List<float> areaVertexOffsets, List<int> indices, ref bool concavity)
        {
            int requiredVertices = 0;

            // There are always three vertices
            float[] offsets = area.Offsets;
            areaVertices.Add(area.J1.Position); areaVertexOffsets.Add(offsets[0]);
            areaVertices.Add(area.J2.Position); areaVertexOffsets.Add(offsets[1]);
            areaVertices.Add(area.J3.Position); areaVertexOffsets.Add(offsets[2]);

            // Ge indices and consider the quad as no degenerated so
            indices.Add(0); indices.Add(1); indices.Add(2);

            requiredVertices = verticesNeeded4Triangles;

            // Check if area is conformed by four vertices
            if (area.J4 != null)
            {
                areaVertices.Add(area.J4.Position); areaVertexOffsets.Add(offsets[3]);

                // Ge indices and consider the quad as no degenerated so
                indices.Clear();
                indices.Add(0); indices.Add(2); indices.Add(3);
                indices.Add(0); indices.Add(1); indices.Add(2);

                concavity = rearrangeIfConcavities(areaVertices, indices, localAxes[2]);

                requiredVertices = verticesNeeded4Quads;
            }

            return requiredVertices;
        }
    }
}
