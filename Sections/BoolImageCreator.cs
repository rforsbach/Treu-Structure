using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Canguro.Analysis.Sections
{
    public class BoolImageCreator
    {
        protected const int imageSize = 100;
        protected bool[,] image = new bool[imageSize, imageSize];
        List<List<Point>> contours;

        protected BoolImageCreator(List<List<Point>> contours, IList<Material> materials)
        {
            this.contours = contours;
            CalcImageFromBMP(contours, materials);
        }

        private void CalcImageFromBMP(List<List<Point>> contours, IList<Material> materials)
        {
            Bitmap bmp = new Bitmap(imageSize, imageSize);
            Graphics g = Graphics.FromImage(bmp);
            int black = Color.Black.ToArgb();
            for (int i = 0; i < contours.Count; i++)
            {
                Brush brush = (materials[i] == Material.None) ? Brushes.Black : Brushes.White;
                g.FillPolygon(brush, contours[i].ToArray());
            }

            for (int i = 0; i < imageSize; i++)
                for (int j = 0; j < imageSize; j++)
                {
                    int px = bmp.GetPixel(i, j).ToArgb();
                    image[i, j] = (px != 0 && px != black);
                }
        }

        public static BoolImageCreator CreateImage(IList<IList<PointF>> contours, IList<Material> materials, out double pixelSize)
        {
            PointF min, max;
            GetBoundingBox(contours, out min, out max);
            pixelSize = Math.Max(max.X - min.X, max.Y - min.Y) / imageSize;
            BoolImageCreator creator = new BoolImageCreator(NormalizeContours(contours, min, pixelSize), materials);
            return creator;
        }

        protected static List<List<Point>> NormalizeContours(IList<IList<PointF>> contours, PointF minPoint, double pixelSize)
        {
            List<List<Point>> newContours = new List<List<Point>>();
            foreach (IList<PointF> c in contours)
            {
                List<Point> points = new List<Point>();
                newContours.Add(points);
                foreach (PointF p in c)
                    points.Add(new Point((int)((p.X - minPoint.X) / pixelSize), (int)((p.Y - minPoint.Y) / pixelSize)));
            }
            return newContours;
        }

        protected static void GetBoundingBox(IList<IList<PointF>> contours, out PointF min, out PointF max)
        {
            if (contours.Count == 0)
            {
                max = new PointF(1, 1);
                min = new PointF(0, 0);
                return;
            }
            PointF first = contours[0][0];
            min = first;
            max = first;

            foreach (IList<PointF> c in contours)
            {
                foreach (PointF p in c)
                {
                    min.X = (min.X > p.X) ? p.X : min.X;
                    max.X = (max.X < p.X) ? p.X : max.X;
                    min.Y = (min.Y > p.Y) ? p.Y : min.Y;
                    max.Y = (max.Y < p.Y) ? p.Y : max.Y;
                }
            }
        }

        public static void WriteText(string path, bool[,] image)
        {
            StringBuilder str = new StringBuilder();
            for (int i = image.GetLength(1) - 1; i >= 0 ; i--)
            {
                for (int j = 0; j < image.GetLength(0); j++)
                    str.Append((image[j, i]) ? "X" : " ");
                str.Append("\n");
            }
            System.IO.File.WriteAllText(path, str.ToString());
        }

        public bool[,] Image
        {
            get { return image; }
        }

        public List<List<Point>> Contours
        {
            get { return contours; }
        }
    }
}
