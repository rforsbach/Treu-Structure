using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public class ShellSection : AreaSection
    {
        private float thicknessMembrane;
        private float thicknessBending;
        private ShellType shellType;
        private float materialAngle;
        private ShellDesignParams designParams;

        public ShellSection(string name, string shape, Material.Material material, float thicknessMembrane, float thicknessBending, ShellType type, float materialAngle, ShellDesignParams designParams)
            : base(name, shape, material)
        {
            this.thicknessBending = thicknessBending;
            this.thicknessMembrane = thicknessMembrane;
            this.shellType = type;
            this.materialAngle = materialAngle;
            
            if (designParams != null)
                this.designParams = designParams;
            else
                this.designParams = new ShellDesignParams();
        }
        
        public ShellType ShellType
        {
            get
            {
                return shellType;
            }
            set
            {
                Model.Instance.Undo.Change(this, shellType, GetType().GetProperty("Shelltype"));
                shellType = value;
            }
        }

        public float MaterialAngle
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(materialAngle, Canguro.Model.UnitSystem.Units.Angle);
            }
            set
            {
                Model.Instance.Undo.Change(this, materialAngle, GetType().GetProperty("MaterialAngle"));
                materialAngle = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Angle);
            }
        }

        /// <summary>
        /// The element volume for the element self-weight and mass calculations
        /// </summary>
        public float ThicknessMembrane
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(thicknessMembrane, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                Model.Instance.Undo.Change(this, thicknessMembrane, GetType().GetProperty("ThicknessMembrane"));
                thicknessMembrane = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
        }

        /// <summary>
        /// The plate-bending and transverse-shearing stiffnesses for full-shell and pure-plate sections
        /// </summary>
        public float ThicknessBending
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(thicknessBending, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
            set
            {
                Model.Instance.Undo.Change(this, thicknessBending, GetType().GetProperty("ThicknessBending"));
                thicknessBending = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.SmallDistance);
            }
        }

        public ShellDesignParams DesignParams
        {
            get
            {
                return designParams;
            }
        }
    }
}
