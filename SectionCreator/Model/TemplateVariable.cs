using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator
{
    class TemplateVariable
    {
        string name;
        double value;

        public TemplateVariable(string name, double value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
