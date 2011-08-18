using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Canguro.Model.ModelAttributes
{
    [AttributeUsage (AttributeTargets.Property)]
    class GridPositionAttribute : Attribute
    {
        const int defaultWidth = 40;

        int position;
        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        int width;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        bool expand;
        public bool Expand
        {
            get { return expand; }
            set { expand = value; }
        }

        public GridPositionAttribute(int position)
        {
            this.position = position;
            width = defaultWidth;
            expand = false;
        }

        public GridPositionAttribute(int position, int width)
            : this(position)
        {
            this.width = width;
        }

        public GridPositionAttribute(int position, int width, bool expand)
            : this(position, width)
        {
            this.expand = expand;
        }

        public static GridPositionAttribute Default = new GridPositionAttribute(9999);
    }

    [AttributeUsage (AttributeTargets.Property)]
    class UnitsAttribute : Attribute
    {
        Canguro.Model.UnitSystem.Units units;
        public Canguro.Model.UnitSystem.Units Units
        {
            get { return units; }
            set { units = value; }
        }

        public UnitsAttribute(Canguro.Model.UnitSystem.Units units)
        {
            this.units = units;
        }

        public static UnitsAttribute Default = new UnitsAttribute(Canguro.Model.UnitSystem.Units.NoUnit);
    }

    class GridPositionComparer : System.Collections.IComparer {
        #region IComparer Members

        public int Compare(object x, object y) {
            GridPositionAttribute gpx = (GridPositionAttribute)((PropertyDescriptor)x).Attributes[typeof(GridPositionAttribute)];
            GridPositionAttribute gpy = (GridPositionAttribute)((PropertyDescriptor)y).Attributes[typeof(GridPositionAttribute)];
            return gpx.Position.CompareTo(gpy.Position);
        }

        #endregion
    }

    class GridPositionComparerGeneric : System.Collections.Generic.Comparer<PropertyDescriptor> {
        public override int Compare(PropertyDescriptor x, PropertyDescriptor y) {
            GridPositionAttribute gpx = (GridPositionAttribute)x.Attributes[typeof(GridPositionAttribute)];
            GridPositionAttribute gpy = (GridPositionAttribute)y.Attributes[typeof(GridPositionAttribute)];
            return gpx.Position.CompareTo(gpy.Position);
        }
    }
}
