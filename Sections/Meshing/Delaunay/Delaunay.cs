using System;
using System.Collections.Generic;

// Credit to Paul Bourke (paul.bourke@uwa.edu.au) for the original Fortran 77 Program.
// Check out: http://local.wasp.uwa.edu.au/~pbourke/ 
// You can use this code however you like providing the above credits remain in tact.
// First converted to a standalone C# 2.0 library by Morten Nielsen (www.iter.dk)
// Performance enhanced C# 2.0 library by Christian Stelzl (www.ceometric.com) Sep. 2008

namespace Canguro.Analysis.Sections.Meshing.Delaunay
{
    /// <summary>A 2d Delaunay triangulation class.</summary>
    /// <remarks>The triangulation doesn't support multiple points with identical x and y coordinates,
    /// nor does it support dublicate points.
    /// Vertex-lists with duplicate points may result in strange triangulations with intersecting edges or
    /// may cause the algorithm to fail. 
    /// Uses a simple O(n**2) algorithm based on Paul Bourke's "An Algorithm for Interpolating 
    /// Irregularly-Spaced Data with Applications in Terrain Modelling". 
    /// Uses an enhanced incircle-predicate for counterclockwise orientated triangles.</remarks>
    public class DelaunayTriangulation2d
    {
        /// <summary>Performs the 2d Delaunay triangulation on a set of n vertices in O(n**2) time.</summary>
        /// <param name="triangulationPoints">The points to triangulate.</param>
        /// <returns>A list of Delaunay-triangles.</returns>
        public static List<Triangle> Triangulate(List<Point> triangulationPoints)
        {
            if ( triangulationPoints.Count < 3 ) throw new ArgumentException("Can not triangulate less than three vertices!");

            // The triangle list
            List<Triangle> triangles = new List<Triangle>();

            // The "supertriangle" which encompasses all triangulation points.
            // This triangle initializes the algorithm and will be removed later.
            Triangle superTriangle = SuperTriangle(triangulationPoints);
            triangles.Add(superTriangle);
            
            // For stability purposes            
            // Store the vertices touched (from removed triangles) for each vertex
            int[] touchedVertices = new int[triangulationPoints.Count + 3];

            // Assign vertices' local id's (for touchedVertices indexing)
            for (int i = 0; i < triangulationPoints.Count; i++)
                triangulationPoints[i].LocalId = i;

            // Set a tolerance according to the size of the mesh to check CircumTriangles that are close to 0
            double tolerance;

            // Include each point one at a time into the existing triangulation
            for ( int i = 0; i < triangulationPoints.Count; i++ )
            {
                // Initialize the edge buffer.
                List<Edge> EdgeBuffer = new List<Edge>();

                // If the actual vertex lies inside the circumcircle, then the three edges of the 
                // triangle are added to the edge buffer and the triangle is removed from list.                             
                for ( int j = triangles.Count - 1; j >= 0; j-- )
                {
                    Triangle t = triangles[j];
                    double circum = t.ContainsInCircumcircle(triangulationPoints[i]);
                    
                    // For stability of the algorithm, set a tolerance as a function of the radius of
                    // The circumcircle of t
                    tolerance = 1e-15 * GetCircumRadius(t);

                    if (circum > 0 + tolerance)
                    {
                        EdgeBuffer.Add(new Edge(t.Vertex1, t.Vertex2));
                        EdgeBuffer.Add(new Edge(t.Vertex2, t.Vertex3));
                        EdgeBuffer.Add(new Edge(t.Vertex3, t.Vertex1));
                        triangles.RemoveAt(j);
                        
                        // Set triangle vertices as touched
                        touchedVertices[t.Vertex1.LocalId + 3] = i;
                        touchedVertices[t.Vertex2.LocalId + 3] = i;
                        touchedVertices[t.Vertex3.LocalId + 3] = i;
                    }
                }

                // Remove duplicate edges. This leaves the convex hull of the edges.
                // The edges in this convex hull are oriented counterclockwise!
                for ( int j = EdgeBuffer.Count - 2; j >= 0; j-- )
                {
                    for ( int k = EdgeBuffer.Count - 1; k >= j + 1; k-- )
                    {
                        if ( EdgeBuffer[j] == EdgeBuffer[k] )
                        {
                            EdgeBuffer.RemoveAt(k);
                            EdgeBuffer.RemoveAt(j);
                            k--;
                            continue;
                        }
                    }
                }

                // Generate new counterclockwise oriented triangles filling the "hole" in
                // the existing triangulation. These triangles all share the actual vertex.
                for ( int j = 0; j < EdgeBuffer.Count; j++ )
                {
                    triangles.Add(new Triangle(EdgeBuffer[j].StartPoint, EdgeBuffer[j].EndPoint, triangulationPoints[i]));
                }
            }

            // We don't want the supertriangle in the triangulation, so
            // remove all triangles sharing a vertex with the supertriangle.
            for ( int i = triangles.Count - 1; i >= 0; i-- )
            {
                if ( triangles[i].SharesVertexWith(superTriangle) ) triangles.RemoveAt(i);
            }

            // Return the triangles
            return triangles;
        }

        private static double GetCircumRadius(Triangle t)
        {
            Point v12, v23, v31, v1234;
            v12 = new Point(t.Vertex1.X - t.Vertex2.X, t.Vertex1.Y - t.Vertex2.Y, 0, 0);
            v23 = new Point(t.Vertex2.X - t.Vertex3.X, t.Vertex2.Y - t.Vertex3.Y, 0, 0);
            v31 = new Point(t.Vertex3.X - t.Vertex1.X, t.Vertex3.Y - t.Vertex1.Y, 0, 0);
            v1234 = new Point(0, 0, v12.X * v23.Y - v12.Y * v23.X, 0);

            return Math.Sqrt(
                (v12.X * v12.X + v12.Y * v12.Y) * (v23.X * v23.X + v23.Y * v23.Y) * (v31.X * v31.X + v31.Y * v31.Y) /
                (4.0 * v1234.Z * v1234.Z));
        }

        /// <summary>Returns a triangle that encompasses all triangulation points.</summary>
        /// <param name="triangulationPoints">A list of triangulation points.</param>
        /// <returns>Returns a triangle that encompasses all triangulation points.</returns>
        private static Triangle SuperTriangle(List<Point> triangulationPoints)
        {
            double M = triangulationPoints[0].X;

            // get the extremal x and y coordinates
            for ( int i = 1; i < triangulationPoints.Count; i++ )
            {
                double xAbs = Math.Abs(triangulationPoints[i].X);
                double yAbs = Math.Abs(triangulationPoints[i].Y);
                if ( xAbs > M ) M = xAbs;
                if ( yAbs > M ) M = yAbs;
            }

            // make a triangle
            Point sp1 = new Point(10 * M, 0, 0, -1);
            Point sp2 = new Point(0, 10 * M, 0, -2);
            Point sp3 = new Point(-10 * M, -10 * M, 0, -3);
            sp1.LocalId = -1;
            sp2.LocalId = -2;
            sp3.LocalId = -3;

            return new Triangle(sp1, sp2, sp3);
        }
    }
}
