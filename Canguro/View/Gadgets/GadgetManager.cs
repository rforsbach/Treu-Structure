using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;

namespace Canguro.View.Gadgets
{
    public class GadgetManager
    {
        private PointGadgetService pointGadgets;
        private LineGadgetService lineGadgets;
        private AreaGadgetService areaGadgets;
        private LinkedList<Gadget> gadgetList;

        private ResourceManager resourceManager;

        public GadgetManager(ResourceManager rm)
        {
            resourceManager = rm;
            pointGadgets = new PointGadgetService(this);
            lineGadgets = new LineGadgetService(this);
            areaGadgets = new AreaGadgetService(this);

            gadgetList = new LinkedList<Gadget>();
        }

        #region GadgetServices
        public PointGadgetService PointGadgets
        {
            get { return pointGadgets; }
            set { pointGadgets = value; }
        }

        public LineGadgetService LineGadgets
        {
            get { return lineGadgets; }
            set { lineGadgets = value; }
        }

        public AreaGadgetService AreaGadgets
        {
            get { return areaGadgets; }
            set { areaGadgets = value; }
        }
        #endregion

        #region Life Cycle Control
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            pointGadgets.ClearLocators();
            lineGadgets.ClearLocators();

            //areaGadgets.ClearLocators();
        }
        #endregion

        public LinkedList<Gadget> GadgetList
        {
            get { return gadgetList; }
            set { gadgetList = value; }
        }

        #region Buffers Control
        public ResourcePackage CaptureBuffer(ResourceStreamType stream, int minimumIndices, int minimumVertices)
        {
            return resourceManager.CaptureBuffer(stream, minimumIndices, minimumVertices, false);
        }

        public void ReleaseBuffer(int numVerticesDrawn, int numIndicesDrawn, ResourceStreamType stream)
        {
            resourceManager.ReleaseBuffer(numVerticesDrawn, numIndicesDrawn, stream);
        }        
        #endregion

        public void SetActiveStream(ResourceStreamType stream)
        {
            resourceManager.ActiveStream = stream;
        }
    }
}
