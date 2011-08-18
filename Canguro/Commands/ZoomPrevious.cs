using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to set the view to the previous state, before the last view command. (Not Implemented)
    /// </summary>
    public class ZoomPrevious : Canguro.Commands.ViewCommand
    {
        private ZoomPrevious() { }
        public static ZoomPrevious Instance = new ZoomPrevious();

        public override bool IsInteractive
        {
            get { return false; }
        }

        public override void Run(Canguro.View.GraphicView activeView)
        {
            if (activeView.ArcBallsInStack > 1)
            {
                activeView.PopArcBall();
                // Update activeView view matrix
                activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
            }
            else
                System.Windows.Forms.MessageBox.Show(Culture.Get(Canguro.Properties.Resources.noZoomPreviousToDo));

            //System.Windows.Forms.MessageBox.Show("Javier... Implementa!!!!");
        }
    }
}
