using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to animate the Model with results
    /// </summary>
    public class Animation : Canguro.Commands.ViewCommand
    {
        private Animation() { }
        public static Animation Instance = new Animation();

        /// <summary>
        /// This command is not interactive. Returns false.
        /// </summary>
        public override bool IsInteractive
        {
            get { return false; }
        }

        /// <summary>
        /// Executes the command. 
        /// Toggles activeView.IsAnimated.
        /// </summary>
        /// <param name="activeView">The current Active View object</param>
        public override void  Run(Canguro.View.GraphicView activeView)
        {
            activeView.IsAnimated = !activeView.IsAnimated;
        }
    }
}

