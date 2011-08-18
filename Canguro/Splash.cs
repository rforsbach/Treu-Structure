using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Canguro
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();

            Bitmap b = new Bitmap(this.BackgroundImage);
            b.MakeTransparent(b.GetPixel(1, 1));
            this.BackgroundImage = b;
        }

        private static int time;
        public static void Show(int milliseconds)
        {
            Thread th = new Thread(new ThreadStart(DoSplash));
            time = milliseconds;
            th.Start();
        }

        private static void DoSplash()
        {
            Splash sp = new Splash();

            sp.Show();
            Thread.Sleep(time);
            sp.Hide();
        }
    }
}