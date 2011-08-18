using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results
{
    public class AluminiumDesign1
    {
        public Section.Section designSect;
        public DesignType designType;
        public DesignStatus status;
        public float ratio;
        public DesignRatioType ratioType;
        public float location;
        public int error;
        public int warning;
        private Canguro.Model.Load.LoadCombination combo;
    }
}
