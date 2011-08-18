using System; 
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Support class for StaticCaseProps. Includes a LoadCase (or AccelLoad) and a scale factor.
    /// </summary>
    [Serializable]
    public class StaticCaseFactor
    {
        private AnalysisCaseAppliedLoad appliedLoad;
        private float factor;

        /// <summary>
        /// Initializes the object with an AnalysisCaseAppliedLoad and default factor 1.
        /// </summary>
        /// <param name="load">The initial Applied Load</param>
        public StaticCaseFactor(AnalysisCaseAppliedLoad load)
            : this (load, 1)
        {  }

        /// <summary>
        /// Initializes the object with an Applied Load and a factor.
        /// </summary>
        /// <param name="load">The initial applied load</param>
        /// <param name="factor">The initial factor</param>
        public StaticCaseFactor(AnalysisCaseAppliedLoad load, float factor)
        {
            appliedLoad = load;
            this.factor = factor;
        }

        /// <summary>
        /// Property representing the applied load.
        /// Can be a LoadCase or an AccelLoad
        /// </summary>
        public AnalysisCaseAppliedLoad AppliedLoad
        {
            get
            {
                return appliedLoad;
            }
            set
            {
                if (value is LoadCase || value is AccelLoad)
                {
                    Model.Instance.Undo.Change(this, appliedLoad, GetType().GetProperty("AppliedLoad"));
                    appliedLoad = value;
                }
            }
        }

        /// <summary>
        /// The scale factor to be applied to the Applied Load. By default it's 1.0.
        /// </summary>
        public float Factor
        {
            get
            {
                return factor;
            }
            set
            {
                Model.Instance.Undo.Change(this, factor, GetType().GetProperty("Factor"));
                factor = value;
            }
        }
    }
}
