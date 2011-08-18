using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Support class for ModalCaseProps. Includes an AccelLoad (or LoadCase) and related information.
    /// </summary>
    [Serializable]
    public class ModalCaseFactor
    {
        private AnalysisCaseAppliedLoad appliedLoad;
        private int cycles;
        private float ratio;

        /// <summary>
        /// Assigns all values.
        /// </summary>
        /// <param name="load">Applied Load Case or Acceleration Load</param>
        /// <param name="maxCycles">Max Cycles for the algorithm to converge</param>
        /// <param name="partRatio">Target Participation Ratio</param>
        public ModalCaseFactor(AnalysisCaseAppliedLoad load, int maxCycles, float partRatio)
        {
            appliedLoad = load;
            cycles = (maxCycles < 0) ? 0 : maxCycles;
            ratio = (partRatio <= 0) ? 0.01F : (partRatio >= 100) ? 99.99F : partRatio;
        }

        /// <summary>
        /// Assigns the Applied Load and default values for Cycles(0) and Ratio(99)
        /// </summary>
        /// <param name="load">Applied Load Case or Acceleration Load</param>
        public ModalCaseFactor(AnalysisCaseAppliedLoad load)
            : this(load, 0, 99)
        { }

        /// <summary>
        /// The AccelLoad or LoadCase.
        /// </summary>
        public AnalysisCaseAppliedLoad AppliedLoad
        {
            get
            {
                return appliedLoad;
            }
            set
            {
                if (value is AccelLoad || value is LoadCase)
                {
                    Model.Instance.Undo.Change(this, appliedLoad, GetType().GetProperty("AppliedLoad"));
                    appliedLoad = value;
                }
            }
        }

        /// <summary>
        /// Max number of cycles. Has to be above 0;
        /// </summary>
        public int Cycles
        {
            get
            {
                return cycles;
            }
            set
            {
                if (value > 0)
                {
                    Model.Instance.Undo.Change(this, cycles, GetType().GetProperty("Cycles"));
                    cycles = value;
                }
            }
        }

        /// <summary>
        /// Target Participation Ratio (%). Should be above 0 and below 100;
        /// </summary>
        public float Ratio
        {
            get
            {
                return ratio;
            }
            set
            {
                value = (value <= 0) ? 0.01F : (value >= 100) ? 99.99F : value;
                Model.Instance.Undo.Change(this, ratio, GetType().GetProperty("Ratio"));
                ratio = value;
            }
        }
    }
}
