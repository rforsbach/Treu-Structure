using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Canguro.Commands.Forms
{
    public partial class PrintPreview : Form
    {
        public PrintPreview()
        {
            InitializeComponent();
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            Canguro.Commands.Model.PrintCmd.PrintDocument();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public DialogResult ShowDialog(PrintDocument pDoc)
        {
            printPreviewControl.Document = pDoc;

            return ShowDialog();
        }

        private void orientButton_Click(object sender, EventArgs e)
        {
            printPreviewControl.Document.DefaultPageSettings.Landscape = !printPreviewControl.Document.DefaultPageSettings.Landscape;

            printPreviewControl.InvalidatePreview();
        }
    }
}