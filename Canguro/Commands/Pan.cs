using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to translate the view.
    /// </summary>
    public class Pan : Canguro.Commands.ViewCommand
    {
        private Matrix oldM;
        private Canguro.View.GraphicView gv;
        private System.Windows.Forms.Cursor cursor = new System.Windows.Forms.Cursor(typeof(MainFrm), "Commands.Pan.cur");

        private Pan() {}
        public static Pan Instance = new Pan();

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
/*            
            // Store fx and fy as first interaction point
            fx = e.X; fy = e.Y;
            
            // Calculate unitary (in terms of pixels) basis for displacement
            Vector3 zero = new Vector3(0, 0, 0.5f);
            basisU = new Vector3(1, 0, 0.5f);
            basisV = new Vector3(0, 1, 0.5f);
            gv.Unproject(ref zero);
            gv.Unproject(ref basisU);
            gv.Unproject(ref basisV);
            basisU -= zero;
            basisV -= zero;
*/
            gv.ArcBallCtrl.OnBeginPan(e);
 }

        public override void ButtonUp(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            //setTransform(e.X, e.Y);
            //gv.ArcBallCtrl.OnMovePan(e);
            //gv.ViewMatrix = gv.ArcBallCtrl.TranslationMatrix * gv.ArcBallCtrl.RotationMatrix;
        }

        public override void MouseMove(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            //setTransform(e.X, e.Y);
            gv.ArcBallCtrl.OnMovePan(e);
            gv.ViewMatrix = gv.ArcBallCtrl.ViewMatrix;
        }

        //private void setTransform(int x, int y)
        //{
        //    Vector3 delta = basisU * (x-fx) + basisV * (y-fy);
        //    gv.ViewMatrix = Matrix.Translation(delta) * oldM;
        //}
    }
}
