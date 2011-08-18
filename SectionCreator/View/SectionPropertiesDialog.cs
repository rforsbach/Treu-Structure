using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Canguro.Analysis.Sections;

namespace Canguro.SectionCreator
{
    public partial class SectionPropertiesDialog : Form
    {
        protected Model model;
        CrossSectionPixelated section;

        public SectionPropertiesDialog(Model model)
        {
            this.model = model;
            InitializeComponent();
        }

        private void SectionPropertiesDialog_Load(object sender, EventArgs e)
        {
            IList<IList<System.Drawing.PointF>> contours = new List<IList<System.Drawing.PointF>>();
            IList<Material> materials = new List<Material>();
            foreach (Contour con in model.Contours)
            {
                IList<System.Drawing.PointF> list = new List<System.Drawing.PointF>();
                foreach (Point p in con.Points)
                    list.Add(new System.Drawing.PointF(p.X, p.Y));
                contours.Add(list);
                materials.Add(con.Material);
            }
            double pixelSize;
            bool[,] image = BoolImageCreator.CreateImage(contours, materials, out pixelSize).Image;
            section = new CrossSectionPixelated(image, pixelSize);
            propertyGrid.SelectedObject = section;
            BoolImageCreator.WriteText("E:\\img.txt", image);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}