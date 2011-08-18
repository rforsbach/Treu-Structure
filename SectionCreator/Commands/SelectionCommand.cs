using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator.Commands
{
    class SelectionCommand : ViewCommand
    {
        public override void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & System.Windows.Forms.MouseButtons.Left) > 0)
            {
                object obj = GetObjectAt(e.Location);
                if (obj is ISelectable)
                {
                    ((ISelectable)obj).IsSelected = !((ISelectable)obj).IsSelected;
                    Model.Instance.ChangeSelection();
                }
            }
            else if ((e.Button & System.Windows.Forms.MouseButtons.Right) > 0)
            {
                Model.Instance.ClearSelection();
                Model.Instance.ChangeSelection();
            }
        }

        public override void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseUp(e);
        }


        public object GetObjectAt(System.Drawing.Point screenPosition)
        {
            if (controller != null && controller.View != null)
            {
                Point p;
                return GetObjectAt(screenPosition, out p);
            }
            else
                return null;
        }

        public object GetObjectAt(System.Drawing.Point screenPosition, out Point last)
        {
            foreach (Contour con in Model.Instance.Contours)
            {
                int count = con.Points.Count;
                if (count > 2)
                {
                    last = con.Points[count - 1];
                    foreach (Point p in con.Points)
                    {
                        if (Equals(p, screenPosition))
                            return p;
                        if (Crosses(last, p, screenPosition))
                            return con;
                        last = p;
                    }
                }
            }
            last = null;
            return null;
        }

        public object GetObjectAt(System.Drawing.PointF modelPosition)
        {
            return GetObjectAt(controller.View.GetScreenPosition(modelPosition));
        }

        protected bool Equals(Point p, System.Drawing.Point position)
        {
            System.Drawing.Point pos = Controller.Instance.View.GetScreenPosition(p.Position);
            bool eqx = position.X - pos.X < 5 && position.X - pos.X > -5;
            bool eqy = position.Y - pos.Y < 5 && position.Y - pos.Y > -5;
            return (eqx && eqy);
        }

        protected bool Crosses(Point p1, Point p2, System.Drawing.Point pos)
        {
            System.Drawing.Point pos1 = Controller.Instance.View.GetScreenPosition(p1.Position);
            System.Drawing.Point pos2 = Controller.Instance.View.GetScreenPosition(p2.Position);

            double xmp = pos.X - pos1.X;
            double ymp = pos.Y - pos1.Y;

            double dirx = pos2.X - pos1.X;
            double diry = pos2.Y - pos1.Y;

            double ret = ((Math.Abs(xmp) > Math.Abs(ymp) && dirx > 0) || diry == 0) ? xmp / dirx : ymp / diry;

            if (equals(ret * dirx, xmp) && equals(ret * diry, ymp))
                return ret > 0 && ret < 1;

            return false;
        }

        private bool equals(double a, double b)
        {
            return (a - b < 5 && a - b > -5);
        }

        public override bool AllowSelection
        {
            get
            {
                return true;
            }
        }
    }
}
