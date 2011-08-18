using System;

namespace Canguro.Analysis.Sections.Meshing.Delaunay
{
    /// <summary>A class defining a 3d point (x, y, z) and some methods for the Delaunay algorithm.</summary>
    public class Point
    {
        /// <summary>The x-coordinate of the point.</summary>
        public double X;
        /// <summary>The y-coordinate of the point.</summary>
        public double Y;
        /// <summary>The z-coordinate of the point.</summary>
        public double Z;

        public int GlobalId;
        public int LocalId;

        #region Constructor

        /// <summary>Constructs a point from three coordinates (x, y, z).</summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <param name="z">The z-coordinate of the point.</param>
        public Point(double x, double y, double z, int globalId)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.GlobalId = globalId;
        }

        #endregion

        #region Operators

        /// <summary>A hash code for the point.</summary>
        /// <returns>Returns the hash code for the point.</returns>
        public override int GetHashCode()
        {
            int xHc = this.X.ToString().GetHashCode();
            int yHc = this.Y.ToString().GetHashCode();
            int zHc = this.Z.ToString().GetHashCode();

            return xHc ^ yHc ^ zHc;
        }

        /// <summary>Tests if two points are considered equal.</summary>
        /// <param name="obj">A <see cref="Point"/> object.</param>
        /// <returns>Returns true if two points are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return this == (Point)obj;
        }

        /// <summary>Tests if two points are considered equal.</summary>
        /// <param name="left">A <see cref="Point"/>.</param>
        /// <param name="right">A <see cref="Point"/>.</param>
        /// <returns>Returns true if two points are considered equal, false otherwise.</returns>
        /// <remarks>Two points are considered equal if the x and y coordinate of one point
        /// equals the respective coordinate of the other point.</remarks>
        public static bool operator ==(Point left, Point right)
        {
            if ( ( (object)left ) == ( (object)right ) )
            {
                return true;
            }

            if ( ( ( (object)left ) == null ) || ( ( (object)right ) == null ) )
            {
                return false;
            }

            // Just compare x and y here...
            if ( left.X != right.X ) return false;
            if ( left.Y != right.Y ) return false;

            return true;
        }

        /// <summary>Tests if two points are considered equal.</summary>
        /// <param name="left">A <see cref="Point"/>.</param>
        /// <param name="right">A <see cref="Point"/>.</param>
        /// <returns>Returns false if two points are considered equal, true otherwise.</returns>
        /// <remarks>Two points are considered equal if the x and y coordinate of one point
        /// equals the respective coordinate of the other point.</remarks>
        public static bool operator !=(Point left, Point right)
        {
            return !( left == right );
        }

        #endregion
    }
}
