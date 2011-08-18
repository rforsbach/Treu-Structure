using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Model.Load;
using Canguro.Commands.Forms;

namespace Canguro.Commands.Load
{
    /// <summary>
    /// Model Command to add a Load Case to the Model
    /// </summary>
    public class AddLoadCaseCmd : Canguro.Commands.ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Gets the Load Case properties from the User, adds it to the Model and sets it as Active.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            string name = Culture.Get("defaultLoadCase");
            LoadCase lCase = new LoadCase(name, LoadCase.LoadCaseType.Dead);
            lCase.Name = name;
//            services.GetProperties(lCase.Name, lCase, false);

            EditLoadCaseDialog dlg = new EditLoadCaseDialog(lCase);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!services.Model.LoadCases.ContainsKey(lCase.Name))
                    services.Model.LoadCases.Add(lCase.Name, lCase);
                services.Model.ActiveLoadCase = lCase;

                AnalysisCase aCase = new AnalysisCase(lCase.Name);
                StaticCaseProps props = aCase.Properties as StaticCaseProps;
                if (props != null)
                {
                    List<StaticCaseFactor> list = props.Loads;
                    list.Add(new StaticCaseFactor(lCase));
                    props.Loads = list;
                    services.Model.AbstractCases.Add(aCase);
                }
            }
            else
                services.Model.Undo.Rollback();
        }
    }
}
