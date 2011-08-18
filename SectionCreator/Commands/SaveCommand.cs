using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator.Commands
{
    class SaveCommand : RunnableCommand
    {
        protected override void Run()
        {
            string path = "";
            string currentPath = model.CurrentPath;
            if (currentPath.Length == 0)
            {
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Filter = "Section Creator File (*.xsec)|*.xsec";
                dlg.DefaultExt = "xsec";
                dlg.AddExtension = true;
                dlg.Title = Culture.Get("SaveTitle");
                dlg.FileName = Culture.Get("defaultModelName");
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    path = dlg.FileName;
            }
            else
                path = currentPath;
            if (path.Length > 0)
                model.Save(path);
        }
    }
}
