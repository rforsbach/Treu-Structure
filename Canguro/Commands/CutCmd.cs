using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to copy the selected items to the clipboard and delete them.
    /// </summary>
    public class CutCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Executes CopyCmd and DeleteCmd.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            new CopyCmd().Run(services);
            new DeleteCmd().Run(services);
        }
    }
}
