using System;
using System.Collections.Generic;
using System.Text;
using Canguro.SectionCreator.View;

namespace Canguro.SectionCreator.Commands
{
    class PanCommand : ViewCommand
    {
        System.Drawing.PointF origin;
        ViewState originalState;

        public override void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            origin = e.Location;
            originalState = (ViewState)controller.View.Clone();
        }

        public override void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (((e.Button & System.Windows.Forms.MouseButtons.Left) > 0) ||
                ((e.Button & System.Windows.Forms.MouseButtons.Middle) > 0))
            {
                ViewState view = controller.View;
                view.Pan.X = (originalState.Pan.X * view.Zoom + e.X - origin.X) / view.Zoom;
                view.Pan.Y = (originalState.Pan.Y * view.Zoom - e.Y + origin.Y) / view.Zoom;
            }
        }

        public override void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (((e.Button & System.Windows.Forms.MouseButtons.Right) > 0) ||
                ((e.Button & System.Windows.Forms.MouseButtons.Middle) > 0))
                controller.EndCommand();
        }

        public static void Pan(ViewState vs, System.Drawing.Point delta)
        {
        }

        private System.Windows.Forms.Cursor cursor = new System.Windows.Forms.Cursor(typeof(MainFrm), "Commands.Pan.cur");
        public override System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return cursor;
            }
        }
    }
}
