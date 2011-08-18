using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.SectionCreator.Commands
{
    class NewCommand : RunnableCommand
    {
        protected override void Run()
        {
            new TemplateWizard(model, controller).ShowDialog();
        }
    }
}
