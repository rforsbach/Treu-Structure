using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Save the Model to a user selected file.
    /// </summary>
    class SaveAsCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Displays the Save File Dialog and saves the current Model in the selected file.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            string path = "";
            string currentPath = services.Model.CurrentPath;
            currentPath = (string.IsNullOrEmpty(currentPath)) ? Culture.Get("defaultModelName") : currentPath;
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Filter = "Treu Structure Model (*.tsm)|*.tsm";
            dlg.DefaultExt = "tsm";
            dlg.AddExtension = true;
            dlg.Title = Culture.Get("SaveAsTitle");
            dlg.FileName = currentPath;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = dlg.FileName;
            
            if (path.Length > 0)
                services.Model.Save(path);
        }
    }
}
