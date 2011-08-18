using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class MaterialWrapper : ItemWrapper
    {
        private Canguro.Model.Material.Material material;
        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;
        public MaterialWrapper(Canguro.Model.Material.Material material)
        {
            this.material = material;
        }

        private static List<System.ComponentModel.PropertyDescriptor> myProps = null;
        [System.ComponentModel.Browsable(false)]
        public override List<System.ComponentModel.PropertyDescriptor> Properties
        {
            get
            {
                if (myProps == null)
                    myProps = InitProps();
                return myProps;
            }
        }

        [Canguro.Model.ModelAttributes.GridPosition(1, 2000)]
        public string Name
        {
            get { return material.Name; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        public string Type
        {
            get { return material.TypeProperties.Name; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 4000)]
        public string Description
        {
            get { return string.Format("{0}, {1}, {2:#,0.#} {3}", material.DesignProperties.Name, material.TypeProperties.Name, material.Density, us.UnitName(Canguro.Model.UnitSystem.Units.Density)); }
            set { }
        }

        //public string TotalMass
        //{
        //    get
        //    {
        //        float mass = 0;
        //        Model.UnitSystem.UnitSystem us = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;

        //        foreach (Model.LineElement elem in Model.Model.Instance.LineList)
        //            if (elem != null && elem.Properties is Model.StraightFrameProps)
        //            {
        //                Model.Section.FrameSection section = ((Model.StraightFrameProps)elem.Properties).Section;
        //                if (section.Material == material)
        //                {
        //                    float curr = elem.LengthInt * us.ToInternational(material.Density, Canguro.Model.UnitSystem.Units.Density);
        //                    float area = us.ToInternational(section.Area, Canguro.Model.UnitSystem.Units.Area);
        //                    mass += curr * area;
        //                }
        //            }
        //        return string.Format("{0:G3}", us.FromInternational(mass, Canguro.Model.UnitSystem.Units.Density));
        //    }
        //    set { }
        //}
    }
}
