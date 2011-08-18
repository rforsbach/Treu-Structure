using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Controller.Tracking
{
    public abstract class TrackingService
    {
        protected Canguro.View.GraphicView graphicView;

        /// <summary>
        /// This method is called by the Controller to set up the Tracking service at
        /// the initial state and with the correct parameters
        /// </summary>
        /// <param name="gv">The ActiveView as known by the GraphicViewManager</param>
        public void Reset(Canguro.View.GraphicView gv)
        {
            graphicView = gv;
            reset();
        }
        protected virtual void reset() { }

        public virtual void Start() { }

        /// <summary>
        /// Este método lo llama CommandServices ya que depende de la selección de algún Item o algún Vertex3D
        /// </summary>
        public virtual void SetPoint(Point pt) { }
        public virtual void SetPoint(Vector3 vecInternational) { }

        public abstract void MouseMove(Point pt);
        public abstract void Paint(Device device);
    }
}
