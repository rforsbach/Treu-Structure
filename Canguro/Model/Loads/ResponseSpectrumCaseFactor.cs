using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Support class for ResponseSpectrumProps
    /// </summary>
    [Serializable]
    public class ResponseSpectrumCaseFactor
    {
        private AnalysisCaseAppliedLoad accel;
        //private string function;
        private float factor;

        /// <summary>
        /// Initializes the object with the AccelLoad and Scale factor.
        /// </summary>
        /// <param name="acceleration">AccelLoad applied to the Response Spectrum</param>
        /// <param name="factor">Scale factor for the AccelLoad</param>
        public ResponseSpectrumCaseFactor(AnalysisCaseAppliedLoad acceleration, float factor)
        {
            accel = (acceleration is AccelLoad) ? acceleration : new AccelLoad(AccelLoad.AccelLoadValues.UX);
            this.factor = factor;
        }
        
        /// <summary>
        /// Initializes the object with the AccelLoad and default Scale factor(1.0).
        /// </summary>
        /// <param name="acceleration">AccelLoad applied to the Response Spectrum</param>
        public ResponseSpectrumCaseFactor(AnalysisCaseAppliedLoad acceleration)
            : this(acceleration, 1)
        { }

        /// <summary>
        /// Applied Acceleration Load
        /// </summary>
        public AnalysisCaseAppliedLoad Accel
        {
            get
            {
                return accel;
            }
            set
            {
                if (value is AccelLoad)
                {
                    Model.Instance.Undo.Change(this, accel, GetType().GetProperty("Accel"));
                    accel = value;
                }
            }
        }

        /*
        public string Function
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        */

        /// <summary>
        /// Scale factor applied to the AccelLoad
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
