using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model
{
    public class ListChangedEventArgs <Tvalue>: EventArgs
    {
        private Tvalue changedObject;
        private bool cancel;

        public ListChangedEventArgs(Tvalue obj)
        {
            changedObject = obj;
            cancel = false;
        }

        public Tvalue ChangedObject
        {
            get { return changedObject; }
        }

        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
    }
}
