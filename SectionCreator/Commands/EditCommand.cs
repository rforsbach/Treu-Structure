using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.SectionCreator.Commands
{
    class EditCommand : ViewCommand
    {
        object currentObject;
        Contour currentContour;
        Point lastPoint;
        bool allowSelection = true;
        System.Drawing.PointF basePoint;

        public override void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            allowSelection = false;

            if ((e.Button & MouseButtons.Left) > 0 && Control.ModifierKeys != Keys.Shift)
            {
                if (currentObject == null)
                {
                    currentObject = controller.GetHoverObject(out lastPoint);
                    currentContour = (currentObject is Contour) ? (Contour)currentObject : null;
                    lastPoint = (currentObject is Point) ? (Point)currentObject : lastPoint;
                }
                if (currentContour != null && lastPoint != null)
                {
                    int index = currentContour.Points.IndexOf(lastPoint) + 1;
                    if (index > 0)
                    {
                        System.Drawing.PointF pos = controller.View.GetModelPosition(e.Location);
                        lastPoint = new Point(pos);
                        currentContour.Points.Insert(index, lastPoint);
                    }
                }
            }

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
            else if ((e.Button & MouseButtons.Middle) > 0 || Control.ModifierKeys == Keys.Shift)
            {
                currentObject = controller.GetHoverObject(out lastPoint);
                currentContour = (currentObject is Contour) ? (Contour)currentObject : null;
                if (currentContour != null)
                    basePoint = controller.View.GetModelPosition(e.Location);
            }
        }

        public override void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) > 0 && lastPoint != null && Control.ModifierKeys != Keys.Shift)
            {
                lastPoint.Position = controller.View.GetModelPosition(e.Location);
            }
            else if (((e.Button & MouseButtons.Middle) > 0 || Control.ModifierKeys == Keys.Shift) && currentContour != null)
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
            if ((e.Button & MouseButtons.Left) > 0 && lastPoint != null && currentContour != null)
            {
                IList<Point> points = currentContour.Points;
                Point last = points[0];

                for (int i = points.Count - 1; i >= 0; i--)
                    if (points[i].Equals(lastPoint) && points[i] != lastPoint)
                        points.RemoveAt(i);
                // Make undoable
                lastPoint.X = lastPoint.X;
                lastPoint.Y = lastPoint.Y;
            }
            else if ((e.Button & MouseButtons.Middle) > 0 && currentContour != null)
            {
                foreach (Point p in currentContour.Points)
                {
                    // Make undoable
                    p.X = p.X;
                    p.Y = p.Y;
                }
            }
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
                return System.Windows.Forms.Cursors.Cross;
            }
        }

        public override void Init()
        {
            Model.Instance.ClearSelection();            
        }
    }
}
