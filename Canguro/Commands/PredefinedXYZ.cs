using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to set a predefined view perpendicular to an arbitrary plane (3D view)
    /// </summary>
    public class PredefinedXYZ : Canguro.Commands.ViewCommand
    {
         private PredefinedXYZ() { }
        public static PredefinedXYZ Instance = new PredefinedXYZ();

        /// <summary>
        /// This command is not interactive. Returns false.
        /// </summary>
        public override bool IsInteractive
        {
            get
            {
                return false;
            }
        }

        public override bool SavePrevious
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Executes the Non-Interactive Command.
        /// Sets the RotationMatrix for the View with a Default 3D View and Executes ZoomAll.
        /// </summary>
        /// <param name="activeView">The Current Active View object</param>
        public override void Run(Canguro.View.GraphicView activeView)
        {
            activeView.ArcBallCtrl.ResetRotation();
            activeView.ArcBallCtrl.RotationMatrix = Matrix.RotationX(-(float)Math.PI / 2.0f) * Matrix.RotationY(-3.0f * (float)Math.PI / 4.0f) * Matrix.RotationX((float)Math.PI / 6.0f);
            activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
            ZoomAll.Instance.Run(activeView);
        }
    }
}
