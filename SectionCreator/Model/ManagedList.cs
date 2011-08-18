using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.SectionCreator
{
    [Serializable]
    public class ManagedList<Tvalue> : List<Tvalue>
    {
        private int maxItems = 0;
        public ManagedList(IEnumerable<Tvalue> collection)
            : base(collection)
        {
        }

        public ManagedList()
        {
        }

        public ManagedList(int maxItems)
        {
            this.maxItems = maxItems;
        }

        public new void Add(Tvalue value)
        {
            if (maxItems > 0 && Count >= maxItems)
                throw new InvalidOperationException("Cannot add more than " + maxItems + " items to the list");

            Canguro.Model.ListChangedEventArgs<Tvalue> args = new Canguro.Model.ListChangedEventArgs<Tvalue>(value);
            if (ElementAddedHandler != null)
                ElementAddedHandler(this, args);
            if (!args.Cancel)
            {
                Model.Instance.Undo.Add(Count, value, this);
                base.Add(value);
            }
        }

        public new bool Remove(Tvalue value)
        {
            Canguro.Model.ListChangedEventArgs<Tvalue> args = new Canguro.Model.ListChangedEventArgs<Tvalue>(value);
            if (ElementRemovedHandler != null)
                ElementRemovedHandler(this, args);

            if (!args.Cancel)
            {
                Model.Instance.Undo.Remove(IndexOf(value), value, this);
                return base.Remove(value);
            }
            else
                return false;
        }

        public new void RemoveAt(int index)
        {
            if (base[index] == null)
                base.RemoveAt(index);
            else
                Remove(base[index]);
        }

        public new int RemoveAll(Predicate<Tvalue> match)
        {
            throw new InvalidOperationException(Culture.Get("EM0001"));
        }

        public new void InsertRange(int index, IEnumerable<Tvalue> collection)
        {
            throw new InvalidOperationException(Culture.Get("EM0001"));
        }

        public new void Insert(int index, Tvalue item)
        {
            if (maxItems > 0 && Count >= maxItems)
                throw new InvalidOperationException("Cannot add more than " + maxItems + " items to the list");

            Canguro.Model.ListChangedEventArgs<Tvalue> args = new Canguro.Model.ListChangedEventArgs<Tvalue>(item);
            if (ElementAddedHandler != null)
                ElementAddedHandler(this, args);
            if (!args.Cancel)
            {
                Model.Instance.Undo.Add(index, item, this);
                base.Insert(index, item);
            }
        }

        public new void Clear()
        {
            for (int i = Count-1; i >= 0; i--)
                RemoveAt(i);
        }

        public new void AddRange(IEnumerable<Tvalue> collection)
        {
            foreach (Tvalue val in collection)
                Add(val);
        }

        public new void RemoveRange(int index, int count)
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
        public new Tvalue this[int index]
        {
            get
            {
                return base[index];
            }
            protected set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Propiedad de sólo lectura que regresa el Tvalue con id=index
        /// </summary>
        /// <param name="index">ID of the Tvalue to retrieve</param>
        /// <returns>The Tvalue with ID=index</returns>
        public Tvalue this[uint index]
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
        public delegate void ListChangedEventHandler(object sender, Canguro.Model.ListChangedEventArgs<Tvalue> args);

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
    }
}
