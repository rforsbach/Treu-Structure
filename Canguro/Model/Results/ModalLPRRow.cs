using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results
{
    [Serializable]
    public class ModalLPRRow
    {
        private string item;
        private string itemType;
        private float staticVal;
        private float dynamicVal;

        public ModalLPRRow(string item, string itemType, float staticVal, float dynamicVal)
        {
            this.item = item;
            this.itemType = itemType;
            this.staticVal = staticVal;
            this.dynamicVal = dynamicVal;
        }

        public string Item
        {
            get { return item; }
            set { item = value; }
        }

        public string ItemType
        {
            get { return itemType; }
            set { itemType = value; }
        }

        public float StaticVal
        {
            get { return staticVal; }
            set { staticVal = value; }
        }

        public float DynamicVal
        {
            get { return dynamicVal; }
            set { dynamicVal = value; }
        }
    }
}