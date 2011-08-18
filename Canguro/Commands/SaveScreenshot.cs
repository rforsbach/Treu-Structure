using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Canguro.Commands.Model
{
    class SaveScreenshot : ModelCommand
    {
        SaveFileDialog saveDialog = new SaveFileDialog();   

        public SaveScreenshot()
        {
        }

        public override void Run(Canguro.Controller.CommandServices services)
        {
            ImageFormat imFormat;

            saveDialog.Title = Canguro.Properties.Resources.saveScreenshotWinTitle;
            saveDialog.ValidateNames = true;
            saveDialog.SupportMultiDottedExtensions = false;
            saveDialog.OverwritePrompt = true;
            saveDialog.CreatePrompt = false;
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = "PNG";
            saveDialog.Filter = "Windows Bitmap (*.BMP) | *.BMP;*.bmp |Windows Enhanced Metafile (*.EMF) | *.EMF; *.emf; |Graphics Interchange Format (*.GIF) | *.GIF; *.gif |JPEG (*.JPG) | *.JPG; *.jpg |Portable Network Graphics (*.PNG) | *.PNG; *.png |Tag Image File Format (*.TIFF) | *.TIFF; *.tiff; *.TIF; *.tif |Windows Meta File (*.WMF) | *.WMF; *.wmf";

            DialogResult dr = saveDialog.ShowDialog();

            if (dr == DialogResult.OK || dr == DialogResult.Yes)
            {
                imFormat = selectFromExtension(saveDialog.FileName);
                Canguro.View.Printer.Instance.PrintHRImage(Canguro.View.GraphicViewManager.Instance.Device);
                Canguro.View.Printer.Instance.HiResBitmap.Save(saveDialog.FileName, imFormat);
            }
        }

        private ImageFormat selectFromExtension(string filename)
        {
            ImageFormat imFormat;

            string ext = filename.Substring(filename.LastIndexOf(".") + 1);
            ext.ToUpperInvariant();

            if (ext == "BMP")
                imFormat = ImageFormat.Bmp;
            else if (ext == "EMF")
                imFormat = ImageFormat.Emf;
            else if (ext == "GIF")
                imFormat = ImageFormat.Gif;
            else if (ext == "JPG")
                imFormat = ImageFormat.Jpeg;
            else if (ext == "PNG")
                imFormat = ImageFormat.Png;
            else if (ext == "TIFF")
                imFormat = ImageFormat.Tiff;
            else if (ext == "WMF")
                imFormat = ImageFormat.Wmf;
            else
                imFormat = ImageFormat.Png;

            return imFormat;
        }
    }
}
