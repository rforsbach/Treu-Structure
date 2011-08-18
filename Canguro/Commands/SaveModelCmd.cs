using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Save the Model to disk.
    /// </summary>
    class SaveModelCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// If the CurrentPath is not set, displays the Save File Dialog. 
        /// Saves the Model in a file.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            string path = "";
            string currentPath = services.Model.CurrentPath;
            if (currentPath.Length == 0)
            {
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Filter = "Treu Structure Model (*.tsm)|*.tsm";
                dlg.DefaultExt = "tsm";
                dlg.AddExtension = true;
                dlg.Title = Culture.Get("SaveTitle");
                dlg.FileName = Culture.Get("defaultModelName");
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    path = dlg.FileName;
            }
            else
                path = currentPath;
            if (path.Length > 0)
                services.Model.Save(path);
        }
    }
}
