using System;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Controller
{
    internal class TrackingController
    {
        private Snap.SnapController snapController;
        private Tracking.TrackingService trackingService;
        private Tracking.HoverController hoverController;
    
        public TrackingController()
        {
            this.trackingService = null;
            this.snapController = new Snap.SnapController();
            this.hoverController = new Tracking.HoverController();
        }

        public Tracking.TrackingService TrackingService
        {
            get { return trackingService; }
            set
            {
                trackingService = value;
                if (trackingService != null)
                {
                    trackingService.Reset(Canguro.View.GraphicViewManager.Instance.ActiveView);
                    trackingService.Start();
                }
            }
        }

        internal Snap.SnapController SnapController
        {
            get { return snapController; }
        }

        public Canguro.Controller.Tracking.HoverController HoverController
        {
            get { return hoverController; }
        }

        public void Reset(Canguro.View.GraphicView activeView)
        {
            if (trackingService != null)
                trackingService.Reset(activeView);
            snapController.Reset(activeView);
            hoverController.Reset(activeView);
        }

        public void ResetStatus(Canguro.View.GraphicView activeView)
        {
            snapController.ResetStatus(activeView);
        }

        public bool MouseMove(object sender, MouseEventArgs e, Canguro.View.GraphicView activeView)
        {
            Viewport vp = activeView.Viewport;
            if (e.X >= vp.X && e.X <= vp.X + vp.Width && e.Y >= vp.Y && e.Y <= vp.Y + vp.Height)
            {
                bool needPaint = (trackingService != null);

                if (snapController.IsActive)
                    needPaint |= snapController.MouseMove(activeView, e) || (trackingService != null);

                if (hoverController.IsActive && !activeView.ModelRenderer.RenderOptions.ShowAnimated)
                    needPaint |= hoverController.MouseMove(activeView, e);

                if (needPaint && trackingService != null)
                    trackingService.MouseMove(e.Location);

                return needPaint;
            }

            return false;
        }
    }
}
