using System;
using System.Collections.Generic;
using System.Text;
using Canguro;
using Canguro.Model;
using Canguro.Model.Load;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Command to add an analysis case
    /// </summary>
    public class AddAnalysisCaseCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Returns the Locale dependent title for the command
        /// </summary>
        public override string Title
        {
            get
            {
                return Culture.Get("analysisCaseTitle");
            }
        }

        /// <summary>
        /// Executes the command. 
        /// Gets the parameters and creates the analysis case.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            string name = Culture.Get("defaultAnalysisCaseName");
            Canguro.Model.Model model = services.Model;
            StaticCaseProps props = new StaticCaseProps();
            AnalysisCase aCase = new AnalysisCase(name, props);
            services.GetProperties(aCase.Name, aCase, false);
            
            model.AbstractCases.Add(aCase);
        }
    }
}
