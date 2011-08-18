using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator.Commands
{
    class ZoomInCommand : RunnableCommand
    {
        protected override void Run()
        {
            controller.View.Zoom *= 1.414213562f; // sqrt(2)
        }
    }
}
