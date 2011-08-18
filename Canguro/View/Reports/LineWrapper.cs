using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.View.Reports
{
    class LineWrapper : ItemWrapper
    {
        private Canguro.Model.LineElement line;

        public LineWrapper(Canguro.Model.LineElement element)
        {
            line = element;
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

        [Canguro.Model.ModelAttributes.GridPosition(1, 1000)]
        public string ID
        {
            get { return line.Id.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(2, 1000)]
        public string I
        {
            get { return line.I.Id.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(3, 1000)]
        public string J
        {
            get { return line.J.Id.ToString(); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(4, 2000)]
        [Canguro.Model.ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Angle)]
        public string Angle
        {
            get { return string.Format("{0:#,0.#}", line.Angle); }
            set { }
        }

        [Canguro.Model.ModelAttributes.GridPosition(5, 2000)]
        public string Section
        {
            get
            {
                if (line.Properties is Canguro.Model.StraightFrameProps)
                    return ((Canguro.Model.StraightFrameProps)line.Properties).Section.ToString();
                return "";
            }
            set
            {
            }
        }
    }
}
