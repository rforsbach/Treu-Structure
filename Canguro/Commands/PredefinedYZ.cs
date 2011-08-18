using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to set a predefined view perpendicular to the YZ plane (Side view)
    /// </summary>
    public class PredefinedYZ : Canguro.Commands.ViewCommand
    {
        private PredefinedYZ() { }
        public static PredefinedYZ Instance = new PredefinedYZ();

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
            activeView.ArcBallCtrl.RotationMatrix = Matrix.RotationY(-(float)Math.PI / 2.0f) * Matrix.RotationZ(-(float)Math.PI / 2.0f);
            activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
            ZoomAll.Instance.Run(activeView);
        }
    }
}
