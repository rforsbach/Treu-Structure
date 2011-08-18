using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.SectionCreator.Commands
{
    class OpenCommand : RunnableCommand
    {
        protected override void Run()
        {
            if (model.Modified)
            {
                DialogResult dr = MessageBox.Show(Culture.Get("askSaveChangesAndExit"), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                    return;
                else if (dr == DialogResult.Yes)
                    new SaveCommand().Run(controller, model);
            }
            string path = "";
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Section Creator File (*.xsec)|*.xsec";
            dlg.DefaultExt = "xsec";
            dlg.AddExtension = true;
            dlg.Title = Culture.Get("OpenFileTitle");
            if (model.CurrentPath.Length > 0)
                dlg.FileName = model.CurrentPath;
            dlg.CheckPathExists = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = dlg.FileName;
            try
            {
                if (path.Length > 0)
                {
                    model.Load(path);
                    new ZoomAllCommand().Run(controller, model);
                }
            }
            catch
            {
                MessageBox.Show(Culture.Get("errorLoadingFile") + " " + path, Culture.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
