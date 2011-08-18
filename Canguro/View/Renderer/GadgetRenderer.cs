using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View.Renderer
{
    public class GadgetRenderer : ItemRenderer
    {
        public void Render(Device device, LinkedList<Gadgets.Gadget> gadgetList)
        {
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;

            LinkedListNode<Gadgets.Gadget> gadget = gadgetList.First;
            LinkedListNode<Gadgets.Gadget> nextGadget;

            device.RenderState.Lighting = false;

            while (gadget != null)
            {
                nextGadget = gadget.Next;

                if (gadget.Value.Type == Canguro.View.Gadgets.GadgetType.Restraint)
                {
                    Canguro.Model.Joint j = (Canguro.Model.Joint)gadget.Value.Item;

                    rc.GadgetManager.PointGadgets.DrawCanonicalConstraints(device, j.DoF, j.Position);

                    gadgetList.Remove(gadget);
                }
                else if (gadget.Value.Type == Canguro.View.Gadgets.GadgetType.Release)
                {
                    Canguro.Model.LineElement l = (Canguro.Model.LineElement)gadget.Value.Item;

                    rc.GadgetManager.LineGadgets.PrepareReleases(device, l);

                    gadgetList.Remove(gadget);
                }

                gadget = nextGadget;
            }

            gadgetList = gadgetList;

            rc.GadgetManager.LineGadgets.DrawReleases(device);

            device.Transform.World = Matrix.Identity;
        }
    }
}
