using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Canguro.Model
{
    /// <summary>
    /// La clase Layer agrupa Items en una relación 1 : N
    /// Layer es un Item, por lo que se forma un Composite
    /// </summary>
    [Serializable]
    public class Layer : Item, INamed
    {
        private string name;

        /// <summary>
        /// Constructora que asigna un nombre al Layer. 
        /// El sistema permite que existan dos Layers con el mismo nombre,
        /// aunque no se recomienda.
        /// </summary>
        /// <param name="name"></param>
        public Layer(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Genera una lista de solo lectura de Items.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public System.Collections.ObjectModel.ReadOnlyCollection<Canguro.Model.Item> Items
        {
            get
            {
                List<Item> list = new List<Item>();
                // TODO: Tomar en cuenta a PointLink
                System.Collections.IEnumerable[] arr = {Model.Instance.JointList,
                                                        Model.Instance.LineList,
                                                        Model.Instance.AreaList};
                foreach (System.Collections.IEnumerable il in arr)
                    foreach (Item i in il)
                        if (i != null && i.Layer != null && i.Layer.Id == Id)
                            list.Add(i);
                return list.AsReadOnly();
            }
        }

        /// <summary>
        /// Asigna / regresa el nombre de la Layer.
        /// Utiliza Undo, por lo que no se puede cambiar cuando el Model está bloqueado.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != null && !value.Equals(name))
                {
                    value = value.Trim().Replace("\"", "''");
                    value = (value.Length > 0) ? value : Culture.Get("Layer");
                    string aux = value;
                    bool ok = false;
                    int i = 0;
                    while (!ok)
                    {
                        ok = true;
                        foreach (Layer la in Model.Instance.Layers)
                            if (la != null && la.name.Equals(aux) && la.Id != Id)
                                ok = false;
                        if (!ok)
                            aux = value + "(" + ++i + ")";
                    }
                    if (name != null)
                        Model.Instance.Undo.Change(this, name, GetType().GetProperty("Name"));
                    name = aux;
                }
            }
        }

        /// <summary>
        /// Asigna / regresa el Id de la Layer.
        /// No utiliza Undo, pero revisa que el valor nuevo sea válido.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public override uint Id
        {
            get
            {
                return id;
            }
            set
            {
                ItemList<Layer> list = Model.Instance.Layers;
                if (id == 0 || list[(int)value] == this)
                    id = value;
                else
                    throw new InvalidIndexException();
            }
        }

        /// <summary>
        /// Selecciona todos los elementos del Layer.
        /// </summary>
        public void Select()
        {
            ReadOnlyCollection<Item> list = Items;
            foreach (Item i in list)
                i.IsSelected = true;

            // TODO: Falta disparar el evento Model.SelectionChanged
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
