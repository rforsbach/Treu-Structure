using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.SectionCreator.Commands
{
    class PasteCommand : ViewCommand
    {
        List<Point> allPoints = null;

        public override void Init()
        {
            object obj = Clipboard.GetData("SectionCreator");
            if (obj is List<Contour>)
            {
                allPoints = new List<Point>();
                List<Contour> contours = (List<Contour>)obj;
                foreach (Contour con in contours)
                {
                    Model.Instance.Contours.Add(con);
                    allPoints.AddRange(con.Points);
                }
                MouseMove(null);
            }
        }

        public override Cursor Cursor
        {
            get
            {
                return Cursors.SizeAll;
            }
        }

        public override void MouseMove(MouseEventArgs e)
        {
            if (allPoints != null && allPoints.Count > 0)
            {
                System.Drawing.PointF mouse = controller.View.GetModelPosition(controller.MousePosition);
                float dx = mouse.X - allPoints[0].Position.X;
                float dy = mouse.Y - allPoints[0].Position.Y;
                foreach (Point p in allPoints)
                {
                    System.Drawing.PointF pos = p.Position;
                    pos.X += dx;
                    pos.Y += dy;
                    p.Position = pos;
                }
            }
        }

        public override void MouseUp(MouseEventArgs e)
        {
            controller.EndCommand();
        }
    }
}
