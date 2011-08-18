using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator
{
    class TemplatePoint
    {
        string x = "0";
        string y = "0";
        private readonly List<TemplateVariable> variables;

        public TemplatePoint(string xFormula, string yFormula, List<TemplateVariable> vars)
        {
            x = xFormula;
            y = yFormula;
            variables = vars;
        }

        public string XFormula
        {
            get { return x; }
            set { x = value; }
        }

        public string YFormula
        {
            get { return y; }
            set { y = value; }
        }

        public double X
        {
            get { return 0; }
        }

        public double Y
        {
            get { return 0; }
        }

        public override string ToString()
        {
            return x + ", " + y;
        }

        public void SetValue(string csv)
        {
            string[] vals = csv.Split(new char[] { ',' });
            if (vals.Length == 2)
            {
                XFormula = vals[0];
                YFormula = vals[1];
            }
        }
    }
}
