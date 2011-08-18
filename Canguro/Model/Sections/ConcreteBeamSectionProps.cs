using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class ConcreteBeamSectionProps : ConcreteSectionProps
    {
        private float concreteCoverTop = 0.0635f;
        private float concreteCoverBottom = 0.0635f;
        private float roTopLeft = 0;
        private float roTopRight = 0;
        private float roBottomLeft = 0;
        private float roBottomRight = 0;

        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float ConcreteCoverTop
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(concreteCoverTop, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                if (value != concreteCoverTop)
                {
                    Model.Instance.Undo.Change(this, ConcreteCoverTop, GetType().GetProperty("ConcreteCoverTop"));
                    concreteCoverTop = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }

        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float ConcreteCoverBottom
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(concreteCoverBottom, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                if (value != concreteCoverBottom)
                {
                    Model.Instance.Undo.Change(this, ConcreteCoverBottom, GetType().GetProperty("ConcreteCoverBottom"));
                    concreteCoverBottom = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }

        /// <summary>
        /// Reinforcement Overrides for Ductile Beam
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float RoTopLeft
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(roTopLeft, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                if (value != roTopLeft)
                {
                    Model.Instance.Undo.Change(this, ConcreteCoverBottom, GetType().GetProperty("RoTopLeft"));
                    roTopLeft = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }

        /// <summary>
        /// Reinforcement Overrides for Ductile Beam
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float RoTopRight
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(roTopRight, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                if (value != roTopRight)
                {
                    Model.Instance.Undo.Change(this, RoTopRight, GetType().GetProperty("RoTopRight"));
                    roTopRight = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }

        /// <summary>
        /// Reinforcement Overrides for Ductile Beam
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float RoBottomLeft
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(roBottomLeft, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                if (value != roBottomLeft)
                {
                    Model.Instance.Undo.Change(this, RoBottomLeft, GetType().GetProperty("RoBottomLeft"));
                    roBottomLeft = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }

        /// <summary>
        /// Reinforcement Overrides for Ductile Beam
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.SmallDistance)]
        public float RoBottomRight
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(roBottomRight, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                if (value != roBottomRight)
                {
                    Model.Instance.Undo.Change(this, RoBottomRight, GetType().GetProperty("RoBottomRight"));
                    roBottomRight = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
                }
            }
        }
    }
}
