using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Analysis.Sections
{
    class TestMain
    {
        public static void Main(string[] args)
        {

            //TestBoolImageCreator();

            TestJFem();
        }

        static void TestBoolImageCreator()
        {
            //IList<IList<PointF>> contours = new List<IList<PointF>>();
            //List<PointF> points = new List<PointF>();
            //contours.Add(points);
            //points.Add(new PointF(0, 0));
            //points.Add(new PointF(2, 0));
            //points.Add(new PointF(2, 1));
            //points.Add(new PointF(0, 1));
            //double px;
            //bool[,] image = BoolImageCreator.CreateImage(contours, out px);
            //BoolImageCreator.WriteText("E:\\img.txt", image);
        }

        static void TestJ()
        {
            #region Test Section Props calc

            Bitmap bm = new Bitmap(@"c:\sqr.png", false);
            if (bm.Width != 1000 || bm.Height != 1000)
                throw new ArgumentException("R2x2.png does not have the correct size of 1000x1000");

            bool[,] pixelatedSection = new bool[1000, 1000];
            for (int i = 0; i < 1000; i++)
                for (int j = 0; j < 1000; j++)
                    pixelatedSection[i, j] = (bm.GetPixel(i, j).ToArgb() != -1);

            DateTime start = DateTime.Now;
            CrossSectionPixelated sec = new CrossSectionPixelated(pixelatedSection, 0.0001);

            #endregion

            #region Test J

            float[] js = new float[8];
            float[] expectedJs = new float[] { 0.141f, 2.256f, 36.096f, 1410f, 0.458f, 1.124f, 3.12f, 23.28f };
            float[] ratios = new float[8];
            
            //js[0] = sec.getJFromRect(1, 1);
            js[1] = sec.getJFromRect(2, 2);
            //js[2] = sec.getJFromRect(4, 4);
            //js[3] = sec.getJFromRect(10, 10);

            //js[4] = sec.getJFromRect(1, 2);
            //js[5] = sec.getJFromRect(1, 4);
            //js[6] = sec.getJFromRect(1, 10);

            //js[7] = sec.getJFromRect(2, 10);

            Console.WriteLine("\n\n");

            //for (int i = 0; i < 8; i++)
            //{
            //    ratios[i] = js[i] / expectedJs[i];
            //    Console.WriteLine("Treu = " + js[i] + ", Expected = " + expectedJs[i] + ", ratio = " + ratios[i]);
            //}

            Console.WriteLine("\n\n");

            #endregion

            #region End Section Props calc
            TimeSpan elapsedTime = DateTime.Now - start;
            Console.WriteLine(sec);
            Console.WriteLine("Time calculating props: " + elapsedTime);

#if DEBUG
            Console.ReadLine();
#endif
#endregion
        }

        static void TestJFem()
        {
            FemCrossSection femSection = new FemCrossSection(new Meshing.Mesh());
            femSection.GetSectionProperties();
            
            Console.WriteLine(femSection);

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
