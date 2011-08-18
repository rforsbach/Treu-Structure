using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View.Renderer
{
    public abstract class ModelRenderer
    {
        private AreaRenderer areaRenderer = null;
        private JointRenderer jointRenderer = null;
        private LineRenderer lineRenderer = null;
        private LoadRenderer loadRenderer = null;
        private ElementForcesRenderer forcesRenderer = null;
        private RenderOptions renderOptions;
        private Dictionary<string, ItemRenderer> renderers;
        private GadgetRenderer gadgetRenderer;
        protected List<Canguro.Model.Item> itemsInView = new List<Canguro.Model.Item>();

        public ModelRenderer()
        {
            renderers = new Dictionary<string, ItemRenderer>();
            
            // Wireframe
            renderers.Add("wj", new WireframeJointRenderer());
            renderers.Add("wl", new WireframeLineRenderer());
            renderers.Add("wa", new WireframeAreaRenderer());
            
            // Shaded
            renderers.Add("sl", new ShadedLineRenderer((WireframeLineRenderer)renderers["wl"]));
            renderers.Add("sa", new ShadedAreaRenderer());

            // Deformed
            renderers.Add("dwl", new DeformedLineWireframeRenderer());
            renderers.Add("dsl", new DeformedLineShadedRenderer((DeformedLineWireframeRenderer)renderers["dwl"]));
            renderers.Add("dwa", new DeformedAreaWireframeRenderer());
            renderers.Add("dsa", new DeformedAreaShadedRenderer((DeformedAreaWireframeRenderer)renderers["dwa"]));

            // Stressed
            renderers.Add("fwl", new StressWireframeLineRenderer());
            renderers.Add("fl", new StressLineRenderer((DeformedLineWireframeRenderer)renderers["dwl"], (StressWireframeLineRenderer)renderers["fwl"]));
            renderers.Add("fa", new StressAreaRenderer());

            // Loads and Element Forces
            renderers.Add("lr", new SimpleLoadRenderer());
            renderers.Add("ef", new ElementForcesRenderer());

            renderers.Add("gr", new GadgetRenderer());

            renderOptions = new RenderOptions(this);
        }
    
        public JointRenderer JointRenderer
        {
            get
            {
                return jointRenderer;
            }
            set
            {
                if (jointRenderer != value)
                {
                    // Delete last Renderer
                    if (jointRenderer != null)
                        jointRenderer.DisposeResources();

                    // Set up new renderer
                    jointRenderer = value;
                    if (jointRenderer != null)
                        jointRenderer.UpdateResources();
                }
            }
        }

        public LineRenderer LineRenderer
        {
            get
            {
                return lineRenderer;
            }
            set
            {
                if (lineRenderer != value)
                {
                    // Delete last Renderer
                    if (lineRenderer != null)
                        lineRenderer.DisposeResources();

                    // Set up new renderer
                    lineRenderer = value;
                    if (lineRenderer != null)
                        lineRenderer.UpdateResources();
                }
            }
        }

        public AreaRenderer AreaRenderer
        {
            get
            {
                return areaRenderer;
            }
            set
            {
                if (areaRenderer != value)
                {
                    // Delete last Renderer
                    if (areaRenderer != null)
                        areaRenderer.DisposeResources();

                    // Set up new renderer
                    areaRenderer = value;
                    if (areaRenderer != null)
                        areaRenderer.UpdateResources();
                }
            }
        }

        public LoadRenderer LoadRenderer
        {
            get
            {
                return loadRenderer;
            }
            set
            {
                if (loadRenderer != value)
                {
                    // Delete last Renderer
                    if (loadRenderer != null)
                        loadRenderer.DisposeResources();

                    // Set up new renderer
                    loadRenderer = value;
                    if (loadRenderer != null)
                        loadRenderer.UpdateResources();
                }
            }
        }

        public ElementForcesRenderer ForcesRenderer
        {
            get
            {
                return forcesRenderer;
            }
            set
            {
                if (forcesRenderer != value)
                {
                    // Delete last Renderer
                    if (forcesRenderer != null)
                        forcesRenderer.DisposeResources();

                    // Set up new renderer
                    forcesRenderer = value;
                    if (forcesRenderer != null)
                        forcesRenderer.UpdateResources();
                }
            }
        }

        public RenderOptions RenderOptions
        {
            get { return renderOptions; }
            set { renderOptions = value; }
        }

        public abstract void ReconfigureRenderers();

        public Dictionary<string, ItemRenderer> Renderers
        {
            get { return renderers; }
        }

        public void Reset(bool fullReset)
        {
            if (fullReset)
                renderOptions = new RenderOptions(this);
            else
            {
                if (renderOptions.InternalForcesShown != RenderOptions.InternalForces.None)
                    renderOptions.InternalForcesShown = RenderOptions.InternalForces.None;
                if (renderOptions.ShowAnimated)
                    renderOptions.ShowAnimated = false;
                if (renderOptions.ShowDeformed)
                    renderOptions.ShowDeformed = false;
                if (renderOptions.ShowDesigned)
                    renderOptions.ShowDesigned = false;
                if (renderOptions.ShowStressed)
                    renderOptions.ShowStressed = false;
            }
            
            ReconfigureRenderers();
        }

        protected void invalidateView()
        {
            GraphicViewManager.Instance.updateView(true);
        }

        public abstract void Render(Device device);
        public virtual void UpdateModel() { }
        public virtual void UpdateResources()
        {
            if (jointRenderer != null)
                jointRenderer.UpdateResources();
            if (lineRenderer != null)
                lineRenderer.UpdateResources();
            if (areaRenderer != null)
                areaRenderer.UpdateResources();
            if (loadRenderer != null)
                loadRenderer.UpdateResources();
            if (forcesRenderer != null)
                forcesRenderer.UpdateResources();
        }

        protected Model.Model model
        {
            get { return Model.Model.Instance; }
        }
        
        public bool HasResults
        {
            get { return model.HasResults; }
        }

        public GadgetRenderer GadgetRenderer
        {
            get { return gadgetRenderer; }
            set 
            {
                if (gadgetRenderer != value)
                {
                    // Delete last Renderer
                    if (gadgetRenderer != null)
                        gadgetRenderer.DisposeResources();

                    // Set up new renderer
                    gadgetRenderer = value;
                    if (gadgetRenderer != null)
                        gadgetRenderer.UpdateResources();
                }
            }
        }
    }
}
