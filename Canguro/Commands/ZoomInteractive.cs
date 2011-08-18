using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to Zoom in and Out with mouse movements.
    /// </summary>
    public class ZoomInteractive : Canguro.Commands.ViewCommand
    {
        private Matrix oldM;
        private int firstY;
        private Canguro.View.GraphicView gv;
        private System.Windows.Forms.Cursor cursor = new System.Windows.Forms.Cursor(typeof(MainFrm), "Commands.Zoom.cur");
        
        private ZoomInteractive() { }
        public static ZoomInteractive Instance = new ZoomInteractive();
    
        public override System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return cursor;
            }
        }

        /// <summary>
        /// This command is interactive. Returns true.
        /// </summary>
        public override bool IsInteractive
        {
            get
            {
                return true;
            }
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
            gv = activeView;
            oldM = gv.ViewMatrix;
            firstY = e.Y;

            gv.ArcBallCtrl.OnBeginZoom(e);
        }

        public override void ButtonUp(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            //setTransform(e.Y);
            gv.ArcBallCtrl.OnEndZoom(e);
            gv.ViewMatrix = gv.ArcBallCtrl.ViewMatrix;
        }

        public override void MouseMove(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            //setTransform(e.Y);
            gv.ArcBallCtrl.OnMoveZoom(e);
            gv.ViewMatrix = gv.ArcBallCtrl.ViewMatrix;
        }

        private void setTransform(int y)
        {
            int deltaY = firstY - y;
            float s = (float)Math.Pow(2, deltaY * 0.02);
            gv.ViewMatrix = oldM * Matrix.Scaling(s, s, s);
        }
    }
}
