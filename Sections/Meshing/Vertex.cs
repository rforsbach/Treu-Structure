using System;
using System.Collections.Generic;

namespace Canguro.Analysis.Sections.Meshing
{
    public class Vertex
    {
        int id;
        double x, y;
        bool isBoundary = false;
        List<Edge> edges;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public List<Edge> Edges
        {
            get { return edges; }
        }

        public bool IsBoundary
        {
            get { return isBoundary; }
            set { isBoundary = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public Vertex(int id)
        {
            this.id = id;
            edges = new List<Edge>();
        }

        public Vertex(int id, double x, double y) : this(id)
        {
            this.x = x;
            this.y = y;
        }

        public Vertex(int id, double x, double y, bool isBoundary) : this(id, x, y)
        {
            this.isBoundary = isBoundary;
        }

        /// <summary>
        /// Returns the Edge that connects the two vertices (if any). Otherwise returns null
        /// </summary>
        /// <param name="v">The vertice to check connectivity with</param>
        /// <returns>The Edge that makes the two vertices adjacent</returns>
        public Edge IsAdjacentTo(Vertex v)
        {
            foreach (Edge e in edges)
                if (e.ContainsVertex(v))
                    return e;

            return null;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1} : [{2}]", x, y, id);
        }

        public override bool Equals(object obj)
        {
            Vertex v = (Vertex)obj;
            return Equals(v, 1e-5);
        }

        public bool Equals(Vertex v, double tolerance)
        {
            return (v.x - x) * (v.x - x) + (v.y - y) * (v.y - y) < tolerance * tolerance;
        }
    }
}
