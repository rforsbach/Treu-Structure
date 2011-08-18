using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to Create a New file
    /// </summary>
    public class NewModelWizard : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Asks to save changes if needed and Resets the current Model.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            try
            {
                if (services.Model.Modified)
                {
                    DialogResult dr = MessageBox.Show(Culture.Get("askSaveChangesAndExit"), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (dr == DialogResult.Cancel)
                        return;
                    else if (dr == DialogResult.Yes)
                        services.Run(new SaveModelCmd());
                }

                services.Model.Reset();
            }
            catch (Exception)
            {
                MessageBox.Show("Error creating new file.");
            }
        }
    }
}
