using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Controller.Snap
{
    public class PointMagnet : Magnet
    {
        private PointMagnetType type;
        private Canguro.Model.Joint joint;

        public static readonly PointMagnet ZeroMagnet = new PointMagnet(Vector3.Empty, PointMagnetType.EndPoint);

        public PointMagnet() { }
        public PointMagnet(Vector3 pointCurrentUnitSystem)
            : this(new Vector3( Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.ToInternational(pointCurrentUnitSystem.X, Canguro.Model.UnitSystem.Units.Distance),
                                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.ToInternational(pointCurrentUnitSystem.Y, Canguro.Model.UnitSystem.Units.Distance),
                                Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem.ToInternational(pointCurrentUnitSystem.Z, Canguro.Model.UnitSystem.Units.Distance)),
                   PointMagnetType.SimplePoint) {}
        public PointMagnet(Vector3 pointInternational, PointMagnetType type)
            : base(pointInternational) 
        {
            this.type = type;
            joint = null;
        }
        public PointMagnet(Canguro.Model.Joint joint) : this(joint.Position, PointMagnetType.EndPoint)
        {
            this.joint = joint;            
        }

        public PointMagnetType Type
        {
            get { return type; }
            set { type = value; }
        }

        public override float Snap(Canguro.View.GraphicView activeView, Point mousePoint)
        {
            // Project point
            Vector3 p = position, q = new Vector3((float)mousePoint.X, (float)mousePoint.Y, 0.1f);
            activeView.Project(ref p);
            p.Z = 0.1f;

            Vector3 d = q - p;
            return lastSnapFitness = Vector3.Dot(d, d);
        }

        public override Vector3 SnapPositionInt
        {
            get { return position; }
        }

        public override Vector3 SnapPosition
        {
            get
            {
                Canguro.Model.UnitSystem.UnitSystem us = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;
                return new Vector3(us.FromInternational(position.X, Canguro.Model.UnitSystem.Units.Distance),
                                    us.FromInternational(position.Y, Canguro.Model.UnitSystem.Units.Distance),
                                    us.FromInternational(position.Z, Canguro.Model.UnitSystem.Units.Distance));
            }
        }

        public Canguro.Model.Joint Joint
        {
            get { return joint; }
            set { joint = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj is PointMagnet)
            {
                if (joint != null)
                    return (((PointMagnet)obj).Joint == joint);
                else if (((PointMagnet)obj).Joint != null)
                    return false;

                return position.Equals(((PointMagnet)obj).position);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override object Clone()
        {
            PointMagnet pm = new PointMagnet();
            copyToMagnet(pm);
            pm.type = type;
            pm.joint = joint;

            return pm;
        }
    }
}
