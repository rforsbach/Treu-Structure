using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Canguro.Model;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Export the Model into a Drawing Exchange Format file.
    /// </summary>
    public class ExportDXFCmd : Canguro.Commands.ModelCommand
    {
        private const string header = "0\nSECTION\n2\nHEADER\n0\nENDSEC\n  0\nSECTION\n2\nENTITIES\n";
        private const string footer = "0\nENDSEC\n0\nEOF\n";

        /// <summary>
        /// Executes the command. 
        /// Displays the Save File Dialog and exports the current model to the selected Drawing Exchange Format file.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            string path = "";
            string currentPath = services.Model.CurrentPath;
            currentPath = (string.IsNullOrEmpty(currentPath)) ? Culture.Get("defaultModelName") : currentPath;
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Filter = "Drawing Exchange Format (*.dxf)|*.dxf";
            dlg.DefaultExt = "dxf";
            dlg.AddExtension = true;
            dlg.Title = Culture.Get("ExportDXFTitle");
            dlg.FileName = Path.GetFileNameWithoutExtension(currentPath) + ".dxf";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = dlg.FileName;

            StreamWriter file = null;
            try
            {
                if (path.Length > 0)
                {
                    file = File.CreateText(path);
                    file.Write(header);
                    foreach (LineElement line in services.Model.LineList)
                    {
                        if (line != null)
                        {
                            file.Write(string.Format("0\nLINE\n8\n0\n10\n{0:F}\n20\n{1:F}\n30\n{2:F}\n11\n{3:F}\n21\n{4:F}\n31\n{5:F}\n",
                                        line.I.X, line.I.Y, line.I.Z, line.J.X, line.J.Y, line.J.Z));
                        }
                    }

                    file.Write(footer);
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }
    }
}
