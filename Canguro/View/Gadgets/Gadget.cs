using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;

namespace Canguro.View.Gadgets
{
    public class Gadget
    {
        private GadgetType type;
        private Element item;

        public GadgetType Type
        {
            get { return type; }
            set { type = value; }
        }

        public Element Item
        {
            get { return item; }
            set { item = value; }
        }

        public Gadget(Element item, GadgetType gadgetType)
        {
            this.item = item;
            type = gadgetType;
        }
    }
}
