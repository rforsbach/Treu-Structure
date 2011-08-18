using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class SectionWrapper : ItemWrapper
    {
        private Canguro.Model.Section.FrameSection section;
        public SectionWrapper(Canguro.Model.Section.FrameSection section)
        {
            this.section = section;
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
            get { return section.Name; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        public string Shape
        {
            get { return section.Shape;  }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 2000)]
        public string Material
        {
            get { return section.Material.Name; }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 4000)]
        public string Description
        {
            get 
            {
                return section.Description;
            }
            set { }
        }
    }
}
