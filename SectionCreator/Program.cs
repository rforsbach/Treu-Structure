using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Canguro.SectionCreator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TestModel();
            Application.Run(new MainFrm());
        }

        static void TestModel()
        {
            Model model = Model.Instance;
            //Contour con = new Contour("outer");
            //con.Points.Add(new Point(0, 0));
            //con.Points.Add(new Point(200, 0));
            //con.Points.Add(new Point(200, 20f));
            //con.Points.Add(new Point(110f, 20f));
            //con.Points.Add(new Point(110f, 180f));
            //con.Points.Add(new Point(200, 180f));
            //con.Points.Add(new Point(200, 200));
            //con.Points.Add(new Point(0, 200));
            //con.Points.Add(new Point(0, 180f));
            //con.Points.Add(new Point(90f, 180f));
            //con.Points.Add(new Point(90f, 20f));
            //con.Points.Add(new Point(0, 20f));
            //model.Contours.Add(con);
            Contour con2 = new Contour("inner 1");
            con2.Points.Add(new Point(5f, 5f));
            con2.Points.Add(new Point(15f, 5f));
            con2.Points.Add(new Point(15f, 15f));
            con2.Points.Add(new Point(5f, 15f));
            model.Contours.Add(con2);
            //con = new Contour("inner 2");
            //con.Points.Add(new Point(60f, 60f));
            //con.Points.Add(new Point(80f, 60f));
            //con.Points.Add(new Point(80f, 80f));
            //con.Points.Add(new Point(60f, 80f));
            //model.Contours.Add(con);
            model.Undo.Commit();
        }
    }
}