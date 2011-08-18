using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Analysis.Sections.Meshing
{
    public abstract class Shape
    {
        protected List<Edge> edges;
        int color;

        /// <summary>
        /// Gets or sets the color of the shape
        /// </summary>
        public int Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Gets the edges of the shape
        /// </summary>
        public List<Edge> Edges
        {
            get { return edges; }
        }

        public List<Edge> OrderedEdges
        {
            get
            {
                List<Edge> ov = new List<Edge>(edges.Count);
                Vertex[] vs = Vertices;
                for (int i = 0; i < vs.Length - 1; i++)
                {
                    foreach (Edge e in edges)
                        if (e.ContainsVertex(vs[i]) && e.ContainsVertex(vs[i + 1]))
                            ov.Add(e);
                }

                foreach (Edge e in edges)
                    if (e.ContainsVertex(vs[vs.Length - 1]) && e.ContainsVertex(vs[0]))
                        ov.Add(e);

                return ov;
            }
        }

            /// <summary>
            /// Gets the vertices of this shape ordered following the edges
            /// </summary>
            public Vertex[] Vertices
        {
            get
            {
                int numEdges = edges.Count;
                if (numEdges != getNumEdges() || numEdges < 1) 
                    throw new MemberAccessException("The shape does not have enough edges");

                Vertex[] vertices = new Vertex[numEdges];

                vertices[0] = edges[0].V1;
                vertices[1] = getNextVertex(vertices[0], null);

                for (int i = 1; i < numEdges - 1; i++)
                    vertices[i + 1] = getNextVertex(vertices[i], vertices[i - 1]);

                // Return vertices en CCW order
                int numV = vertices.Length;
                if (numV > 2)
                {
                    if (((vertices[0].X - vertices[1].X) * (vertices[2].Y - vertices[1].Y) -
                        (vertices[0].Y - vertices[1].Y) * (vertices[2].X - vertices[1].X)) > 0)
                    {
                        for (int i = 0; i < numV / 2; i++)
                        {
                            Vertex swap = vertices[i];
                            vertices[i] = vertices[numV - i - 1];
                            vertices[numV - i - 1] = swap;
                        }
                    }
                }
                
                return vertices;
            }
        }

        private Vertex getNextVertex(Vertex v, Vertex lastV)
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

        private bool _verticesContains(Vertex[] vs, Vertex v, int i)
        {
            for (int j = 0; j < i; j++)
                if (vs[j] == v)
                    return true;
            
            return false;
        }

        public Shape()
        {
            edges = new List<Edge>();
        }

        protected abstract int getNumEdges();
        public abstract System.Drawing.PointF GetCentroid();

        public bool ContainsEdge(Edge edge)
        {
            foreach (Edge e in edges)
                if (edge == e)
                    return true;

            return false;
        }

        public virtual System.Drawing.PointF GetInsidePoint()
        {
            return new System.Drawing.PointF((float)(edges[0].V1.X + edges[0].V2.X + edges[1].V1.X + edges[1].V2.X) / 4f, (float)(edges[0].V1.Y + edges[0].V2.Y + edges[1].V1.Y + edges[1].V2.Y) / 4f);
        }

        public override string ToString()
        {
            Vertex[] vs = Vertices;
            string ret = string.Empty;
            foreach (Vertex v in vs)
                ret += ", " + v.Id;

            return "(" + ret.Substring(2) + ")";
        }
    }
}
