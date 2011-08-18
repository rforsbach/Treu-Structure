using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Controller.Tracking;

using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to zoom a selected area.
    /// </summary>
    public class ZoomWindow : Canguro.Commands.ViewCommand
    {
        private System.Drawing.Point first;
        private System.Drawing.Point last;

        private ZoomWindow() { }
        public static ZoomWindow Instance = new ZoomWindow();
    
        public override bool IsInteractive
        {
            get { return true; }
        }

        public override bool SavePrevious
        {
            get
            {
                return true;
            }
        }

        public override void ButtonDown(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            first = e.Location;
            startTracking(first);
        }

        private void zoom(Canguro.View.GraphicView activeView)
        {
            // use first & last points to find scale an translate values.
            float screenSize = (float)Math.Min(activeView.Viewport.Height, activeView.Viewport.Width);
            float sizeX = last.X - first.X;
            float sizeY = last.Y - first.Y;

            MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1, (int)(first.X + sizeX / 2), (int)(first.Y + sizeY / 2), 0);
            activeView.ArcBallCtrl.OnBeginPan(e);

            e = new MouseEventArgs(MouseButtons.Left, 1, activeView.Viewport.Width / 2, activeView.Viewport.Height / 2, 0);
            activeView.ArcBallCtrl.OnMovePan(e);

            activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;

            // View volume scaling
            float newScale = 0.0f;

            newScale = screenSize * activeView.ArcBallCtrl.ScalingFac / (float)Math.Max(sizeX, sizeY);

            activeView.ArcBallCtrl.ZoomAbsolute(newScale);
            activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
        }

        public override void ButtonUp(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            last = e.Location;
            endTracking();
            zoom(activeView);
        }

        public override void MouseMove(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
        }

        private void setTransform(int y)
        {
        }

        #region Tracking
        private void startTracking(System.Drawing.Point p)
        {
            TrackingService ts;
            ts = Canguro.Controller.Controller.Instance.TrackingController.TrackingService = RectangleTrackingService.Instance;
            ts.SetPoint(p);
        }

        private void endTracking()
        {
            Canguro.Controller.Controller.Instance.TrackingController.TrackingService = null;
        }
        #endregion
    }
}

