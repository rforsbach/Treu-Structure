using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    [Serializable]
    public class MultistepStaticCaseProps : AnalysisCaseProps
    {
        public string Name
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public AnalysisCase DependsOn
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
