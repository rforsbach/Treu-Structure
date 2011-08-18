using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Commands.Forms;

namespace Canguro.Commands.Model
{
    public class ConstraintsCmd : ModelCommand
    {
        public override void Run(Canguro.Controller.CommandServices services)
        {
            ConstraintsDialog dlg = new ConstraintsDialog(services.Model);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                services.Model.Undo.Rollback();
        }
    }
}
