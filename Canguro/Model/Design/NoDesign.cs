using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Design
{
    [Serializable]
    public class NoDesign : DesignOptions
    {
        private NoDesign() { }

        public static readonly NoDesign Instance = new NoDesign();

        public override string ToString()
        {
            return Culture.Get("NoDesign");
        }

        public override void SetDefaults()
        {
        }

        public override void CopyFrom(DesignOptions copy)
        {
        }

        public override List<Canguro.Model.Load.LoadCombination> AddDefaultCombos()
        {
            return new List<Canguro.Model.Load.LoadCombination>();
        }
    }
}
