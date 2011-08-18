using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Load
{
    /// <summary>
    /// Class representing an acceleration load. Only the direction is important.
    /// </summary>
    [Serializable]
    public class AccelLoad : AnalysisCaseAppliedLoad
    {
        /// <summary>
        /// Enum for defining the 6 directions the acceleration can take.
        /// </summary>
        public enum AccelLoadValues : byte
        {
            /// <summary>
            /// Translational direction X
            /// </summary>
            UX,
            /// <summary>
            /// Translational direction Y
            /// </summary>
            UY,
            /// <summary>
            /// Translational direction Z
            /// </summary>
            UZ,
            /// <summary>
            /// Rotation around Z
            /// </summary>
            RX,
            /// <summary>
            /// Rotation around Y
            /// </summary>
            RY,
            /// <summary>
            /// Rotation around Z
            /// </summary>
            RZ
        }

        private AccelLoadValues val;

        /// <summary>
        /// Constructor which initializes the load with a specific direction.
        /// </summary>
        /// <param name="val"></param>
        public AccelLoad(AccelLoadValues val)
        {
            this.val = val;
        }

        /// <summary>
        /// Value for the direction of the acceleration.
        /// </summary>
        public AccelLoadValues Value
        {
            get
            {
                return val;
            }
            set
            {
                Model.Instance.Undo.Change(this, val, GetType().GetProperty("Value"));
                val = value;
            }
        }

        public override string ToString()
        {
            return val.ToString();
        }

        public override bool Equals(object obj)
        {
            return val.Equals(((AccelLoad)obj).val);
        }
    }
}
