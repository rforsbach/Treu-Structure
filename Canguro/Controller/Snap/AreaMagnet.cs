using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Utility;

namespace Canguro.Controller.Snap
{
    public class AreaMagnet : Magnet
    {
        private const float minZPlaneAngle = 0.707f;
        private Vector3[] screenNormal = new Vector3[2];
        private Vector3 snapPosition;

        private Microsoft.DirectX.Vector3 normal;

        public AreaMagnet(Vector3 position, Vector3 normal) : base(position)
        {
            this.normal = normal;
            screenNormal[0] = new Vector3(0, 0, 0);
            screenNormal[1] = new Vector3(0, 0, 1);
        }

        public Microsoft.DirectX.Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public override float Snap(Canguro.View.GraphicView activeView, Point mousePoint)
        {
            // Get ray from mouse position
            Vector3 rayP1 = new Vector3(mousePoint.X, mousePoint.Y, 1f);
            Vector3 rayP2 = new Vector3(mousePoint.X, mousePoint.Y, 0f);
            activeView.Unproject(ref rayP1);
            activeView.Unproject(ref rayP2);
            Vector3 ray = rayP2 - rayP1;
            ray.Normalize();

            // Check best plane angle
            Vector3 normalTmp = normal;
            float cosAngle = Math.Abs(Vector3.Dot(ray, normal));
            if (cosAngle < 0.03f)
            {
                float xCosAngle = Math.Abs(Vector3.Dot(ray, CommonAxes.GlobalAxes[0]));
                float yCosAngle = Math.Abs(Vector3.Dot(ray, CommonAxes.GlobalAxes[1]));
                float zCosAngle = Math.Abs(Vector3.Dot(ray, CommonAxes.GlobalAxes[2]));

                if (xCosAngle >= yCosAngle)
                {
                    if (xCosAngle > zCosAngle)
                        normalTmp = CommonAxes.GlobalAxes[0];   // YZ Plane
                    else
                        normalTmp = CommonAxes.GlobalAxes[2];   // XY Plane
                }
                else if (yCosAngle > zCosAngle)
                    normalTmp = CommonAxes.GlobalAxes[1];   // XZ Plane
                else
                    normalTmp = CommonAxes.GlobalAxes[2];   // XY Plane
            }
            
            float r = Vector3.Dot(position - rayP1, normalTmp) / Vector3.Dot(ray, normalTmp);
            snapPosition = rayP1 + Vector3.Scale(ray, r);

            return lastSnapFitness = 0f;
        }

        public override Vector3 SnapPositionInt
        {
            get { return snapPosition; }
        }

        public override Vector3 SnapPosition
        {
            get
            {
                Canguro.Model.UnitSystem.UnitSystem us = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;
                return new Vector3(us.FromInternational(snapPosition.X, Canguro.Model.UnitSystem.Units.Distance),
                                    us.FromInternational(snapPosition.Y, Canguro.Model.UnitSystem.Units.Distance),
                                    us.FromInternational(snapPosition.Z, Canguro.Model.UnitSystem.Units.Distance));
            }
        }

        public void RecalcPrimaryDependant(Canguro.View.GraphicView activeView, PointMagnet primaryPoint, LineMagnet[] globalAxes)
        {
            if (primaryPoint != null)
            {
                // Move area to lay on the primaryPoint and to set its direction any canonic 
                // plane (X=x, Y=y or Z=z) which is the most paralell to the screen plane
                position = primaryPoint.Position;

                // Get screen plane normal
                Vector3 s0 = screenNormal[0], s1 = screenNormal[1], sNormal;
                activeView.Unproject(ref s0);
                activeView.Unproject(ref s1);
                sNormal = s0 - s1;

                // Assign the area normal to the most paralell canonical plane
                // (giving priority to the Z plane)
                int maxCosIndex = 2;
                float cosX, cosY, cosZ;
                cosX = Vector3.Dot(sNormal, globalAxes[0].Direction);
                cosY = Vector3.Dot(sNormal, globalAxes[1].Direction);
                cosZ = Vector3.Dot(sNormal, globalAxes[2].Direction);

                if (Math.Abs(cosZ) < minZPlaneAngle)
                    maxCosIndex = (cosX >= cosY) ? ((cosX > cosZ) ? 0 : 2) : ((cosY > cosZ) ? 1 : 2);

                normal = globalAxes[maxCosIndex].Direction;
            }
            else
            {
                position = Vector3.Empty;
                normal = globalAxes[2].Direction;
            }
        }

        #region ICloneable Members

        public override object Clone()
        {
            AreaMagnet am = new AreaMagnet(position, normal);
            copyToMagnet(am);
            screenNormal.CopyTo(am.screenNormal, 0);
            am.snapPosition = snapPosition;
            am.normal = normal;

            return am;
        }

        #endregion
    }
}
