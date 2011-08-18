using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator
{
    class SectionTemplate
    {
        List<TemplateVariable> variables = new List<TemplateVariable>();
        List<TemplatePoint> points = new List<TemplatePoint>();

        public List<TemplateVariable> Variables
        {
            get { return variables; }
        }

        public List<TemplatePoint> Points
        {
            get { return points; }
        }
    }
}
