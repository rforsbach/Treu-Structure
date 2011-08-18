using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator.Commands
{
    abstract class RunnableCommand : ICommand
    {
        protected Controller controller;
        protected Model model;

        public void Run(Controller controller, Model model)
        {
            this.controller = controller;
            this.model = model;
            try
            {
                Run();
                controller.EndCommand();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        abstract protected void Run();
    }
}
