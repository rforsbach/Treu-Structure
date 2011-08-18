using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator.Commands
{
    class AddContourCommand : ViewCommand
    {
        Contour contour = null;
        Point currentPoint = null;

        public override void MouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & System.Windows.Forms.MouseButtons.Left) > 0) // Add
            {
                if (contour == null)
                {
                    contour = new Contour();
                    Model.Instance.Contours.Add(contour);
                }
                System.Drawing.PointF position = controller.View.GetModelPosition(e.Location);

                if (currentPoint == null)
                    contour.Points.Add(new Point(position));
                currentPoint = new Point(position.X, position.Y);
                contour.Points.Add(currentPoint);
            }
            else if ((e.Button & System.Windows.Forms.MouseButtons.Right) > 0) // End command
            {
                if (contour != null)
                {
                    IList<Point> points = contour.Points;
                    int count = points.Count - 1;
                    contour.Points.RemoveAt(count);
                    Point last = points[0];

                    for (int i = points.Count - 1; i >= 0; i--)
                    {
                        if (points[i].Equals(last) && points[i] != last)
                            points.RemoveAt(i);
                        last = points[i];
                    }
                    if (contour != null)
                        contour.IsSelected = true;
                    contour = null;
                    currentPoint = null;
                    if (count >= 3)
                        controller.EndCommand();
                    else
                        controller.CancelCommand();
                }
                else
                    controller.CancelCommand();
            }
        }

        public override void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (currentPoint != null)
                currentPoint.Position = controller.View.GetModelPosition(e.Location);
        }

        public override void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            //if (currentPoint != null)
            //{
            //    // Make undoable
            //    currentPoint.X = currentPoint.X;
            //    currentPoint.Y = currentPoint.Y;
            //}
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
