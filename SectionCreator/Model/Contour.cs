using System;
using System.Collections.Generic;
using System.Text;
using Canguro.Analysis.Sections;

namespace Canguro.SectionCreator
{
    [Serializable]
    public class Contour : ISelectable
    {
        protected string name;
        protected Material material;
        protected System.Drawing.Color color = System.Drawing.Color.DarkGray;

        private ManagedList<Point> points = new ManagedList<Point>();
        [NonSerialized]
        bool isSelected = false;

        public Contour(string name)
        {
            this.name = name;
            material = (Model.Instance.Contours.Count == 0) ? Material.Steel : Material.None;
        }

        public Contour() : this("Contour") 
        { }

        [System.ComponentModel.Browsable(false)]
        public ManagedList<Point> Points
        {
            get
            {
                return points;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                Model.Instance.Undo.Change(this, name, GetType().GetProperty("Name"));
                name = value;
            }
        }


        #region ISelectable Members

        [System.ComponentModel.Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
            }
        }

        #endregion

        public Material Material
        {
            get
            {
                return material;
            }
            set
            {
                Model.Instance.Undo.Change(this, material, GetType().GetProperty("Material"));
                material = value;
            }
        }

        public System.Drawing.Color Color
        {
            get
            {
                return color;
            }
            set
            {
                Model.Instance.Undo.Change(this, color, GetType().GetProperty("Color"));
                color = value;
            }
        }
    }
}
