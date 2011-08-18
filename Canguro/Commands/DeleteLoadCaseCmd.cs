using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Command to delete a LoadCase object
    /// </summary>
    public class DeleteLoadCaseCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Deletes the selected Items. 
        /// If none is selected, it requests a selection.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Canguro.Model.Model model = services.Model;
            if (model.LoadCases.Count > 1)
            {
                Canguro.Model.Load.LoadCase oldCase = model.ActiveLoadCase;

                // Remove associated AnalysisCase
                // Find the corresponding AbstractCase
                Canguro.Model.Load.AnalysisCase aCase = null;
                foreach (Canguro.Model.Load.AbstractCase ac in services.Model.AbstractCases)
                    if (ac is Canguro.Model.Load.AnalysisCase && ac.Name.Equals(oldCase.Name))
                    {
                        aCase = (Canguro.Model.Load.AnalysisCase)ac;
                        break;
                    }

                bool deleteLCase = true;
                // Now remove the AnalysisCase
                if (aCase != null)
                    deleteLCase = services.Model.AbstractCases.Remove(aCase);

                if (deleteLCase)
                {
                    services.Model.LoadCases.Remove(oldCase.Name);
                    services.Model.ChangeModel();
                }
            }
        }
    }
}
