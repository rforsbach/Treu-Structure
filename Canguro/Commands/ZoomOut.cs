using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to Zoom Out the Active View one step.
    /// </summary>
    class ZoomOut : ViewCommand
    {
        private ZoomOut() { }
        public static readonly ZoomOut Instance = new ZoomOut();

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
        /// Executes ArcBallCtrl.ZoomStep(-0.2)
        /// </summary>
        /// <param name="activeView">The Current Active View object</param>
        public override void Run(Canguro.View.GraphicView activeView)
        {
            activeView.ArcBallCtrl.ZoomStep(-0.2f);
            activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
        }
    }
}
