using System;
using System.Collections.Generic;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Controller.Snap
{
    public abstract class Magnet : ICloneable
    {
        protected Vector3 position;
        protected float lastSnapFitness;
        protected List<Magnet> relatedMagnets;

        public Magnet() { }
        public Magnet(Vector3 position) 
        { 
            this.position = position; 
            lastSnapFitness = SnapController.SnapViewDistance;
            relatedMagnets = null;
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float LastSnapFitness
        {
            get { return lastSnapFitness; }
        }

        public abstract float Snap(Canguro.View.GraphicView activeView, Point mousePoint);
        public abstract Vector3 SnapPositionInt { get; }
        public abstract Vector3 SnapPosition { get; }

        public List<Magnet> RelatedMagnets
        {
            get 
            {
                if (relatedMagnets == null)
                    relatedMagnets = new List<Magnet>();

                return relatedMagnets; 
            }
            set { relatedMagnets = value; }
        }

        #region ICloneable Members

        public abstract object Clone();
        protected void copyToMagnet(Magnet dstMagnet)
        {
            dstMagnet.position = position;
            dstMagnet.lastSnapFitness = lastSnapFitness;
            dstMagnet.relatedMagnets = (relatedMagnets != null) ? new List<Magnet>(relatedMagnets) : null;
        }

        #endregion
    }
}
