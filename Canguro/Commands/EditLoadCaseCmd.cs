using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Commands.Forms;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Model Command to Edit the Active Load Case
    /// </summary>
    public class EditLoadCaseCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Opens the properties window to edit the Active Load Case.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Canguro.Model.Load.LoadCase lCase = services.Model.ActiveLoadCase;
            string name = lCase.Name;
            //services.GetProperties(name, lCase, false);
            
            EditLoadCaseDialog dlg = new EditLoadCaseDialog(lCase);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!name.Equals(lCase.Name))
                    foreach (Canguro.Model.Load.AbstractCase aCase in services.Model.AbstractCases)
                        if (name.Equals(aCase.Name) && aCase is Canguro.Model.Load.AnalysisCase)
                            aCase.Name = lCase.Name;

                services.Model.ChangeModel();
            }
            else
                services.Model.Undo.Rollback();
        }
    }
}
