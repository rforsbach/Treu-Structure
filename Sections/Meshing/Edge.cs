using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Analysis.Sections.Meshing
{
    public class Edge
    {
        Vertex v1, v2;
        List<Shape> shapes;
        List<Edge> derivedEdges = null;
        bool isBoundary = false;
        int id;

        public int Id
        {
            get { return id; }
        }

        public bool IsBoundary
        {
            get { return isBoundary; }
            set { isBoundary = value; }
        }

        /// <summary>
        /// Gets or sets the list of derived edges that appear when splittig an edge. 
        /// This property can return null if there are no derived edges.
        /// </summary>
        public List<Edge> DerivedEdges
        {
            get { return derivedEdges; }
            set { derivedEdges = value; }
        }

        /// <summary>
        /// Gets the list of shapes connected to this edge
        /// </summary>
        public List<Shape> Shapes
        {
            get { return shapes; }
        }

        /// <summary>
        /// Gets or sets the second vertex of this edge
        /// </summary>
        public Vertex V2
        {
            get { return v2; }
            set { v2 = value; }
        }

        /// <summary>
        /// Gets or sets the first vertex of this edge
        /// </summary>
        public Vertex V1
        {
            get { return v1; }
            set { v1 = value; }
        }

        public Edge(int id)
        {
            this.id = id;
            shapes = new List<Shape>();
        }

        public Edge(int id, Vertex v1, Vertex v2) : this(id)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public bool ContainsVertex(Vertex v)
        {
            return (v1 == v || v2 == v);
        }

        public double Length()
        {
            return Math.Sqrt((v2.X - v1.X) * (v2.X - v1.X) + (v2.Y - v1.Y) * (v2.Y - v1.Y));
        }

        public List<Shape> IsAdjacentTo(Edge e)
        {
            List<Shape> adjacentShapes = new List<Shape>();

            foreach (Shape s in shapes)
            {
                if (s.ContainsEdge(e))
                    adjacentShapes.Add(s);
            }

            return adjacentShapes;
        }

        public Shape IsAdjacentTo(Edge e1, Edge e2)
        {
            foreach (Shape s in shapes)
            {
                if (s.ContainsEdge(e1) && s.ContainsEdge(e2))
                    return s;
            }

            return null;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}), ( ({2}), ({3}) )", v1.Id, v2.Id, v1.ToString(), v2.ToString());
        }
    }
}
