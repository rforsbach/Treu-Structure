using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Analysis.Sections.Meshing
{
    public class Mesh
    {
        List<Vertex> vertices;
        List<Shape> shapes;
        List<Edge> edges;

        System.Drawing.PointF[] boundingBox = new System.Drawing.PointF[3];
        Material material = Material.None;

        public List<Shape> Shapes
        {
            get { return shapes; }
        }

        public List<Vertex> Vertices
        {
            get { return vertices; }
        }

        public List<Edge> Edges
        {
            get { return edges; }
        }

        public string Printvertices()
        {
            string ret = "";
            foreach (Vertex v in vertices)
                ret += v.ToString() + "\n";

            return ret;
        }

        public Material Material
        {
            get { return material; }
            set { material = value; }
        }

        /// <summary>
        /// Returns the bounding box of the Mesh.
        /// The 1st element [0] is the lower left corner.
        /// The 2nd element [1] is the upper right corner.
        /// The 3rd element is the size [1] - [0]
        /// </summary>
        public System.Drawing.PointF[] BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }

        public Mesh()
        {
            vertices = new List<Vertex>();
            shapes = new List<Shape>();
            edges = new List<Edge>();
        }

        public LinkedList<Vertex> GetContourFromVertex(int vertexID)
        {
            LinkedList<Vertex> contour = new LinkedList<Vertex>();
            LinkedListNode<Vertex> newNode = new LinkedListNode<Vertex>(vertices[vertexID]);
            contour.AddLast(newNode);
            bool[] visitedEdges = new bool[edges.Count];

            GetContourFromVertex(newNode, visitedEdges);

            return contour;
        }

        public void GetContourFromVertex(LinkedListNode<Vertex> node, bool[] visitedEdges)
        {
            Vertex v;
            Edge nextEdge;
            LinkedListNode<Vertex> startNode = node;

            do
            {
                v = node.Value;
                nextEdge = null;

                // Find new edge
                foreach (Edge e in v.Edges)
                    if (!visitedEdges[e.Id])
                    {
                        nextEdge = e;
                        break;
                    }

                if (nextEdge != null)
                {
                    visitedEdges[nextEdge.Id] = true;

                    // Add next vertex to contour
                    if (nextEdge.V1 == v)
                        v = nextEdge.V2;
                    else
                        v = nextEdge.V1;

                    LinkedListNode<Vertex> newNode = new LinkedListNode<Vertex>(v);
                    node.List.AddAfter(node, newNode);
                    node = newNode;
                }
            } while (nextEdge != null);

            // Look for paths (edge loops) that haven't been included yet
            LinkedListNode<Vertex> pos = node;
            while (pos != startNode)
            {
                foreach (Edge e in pos.Value.Edges)
                    if (!visitedEdges[e.Id])
                        GetContourFromVertex(pos, visitedEdges);

                pos = pos.Previous;
            }            
        }
        
        public void CreateMesh(List<Delaunay.Triangle> ts, List<Vertex> vs)
        {
            foreach (Delaunay.Triangle t in ts)
            {                
                // Add Edges
                Edge e1 = AddEdge(vertices[t.Vertex1.GlobalId], vertices[t.Vertex2.GlobalId]);
                Edge e2 = AddEdge(vertices[t.Vertex2.GlobalId], vertices[t.Vertex3.GlobalId]);
                Edge e3 = AddEdge(vertices[t.Vertex3.GlobalId], vertices[t.Vertex1.GlobalId]);

                // Add triangle
                Triangle tri = AddTriangle(e1, e2, e3);
            }
        }

        public Vertex AddVertex(double x, double y, bool isBoundary, bool checkDuplicates)
        {
            Vertex v = new Vertex(vertices.Count, x, y, isBoundary);

            if (checkDuplicates)
            {
                foreach (Vertex vertex in vertices)
                    if (vertex.Equals(v))
                        return vertex;
            }

            vertices.Add(v);

            // Update bounding box
            if (isBoundary)
            {
                if (v.X < boundingBox[0].X || vertices.Count == 1)
                    boundingBox[0].X = (float)v.X;
                if (v.Y < boundingBox[0].Y || vertices.Count == 1)
                    boundingBox[0].Y = (float)v.Y;
                if (v.X > boundingBox[1].X || vertices.Count == 1)
                    boundingBox[1].X = (float)v.X;
                if (v.Y > boundingBox[1].Y || vertices.Count == 1)
                    boundingBox[1].Y = (float)v.Y;

                boundingBox[2].X = boundingBox[1].X - boundingBox[0].X;
                boundingBox[2].Y = boundingBox[1].Y - boundingBox[0].Y;
            }

            return v;
        }
        
        public Vertex AddVertex(double x, double y, bool isBoundary)
        {
            return AddVertex(x, y, isBoundary, false);
        }

        /// <summary>
        /// Creates a triangle from 3 edges
        /// </summary>
        /// <param name="e1">First edge</param>
        /// <param name="e2">Second edge</param>
        /// <param name="e3">Third edge</param>
        /// <returns>The newly created triangle</returns>
        public Triangle AddTriangle(Edge e1, Edge e2, Edge e3)
        {
            Shape s;
            if ((s = e1.IsAdjacentTo(e2, e3)) != null)
            {
                if (s is Triangle)
                    return s as Triangle;
                else
                    throw new InvalidOperationException("Cannot add triangle because another Shape is already connecting the same Edges");
            }

            // Check if edges have just 1 shape attached
            if (e1.Shapes.Count > 1 || e2.Shapes.Count > 1 || e3.Shapes.Count > 1)
                return null;

            Triangle t = new Triangle();
            
            // Add Edges to triangle
            t.Edges.Add(e1);
            t.Edges.Add(e2);
            t.Edges.Add(e3);

            // Check if new triangle is on top of (intersects) an existing one
            foreach (Edge e in new Edge[] { e1, e2, e3 })
            {
                if (e.Shapes.Count == 1)
                {
                    System.Drawing.PointF n = new System.Drawing.PointF((float)(e.V2.Y - e.V1.Y), (float)-(e.V2.X - e.V1.X));
                    System.Drawing.PointF pt1 = e.Shapes[0].GetInsidePoint();
                    System.Drawing.PointF pt2 = t.GetInsidePoint();

                    pt1.X = pt1.X - (float)e.V1.X;
                    pt1.Y = pt1.Y - (float)e.V1.Y;
                    pt2.X = pt2.X - (float)e.V1.X;
                    pt2.Y = pt2.Y - (float)e.V1.Y;

                    float dot1 = n.X * pt1.X + n.Y * pt1.Y;
                    float dot2 = n.X * pt2.X + n.Y * pt2.Y;

                    if (Math.Sign(dot1) == Math.Sign(dot2))
                        return null;
                }
            }

            // Add triangle to edges
            e1.Shapes.Add(t);                
            e2.Shapes.Add(t);
            e3.Shapes.Add(t);

            shapes.Add(t);
            return t;
        }

        /// <summary>
        /// Creates a quad from 4 edges
        /// </summary>
        /// <param name="e1">First edge</param>
        /// <param name="e2">Second edge</param>
        /// <param name="e3">Third edge</param>
        /// <param name="e4">Forth edge</param>
        /// <returns>The newly created triangle</returns>
        public Quad AddQuad(Edge e1, Edge e2, Edge e3, Edge e4)
        {
            Shape s;
            if ((s = e1.IsAdjacentTo(e2, e3)) != null)
            {
                if (s.ContainsEdge(e4))
                    return s as Quad;
                else
                    throw new InvalidOperationException("Cannot add triangle because another Shape is already connecting 3 of the same Edges");
            }

            Quad q = new Quad();

            // Add Edges to quad
            q.Edges.Add(e1);
            q.Edges.Add(e2);
            q.Edges.Add(e3);
            q.Edges.Add(e4);

            // Add quad to edges
            e1.Shapes.Add(q);
            e2.Shapes.Add(q);
            e3.Shapes.Add(q);
            e4.Shapes.Add(q);

            shapes.Add(q);
            return q;
        }

        /// <summary>
        /// Returns the Edge that connects v1 and v2. If none exists it is created. 
        /// </summary>
        /// <param name="v1">First connecting vertex</param>
        /// <param name="v2">Second connecting vertex</param>
        /// <returns>The edge that connects v1 and v2</returns>
        public Edge AddEdge(Vertex v1, Vertex v2, bool isBoundary)
        {
            Edge e;
            if ((e = v1.IsAdjacentTo(v2)) == null)
            {
                e = new Edge(edges.Count, v1, v2);
                v1.Edges.Add(e);
                v2.Edges.Add(e);
                edges.Add(e);
            }

            e.IsBoundary |= isBoundary;

            return e;
        }

        /// <summary>
        /// Returns the Edge that connects v1 and v2. If none exists it is created. 
        /// </summary>
        /// <param name="v1">First connecting vertex</param>
        /// <param name="v2">Second connecting vertex</param>
        /// <returns>The edge that connects v1 and v2</returns>
        public Edge AddEdge(Vertex v1, Vertex v2)
        {
            return AddEdge(v1, v2, false);
        }

        public void RemoveShape(Shape s)
        {
            if (s == null) return;

            foreach (Edge e in s.Edges)
                e.Shapes.Remove(s);

            shapes.Remove(s);
        }

        public void RemoveEdge(Edge e)
        {
            e.V1.Edges.Remove(e);
            e.V2.Edges.Remove(e);

            Shape[] eShapes = e.Shapes.ToArray();
            foreach (Shape s in eShapes)
                RemoveShape(s);
        }

        public void RemoveVertex(Vertex v)
        {
            removeVertex(v, true);
        }

        private void removeVertex(Vertex v, bool renumber)
        {
            Edge[] vEdges = v.Edges.ToArray();
            foreach (Edge e in vEdges)
                RemoveEdge(e);

            for (int i = vertices.Count - 1; i >= 0; i--)
                if (vertices[i].Id == v.Id)
                {
                    vertices.RemoveAt(i);
                    break;
                }

            if (renumber)
                renumberVertices();
        }

        public List<Edge> GetResultantQuadFromEdgeRemoval(Edge e)
        {
            if (e.Shapes.Count != 2)
                return null;

            List<Edge> edges = new List<Edge>(4);

            if (e.Shapes[0] is Triangle)
                edges.AddRange(e.Shapes[0].Edges);
            else
                return null;
            if (e.Shapes[1] is Triangle)
                edges.AddRange(e.Shapes[1].Edges);
            else
                return null;

            // Eliminate duplicates of e
            edges.Remove(e);
            edges.Remove(e);

            return edges;
        }

        #region Convexity test
        public bool IsConvexHull(List<Edge> edges)
        {
            int numEdges = edges.Count;
            if (numEdges < 1)
                throw new MemberAccessException("The shape does not have enough edges");

            Vertex[] vs = new Vertex[numEdges];

            vs[0] = edges[0].V1;
            vs[1] = getNextVertex(vs[0], null, edges);

            for (int i = 1; i < numEdges - 1; i++)
                vs[i + 1] = getNextVertex(vs[i], vs[i - 1], edges);

            Edge[] es = new Edge[numEdges];
            es[0] = new Edge(0, vs[0], vs[1]);
            es[1] = new Edge(0, vs[1], vs[2]);
            es[2] = new Edge(0, vs[2], vs[3]);
            es[3] = new Edge(0, vs[3], vs[0]);
            double angleSum = 0.0;
            for (int i=0;i<numEdges-1;i++)
                angleSum += getAngleCos(es[i], es[i+1]);
            angleSum += getAngleCos(es[numEdges - 1], es[0]);

            if (Math.Abs(angleSum) > 0.1)
                return false;

            return true;            
        }

        private double getAngleCos(Edge e1, Edge e2)
        {
            double len1 = e1.Length();
            double len2 = e2.Length();
            return (e1.V1.X - e1.V2.X) / len1 * (e2.V2.X - e2.V1.X) / len2 + (e1.V1.Y - e1.V2.Y) / len1 * (e2.V2.Y - e2.V1.Y) / len2;
        }

        private Vertex getNextVertex(Vertex v, Vertex lastV, List<Edge> edges)
        {
            foreach (Edge e in edges)
            {
                if (e.V1 == v && e.V2 != lastV)
                    return e.V2;
                if (e.V2 == v && e.V1 != lastV)
                    return e.V1;
            }

            return null;
        }
        #endregion

        public bool RemoveEdgeAndMerge(Edge e)
        {
            List<Edge> quadEdges = GetResultantQuadFromEdgeRemoval(e);

            if (quadEdges != null && quadEdges.Count == 4)
            {
                if (!IsConvexHull(quadEdges))
                    return false;

                e.V1.Edges.Remove(e);
                e.V2.Edges.Remove(e);

                Shape[] eShapes = e.Shapes.ToArray();
                foreach (Shape s in eShapes)
                    RemoveShape(s);

                AddQuad(quadEdges[0], quadEdges[1], quadEdges[2], quadEdges[3]);
                return true;
            }

            return false;
        }

        public void SplitEdge(Edge e, List<double> ts)
        {
            if (ts.Count < 1)
                return;

            RemoveEdge(e);
            double t;
            
            Vertex vOld = e.V1;
            for (int i = 0; i < ts.Count; i++)
            {
                t = ts[i];
                Vertex v = AddVertex(e.V1.X + t * (e.V2.X - e.V1.X), e.V1.Y + t * (e.V2.Y - e.V1.Y), e.IsBoundary);

                AddEdge(vOld, v, e.IsBoundary);
                vOld = v;
            }

            AddEdge(vOld, e.V2, e.IsBoundary);
        }

        public void SplitEdge(Edge e, double t, out Edge e1, out Edge e2, out Vertex v)
        {
            RemoveEdge(e);
            v = AddVertex(e.V1.X + t * (e.V2.X - e.V1.X), e.V1.Y + t * (e.V2.Y - e.V1.Y), e.IsBoundary);

            e1 = AddEdge(e.V1, v, e.IsBoundary);
            e2 = AddEdge(v, e.V2, e.IsBoundary);
        }

        public List<Edge> GetEdgeList()
        {
            return GetEdgeList(false);
        }

        public List<Edge> GetEdgeList(bool onlyEdgesWithShapes)
        {
            List<Edge> edges = new List<Edge>();

            Vertex u;
            Queue<Vertex> next = new Queue<Vertex>();
            int[] parent = new int[vertices.Count];
            for (int i = 0; i < parent.Length; i++)
                parent[i] = -1;

            next.Enqueue(vertices[0]);
            parent[0] = 0;

            while (next.Count != 0)
            {
                u = next.Dequeue();
                foreach (Edge e in u.Edges)
                {
                    if (e.V1 == u && (!onlyEdgesWithShapes || e.Shapes.Count > 0))
                        edges.Add(e);

                    if (e.V1 == u && parent[e.V2.Id] == -1)
                    {
                        parent[e.V2.Id] = u.Id;
                        next.Enqueue(e.V2);
                    }
                    else if (e.V2 == u && parent[e.V1.Id] == -1)
                    {
                        parent[e.V1.Id] = u.Id;
                        next.Enqueue(e.V1);
                    }
                }
            }

            return edges;
        }

        private void renumberVertices()
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i].Id = i;
        }
        
        /// <summary>
        /// Joins the Mesh. Ignores any Shapes
        /// </summary>
        /// <param name="tolerance">Tolerance for the join</param>
        /// <exception cref="InvalidOperationException">Throws exception when Shapes are found in the Mesh</exception>
        internal void Join(double tolerance)
        {
            if (shapes.Count > 0)
                throw new InvalidOperationException("No Shapes allowed when joining Mesh");

            if (vertices.Count == 0) return;

            for (int i = 0; i < vertices.Count; i++)
                for (int j = vertices.Count - 1; j > i; j--)
                    if (vertices[i].Equals(vertices[j], tolerance))
                    {
                        Vertex vIn = vertices[i], vOut = vertices[j];
                        for (int k = vOut.Edges.Count - 1; k >= 0; k--)
                        {
                            Edge e = vOut.Edges[k];

                            if (e.V1 == vOut)
                            {
                                e.V1 = vIn;
                                vIn.Edges.Add(e);
                                vOut.Edges.RemoveAt(k);
                            }
                            if (e.V2 == vOut)
                            {
                                e.V2 = vIn;
                                vIn.Edges.Add(e);
                                vOut.Edges.RemoveAt(k);
                            }
                        }
                        
                        removeVertex(vOut, false);
                    }

            renumberVertices();
            
            // Eliminate loop edges
            Edge[] edges = GetEdgeList().ToArray();
            foreach (Edge e in edges)
                if (e.V1 == e.V2)
                    RemoveEdge(e);

            // Eliminate duplicate edges
            for (int i = 0; i < edges.Length; i++)
                for (int j = edges.Length - 1; j > i; j--)
                    if ((edges[i].V1 == edges[j].V1 && edges[i].V2 == edges[j].V2) ||
                        (edges[i].V1 == edges[j].V2 && edges[i].V2 == edges[j].V1))
                        RemoveEdge(edges[j]);
        }
    }
}
