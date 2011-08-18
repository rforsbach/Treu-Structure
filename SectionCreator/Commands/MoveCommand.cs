using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.SectionCreator.Commands
{
    class MoveCommand : ViewCommand
    {
        object currentObject;
        Contour currentContour;
        Point lastPoint;
        bool allowSelection = true;
        System.Drawing.PointF basePoint;

        public override void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            allowSelection = false;

            if (lastPoint != null)
            {
                // Make undoable
                lastPoint.X = lastPoint.X;
                lastPoint.Y = lastPoint.Y;
            }
            if (currentContour != null)
            {
                foreach (Point p in currentContour.Points)
                {
                    // Make undoable
                    p.X = p.X;
                    p.Y = p.Y;
                }
            }

            if ((e.Button & MouseButtons.Right) > 0)
            {
                currentObject = null;
                currentContour = null;
                lastPoint = null;
                allowSelection = true;
                controller.EndCommand();
            }
            else
            {
                currentObject = controller.GetHoverObject(out lastPoint);
                currentContour = (currentObject is Contour) ? (Contour)currentObject : null;
                if (currentContour != null)
                    basePoint = controller.View.GetModelPosition(e.Location);
            }
        }

        public override void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & (MouseButtons.Middle | MouseButtons.Left)) > 0 && currentContour != null)
            {
                System.Drawing.PointF mov = controller.View.GetModelPosition(e.Location);

                mov.X = mov.X - basePoint.X;
                mov.Y = mov.Y - basePoint.Y;
                foreach (Point p in currentContour.Points)
                {
                    System.Drawing.PointF pos = p.Position;
                    pos.X += mov.X;
                    pos.Y += mov.Y;
                    p.Position = pos;
                }
                basePoint = controller.View.GetModelPosition(e.Location);
            }
        }

        public override void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            currentObject = null;
            currentContour = null;
            lastPoint = null;
            allowSelection = true;
        }


        public override bool AllowSelection
        {
            get { return allowSelection; }
        }

        public override System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return Cursors.SizeAll;
            }
        }

        public override void Init()
        {
            Model.Instance.ClearSelection();
        }
    }
}
