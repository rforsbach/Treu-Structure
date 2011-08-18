using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Material
{
    /// <summary>
    /// Clase que contiene toda la información sobre los materiales, incluyendo
    /// propiedades del tipo de material y de diseño.
    /// </summary>
    [Serializable]
    public class Material : Utility.GlobalizedObject, INamed
    {
        private string name = "";
        private MaterialDesignProps designProperties;
        private MaterialTypeProps typeProperties;
        private float density;
        public readonly bool IsLocked;

        /// <summary>
        /// Constructora que asigna los valores iniciales al material.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isLocked"></param>
        /// <param name="designProps"></param>
        /// <param name="typeProps"></param>
        /// <param name="density"></param>
        public Material(string name, bool isLocked, MaterialDesignProps designProps, MaterialTypeProps typeProps, float density)
        {
            Name = name;
            designProperties = (MaterialDesignProps)designProps.Clone();
            typeProperties = (MaterialTypeProps)typeProps.Clone();
            this.density = density;
            IsLocked = isLocked;
        }
        /// <summary>
        /// Constructora que crea el material en base a un prototipo.
        /// </summary>
        /// <param name="prototype"></param>
        public Material(Material prototype) : this(prototype.Name, false, (MaterialDesignProps)prototype.DesignProperties.Clone(), (MaterialTypeProps)prototype.TypeProperties.Clone(), prototype.density) { }
        /// <summary>
        /// Constructora que crea un material basada en el DefaultMaterial definido en MaterialManager.
        /// </summary>
        public Material() : this(MaterialManager.Instance.DefaultSteel) {}
    
        /// <summary>
        /// Propiedades de diseño del material.
        /// Estas propiedades varían dependiendo de si el material es acero, concreto, aluminio, etc.
        /// Para asignar se copian las propiedades, de forma que dos materiales no tengan una referencia al mismo 
        /// objeto de propiedades.
        /// </summary>
        public MaterialDesignProps DesignProperties
        {
            get
            {
                return (MaterialDesignProps)designProperties.Clone();
            }
            set
            {
                if (!IsLocked)
                {
                    Model.Instance.Undo.Change(this, designProperties, GetType().GetProperty("DesignProperties"));
                    designProperties = (MaterialDesignProps)value.Clone();
                }
            }
        }

        /// <summary>
        /// Nombre del material.
        /// Se asegura que el nombre no esté repetido.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!IsLocked)
                {
                    value = value.Trim().Replace("\"", "''");
                    value = (value.Length > 0) ? value : Culture.Get("Material");
                    string aux = value;
                    int i = 0;
                    Catalog<Material> cat = MaterialManager.Instance.Materials;
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
        }

        /// <summary>
        /// Propiedades de tipo (Isotrópico, Ortotrópico, Uniaxial, Anisotrópico)
        /// Las propiedades se copian para evitar dos referencias al mismo objeto de propiedades.
        /// </summary>
        public MaterialTypeProps TypeProperties
        {
            get
            {
                return typeProperties;
            }
            set
            {
                if (!IsLocked)
                {
                    Model.Instance.Undo.Change(this, typeProperties, GetType().GetProperty("TypeProperties"));
                    typeProperties = (MaterialTypeProps)value.Clone();
                }
            }
        }

        /// <summary>
        /// Densidad (masa por unidad de volumen) del material
        /// </summary>
        [ModelAttributes.Units(Canguro.Model.UnitSystem.Units.Density)]
        [System.ComponentModel.Description("Density: Mass per unit volume")]
        public float Density
        {
            get
            {
                return Model.Instance.UnitSystem.FromInternational(density, Canguro.Model.UnitSystem.Units.Density);
            }
            set
            {
                if (value > 0 && !IsLocked)
                {
                    Model.Instance.Undo.Change(this, Density, GetType().GetProperty("Density"));
                    density = Model.Instance.UnitSystem.ToInternational(value, Canguro.Model.UnitSystem.Units.Density);
                }
            }
        }

        /// <summary>
        /// Calcula el peso unitario en función de la densidad.
        /// </summary>
        [System.ComponentModel.Description("Weight per unit volume (Density * Gravity)")]
        public float UnitWeight
        {
            get
            {
                return Density * UnitSystem.UnitSystemsManager.Instance.CurrentSystem.Gravity;
            }
            set
            {
                Density = value / UnitSystem.UnitSystemsManager.Instance.CurrentSystem.Gravity;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
