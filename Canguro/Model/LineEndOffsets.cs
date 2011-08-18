using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    [Serializable]
    public class LineEndOffsets : Canguro.Utility.GlobalizedObject, ICloneable
    {
        private float offI, offJ, factor;
        private bool isConstant = false;

        public static readonly LineEndOffsets Empty = new LineEndOffsets();
        static LineEndOffsets()
        {
            Empty.isConstant = true;
        }

        public LineEndOffsets() : this(0, 0, 0) { }

        public LineEndOffsets(float offI, float offJ, float factor)
        {
            this.offI = offI;
            this.offJ = offJ;
            this.factor = factor;
        }

        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float EndI
        {
            get { return Canguro.Model.Model.Instance.UnitSystem.FromInternational(offI, Canguro.Model.UnitSystem.Units.Distance); }
            set 
            {
                if (isConstant)
                    throw new InvalidOperationException("Current LineEndOffset is Constant");
                offI = Canguro.Model.Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Distance);
            }
        }

        internal float EndIInternational
        {
            get { return offI; }
        }

        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Distance)]
        public float EndJ
        {
            get { return Canguro.Model.Model.Instance.UnitSystem.FromInternational(offJ, Canguro.Model.UnitSystem.Units.Distance); }
            set
            {
                if (isConstant)
                    throw new InvalidOperationException("Current LineEndOffset is Constant");
                offJ = Canguro.Model.Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Distance);
            }
        }

        internal float EndJInternational
        {
            get { return offJ; }
        }

        public float Factor
        {
            get { return factor; }
            set
            {
                if (isConstant)
                    throw new InvalidOperationException("Current LineEndOffset is Constant"); 
                factor = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0:G3}, {1:G3} ({2:G3})", EndI, EndJ, Factor);
        }

        #region ICloneable Members
        public object Clone()
        {
            return new LineEndOffsets(offI, offJ, factor);
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is LineEndOffsets)
            {
                LineEndOffsets o = (LineEndOffsets)obj;
                return offI.Equals(o.offI) && offJ.Equals(o.offJ) && factor.Equals(o.factor);
            }
            return false;
        }
    }
}
