using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Canguro.SectionCreator.View
{
    class SectionPainter
    {
        Graphics graphics;
        Brush selectionBrush = Brushes.SteelBlue;

        public void Paint(Graphics g, ViewState view)
        {
            graphics = g;

            Brush brush = Brushes.Gray;
            Pen pen = Pens.Black;
            Pen selectionPen = new Pen(Color.Green, 3);

            PaintGrid(view);

            List<Contour> contours = Model.Instance.Contours;

            foreach (Contour con in contours)
            {
                brush = new SolidBrush(con.Color);
                Pen currentPen = (con.IsSelected) ? selectionPen : pen;

                if (con.Material == Canguro.Analysis.Sections.Material.None)
                {
                    brush = Brushes.LightGray;
                    currentPen = new Pen(con.Color);
                }
                
                PaintContour(con, brush, currentPen, view);

                foreach (Point p in con.Points)
                {
                    if (p.IsSelected)
                        PaintPoint(p, selectionBrush, view);
                    else
                        PaintPoint(p, Brushes.Gray, view);
                }
            }

            //PaintTriangulation(view);

            Controller.Instance.Paint(this);

            PaintAxes(view);
        }

        public void PaintContour(Contour con, Brush brush, Pen pen, ViewState view)
        {
            PointF[] points = new PointF[con.Points.Count];
            int i = 0;
            foreach (Point p in con.Points)
                points[i++] = view.GetScreenPosition(p.Position);

            if (points.Length > 2)
            {
                graphics.FillPolygon(brush, points);
                graphics.DrawPolygon(pen, points);
            }
            else if (points.Length == 2)
                graphics.DrawLine(pen, points[0], points[1]);

            brush = Brushes.White;
        }

        public void PaintPoint(Point point, Brush brush, ViewState view)
        {
            System.Drawing.Point pos = view.GetScreenPosition(point.Position);
            graphics.FillEllipse(brush, pos.X - 3, pos.Y - 3, 6, 6);
        }

        protected void PaintGrid(ViewState view)
        {
            double log = Math.Log10(view.Zoom / 5f);
            double flog = Math.Floor(log);
            float step = 1f / (float)Math.Pow(10.0, flog);

            PointF min = view.GetModelPosition(new System.Drawing.Point(0, view.viewport.Y));
            PointF max = view.GetModelPosition(new System.Drawing.Point(view.viewport.X, 0));

            int numRows = (int)((max.Y - min.Y) / step) + 2;
            int numCols = (int)((max.X - min.X) / step) + 2;
            Rectangle[] rects = new Rectangle[numRows];
            int[] xCoords = new int[numCols];
            int[] yCoords = new int[numRows];

            for (int i=0; i<numCols; i++)
                xCoords[i] = view.GetScreenPosition(new PointF(min.X + step * i, 0)).X;

            for (int i=0; i<numRows; i++)
                yCoords[i] = view.GetScreenPosition(new PointF(0, min.Y + step * i)).Y;

            for (int i = 0; i < numRows; i++)
            {
                rects[i].Width = 2;
                rects[i].Height = 2;
            }

            for (int i=0; i<numCols; i++)
            {
                for (int j = 0; j < numRows; j++)
                {
                    rects[j].X = xCoords[i];
                    rects[j].Y = yCoords[j];
                }
                graphics.FillRectangles(Brushes.Gray, rects);
            }
        }

        protected void PaintAxes(ViewState view)
        {
            Pen red = new Pen(Color.Red, 2);
            Pen green = new Pen(Color.Green, 2);
            System.Drawing.Point zero = view.GetScreenPosition(new PointF(0,0));
            System.Drawing.Point xEnd = zero;
            xEnd.X += 200;
            System.Drawing.Point yEnd = zero;
            yEnd.Y -= 200;

            graphics.DrawLine(red, zero, xEnd);
            graphics.DrawLine(green, zero, yEnd);
        }


        private void PaintTriangulation(ViewState view)
        {
            // Triangulation test

            IList<IList<System.Drawing.PointF>> contours = new List<IList<System.Drawing.PointF>>();
            IList<Canguro.Analysis.Sections.Material> materials = new List<Canguro.Analysis.Sections.Material>();
            foreach (Contour con in Model.Instance.Contours)
            {
                IList<System.Drawing.PointF> list = new List<System.Drawing.PointF>();
                foreach (Point p in con.Points)
                    list.Add(new System.Drawing.PointF(p.X, p.Y));
                contours.Add(list);
                materials.Add(con.Material);
            }

            // Get Triangulator Params

            Canguro.Analysis.Sections.Meshing.Quadrangulator quadrangulator = new Canguro.Analysis.Sections.Meshing.Quadrangulator();
            quadrangulator.Quadrangulate(contours, materials);
            Canguro.Analysis.Sections.Meshing.Mesh mesh = quadrangulator.Mesh;
            Canguro.Analysis.Sections.Meshing.Vertex[] v;

            PointF[] tmp3 = new PointF[3];
            PointF[] tmp4 = new PointF[4];
            System.Drawing.Point[] ptmp3 = new System.Drawing.Point[3];
            System.Drawing.Point[] ptmp4 = new System.Drawing.Point[4];
            foreach (Canguro.Analysis.Sections.Meshing.Shape s in mesh.Shapes)
            {                
                v = s.Vertices;
                if (s is Canguro.Analysis.Sections.Meshing.Quad)
                {
                    tmp4[0].X = (float)v[0].X;
                    tmp4[0].Y = (float)v[0].Y;
                    tmp4[1].X = (float)v[1].X;
                    tmp4[1].Y = (float)v[1].Y;
                    tmp4[2].X = (float)v[2].X;
                    tmp4[2].Y = (float)v[2].Y;
                    tmp4[3].X = (float)v[3].X;
                    tmp4[3].Y = (float)v[3].Y;

                    ptmp4[0] = view.GetScreenPosition(tmp4[0]);
                    ptmp4[1] = view.GetScreenPosition(tmp4[1]);
                    ptmp4[2] = view.GetScreenPosition(tmp4[2]);
                    ptmp4[3] = view.GetScreenPosition(tmp4[3]);
                    graphics.DrawPolygon(Pens.DarkMagenta, ptmp4);
                }
                else
                {
                    tmp3[0].X = (float)v[0].X;
                    tmp3[0].Y = (float)v[0].Y;
                    tmp3[1].X = (float)v[1].X;
                    tmp3[1].Y = (float)v[1].Y;
                    tmp3[2].X = (float)v[2].X;
                    tmp3[2].Y = (float)v[2].Y;

                    ptmp3[0] = view.GetScreenPosition(tmp3[0]);
                    ptmp3[1] = view.GetScreenPosition(tmp3[1]);
                    ptmp3[2] = view.GetScreenPosition(tmp3[2]);
                    graphics.DrawPolygon(Pens.DarkMagenta, ptmp3);
                }

            }
            // End test
        }
    }
}
