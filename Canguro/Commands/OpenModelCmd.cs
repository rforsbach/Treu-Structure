using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to load an existing tsm file.
    /// </summary>
    class OpenModelCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Opens the Open File Dialog and Loads the selected tsm file.
        /// Asks to save changes if needed.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            if (services.Model.Modified)
            {
                DialogResult dr = MessageBox.Show(Culture.Get("askSaveChangesAndExit"), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                    return;
                else if (dr == DialogResult.Yes)
                    services.Run(new SaveModelCmd());
            }
            string path = "";
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Treu Structure Model (*.tsm)|*.tsm";
            dlg.DefaultExt = "tsm";
            dlg.AddExtension = true;
            dlg.Title = Culture.Get("OpenFileTitle");
            if (services.Model.CurrentPath.Length > 0)
                dlg.FileName = services.Model.CurrentPath;
            dlg.CheckPathExists = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = dlg.FileName;
            try
            {
                if (path.Length > 0)
                {
                    services.Model.Load(path);
                }
            }
            catch
            {
                MessageBox.Show(Culture.Get("errorLoadingFile") + " " + path, Culture.Get("error"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
