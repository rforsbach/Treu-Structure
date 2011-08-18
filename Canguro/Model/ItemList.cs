using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    [Serializable]
    public class ItemList<Titem> : ManagedList<Titem> where Titem : Item
    {
        /// <summary>
        /// Constructora. Inserta un valor null para forzar que se use con base 1.
        /// </summary>
        public ItemList()
        {
            base.Add(null);
        }

        /// <summary>
        /// Gets the Item with id=index
        /// </summary>
        /// <param name="index">ID of the item to retrieve</param>
        /// <returns>The item with ID=index, null if index is invalid</returns>
        public override Titem this[int index] 
        {
            get
            {
                return (index < Count) ? base[index] : null;
            }
        }

        /// <summary>
        /// Propiedad de sólo lectura que regresa el Item con id=index
        /// </summary>
        /// <param name="index">ID of the item to retrieve</param>
        /// <returns>The item with ID=index</returns>
        public Titem this[uint index]
        {
            get
            {
                return this[(int)index];
            }
        }

        /// <summary>
        /// Agrega un elemento a la lista. Avisa a UndoManager para habilitar
        /// undo y para revisar que el modelo no esté bloqueado.
        /// Si el Item tiene Id asignado (Esto sucede en el caso de undo), se trata de
        /// poner en la lista en el lugar Id. Si este lugar está ocupado por otro Item,
        /// se lanza una InvalidIndexException.
        /// </summary>
        /// <param name="item">Item to add</param>
        public override void Add(Titem item)
        {
            //Model.Instance.Undo.Add(item, this);
            if (item != null)
            {
                int id = (int)item.Id;
                if (id == 0)
                {
                    base.Add(item);
                    item.Id = (uint)(Count - 1);
                }
                else if (id >= Count)
                {
                    for (int i = Count; i < id; i++)
                        base.Add(null);
                    base.Add(item);
                }
                else if (base[id] == null)
                    base[id] = item;
                else if (base[id] != item)
                    throw new InvalidIndexException(Culture.Get("EM0002"));
            }
            else
                base.Add(null);
        }
        
        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        /// <param name="collection"></param>
        public override void AddRange(IEnumerable<Titem> collection)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }


        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        public override void Clear()
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        /// <param name="index">NA</param>
        /// <param name="item">NA</param>
        public override void Insert(int index, Titem item)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        /// <param name="index">NA</param>
        /// <param name="collection">NA</param>
        public override void InsertRange(int index, IEnumerable<Titem> collection)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// Quita un elemento de la lista
        /// </summary>
        /// <param name="item">Item to remove</param>
        public override bool Remove(Titem item)
        {
            if (item == null && base[Count - 1] == null)
            {
                base.RemoveAt(Count - 1);
                return true;
            }
            
            int id = (int)item.Id;
            if ((id < Count) && (base[id] == item))
            {
                RemoveAt(id);
                return true;
            }

            return false;
            //return base.Remove(item);
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        /// <param name="match">NA</param>
        public override int RemoveAll(Predicate<Titem> match)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// Elimina de la lista el elemento con Id=index
        /// Avisa a Undo
        /// </summary>
        /// <param name="index">Index of item to remove</param>
        public override void RemoveAt(int index)
        {
            if (base[index] != null)
            {
                Model.Instance.Undo.Remove(base[index], this);
                base[index] = null;
            }
        }

        /// <summary>
        /// Elimina de la lista el elemento con Id=index
        /// Avisa a Undo
        /// </summary>
        /// <param name="index">Index of item to remove</param>
        public void RemoveAt(uint index)
        {
            RemoveAt((int)index);
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        /// <param name="index">NA</param>
        /// <param name="count">NA</param>
        public override void RemoveRange(int index, int count)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// Compacta la lista de modo que no haya valores null, dejando consistentes
        /// los valores de Id y de índice.
        /// Éste método no permite Undo
        /// </summary>
        public void Compact()
        {
            Titem item;
            int i, newi;
            for (i = 1, newi = 1; i < Count; i++)
            {
                if ((item = base[i]) != null)
                {
                    if (i != newi)
                    {
                        base[newi] = item;
                        base[i] = null;
                    }
                    item.Id = (uint)newi++;
                }
            }
            base.RemoveRange(newi, Count - newi);
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        public new void Reverse()
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        /// <param name="index">NA</param>
        /// <param name="count">NA</param>
        public new void Reverse(int index, int count)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        public new void Sort()
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        /// <param name="comparison">NA</param>
        public new void Sort(Comparison<Titem> comparison)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        /// <param name="comparer">NA</param>
        public new void Sort(IComparer<Titem> comparer)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        /// <summary>
        /// No se soporta para limitar el uso de listas de Item
        /// Se lanza una InvalidCallException.
        /// </summary>
        /// <param name="index">NA</param>
        /// <param name="count">NA</param>
        /// <param name="comparer">NA</param>
        public new void Sort(int index, int count, IComparer<Titem> comparer)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        public override bool Contains(Titem item)
        {
            if (item is INamed)
                return base.list.Exists(delegate(Titem obj) { return (item != null && obj != null && ((INamed)obj).Name.Equals(((INamed)item).Name)); });
            else
                return base.Contains(item);
        }
    }
}
