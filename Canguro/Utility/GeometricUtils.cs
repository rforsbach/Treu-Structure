using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Utility
{
    public class GeometricUtils
    {
        public const float Epsilon = 0.000001f;

        public static float DistancePoint2Plane(ref Vector3 planePoint, ref Vector3 unitaryPlaneNormal, ref Vector3 point)
        {
            return unitaryPlaneNormal.X * (point.X - planePoint.X) + unitaryPlaneNormal.Y * (point.Y - planePoint.Y) + unitaryPlaneNormal.Z * (point.Z - planePoint.Z);
        }

        /// <summary>
        /// 
        /// Extracted from http://www.gamedev.net/community/forums/topic.asp?topic_id=19791 which references the book from Moller and Haines, Real Time Rendering
        /// 
        /// For line 1 we compute:
        /// s = det | O2 - O1 |
        ///         |   D2    |  /  ||D1 X D2||^2
        ///         | D1 X D2 |
        /// For line 2 we compute
        /// t = det | O2 - O1 |
        ///         |   D1    |  /  ||D1 X D2||^2
        ///         | D1 X D2 |
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="d1"></param>
        /// <param name="p2"></param>
        /// <param name="d2"></param>
        /// <param name="t"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool FindIntersection(Vector3 p1, Vector3 d1, Vector3 p2, Vector3 d2, ref float t, ref float s)
        {
            Vector3 d1xd2 = Vector3.Cross(d1, d2);
            float d1xd2SquareLength = d1xd2.LengthSq();

            t = 0.0f;
            s = 0.0f;

            // Lines are parallel
            if (d1xd2SquareLength < float.Epsilon)
                return false;

            Vector3 p1Top2 = p2 - p1;
            float det = p1Top2.X * (d1.Y * d1xd2.Z - d1xd2.Y * d1.Z) -
                        p1Top2.Y * (d1.X * d1xd2.Z - d1xd2.X * d1.Z) +
                        p1Top2.Z * (d1.X * d1xd2.Y - d1xd2.X * d1.Y);

            t = det / d1xd2SquareLength;

            det = p1Top2.X * (d2.Y * d1xd2.Z - d1xd2.Y * d2.Z) -
                  p1Top2.Y * (d2.X * d1xd2.Z - d1xd2.X * d2.Z) +
                  p1Top2.Z * (d2.X * d1xd2.Y - d1xd2.X * d2.Y);

            s = det / d1xd2SquareLength;

            return true;
        }
    }
}
