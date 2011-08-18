using System;
using System.Collections.Generic;
using System.Text;

using Canguro.View;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Canguro.Commands.Model
{
    public class PrintCmd : ModelCommand
    {
        private static PrintDocument printingDocument;

        public PrintDocument PrintingDocument
        {
            get { return printingDocument; }
        }

        public PrintCmd()
        {
            // Create the document and attach an event handler.
            printingDocument = new PrintDocument();
            printingDocument.PrintPage += printPage;
        }

        public override void Run(Canguro.Controller.CommandServices services)
        {
            PrintDocument();
        }

        public static void PrintDocument()
        {
            // Allow the user to choose a printer and specify other settings.
            PrintDialog dlgSettings = new PrintDialog();
            dlgSettings.Document = printingDocument;

            // If the user clicked OK, print the document.
            if (dlgSettings.ShowDialog() == DialogResult.OK)
            {
                // This method returns immediately, before the print job starts.
                // The PrintPage event will fire asynchronously.
                printingDocument.Print();
            }
        }

        private static void printPage(object sender, PrintPageEventArgs e)
        {
            // Define the font.
            using (Font font = new Font("Calibri", 10))
            {
                // Determine the position on the page.
                float x = e.MarginBounds.Left;
                float y = e.MarginBounds.Top;

                // Determine the height of a line (based on the font used).
                float lineHeight = font.GetHeight(e.Graphics);

                Printer printer = Canguro.View.Printer.Instance;
                printer.PrintHRImage(GraphicViewManager.Instance.Device);

                // Set smoothing mode for High Quality
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Put Treu Software Logo in the document
                e.Graphics.DrawImage(Canguro.Properties.Resources.Treu_Logo, new Rectangle((int)x, (int)y, Canguro.Properties.Resources.Treu_Logo.Width / 2, Canguro.Properties.Resources.Treu_Logo.Height / 2));

                // Put some info in the printing
                //string title = (string.IsNullOrEmpty(model.CurrentPath)) ? Culture.Get("defaultModelName") : System.IO.Path.GetFileNameWithoutExtension(model.CurrentPath);
                string title = (string.IsNullOrEmpty(Canguro.Model.Model.Instance.CurrentPath)) ? Culture.Get("defaultModelName") : System.IO.Path.GetFileNameWithoutExtension(Canguro.Model.Model.Instance.CurrentPath);
                e.Graphics.DrawString(Culture.Get("printFileText") + title, font, Brushes.Black, x + Canguro.Properties.Resources.Treu_Logo.PhysicalDimension.Width / 2f + 5f, y);
                e.Graphics.DrawString(Culture.Get("printDate") + DateTime.Today.ToString("dddd, dd MMMM yyyy"), font, Brushes.Black, x + Canguro.Properties.Resources.Treu_Logo.PhysicalDimension.Width / 2f + 5f, y + lineHeight);

                // Update y coordinate
                y = y + Canguro.Properties.Resources.Treu_Logo.PhysicalDimension.Height / 2f + 5f;

                // Draw a line just below Logo and print information
                e.Graphics.DrawLine(new Pen(System.Drawing.Color.Black, 2), x, y, e.MarginBounds.Right, y);

                y += 10f;

                float scale;

                // Get the best appropriate scale for making the Bitmap fit in the paper sheet
                if (printer.IsOrientedLandscape)
                {
                    scale = (float)(e.MarginBounds.Width - x) / (float)(printer.HiResBitmap.Width);
                    if (scale * printer.HiResBitmap.Height > e.MarginBounds.Height - y)
                        scale = (float)(e.MarginBounds.Bottom - y) / (float)(printer.HiResBitmap.Height);
                }
                else
                {
                    scale = (float)(e.MarginBounds.Bottom - y) / (float)(printer.HiResBitmap.Height);
                    if (scale * printer.HiResBitmap.Width > e.MarginBounds.Width - x)
                        scale = (float)(e.MarginBounds.Width - x) / (float)(printer.HiResBitmap.Width);
                }

                // Center the bitmap in the available sheet space
                x += (e.MarginBounds.Right - scale * printer.HiResBitmap.Width - x) / 2f;
                y += (e.MarginBounds.Bottom - scale * printer.HiResBitmap.Height - y) / 2f;
                
                // Draw the scaled bitmap
                e.Graphics.DrawImage(printer.HiResBitmap, new Rectangle((int)x, (int)y, (int)(scale*printer.HiResBitmap.Width), (int)(scale*printer.HiResBitmap.Height)));

                // Draw a rectangle for the image
                e.Graphics.DrawRectangle(new Pen(System.Drawing.Color.Black, 2), x, y, scale * printer.HiResBitmap.Width, scale * printer.HiResBitmap.Height);
            }
        }
    }
}
