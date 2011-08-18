using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Canguro.SectionCreator.Commands
{
    class CutCommand : RunnableCommand
    {
        protected override void Run()
        {
            controller.Execute("Copy");
            controller.Execute("Delete");
        }
    }
}
