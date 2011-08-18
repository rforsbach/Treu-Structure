using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to rotate the view arround a 3D Trackball.
    /// </summary>
    public class Trackball3D : Canguro.Commands.ViewCommand
    {
        private Matrix oldM;
        private Quaternion currentQuat = Quaternion.Identity;
        private Canguro.View.GraphicView gv;
        private System.Windows.Forms.Cursor cursor = System.Windows.Forms.Cursors.SizeAll;
        public const float radius = 0.8f;

        private Trackball3D() { }
        public static Trackball3D Instance = new Trackball3D();

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
            // Store initial state
            gv = activeView;
            oldM = gv.ViewMatrix;

            // Get Screen Coordinates and prepare ArcBall
            //gv.InitArcBall(System.Windows.Forms.MouseEventArgs e)
            
            //gv.ArcBallCtrl.OnBeginRotate(e, new Vector3(8,8,8));
            gv.ArcBallCtrl.OnBeginRotate(e, Controller.Controller.Instance.SelectionCommand.GetViewableObjectsCentroid(activeView));
        }

        public override void ButtonUp(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            //gv.ArcBallCtrl.OnEnd();
            //gv.ViewMatrix = mouseCtrl.RotationMatrix * oldM;
            gv.ArcBallCtrl.OnEndRot();
        }

        public override void MouseMove(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            // Now, mouse is moving then, change ArcBall status
            gv.ArcBallCtrl.OnMoveRot(e);
            gv.ViewMatrix = gv.ArcBallCtrl.ViewMatrix;
        }
    }
}
