using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    [Serializable]
    public class ManagedDictionary< TKey, TValue > : Dictionary< TKey, TValue >
    {
        public ManagedDictionary()
        {
        }

        public ManagedDictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }                

        public ManagedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            foreach (TKey key in dictionary.Keys)
                Add(key, dictionary[key]);
        }

        public new void Add(TKey key, TValue value)
        {
            ListChangedEventArgs<TKey> args = new ListChangedEventArgs<TKey>(key);
            if (ElementAddedHandler != null)
                ElementAddedHandler(this, args);
            if (!args.Cancel)
            {
                Model.Instance.Undo.Add(key, value, this);
                base.Add(key, value);
            }
        }

        public new void Remove(TKey key)
        {
            if (ContainsKey(key))
            {
                ListChangedEventArgs<TKey> args = new ListChangedEventArgs<TKey>(key);
                if (ElementRemovedHandler != null)
                    ElementRemovedHandler(this, args);

                if (!args.Cancel)
                {
                    Model.Instance.Undo.Remove(key, this[key], this);
                    base.Remove(key);
                }
            }
        }

        public new TValue this [TKey key]
        {
            get { return (ContainsKey(key)) ? base[key] : default(TValue); }
            set
            {
                if (ContainsKey(key))
                    Remove(key);
                Add(key, value);
            }
        }

        public new void Clear()
        {
            LinkedList<TKey> keys = new LinkedList<TKey>();
            foreach (TKey key in Keys)
                keys.AddLast(key);
            foreach (TKey key in keys)
                Remove(key);
        }

        #region Events

        public delegate void ListChangedEventHandler(object sender, ListChangedEventArgs<TKey> args);

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
