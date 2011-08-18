using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Canguro.Analysis.Sections.Meshing
{
    public class Triangulator
    {
        public enum IntersectionResult
        {
            Intersect,
            NotIntersect,
            Parallel,
            TooCloseToDecide,
            ShareVertex
        }
        public const double IntersectionEpsilon = 1.0e-20;
        public enum InsideResult
        {
            Inside,
            Outside,
            TooCloseToDecide
        }

        private Triangulator() { }
        
        public static void Triangulate(Mesh mesh)
        {
            List<Delaunay.Point> vertices;

            if (mesh.Vertices.Count > 2)
            {                
                // Get Delauney vertices
                vertices = new List<Canguro.Analysis.Sections.Meshing.Delaunay.Point>();
                foreach (Vertex v in mesh.Vertices)
                    vertices.Add(new Canguro.Analysis.Sections.Meshing.Delaunay.Point(v.X, v.Y, 0.0, v.Id));

                // Create mesh from Delauney triangulation
                mesh.CreateMesh(Delaunay.DelaunayTriangulation2d.Triangulate(vertices), mesh.Vertices);

                // Get list of Edges in mesh
                List<Edge> edges = mesh.GetEdgeList();                
                
                // Correct triangles that do not conform with the boundary Edges
                correctTriangles(mesh,edges);
            }
        }

        protected static void correctTriangles(Mesh mesh, List<Edge> edges)
        {
            bool[] killVIds;
            List<Vertex> killVLeft = new List<Vertex>();
            List<Vertex> killVRight = new List<Vertex>();
            List<Shape> killShapes = new List<Shape>();
            List<Edge> bEdges = new List<Edge>();

            // Find Edge Intersections
            foreach (Edge b in edges)
                if (b.IsBoundary)
                    bEdges.Add(b);

            // Find Edge Intersections
            foreach (Edge b in bEdges)
            {
                killVLeft.Clear();
                killVRight.Clear();
                killVIds = new bool[mesh.Vertices.Count];
                PointF bNormal = new PointF((float)(b.V2.Y - b.V1.Y), -((float)(b.V2.X - b.V1.X)));

                // Get list of Edges in mesh
                edges = mesh.GetEdgeList();                

                foreach (Edge e in edges)
                {
                    if (!e.IsBoundary)
                    {
                        if (edgesIntersection(b, e) == IntersectionResult.Intersect)
                        {
                            // Delete Edge
                            mesh.RemoveEdge(e);

                            // Add Edge vertices to Left/Right re-triangulation lists
                            if (!killVIds[e.V1.Id])
                            {
                                float dot = (float)((e.V1.X - b.V1.X) * bNormal.X + (e.V1.Y - b.V1.Y) * bNormal.Y);

                                if (dot < 0)
                                    killVLeft.Add(e.V1);
                                else
                                    killVRight.Add(e.V1);

                                killVIds[e.V1.Id] = true;
                            }

                            if (!killVIds[e.V2.Id])
                            {
                                float dot = (float)((e.V2.X - b.V1.X) * bNormal.X + (e.V2.Y - b.V1.Y) * bNormal.Y);

                                if (dot < 0)
                                    killVLeft.Add(e.V2);
                                else
                                    killVRight.Add(e.V2);

                                killVIds[e.V2.Id] = true;
                            }
                        }
                    }
                }

                // Re-Triangulate
                re_triangulate(mesh, killVLeft, b);
                re_triangulate(mesh, killVRight, b);
            }

            foreach (Shape s in mesh.Shapes)
            {
                PointF pt = s.GetInsidePoint();
                if (!PointInsidePolygon(pt, bEdges, mesh.BoundingBox))
                    killShapes.Add(s);
            }

            // Kill outside Shapes
            foreach (Shape s in killShapes)
                foreach (Edge e in s.Edges)
                    if (!e.IsBoundary)
                        mesh.RemoveEdge(e);
        }

        public static bool PointInsidePolygon(PointF pt, List<Edge> bEdges, PointF[] bb)
        {
            Random random = new Random();
            double rand = 0.0;
            InsideResult inside = InsideResult.Outside;
            int cycles = 0;

            do
            {
                inside = insidePolygonTest(pt, bEdges, bb, rand);
                rand = random.NextDouble() * (++cycles/10.0);
            } while (inside == InsideResult.TooCloseToDecide && cycles < 100);

            return (inside == InsideResult.Inside);
        }

        protected static InsideResult insidePolygonTest(PointF pt, List<Edge> bEdges, PointF[] bb, double dy)
        {
            int numIntersections;
            double t, s;
            IntersectionResult intRes = IntersectionResult.NotIntersect;

            numIntersections = 0;
            foreach (Edge b in bEdges)
            {
                if (b.V1.Y != b.V2.Y)
                {
                    intRes = LineSegmentsIntersection(b.V1.X, b.V1.Y, b.V2.X, b.V2.Y, pt.X, pt.Y, pt.X + 2.0 * bb[2].X, pt.Y + dy, IntersectionEpsilon, out t, out s);

                    if (intRes == IntersectionResult.Intersect)
                        numIntersections++;
                    else if (intRes == IntersectionResult.TooCloseToDecide)
                        return InsideResult.TooCloseToDecide;
                }
            }

            if (numIntersections % 2 == 1)
                return InsideResult.Inside;

            return InsideResult.Outside;
        }

        protected static void re_triangulate(Mesh mesh, List<Vertex> vs, Edge boundary)
        {
            if (vs.Count > 0)
            {
                List<Delaunay.Point> vertices = new List<Canguro.Analysis.Sections.Meshing.Delaunay.Point>();
                vs.Add(boundary.V1);
                vs.Add(boundary.V2);
                foreach (Vertex v in vs)
                    vertices.Add(new Canguro.Analysis.Sections.Meshing.Delaunay.Point(v.X, v.Y, 0.0, v.Id));

                mesh.CreateMesh(Delaunay.DelaunayTriangulation2d.Triangulate(vertices), vs);
            }
        }

        protected static IntersectionResult edgesIntersection(Edge e1, Edge e2)
        {
            double s, t;
            return edgesIntersection(e1, e2, out t, out s);
        }

        protected static IntersectionResult edgesIntersection(Edge e1, Edge e2, out double t, out double s)
        {
            // If two edges share a vertex they cannot intersect
            if (e1.V1 == e2.V1 || e1.V1 == e2.V2 || e1.V2 == e2.V1 || e1.V2 == e2.V2)
            {
                t = 0;
                s = 0;
                return IntersectionResult.ShareVertex;
            }

            return LineSegmentsIntersection(e1.V1.X, e1.V1.Y, e1.V2.X, e1.V2.Y, e2.V1.X, e2.V1.Y, e2.V2.X, e2.V2.Y, IntersectionEpsilon, out t, out s);
        }

        public static IntersectionResult LineSegmentsIntersection(double x1_1, double y1_1, double x1_2, double y1_2, double x2_1, double y2_1, double x2_2, double y2_2, double tolerance, out double t, out double s)
        {
            PointF dir1 = new PointF((float)(x1_2 - x1_1), (float)(y1_2 - y1_1));
            PointF dir2 = new PointF((float)(x2_2 - x2_1), (float)(y2_2 - y2_1));

            double tmp = dir2.X * dir1.Y - dir2.Y * dir1.X;

            if (tmp == 0f)
            {
                t = 0;
                s = 0;
                return IntersectionResult.Parallel;
            }

            t = (dir2.Y * (x1_1 - x2_1) + dir2.X * (y2_1 - y1_1)) / tmp;

            //if (t >= 0 + tolerance && t <= 1 - tolerance)
            //{
                //double s;
                if (dir2.Y != 0)
                    s = (y1_1 + t * dir1.Y - y2_1) / dir2.Y;
                else
                    s = (x1_1 + t * dir1.X - x2_1) / dir2.X;

                bool sNear0 = (s >= 0 - tolerance && s <= 0 + tolerance);
                bool sNear1 = (s <= 1 + tolerance && s >= 1 - tolerance);
                bool tNear0 = (t >= 0 - tolerance && t <= 0 + tolerance);
                bool tNear1 = (t <= 1 + tolerance && t >= 1 - tolerance);
            bool sOK = (s >= 0 + tolerance && s <= 1 - tolerance);
            bool tOK = (t >= 0 + tolerance && t <= 1 - tolerance);

            if (sOK && tOK)
                return IntersectionResult.Intersect;
            else if ((sNear0 && (tOK || tNear0 || tNear1)) || 
                    (sNear1 && (tOK || tNear0 || tNear1)) || 
                    (sOK && (tNear0 || tNear1))) 
                    return IntersectionResult.TooCloseToDecide;

            //}
            //else if (t >= 0 - tolerance && t <= 1 + tolerance)
            //    return IntersectionResult.TooCloseToDecide;

            return IntersectionResult.NotIntersect;
        }

        //protected void AddGridVertices(bool[,] surface, int distance)
        //{
        //    int w = surface.GetLength(0);
        //    int h = surface.GetLength(1);
        //    for (int i=0; i<w; i+=distance)
        //        for (int j=0; j<h; j+=distance)
        //            if (surface[i, j])
        //            {
        //                Delauney.Vertex v = new Delauney.Vertex(i, j);
        //                if (!vertices.Exists(delegate(Delauney.Vertex ve) { return v.Equals(ve); }))
        //                    vertices.Add(v);
        //            }
        //}
    }
}
