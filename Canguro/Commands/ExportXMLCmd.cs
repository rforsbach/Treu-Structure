using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Commands.Model
{
    public class ExportXMLCmd : Canguro.Commands.ModelCommand
    {
        public override void Run(Canguro.Controller.CommandServices services)
        {
            Canguro.Model.Serializer.Serializer serializer = new Canguro.Model.Serializer.Serializer(services.Model);
            serializer.Serialize("E:\\tmp.xml");
        }
    }
}
