using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Controller.Snap
{
    public class LineMagnet : Magnet
    {
        private Vector3 direction;
        private LineMagnetType type;
        private Canguro.Model.LineElement line;
        private Vector3 snapPosition;

        public LineMagnet() { }
        public LineMagnet(Vector3 position, Vector3 direction, LineMagnetType type) : base(position)
        {
            this.direction = direction;
            this.type = type;
            line = null;
        }
        public LineMagnet(Canguro.Model.LineElement line) : base(line.I.Position)
        {
            this.direction = line.J.Position - line.I.Position;
            this.type = LineMagnetType.FollowProjection;
            this.line = line;
        }

        public Canguro.Model.LineElement Line
        {
            get { return line; }
            set { line = value; }
        }

        public LineMagnetType Type
        {
            get { return type; }
            set { type = value; }
        }

        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public override float Snap(Canguro.View.GraphicView activeView, Point mousePoint)
        {
            Vector3 mousePosition;

            // Project points
            Vector3 p = position, p2 = position + direction;
            activeView.Project(ref p);
            activeView.Project(ref p2);
            Vector3 v = p2 - p;
            p.Z = 0.1f;

            mousePosition = new Vector3((float)mousePoint.X, (float)mousePoint.Y, 0.1f);
            float r = Vector3.Dot(v, mousePosition - p) / Vector3.Dot(v, v);
            Vector3 snapPoint = p + Vector3.Scale(v, r);

            snapPosition = position + Vector3.Scale(direction, r);

            Vector3 d = mousePosition - snapPoint;
            return lastSnapFitness = Vector3.Dot(d, d);
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

        public override bool Equals(object obj)
        {
            LineMagnet lm = obj as LineMagnet;
            if (lm != null)
            {
                if (line == null && lm.line == null)
                {
                    if (direction == lm.direction)
                        return true;
                    else
                        return false;
                }
                else if ((line != null && lm.line == null) || (line == null && lm.line != null))
                    return false;
                else if (line != null && lm.line != null)
                    return (line == lm.line);
            }

            return (line == obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override object Clone()
        {
            LineMagnet lm = new LineMagnet();
            copyToMagnet(lm);
            lm.direction = direction;
            lm.line = line;
            lm.snapPosition = snapPosition;
            lm.type = type;

            return lm;
        }
    }
}
