using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    [Serializable]
    public class ManagedList<Tvalue> : IList<Tvalue>, System.Collections.IList
    {
        private int maxItems = 0;
        protected List<Tvalue> list;

        public ManagedList(IEnumerable<Tvalue> collection)
        {
            list = new List<Tvalue>(collection);
        }

        public ManagedList()
        {
            list = new List<Tvalue>();
        }

        public ManagedList(int maxItems) : this()
        {
            this.maxItems = maxItems;
        }

        public virtual void Add(Tvalue value)
        {
            if (maxItems > 0 && Count >= maxItems)
                throw new InvalidOperationException("Cannot add more than " + maxItems + " items to the list");

            ListChangedEventArgs<Tvalue> args = new ListChangedEventArgs<Tvalue>(value);
            if (ElementAddedHandler != null)
                ElementAddedHandler(this, args);
            if (!args.Cancel)
            {
                Model.Instance.Undo.Add(value, this);
                list.Add(value);
            }
        }

        public virtual bool Remove(Tvalue value)
        {
            ListChangedEventArgs<Tvalue> args = new ListChangedEventArgs<Tvalue>(value);
            if (ElementRemovedHandler != null)
                ElementRemovedHandler(this, args);

            if (!args.Cancel)
            {
                Model.Instance.Undo.Remove(value, this);
                return list.Remove(value);
            }
            else
                return false;
        }

        public virtual void RemoveAt(int index)
        {
            if (list[index] == null)
                list.RemoveAt(index);
            else
                Remove(list[index]);
        }

        public virtual int RemoveAll(Predicate<Tvalue> match)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        public virtual void InsertRange(int index, IEnumerable<Tvalue> collection)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        public virtual void Insert(int index, Tvalue item)
        {
            throw new InvalidCallException(Culture.Get("EM0001"));
        }

        public virtual void Clear()
        {
            for (int i = Count-1; i >= 0; i--)
                RemoveAt(i);
        }

        public virtual void AddRange(IEnumerable<Tvalue> collection)
        {
            foreach (Tvalue val in collection)
                Add(val);
        }

        public virtual void RemoveRange(int index, int count)
        {
            for (int i = count-1; i >= 0; i--)
                RemoveAt(index + i);
        }

        public int CountNotNull()
        {
            int count = 0;
            foreach (object item in this)
                if (item != null)
                    count++;
            return count;
        }

        /// <summary>
        /// Gets the Tvalue with id=index
        /// </summary>
        /// <param name="index">ID of the Tvalue to retrieve</param>
        /// <returns>The Tvalue with ID=index, null if index is invalid</returns>
        public virtual Tvalue this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        /// <summary>
        /// Propiedad de sólo lectura que regresa el Tvalue con id=index
        /// </summary>
        /// <param name="index">ID of the Tvalue to retrieve</param>
        /// <returns>The Tvalue with ID=index</returns>
        public virtual Tvalue this[uint index]
        {
            get
            {
                return this[(int)index];
            }
            protected set
            {
                this[(int)index] = value;
            }
        }

        #region Events
        public delegate void ListChangedEventHandler(object sender, ListChangedEventArgs<Tvalue> args);

        /// <summary>
        /// Hack from http://www.lhotka.net/WeBlog/PermaLink.aspx?guid=c0130b29-fd43-4fb7-8a49-963d0885c917
        /// to avoid serializing clients.
        /// </summary>
        [NonSerialized]
        private ListChangedEventHandler ElementRemovedHandler;
        public event ListChangedEventHandler ElementRemoved
        {
            add { ElementRemovedHandler = (ListChangedEventHandler)Delegate.Combine(ElementRemovedHandler, value); }
            remove { ElementRemovedHandler = (ListChangedEventHandler)Delegate.Remove(ElementRemovedHandler, value); }
        }

        /// <summary>
        /// Hack from http://www.lhotka.net/WeBlog/PermaLink.aspx?guid=c0130b29-fd43-4fb7-8a49-963d0885c917
        /// to avoid serializing clients.
        /// </summary>
        [NonSerialized]
        private ListChangedEventHandler ElementAddedHandler;
        public event ListChangedEventHandler ElementAdded
        {
            add { ElementAddedHandler = (ListChangedEventHandler)Delegate.Combine(ElementAddedHandler, value); }
            remove { ElementAddedHandler = (ListChangedEventHandler)Delegate.Remove(ElementAddedHandler, value); }
        }
        #endregion

        #region IList<Tvalue> Members

        public virtual int IndexOf(Tvalue item)
        {
            return list.IndexOf(item);
        }

        #endregion

        #region ICollection<Tvalue> Members

        public virtual bool Contains(Tvalue item)
        {
            return list.Contains(item);
        }

        public void CopyTo(Tvalue[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public virtual int Count
        {
            get { return list.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable<Tvalue> Members

        public IEnumerator<Tvalue> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IndexOf(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Insert(int index, object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsFixedSize
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void Remove(object value)
        {
            Remove((Tvalue)value);
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (Tvalue)value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsSynchronized
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
