using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Canguro.Analysis.Sections.Meshing
{
    public class Quadrangulator
    {
        public const int NumQuads = 30;

        Mesh mesh = null;

        public Quadrangulator()
        {
            mesh = new Mesh();
        }

        public Mesh Mesh
        {
            get { return mesh; }
        }

        public void Quadrangulate(IList<IList<PointF>> contours, IList<Material> materials)
        {
            triangulate(contours, materials);

            if (mesh.Vertices.Count < 3) return;

            double area = getPolygonArea();
            if (area < Triangulator.IntersectionEpsilon) return;
            double dist = Math.Sqrt(area / NumQuads);

            List<Edge> edges = mesh.GetEdgeList();

            // Split the contour to create boundary grid vertices and
            splitContour(dist, edges);

            // Create interior grid vertices
            addInteriorGridPoints(dist, edges);

            // Triangulate mesh
            Triangulator.Triangulate(mesh);

            // Convert to Quad Dominant Mesh
            makeQuadDominant(mesh);

            // Convert to all Quad Mesh
            makeAllQuads(mesh);
        }

        private void triangulate(IList<IList<PointF>> contours, IList<Material> materials)
        {
            List<Vertex> vertices = mesh.Vertices;

            // Apply contour operations (intersections, additions, etc) to obtain 
            // one contour (+ holes) per triangulation, and therefore, one mesh per
            // triangulation
            
            // Add boundary vertices and boundary edges to mesh
            foreach (List<PointF> con in contours)
            {
                Vertex last = null, first = null;
                foreach (PointF p in con)
                {
                    Vertex v = mesh.AddVertex(p.X, p.Y, true, true);

                    if (last != null)
                        mesh.AddEdge(last, v, true);
                    else
                        first = v;
                    
                    last = v;
                }

                if (first != null && last != null)
                    mesh.AddEdge(last, first, true);
            }

            mesh.Join(Triangulator.IntersectionEpsilon);
        }

        private void makeQuadDominant(Mesh mesh)
        {
            List<Edge> edges = mesh.GetEdgeList();
            double onesqrtwo = 1.0 / Math.Sqrt(2.0);
            List<lambdaEdge> lambdas = new List<lambdaEdge>();

            foreach (Edge e in edges)
                if (!e.IsBoundary)
                {
                    double Lambda = 0.0;
                    List<Edge> adjE = mesh.GetResultantQuadFromEdgeRemoval(e);

                    if (adjE != null)
                    {
                        foreach (Edge ae in adjE)
                        {
                            double len = ae.Length();
                            double lambda = Math.Abs(ae.V2.X - ae.V1.X) / len;

                            if (lambda < onesqrtwo)
                                lambda = Math.Sqrt(1.0 - (lambda * lambda));

                            Lambda += lambda;
                        }

                        lambdas.Add(new lambdaEdge(Lambda, e));
                    }
                }

            lambdaSort(lambdas);

            int j = 0;
            for (int i = 0; i < lambdas.Count; i++)
            {
                Edge e = lambdas[i].e;
                if (e.Shapes.Count == 2 && (e.Shapes[0] is Triangle && e.Shapes[1] is Triangle))
                {
                    if (mesh.RemoveEdgeAndMerge(e))
                        j++;
                }
            }
        }

        protected void lambdaSort(List<lambdaEdge> lambdas)
        {
            lambdas.Sort(new Comparison<lambdaEdge>(delegate(lambdaEdge l1, lambdaEdge l2) { if (l1.lambda < l2.lambda) return 1; else if (l1.lambda == l2.lambda) return 0; else return -1; }));
        }

        protected class lambdaEdge
        {
            public double lambda;
            public Edge e;

            public lambdaEdge(double lambda, Edge e)
            {
                this.lambda = lambda;
                this.e = e;
            }

            public override string ToString()
            {
                return string.Format("{0} - ({1})", lambda, e.ToString());
            }
        }

        private void addInteriorGridPoints(double dist, List<Edge> edges)
        {
            LinkedList<PointF> possibleGridPoints = new LinkedList<PointF>();

            // Get grid points that lie inside the polygon
            for (float x = mesh.BoundingBox[0].X; x < mesh.BoundingBox[1].X; x += (float)dist)
                for (float y = mesh.BoundingBox[0].Y; y < mesh.BoundingBox[1].Y; y += (float)dist)
                {
                    PointF pt = new PointF(x, y);
                    if (Triangulator.PointInsidePolygon(pt, edges, mesh.BoundingBox))
                        possibleGridPoints.AddLast(new LinkedListNode<PointF>(pt));
                }

            // Eliminate the points that are too close to existing vertices
            LinkedListNode<PointF> next, pos = possibleGridPoints.First;
            while (pos != null)
            {
                next = pos.Next;
                if (isTooClose(pos.Value, mesh.Vertices, (float)(dist*dist)/4.0f))
                    possibleGridPoints.Remove(pos);
                pos = next;
            }

            foreach (PointF pt in possibleGridPoints)
                mesh.AddVertex(pt.X, pt.Y, false);
        }
       
        private bool isTooClose(PointF pt, List<Vertex> vs, float minDistSqr)
        {
            foreach (Vertex v in vs)
                //if (((pt.X - v.X) * (pt.X - v.X) + (pt.Y - v.Y) * (pt.Y - v.Y)) < (1.5 * minDistSqr))
                if ((pt.X - v.X) * (pt.X - v.X) < minDistSqr && (pt.Y - v.Y) * (pt.Y - v.Y) < minDistSqr)
                        return true;

            return false;
        }

        private void splitContour(double dist, List<Edge> edges)
        {
            double t, s;
            List<double> splitTs = new List<double>();
            foreach (Edge e in edges)
            {
                splitTs.Clear();

                for (double x = mesh.BoundingBox[0].X; x < mesh.BoundingBox[1].X; x += dist)
                    if (Triangulator.LineSegmentsIntersection(e.V1.X, e.V1.Y, e.V2.X, e.V2.Y, x, mesh.BoundingBox[0].Y - 1.0, x, mesh.BoundingBox[1].Y + 1.0, Triangulator.IntersectionEpsilon, out t, out s) == Triangulator.IntersectionResult.Intersect)
                        splitTs.Add(t);
                for (double y = mesh.BoundingBox[0].Y; y < mesh.BoundingBox[1].Y; y += dist)
                    if (Triangulator.LineSegmentsIntersection(e.V1.X, e.V1.Y, e.V2.X, e.V2.Y, mesh.BoundingBox[0].X - 1.0, y, mesh.BoundingBox[1].X + 1.0, y, Triangulator.IntersectionEpsilon, out t, out s) == Triangulator.IntersectionResult.Intersect)
                        splitTs.Add(t);

                double minTDist = dist / e.Length() / 2.0;
                splitTs.Sort();
                
                while ((splitTs.Count > 0) && (splitTs[0] < minTDist))
                    splitTs.RemoveAt(0);

                while ((splitTs.Count > 0) && (splitTs[splitTs.Count - 1] > (1.0 - minTDist)))
                    splitTs.RemoveAt(splitTs.Count - 1);
                
                for (int i = 0; i < splitTs.Count - 1; i++)
                    if (splitTs[i + 1] - splitTs[i] < minTDist)
                        splitTs.RemoveAt(i + 1);
                
                mesh.SplitEdge(e, splitTs);
            }
        }

        private double getPolygonArea()
        {
            // Obtener area del poligono (http://en.wikipedia.org/wiki/Polygon_area#Properties)
            // dist = Sqrt(area / NumQuads)
            double area = 0;
            for (int i = 0; i < mesh.Vertices.Count - 1; i++)
                area += mesh.Vertices[i].X * mesh.Vertices[i + 1].Y - mesh.Vertices[i + 1].X * mesh.Vertices[i].Y;
            area += mesh.Vertices[mesh.Vertices.Count - 1].X * mesh.Vertices[0].Y - mesh.Vertices[0].X * mesh.Vertices[mesh.Vertices.Count - 1].Y;
            area *= 0.5;
            return Math.Abs(area);
        }

        private class EdgeReplacements
        {
            public Vertex newV;
            public Edge new1, new2;
            public EdgeReplacements(Edge new1, Edge new2, Vertex newV)
            {
                this.new1 = new1;
                this.new2 = new2;
                this.newV = newV;
            }
        }

        private void makeAllQuads(Mesh mesh)
        {
            Queue<Shape> quadQueue = new Queue<Shape>();
            Dictionary<Edge, EdgeReplacements> newEdges = new Dictionary<Edge, EdgeReplacements>();

            if (mesh.Shapes == null || mesh.Shapes.Count == 0) return;

            quadQueue.Enqueue(mesh.Shapes[0]);
            mesh.RemoveShape(mesh.Shapes[0]);

            // Asumming connected mesh
            while (quadQueue.Count != 0)
            {
                Shape s = quadQueue.Dequeue();

                foreach (Edge e in s.Edges)
                {
                    // Remove and Enqueue adjacent Shapes
                    if (e.Shapes.Count > 0)
                    {
                        Shape adjacent = e.Shapes[0];
                        mesh.RemoveShape(adjacent);
                        quadQueue.Enqueue(adjacent);
                    }


                    // Split adjacent edges
                    Vertex newV;
                    Edge newEdge1, newEdge2;
                    if (!newEdges.ContainsKey(e))
                    {
                        mesh.SplitEdge(e, 0.5, out newEdge1, out newEdge2, out newV);
                        newEdges.Add(e, new EdgeReplacements(newEdge1, newEdge2, newV));
                    }
                }

                // (Re-)Quadrangulate
                
                // Create center vertex
                PointF ptc = s.GetCentroid();
                Vertex vc = mesh.AddVertex(ptc.X, ptc.Y, false);

                // Get ordered (and splitted) vertices and edges
                Vertex[] vs = s.Vertices;
                List<Edge> orderedNewEdges = new List<Edge>();
                //Vertex v1 = null, v2 = null, v3 = null, v4 = null;
                List<Vertex> orderedvertices = new List<Vertex>();

                for (int i = 0; i < vs.Length; i++)
                {
                    Edge e1, e2;

                    getVertexEdges(vs[i], s.Edges, newEdges, out e1, out e2);
                    orderedNewEdges.Add(e1);
                    orderedNewEdges.Add(e2);

                    Vertex midV = getMidVertex(vs[i], vs[(i + 1) % vs.Length], s.Edges, newEdges);
                    if (midV != null)
                        orderedvertices.Add(midV);
                }

                // Create new Edges
                Vertex lastV = orderedvertices[orderedvertices.Count - 1];
                List<Edge> crossEdges = new List<Edge>();
                foreach (Vertex v in orderedvertices)
                    crossEdges.Add(mesh.AddEdge(vc, v));

                // Create new Quads
                int numVs = orderedvertices.Count;
                for (int j, i = 0; i < numVs; i++)
                {
                    j = i - 1;
                    if (j < 0) j = numVs + j;                    
                    mesh.AddQuad(orderedNewEdges[2 * i], orderedNewEdges[(2 * i + 1) % (2 * numVs)], crossEdges[i], crossEdges[j]);
                }
            }
        }

        private Vertex getMidVertex(Vertex v1, Vertex v2, List<Edge> edges, Dictionary<Edge, EdgeReplacements> newEdges)
        {
            foreach (Edge e in edges)
            {
                if (e.ContainsVertex(v1) && e.ContainsVertex(v2))
                    return newEdges[e].newV;
            }

            return null;
        }

        private void getVertexEdges(Vertex v, List<Edge> edges, Dictionary<Edge, EdgeReplacements> er, out Edge e1, out Edge e2)
        {
            int i = 0;
            e1 = e2 = null;
            // Find old Edges
            foreach (Edge e in edges)
                if (e.ContainsVertex(v))
                {
                    if (i == 0)
                        e1 = e;
                    else if (i == 1)
                    {
                        e2 = e;
                        break;
                    }
                    ++i;
                }

            System.Diagnostics.Debug.Assert(e1 != null && e2 != null);

            // Find new Edges
            EdgeReplacements er1 = er[e1];
            EdgeReplacements er2 = er[e2];

            if (er1.new1.ContainsVertex(v))
                e1 = er1.new1;
            else
                e1 = er1.new2;

            if (er2.new1.ContainsVertex(v))
                e2 = er2.new1;
            else
                e2 = er2.new2;
        }
    }
}
