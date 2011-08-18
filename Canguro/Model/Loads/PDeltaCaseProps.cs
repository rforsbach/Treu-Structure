using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    public class PDeltaCaseProps : StaticCaseProps
    {
        private NonLinearParams nlParams = new NonLinearParams();

        /// <summary>
        /// Creates an object with all the dead load cases.
        /// </summary>
        public PDeltaCaseProps()
        {
            ReloadDefaultLoads();
        }

        public void ReloadDefaultLoads()
        {
            loads.Clear();
            foreach (LoadCase lc in Model.Instance.LoadCases.Values)
                if (lc != null && lc.CaseType == LoadCase.LoadCaseType.Dead)
                    loads.Add(new StaticCaseFactor(lc, 1.0f));

            if (loads.Count == 0)
                foreach (LoadCase lc in Model.Instance.LoadCases.Values)
                    if (lc != null)
                    {
                        loads.Add(new StaticCaseFactor(lc, 1.0f));
                        break;
                    }
        }

        public NonLinearParams NonLinearParams
        {
            get { return nlParams; }
        }

        #region INamed Members
        public override string Name
        {
            get
            {
                return Culture.Get("PDeltaCaseName");
            }
        }
        #endregion
    }
}
