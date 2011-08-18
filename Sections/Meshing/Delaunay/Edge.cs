using System;

namespace Canguro.Analysis.Sections.Meshing.Delaunay
{
    /// <summary>A class defining an edge and some methods for the Delaunay algorithm.</summary>
    public class Edge
    {
        /// <summary>The start point of the edge.</summary>
        public Point StartPoint;
        /// <summary>The end point of the edge.</summary>
        public Point EndPoint;

        #region Constructor 

        /// <summary>Constructs an edge from two points.</summary>
        /// <param name="startPoint">The start point of the edge.</param>
        /// <param name="endPoint">The end point of the edge.</param>
        public Edge(Point startPoint, Point endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

        #endregion

        #region Operators

        /// <summary>A hash code for this edge.</summary>
        /// <returns>Returns the hash code for this edge.</returns>
        public override int GetHashCode()
        {
            return this.StartPoint.GetHashCode() ^ this.EndPoint.GetHashCode();
        }

        /// <summary>Tests if two edges are considered equal.</summary>
        /// <param name="obj">An <see cref="Edge"/> object.</param>
        /// <returns>Returns true if two edges are considered equal, false otherwise.</returns>
        /// <remarks>Two edges are considered equal if they contain the same points.
        /// This is, two equal edges may have interchanged start and end points.</remarks>
        public override bool Equals(object obj)
        {
            return this == (Edge)obj;
        }

        /// <summary>Tests if two edges are equal.</summary>
        /// <param name="left">A first <see cref="Edge"/>.</param>
        /// <param name="right">A second <see cref="Edge"/>.</param>
        /// <returns>Returns true if two edges are considered equal, false otherwise.</returns>
        /// <remarks>Two edges are considered equal if they contain the same points.
        /// This is, two equal edges may have interchanged start and end points.</remarks>
        public static bool operator ==(Edge left, Edge right)
        {
            if ( ( (object)left ) == ( (object)right ) )
            {
                return true;
            }

            if ( ( ( (object)left ) == null ) || ( ( (object)right ) == null ) )
            {
                return false;
            }

            return ( ( left.StartPoint == right.StartPoint && left.EndPoint == right.EndPoint ) ||
                     ( left.StartPoint == right.EndPoint && left.EndPoint == right.StartPoint ) );
        }

        /// <summary>Tests if two edges are considered equal.</summary>
        /// <param name="left">A first <see cref="Edge"/>.</param>
        /// <param name="right">A second <see cref="Edge"/>.</param>
        /// <returns>Returns false if two edges are considered equal, true otherwise.</returns>
        /// <remarks>Two edges are considered equal if they contain the same points.
        /// This is, two equal edges may have interchanged start and end points.</remarks>
        public static bool operator !=(Edge left, Edge right)
        {
            return left != right;
        }

        #endregion

    }
}
