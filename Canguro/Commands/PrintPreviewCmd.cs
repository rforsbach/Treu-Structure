using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using Canguro.View;

namespace Canguro.Commands.Model
{
    class PrintPreviewCmd : PrintCmd
    {
        public override void Run(Canguro.Controller.CommandServices services)
        {
            previewFile_Click(PrintingDocument, null);
        }

        private void previewFile_Click(object sender, System.EventArgs e)
        {
            try
            {
                Canguro.Commands.Forms.PrintPreview printPreviewDialog = new Canguro.Commands.Forms.PrintPreview();

                PrintDocument doc = (PrintDocument)sender;
                doc.DefaultPageSettings.Landscape = Canguro.View.Printer.Instance.IsOrientedLandscape;

                printPreviewDialog.ShowDialog(doc);

                printPreviewDialog.Dispose();
                printPreviewDialog = null;

            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.Message.ToString());
            }
        }
    }
}
