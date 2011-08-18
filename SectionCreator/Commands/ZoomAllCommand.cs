using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator.Commands
{
    class ZoomAllCommand : RunnableCommand
    {
        protected override void Run()
        {
            System.Drawing.PointF max = new System.Drawing.PointF(0.1f, 0.1f);
            System.Drawing.PointF min = new System.Drawing.PointF(-0.1f, -0.1f);

            foreach (Contour cont in model.Contours)
            {
                foreach (Point p in cont.Points)
                {
                    System.Drawing.PointF pos = p.Position;
                    max.X = (max.X < pos.X) ? pos.X : max.X;
                    max.Y = (max.Y < pos.Y) ? pos.Y : max.Y;
                    min.X = (min.X > pos.X) ? pos.X : min.X;
                    min.Y = (min.Y > pos.Y) ? pos.Y : min.Y;
                }
            }

            Canguro.SectionCreator.View.ViewState view = controller.View;
            view.Pan.X = -(max.X + min.X) / 2f;
            view.Pan.Y = -(max.Y + min.Y) / 2f;
            float zoomX = view.viewport.X / (max.X - min.X) * 0.95f;
            float zoomY = view.viewport.Y / (max.Y - min.Y) * 0.95f;
            view.Zoom = (zoomX < zoomY) ? zoomX : zoomY;
        }
    }
}
