using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.Model
{
    /// <summary>
    /// Model Command to show the Reports Window.
    /// </summary>
    class MakeReportCmd : ModelCommand
    {
        /// <summary>
        /// Executes the command. 
        /// Opens the Reports Window.
        /// </summary>
        /// <param name="services">CommandServices object to interact with the system</param>
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Canguro.Commands.Forms.ReportOptionsDialog opt = new Canguro.Commands.Forms.ReportOptionsDialog(services.Model);
            if (opt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<Canguro.View.Reports.ReportOptions> options = opt.GetSelectedOptions();
                bool onlySelected = opt.OnlySelected;
                if (options.Count > 0)
                {
                    Canguro.View.Reports.ReportsWindow wnd = new Canguro.View.Reports.ReportsWindow(services, options, onlySelected);
                    //wnd.ShowDialog();
                    wnd.Show();
                }
            }
        }

        public override string Title
        {
            get
            {
                return Culture.Get("generatingReport");
            }
        }
    }
}