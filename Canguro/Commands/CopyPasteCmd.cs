using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Canguro.Model;
using Canguro.Controller.Snap;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to copy to the clipboard and paste many times.
    /// </summary>
    public class CopyPasteCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Execute CopyCmd and PasteCmd until cancelled.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            services.Run(new CopyCmd());
            PasteCmd cmd;
            do
            {
                services.Run(cmd = new PasteCmd());
                services.Model.ChangeModel();
            }
            while (cmd.ObjectCount > 0);
        }
    }
}
