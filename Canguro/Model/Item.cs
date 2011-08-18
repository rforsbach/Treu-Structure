using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    /// <summary>
    /// Clase que abstrae el concepto de objetos físicos, tales como Joints,
    /// elementos estructurales y grupos de objetos (Layers)
    /// </summary>
    [Serializable]
    public abstract class Item : Canguro.Utility.GlobalizedObject
    {
        /// <summary>
        /// Item unique identifier
        /// </summary>
        protected uint id;
        /// <summary>
        /// true if the item is selected
        /// </summary>
        private bool isSelected;
        /// <summary>
        /// Layer object containing the item
        /// </summary>
        private Layer layer;
        /// <summary>
        /// true if the item is visible
        /// </summary>
        private bool isVisible = true;

        /// <summary>
        /// Constructora que asigna valores por defecto a los campos:
        /// </summary>
        public Item()
        {
            id = 0;
            layer = Model.Instance.ActiveLayer;
            isVisible = true;
            isSelected = false;
        }

        /// <summary>
        /// Constructora que asigna valores por defecto a los campos:
        /// </summary>
        /// <param name="src">The item to copy from</param>
        internal Item(Item src)
        {
            id = 0;
            layer = src.layer;
            isVisible = src.isVisible;
            isSelected = src.isSelected;
        }

        /// <summary>
        /// Cada subclase debe sobreescribir esta propiedad para revisar que el valor que se asigne al id sea válido.
        /// </summary>
        [System.ComponentModel.ReadOnly(true)]
        public abstract uint Id
        {
            get;
            set;
        }

        /// <summary>
        /// Regresa (asigna) el valor de selección.
        /// Esta propiedad no es suceptible a Undo ni a cambios de unidades.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value && isVisible;
            }
        }

        /// <summary>
        /// La capa a la que pertenece el objeto.
        /// Cada Item pertenece a una sola capa.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Layer Layer
        {
            get
            {
                return layer;
            }
            set
            {
                if (Model.Instance.Layers.Contains(value))
                {
                    Model.Instance.Undo.Change(this, layer, GetType().GetProperty("Layer"));
                    layer = value;
                }
                else
                    throw new LayerNotFoundException();
            }
        }

        /// <summary>
        /// Propiedad que permite ocultar los objetos.
        /// Sólo afecta a la visualización, por lo que no es suceptible a Undo.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
            }
        }
    }
}
