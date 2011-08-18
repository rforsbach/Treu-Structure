using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to set a predefined view perpendicular to the XY plane (Top view)
    /// </summary>
    public class PredefinedXY : Canguro.Commands.ViewCommand
    {
        private PredefinedXY() { }
        public static PredefinedXY Instance = new PredefinedXY();

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
        /// Sets the RotationMatrix for the View with a Default YZ View and Executes ZoomAll.
        /// </summary>
        /// <param name="activeView">The Current Active View object</param>
        public override void Run(Canguro.View.GraphicView activeView)
        {
            activeView.ArcBallCtrl.ResetRotation();
            activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
            ZoomAll.Instance.Run(activeView);
        }
    }
}
