using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to set a predefined view perpendicular to the XZ plane (Front view)
    /// </summary>
    public class PredefinedXZ : Canguro.Commands.ViewCommand
    {
        private PredefinedXZ() { }
        public static PredefinedXZ Instance = new PredefinedXZ();

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
        /// Sets the RotationMatrix for the View with a Default XZ View and Executes ZoomAll.
        /// </summary>
        /// <param name="activeView">The Current Active View object</param>
        public override void Run(Canguro.View.GraphicView activeView)
        {
            activeView.ArcBallCtrl.ResetRotation();
            activeView.ArcBallCtrl.RotationMatrix = Matrix.RotationX(-(float)Math.PI / 2.0f);
            activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
            ZoomAll.Instance.Run(activeView);
        }
    }
}
