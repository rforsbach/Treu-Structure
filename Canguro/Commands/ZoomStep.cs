using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.View
{
    /// <summary>
    ///  Makes zoom on active view when wheel mouse is rolled.
    /// </summary>
    public class ZoomStep : Canguro.Commands.ViewCommand
    {
        /// <summary>
        /// Defines an active cursor when this command is active
        /// </summary>
        private System.Windows.Forms.Cursor cursor = new System.Windows.Forms.Cursor(typeof(MainFrm), "Commands.Zoom.cur");

        /// <summary>
        /// Class constructor
        /// </summary>
        private ZoomStep() { }

        /// <summary>
        /// Singleton for this command
        /// </summary>
        public static readonly ZoomStep Instance = new ZoomStep();

        /// <summary>
        /// Property: Gets the cursor used by this command
        /// </summary>
        public override System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return cursor;
            }
        }

        /// <summary>
        /// Property: Tells if this is an interactive command. 
        /// </summary>
        public override bool IsInteractive
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Event Handler: Action to take when a button is pressed (For now, do nothing)
        /// </summary>
        /// <param name="activeView"> Active view in the scene panel </param>
        /// <param name="e"> Mouse arguments </param>
        public override void ButtonDown(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
        }

        /// <summary>
        /// Event Handler: Action to take when a button is released (For now, do nothing)
        /// </summary>
        /// <param name="activeView"> Active view in scene panel </param>
        /// <param name="e"> Mouse arguments </param>
        public override void ButtonUp(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
        }

        /// <summary>
        /// Event Handler: Action to take when mouse is moving
        /// </summary>
        /// <param name="activeView"> Active view in scene panel </param>
        /// <param name="e"> Mouse Arguments </param>
        public override void MouseMove(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
        }

        public override bool SavePrevious
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// EventHandler: Response to mouse wheel movement. When scrolling, react by xooming in/out the active view
        /// </summary>
        /// <param name="activeView"> Active view in scene panel </param>
        /// <param name="e"> Mouse Arguments </param>
        public override void MouseWheel(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            Zoom(activeView, e.Delta / 500.0f);
        }

        /// <summary>
        /// Makes zoom based in an arcball property and a step value for unification with the rest of zoom commands
        /// </summary>
        /// <param name="activeView"> Active view in scene panel </param>
        /// <param name="step"> Zoom step: How much the scene view is zoomed </param>
        private void Zoom(Canguro.View.GraphicView activeView, float step)
        {
            // Invoke arcball's zooming routine
            activeView.ArcBallCtrl.ZoomStep(step);

            // Update activeView view matrix
            activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
        }
    }
}
