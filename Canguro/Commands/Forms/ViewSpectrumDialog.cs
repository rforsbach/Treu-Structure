using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Model.Load;

namespace Canguro.Commands.Forms
{
    public partial class ViewSpectrumDialog : Form
    {
        ResponseSpectrum spectrum;

        public ViewSpectrumDialog(ResponseSpectrum spectrum)
        {
            this.spectrum = spectrum;
            InitializeComponent();
        }

        private void ViewSpectrumDialog_Load(object sender, EventArgs e)
        {
            float[,] arr = spectrum.Function;
            List<Vector> list = new List<Vector>();
            for (int i=0; i<arr.GetLength(0); i++)
                list.Add(new Vector(arr[i,0], arr[i,1]));
            grid.DataSource = list;
            Text = spectrum.ToString();
        }

        private class Vector : Canguro.Utility.GlobalizedObject
        {
            float x, y;
            public Vector(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Time)]
            public float Period { get { return x; } }

            public float Acceleration { get { return y; } }


        }
    }
}