using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Section
{
    [Serializable]
    public abstract class AreaSection : Utility.GlobalizedObject, Section
    {
        private string name = "";
        private string shape = "";
        private Material.Material material;

        [System.ComponentModel.Browsable(false)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                value = value.Trim().Replace("\"", "''");
                value = (value.Length > 0) ? value : Culture.Get("Section");
                string aux = value;
                int i = 0;
                Catalog<Section> cat = Model.Instance.Sections;
                if (cat != null && !(name.Equals(value) && cat[name] == this))
                {
                    while (cat[aux] != null)
                    {
                        aux = value + "(" + ++i + ")";
                    }
                    Model.Instance.Undo.Change(this, name, GetType().GetProperty("Name"));
                    if (cat[name] == this)
                        cat.MoveValue(name, aux);
                    name = aux;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        [System.ComponentModel.Browsable(false)]
        public Material.Material Material
        {
            get
            {
                return material;
            }
            set
            {
                if (value != null)
                {
                    Model.Instance.Undo.Change(this, material, GetType().GetProperty("Material"));
                    material = value;
                }
            }
        }

        public string Description
        {
            get 
            { 
                return string.Format("{0} ({1})", Name, material.Name);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public string Shape
        {
            get { return "Area"; }
        }

        public AreaSection(string name, string shape, Material.Material material)
        {
            Name = name;
            this.shape = shape;
            this.material = material;
        }
    }
}
