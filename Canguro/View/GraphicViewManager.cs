using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View
{
    /// <summary>
    /// GraphicViewManager Class. Render, update, recovery, and graphics related tasks
    /// </summary>
    sealed public class GraphicViewManager : ModelObserver
    {
        /// <summary>
        /// Enumeration telling how many views can be managed by the same device and window
        /// </summary>
        public enum ViewportsLayout : byte
        {
            /// <summary> Just manage one view </summary>
            OneView,
            /// <summary> Two views separated vertically </summary>
            TwoViewsVertical, 
            /// <summary> Two views separated horizontally </summary>
            TwoViewsHorizontal, 
            /// <summary> Three views: bigger view on left, and two small on right </summary>
            ThreeViewsBigLeft,
            /// <summary> Three views: bigger view on right, and two small on left </summary>
            ThreeViewsBigRight,
            /// <summary> Three views: bigger view on top, and two small on bottom </summary>
            ThreeViewsBigTop,
            /// <summary> Three views: bigger view on bottom, and two small on top </summary>
            ThreeViewsBigBottom,
            /// <summary> Divides the view in four equal spaces </summary>
            FourViews,
        }

        /// <summary>
        /// Delegate for responding to changes in active view
        /// </summary>
        /// <param name="sender"> The sending object </param>
        /// <param name="e"> Arguments of the event </param>
        public delegate void ActiveViewChangedEventHandler(object sender, ActiveViewChangedEventArgs e);
        
        /// <summary>
        /// Arguments for an active view change event class
        /// </summary>
        public class ActiveViewChangedEventArgs : EventArgs
        {
            /// <summary>
            /// A view object for storing which view has changed
            /// </summary>
            public readonly GraphicView NewView;

            /// <summary>
            /// Method for changing the active view
            /// </summary>
            /// <param name="newView"> The new active view </param>
            public ActiveViewChangedEventArgs(GraphicView newView)
            {
                NewView = newView;
            }
        }

        /// <summary>
        /// The notifying event
        /// </summary>
        public event ActiveViewChangedEventHandler ActiveViewChange;

        /// <summary>
        /// The resource cache instance. Contains any resource (vertex and index buffers, fonts, etc) used bye view operations 
        /// </summary>
        private ResourceManager resourceManager;

        /// <summary>
        /// Class constructor
        /// </summary>
        private GraphicViewManager()
        {
            // By default, manager is not subscribed to Model.ModelChanged event
            enabled = false;

            this.activeView = null;                     // No default active view
            this.device = null;                         // No active device
            this.layout = ViewportsLayout.OneView;      // Layout of one single viewport
            this.views = null;                          // No views in the current context
            this.resourceManager = null;                  // No resource cache
        }

        /// <summary>
        /// Configures class as a singleton
        /// </summary>
        public static readonly GraphicViewManager Instance = new GraphicViewManager();

        /// <summary> Is manager subscribed to Model.ModelChanged event? </summary>
        private bool enabled;

        /// <summary> Device attached to the manager </summary>
        private Device device;

        /// <summary> Array of views managed by the GVM </summary>
        private Canguro.View.GraphicView[] views;

        /// <summary> Store device presentation parameters </summary>
        private PresentParameters presentParams = null;

        public PresentParameters PresentParams
        {
            get { return (PresentParameters)presentParams.Clone(); }
        }
        
        /// <summary>
        /// Modifies the subscription to the Model.ModelChanged event
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (value ^ enabled)
                {
                    if (value)
                    {
                        Model.Model.Instance.ModelChanged += updateModel;
                        Model.Model.Instance.SelectionChanged += new Canguro.Model.Model.SelectionChangedEventHandler(updateSelection);
                        Model.Model.Instance.ResultsArrived += new EventHandler(updateResults);
                        Model.Model.Instance.ModelReset += new EventHandler(updateModelReset);
                    }
                    else
                    {
                        Model.Model.Instance.ModelChanged -= updateModel;
                        Model.Model.Instance.SelectionChanged -= new Canguro.Model.Model.SelectionChangedEventHandler(updateSelection);
                        Model.Model.Instance.ResultsArrived -= new EventHandler(updateResults);
                        Model.Model.Instance.ModelReset -= new EventHandler(updateModelReset);
                    }
                    enabled = value;
                }
            }
        }

        internal Device Device
        {
            get { return device; }
        }

        // Este será para cuando el SmallPanel esté integrado con DirectX        
        //public event EnterDataEventHandler EnterData;

        /// <summary> Property: Gets the active view </summary>
        public GraphicView ActiveView
        {
            get
            {
                return activeView;
            }
        }

        private Surface presentSurface;

        public Surface PresentRender
        {
            get
            {
                presentSurface = device.GetBackBuffer(0, 0, BackBufferType.Mono);
                return presentSurface;
            }
        }

        /// <summary> Stores the active view </summary>
        private GraphicView activeView;

        /// <summary> Property: Gets/Sets a layout for the scene. Canm be any of ViewportsLayout </summary>
        public ViewportsLayout Layout
        {
            get
            {
                return layout;
            }
            set
            {
                // Set new layout
                layout = value;
                // Resize viewports according to the new layout
                resizeViewports(wnd);
            }
        }

        /// <summary> Store the current layout </summary>
        private ViewportsLayout layout;

        /// <summary> The full viewport, no layout is selected </summary>
        private Microsoft.DirectX.Direct3D.Viewport fullViewport;

        public Microsoft.DirectX.Direct3D.Viewport FullViewport
        {
            get { return fullViewport; }
        }

        /// <summary>
        /// Prints a message. NOT IMPLEMENTED YET
        /// </summary>
        /// <param name="message"> Message to show </param>
        public void Print(string message)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Prints a message. NOT IMPLEMENTED YET
        /// </summary>
        /// <param name="title"> Title </param>
        /// <param name="message"> Message content</param>
        /// <param name="expires"> Does it expires? </param>
        public void Print(string title, string message, bool expires)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Clear printed messages. NOT IMPLEMENTED YET
        /// </summary>
        public void ClearMessages()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Reset every view contained in GVM
        /// </summary>
        public void Reset(bool fullReset)
        {
            foreach (GraphicView gv in views)
                gv.ModelRenderer.Reset(fullReset);
        }
        
        /// <summary>
        /// Resets the GVM
        /// </summary>
        /// <param name="device"> Attached device </param>
        /// <param name="createViews"> Boolean telling if views need to be created </param>
        private void reset(Device device, bool createViews)
        {
            // Create Views
            if (createViews)
            {
                views = new GraphicView[4];
                for (int i = 0; i < 4; i++)
                    views[i] = new GraphicView(i);
            }

            // Set device
            //for (int i = 0; i < 4; i++)
            //    views[i].Device = device;

            // Current viewport
            this.activeView = views[0];

            // Resize Viewports
            resizeViewports(wnd);
        }

        /// <summary> Timeout for message displaying (if message expires) </summary>
        private const uint messageTimeout = 5000;
        /// <summary> The window used for displaying tasks (device window) </summary>
        private System.Windows.Forms.Control wnd = null;
        /// <summary> Main Application Form</summary>
        private MainFrm form = null;
        /// <summary> Integer representing the current video adapter </summary>
        private int currentDXAdapter = -1;

        /// <summary> Are graphics initialized? </summary>
        private bool initializedGraphics = false;

        internal void RestartGraphics()
        {
            device.Dispose();
            resourceManager.device_Disposing(this, EventArgs.Empty);
            presenter = null;

            try
            {
                device.DeviceLost -= new EventHandler(device_DeviceLost);
                device.DeviceReset -= new EventHandler(device_DeviceReset);
                device.Disposing -= new EventHandler(device_Disposing);
                device.DeviceResizing -= new System.ComponentModel.CancelEventHandler(device_DeviceResizing);
            }
            catch { }

            try
            {
                wnd.Paint -= new System.Windows.Forms.PaintEventHandler(wnd_Paint);
                wnd.Resize -= new EventHandler(wnd_Resize);
                form.Move -= new EventHandler(form_Move);
            }
            catch { }

            try
            {
                Controller.Controller.Instance.EndViewCommand -= new Canguro.Controller.Controller.EndViewCommandEventHandler(controller_EndViewCommand);
                //Controller.Controller.Instance.EndModelCommand += new EventHandler(controller_EndModelViewCommand);
                Controller.Controller.Instance.Idle -= new EventHandler(controller_Idle);
            }
            catch { }

            initializedGraphics = false;
            InitializeGraphics(wnd, form);
        }

        /// <summary>
        /// Inicializa el Device para DirectX
        /// </summary>
        internal void InitializeGraphics(System.Windows.Forms.Control wnd, MainFrm form)
        {
            // Allow only one time initialization
            if (initializedGraphics) return;

            this.device = null;                     // There's still no device
            this.layout = ViewportsLayout.OneView;  // Onew view as predefined layout
            this.views = new GraphicView[4];        // Four possible views
            this.activeView = null;                 // There's no active view
            this.resourceManager = null;              // There's no resource manager
            Enabled = true;                         // Enabel manager subscription to Model.ModelChanged event

            // Store DirectX Device container window for further reference
            this.wnd = wnd;
            this.form = form;

            // Encontrar adaptador y sus capacidades
            int adapter;
            CreateFlags createFlags;
            getDeviceAdapter(wnd, out adapter, out createFlags, out presentParams);

            // Si se ativa la siguiente opción es necesario manejar todos los Resets
            // Device.IsUsingEventHandlers = false;

            // Create ResourceManager
            resourceManager = new ResourceManager();

            // Create presenter object
            presenter = new Presenter(this, form);

            // Crear Device de DirectX (Si truena, no se puede utilizar la aplicación)
            device = createDevice(adapter, wnd, createFlags, presentParams);
            reset(device, true);

            // Subscribe to events
            wnd.Paint += new System.Windows.Forms.PaintEventHandler(wnd_Paint);
            wnd.Resize += new EventHandler(wnd_Resize);
            form.Move += new EventHandler(form_Move);
            Controller.Controller.Instance.EndViewCommand += new Canguro.Controller.Controller.EndViewCommandEventHandler(controller_EndViewCommand);
            //Controller.Controller.Instance.EndModelCommand += new EventHandler(controller_EndModelViewCommand);
            Controller.Controller.Instance.Idle += new EventHandler(controller_Idle);

            initializedGraphics = true;
        }

        long lastTicks = 0;
        /// <summary>
        /// Idle event handler. The strategy to support animation based on this event
        /// was taken from http://blogs.msdn.com/tmiller/archive/2005/05/05/415008.aspx
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The EventArgs</param>
        void controller_Idle(object sender, EventArgs e)
        {
            while (Utility.NativeHelperMethods.AppStillIdle)
            {
                // Perform animation step if animation is active in the active view
                if (activeView.ModelRenderer.RenderOptions.ShowAnimated)
                {
                    if (!form.ContainsFocus)
                    {
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }

                    // Check if everything is ok
                    if ((!enabled) || (device == null) || presenter.RecoverFromDeviceLostState(device)) return;

                    // Update view
                    float progress = activeView.ModelRenderer.RenderOptions.AnimationProgress;
                    activeView.ModelRenderer.RenderOptions.AnimationProgress = progress + 0.03125f;

                    // Get cachedStep
                    int step = (int)(activeView.ModelRenderer.RenderOptions.AnimationProgress * (2f * (ResourceManager.AnimationFrames - 1f)));
                    int cachedStep = step - 8;
                    if (step < 8)
                        cachedStep = 8 - step;
                    else if (step > 24)
                        cachedStep = 40 - step;

                    // Get cached surface
                    Surface surface = resourceManager.AnimationCache[cachedStep];

                    if (surface == null || surface.Disposed) // If frame has not been captured, capture it
                    {
                        // Draw frame
                        activeView.Update(device);
                        presenter.PresentImmediately(activeView);

                        // Capture frame
                        Viewport vp = activeView.Viewport;
                        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(vp.X, vp.Y, vp.Width, vp.Height);
                        lock (device)
                        {
                            //resourceManager.AnimationCache[cachedStep] = surface = device.CreateRenderTarget(vp.Width, vp.Height, device.GetBackBuffer(0, 0, BackBufferType.Mono).Description.Format, MultiSampleType.None, 0, false);
                            resourceManager.AnimationCache[cachedStep] = surface = device.CreateOffscreenPlainSurface(device.CreationParameters.FocusWindow.Width, device.CreationParameters.FocusWindow.Height, device.DisplayMode.Format, Pool.SystemMemory);
                            SurfaceLoader.FromSurface(surface, rect, device.GetBackBuffer(0, 0, BackBufferType.Mono), rect, Filter.None, 0);
                        }
                    }
                    else   // If frame had been captured, paint it
                    {
                        long ticks = DateTime.Now.Ticks;
                        long delta = Math.Abs(ticks - lastTicks);
                        lastTicks = ticks;
                        if (delta < 1250000)
                            System.Threading.Thread.Sleep((int)(1250000 - delta) / 10000);

                        presenter.AnimationPaintImmediately(this, activeView, cachedStep);
                    }
                }
                else
                {
                    AnimationStop();
                    return;
                }
            }
        }

        internal void AnimationStart()
        {
            Controller.Controller.Instance.IdleStart(Canguro.Controller.Controller.IdleReason.Animating);
        }

        internal void AnimationStop()
        {
            Controller.Controller.Instance.IdleStop(Canguro.Controller.Controller.IdleReason.Animating);
            resourceManager.ResetAnimationCache();
        }

        /// <summary> Get current presentation parameters </summary>
        /// <param name="depthStencilFormat"> Argument telling which depth stencil format to use </param>
        /// <returns> The selected presentation parameters </returns>
        private PresentParameters getDXPresentParams(DepthFormat depthStencilFormat)
        {
            // Definir parámetros del Device
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;

            // Se puso por la pantalla verde/magenta
            //presentParams.PresentFlag = PresentFlag.LockableBackBuffer;
            presentParams.SwapEffect = SwapEffect.Copy;
            
            //presentParams.ForceNoMultiThreadedFlag = true;
            presentParams.AutoDepthStencilFormat = depthStencilFormat;
            presentParams.EnableAutoDepthStencil = true;
            //presentParams.BackBufferCount = 1;
            //presentParams.PresentationInterval = PresentInterval.One;
            //presentParams.PresentFlag = PresentFlag.LockableBackBuffer;

            return presentParams;
        }

        /// <summary> Creates a Direct3D device </summary>
        /// <param name="adapter"> The video adapter to put in the device </param>
        /// <param name="wnd"> Window to attach to the device </param>
        /// <param name="createFlags"> Creation flags </param>
        /// <param name="presentParams"> Presentation parameters </param>
        /// <returns></returns>
        private Device createDevice(int adapter, System.Windows.Forms.Control wnd,
            CreateFlags createFlags, PresentParameters presentParams)
        {
            // If there is a handle to a device, check if it is disposed, if not, then dispose it
            if (device != null && !device.Disposed)
            {
                device.Dispose();
            }

            // Create the device
            device = new Device(adapter, DeviceType.Hardware, wnd, createFlags, presentParams);

            // Call reset device event
            device_DeviceReset(device, EventArgs.Empty);

            // Store presentation parameters
            this.presentParams = presentParams;

            // Store current adapter
            currentDXAdapter = adapter;

            // Set the device for the resource cache
            resourceManager.SetDevice(device);

            // Subscribe to Device events
            device.DeviceLost += new EventHandler(device_DeviceLost);
            device.DeviceReset += new EventHandler(device_DeviceReset);
            device.Disposing += new EventHandler(device_Disposing);
            device.DeviceResizing += new System.ComponentModel.CancelEventHandler(device_DeviceResizing);

            return device;
        }

        #region Device Events
        /// <summary>
        /// This callback intends to check for the appropriate adapter to be used by DirectX
        /// depending on the one who displays most of the viewing area
        /// </summary>
        void form_Move(object sender, EventArgs e)
        {
            // Check if behaviour is allowed
            if (!Properties.Settings.Default.CanAutoChangeAdapter) return;

            int adapter;
            CreateFlags createFlags;

            getDeviceAdapter(wnd, out adapter, out createFlags, out presentParams);
            if (adapter != currentDXAdapter)
            {
                // Reconstruir el device con el nuevo adaptador. Primero tirar el device anterior
                Device deviceTmp = device;
                device = null;
                deviceTmp.Dispose();
                deviceTmp = null;

                // Si se ativa la siguiente opción es necesario manejar todos los Resets
                // Device.IsUsingEventHandlers = false;

                // Crear Device de DirectX (Si truena, no se puede utilizar la aplicación)
                device = createDevice(adapter, wnd, createFlags, presentParams);
                reset(device, false);
            }
        }

        /// <summary>
        /// This callback function may override the default Resizing behaviour, which resets
        /// the Device every resize event
        /// </summary>
        void device_DeviceResizing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            presentParams.BackBufferWidth = wnd.Width;
            presentParams.BackBufferHeight = wnd.Height;

            if (!presenter.RecoverFromDeviceLostState(device))
                device.Reset(presentParams);
        }

        /// <summary>
        /// This callback function will be called immediately after the Direct3D device has 
        /// been destroyed, which generally happens as a result of application termination or 
        /// windowed/full screen toggles. Resources created in the OnCreateDevice callback 
        /// should be released here, which generally includes all Pool.Managed resources. 
        /// </summary>
        void device_Disposing(object sender, EventArgs e)
        {
            //animating = false;
            AnimationStop();
            resourceManager.device_Disposing(sender, e);
        }

        /// <summary>
        /// This event will be fired immediately after the Direct3D device has been 
        /// reset, which will happen after a lost device scenario. This is the best location to 
        /// create Pool.Default resources since these resources need to be reloaded whenever 
        /// the device is lost. Resources created here should be released in the OnLostDevice 
        /// event. 
        /// </summary>
        void device_DeviceReset(object sender, EventArgs e)
        {
            // The World Transform should be equal for all views, and therefore should be Identity
            device.Transform.World = Matrix.Identity;

            device.RenderState.ZBufferEnable = true;
            device.RenderState.PointSize = 4.0f;
            device.RenderState.CullMode = Cull.None;
            device.RenderState.NormalizeNormals = true;
            device.RenderState.ShadeMode = ShadeMode.Flat;
            device.RenderState.Ambient = System.Drawing.Color.FromArgb(0x404040);
            device.RenderState.Lighting = false;

            // Set Lights
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = System.Drawing.Color.White;
            device.Lights[0].Direction = new Vector3(0, 0, -1);
            //device.Lights[0].Direction = new Vector3((float)Math.Cos(Environment.TickCount / 250.0f),1.0f,(float)Math.Sin(Environment.TickCount / 250.0f));
            device.Lights[0].Enabled = true;

            foreach (GraphicView gv in views)
                if (gv != null)
                    gv.SetUpdateResourcesFlag();

            resourceManager.device_DeviceReset(sender, e);
        }

        /// <summary>
        /// This event function will be called fired after the Direct3D device has 
        /// entered a lost state and before Device.Reset() is called. Resources created
        /// in the OnResetDevice callback should be released here, which generally includes all 
        /// Pool.Default resources. See the "Lost Devices" section of the documentation for 
        /// information about lost devices.
        /// </summary>
        void device_DeviceLost(object sender, EventArgs e)
        {
        }
        #endregion

        /// <summary>
        /// Invoked method when display form is resizing
        /// </summary>
        /// <param name="sender"> The display form </param>
        /// <param name="e"> Events fired </param>
        void wnd_Resize(object sender, EventArgs e)
        {
            // Get form handle
            System.Windows.Forms.Control wnd = sender as System.Windows.Forms.Control;
            //device.Reset(presentParams);
            resizeViewports(wnd);
        }

        /// <summary>
        /// Manage viewport resizing. Fired when the form is resized and has to resize each of views in the chosen layout
        /// </summary>
        /// <param name="wnd"> The resizing window </param>
        void resizeViewports(System.Windows.Forms.Control wnd)
        {
            // Get viewport dimensions
            fullViewport.X = fullViewport.Y = 0;
            fullViewport.Width = wnd.Width;
            fullViewport.Height = wnd.Height;
            fullViewport.MinZ = 0;
            fullViewport.MaxZ = 1;

            Viewport vp = new Viewport();

            // Resize each of the possible viewports according to the contents chosen layout
            switch (layout)
            {
                case ViewportsLayout.OneView:
                    setViewportParams(ref vp, 0, 0, 0, 1, wnd.Width, wnd.Height); views[0].Viewport = vp;
                    break;
                case ViewportsLayout.TwoViewsHorizontal:
                    setViewportParams(ref vp, 0, 0, 0, 1, wnd.Width, wnd.Height / 2); views[0].Viewport = vp;
                    setViewportParams(ref vp, 0, wnd.Height / 2, 0, 1, wnd.Width, wnd.Height - (wnd.Height / 2)); views[1].Viewport = vp;
                    break;
                case ViewportsLayout.TwoViewsVertical:
                    setViewportParams(ref vp, 0, 0, 0, 1, wnd.Width / 2, wnd.Height); views[0].Viewport = vp;
                    setViewportParams(ref vp, wnd.Width / 2, 0, 0, 1, wnd.Width - (wnd.Width / 2), wnd.Height); views[1].Viewport = vp;
                    break;
                case ViewportsLayout.ThreeViewsBigBottom:
                    setViewportParams(ref vp, 0, wnd.Height / 2, 0, 1, wnd.Width, wnd.Height - (wnd.Height / 2)); views[0].Viewport = vp;
                    setViewportParams(ref vp, 0, 0, 0, 1, wnd.Width / 2, wnd.Height / 2); views[1].Viewport = vp;
                    setViewportParams(ref vp, wnd.Width / 2, 0, 0, 1, wnd.Width - (wnd.Width / 2), wnd.Height / 2); views[2].Viewport = vp;
                    break;
                case ViewportsLayout.ThreeViewsBigLeft:
                    setViewportParams(ref vp, 0, 0, 0, 1, wnd.Width / 2, wnd.Height); views[0].Viewport = vp;
                    setViewportParams(ref vp, wnd.Width / 2, 0, 0, 1, wnd.Width - (wnd.Width / 2), wnd.Height / 2); views[1].Viewport = vp;
                    setViewportParams(ref vp, wnd.Width / 2, wnd.Height / 2, 0, 1, wnd.Width - (wnd.Width / 2), wnd.Height - (wnd.Height / 2)); views[2].Viewport = vp;
                    break;
                case ViewportsLayout.ThreeViewsBigRight:
                    setViewportParams(ref vp, wnd.Width / 2, 0, 0, 1, wnd.Width - (wnd.Width / 2), wnd.Height); views[0].Viewport = vp;
                    setViewportParams(ref vp, 0, 0, 0, 1, wnd.Width / 2, wnd.Height / 2); views[1].Viewport = vp;
                    setViewportParams(ref vp, 0, wnd.Height / 2, 0, 1, wnd.Width / 2, wnd.Height - (wnd.Height / 2)); views[2].Viewport = vp;
                    break;
                case ViewportsLayout.ThreeViewsBigTop:
                    setViewportParams(ref vp, 0, 0, 0, 1, wnd.Width, wnd.Height / 2); views[0].Viewport = vp;
                    setViewportParams(ref vp, 0, wnd.Height / 2, 0, 1, wnd.Width / 2, wnd.Height - (wnd.Height / 2)); views[1].Viewport = vp;
                    setViewportParams(ref vp, wnd.Width / 2, wnd.Height / 2, 0, 1, wnd.Width - (wnd.Width / 2), wnd.Height - (wnd.Height / 2)); views[2].Viewport = vp;
                    break;
                case ViewportsLayout.FourViews:
                    setViewportParams(ref vp, 0, 0, 0, 1, wnd.Width / 2, wnd.Height / 2); views[0].Viewport = vp;
                    setViewportParams(ref vp, wnd.Width / 2, 0, 0, 1, wnd.Width - (wnd.Width / 2), wnd.Height / 2); views[1].Viewport = vp;
                    setViewportParams(ref vp, 0, wnd.Height / 2, 0, 1, wnd.Width / 2, wnd.Height - (wnd.Height / 2)); views[2].Viewport = vp;
                    setViewportParams(ref vp, wnd.Width / 2, wnd.Height / 2, 0, 1, wnd.Width - (wnd.Width / 2), wnd.Height - (wnd.Height / 2)); views[3].Viewport = vp;
                    break;
            }

            // Because all viewports were changed, its needed to rebuild picking and tracking surfaces, and also to render again the scene
            wnd.Invalidate();
            activeViewChange(activeView);
        }

        /// <summary>
        /// Call the event for a change in the active view
        /// </summary>
        /// <param name="newView"> Which view needs to be updated </param>
        void activeViewChange(GraphicView newView)
        {
            if (ActiveViewChange != null)
                ActiveViewChange(this, new ActiveViewChangedEventArgs(newView));
        }

        /// <summary>
        /// Set the new viewport parameters
        /// </summary>
        /// <param name="vp"> The updating viewport </param>
        /// <param name="x"> X position </param>
        /// <param name="y"> Y position </param>
        /// <param name="minZ"> Z near </param>
        /// <param name="maxZ"> Z far </param>
        /// <param name="width"> Viewport width </param>
        /// <param name="height"> Viewport height </param>
        void setViewportParams(ref Viewport vp, int x, int y, float minZ, float maxZ, int width, int height)
        {
            vp.X = x;
            vp.Y = y;
            vp.Width = width;
            vp.Height = height;
            vp.MinZ = minZ;
            vp.MaxZ = maxZ;
        }

        /// <summary>
        /// According to the current display window and some parametes, get a rendering device
        /// </summary>
        /// <param name="wnd"> Display window </param>
        /// <param name="adapter"> Identifier od the gotten adapter </param>
        /// <param name="createFlags"> Creation flags </param>
        /// <param name="presentParams"> Presentation parameters </param>
        void getDeviceAdapter(System.Windows.Forms.Control wnd, out int adapter, out CreateFlags createFlags, out PresentParameters presentParams)
        {
            // Get the default video adapter
            adapter = Manager.Adapters.Default.Adapter;
            // Set creation flags for allowing use of the software in most of the computers 
            createFlags = CreateFlags.SoftwareVertexProcessing;

            // Seleccionar el adaptador que corresponda a la mayor area de le ventana,
            // depende del monitor en el caso de escritorio extendido o de más de una tarjeta de video
            foreach (AdapterInformation ai in Manager.Adapters)
                if (System.Windows.Forms.Screen.FromControl(wnd).DeviceName.Contains(ai.Information.DeviceName))
                {
                    // Adpater found!!
                    adapter = ai.Adapter;
                    break;
                }

            // Select Depth buffer format
            System.Collections.ArrayList depthStencilPossibleList = new System.Collections.ArrayList();
            depthStencilPossibleList.AddRange(new DepthFormat[] {
                                                             DepthFormat.D16,
                                                             DepthFormat.D15S1,
                                                             DepthFormat.D24X8,
                                                             DepthFormat.D24S8,
                                                             DepthFormat.D24X4S4,
                                                             DepthFormat.D32 });

            // Select depth stencil format
            DepthFormat depthStencilFormat = DepthFormat.Unknown;
            foreach (DepthFormat depthStencil in depthStencilPossibleList)
            {
                if (Manager.CheckDeviceFormat(adapter,
                    DeviceType.Hardware, Manager.Adapters[adapter].CurrentDisplayMode.Format,
                    Usage.DepthStencil, ResourceType.Surface, depthStencil))
                {
                    // This can be used as a depth stencil, make sure it matches
                    if (Manager.CheckDepthStencilMatch(adapter,
                        DeviceType.Hardware, Manager.Adapters[adapter].CurrentDisplayMode.Format,
                        Manager.Adapters[adapter].CurrentDisplayMode.Format, depthStencil))
                    {
                        // Yup, add it
                        depthStencilFormat = depthStencil;
                        break;
                    }
                }
            }

            // Possible formats for back buffer. If no 32bit color available, Treu Structure cannot run
            List<Format> possibleFormats = new List<Format>();
            possibleFormats.Add(Format.A8B8G8R8);
            possibleFormats.Add(Format.A8R8G8B8);
            possibleFormats.Add(Format.X8B8G8R8);
            possibleFormats.Add(Format.X8R8G8B8);
            Format format = Manager.Adapters[adapter].CurrentDisplayMode.Format;
            if (!possibleFormats.Contains(format))
                throw new NotSupportedException(Culture.Get("strFatalErrorNo32bit"));

            // Obtener parámetros del Device
            presentParams = getDXPresentParams(depthStencilFormat);
            if (Manager.GetDeviceCaps(adapter, DeviceType.Hardware).DeviceCaps.SupportsHardwareTransformAndLight)
            {
                createFlags = CreateFlags.HardwareVertexProcessing;

                // Posiblemente sea necesario checar los VertexProcessingCaps también
                return;
            }
        }

        /// <summary>
        /// Rendering method. Subscription to window paint event
        /// </summary>
        /// <param name="sender"> Form repainting </param>
        /// <param name="e"> Event arguments </param>
        void wnd_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //if (!drawingPickingSurface)
            //    if (fullViewport.X != e.ClipRectangle.X || fullViewport.Y != e.ClipRectangle.Y || fullViewport.Width != e.ClipRectangle.Width || fullViewport.Height != e.ClipRectangle.Height)
            //    {
            //        updateView(getActiveViewFromPoint(e.ClipRectangle.X, e.ClipRectangle.Y));
            //        animate();
            //        return;
            //    }

            // Update all views in the layout
            UpdateAllViews();

            // Continue with the animation cycle
            //animate();

            if (activeView.ModelRenderer.RenderOptions.ShowAnimated)
                AnimationStart();
        }

        #region ModelChanged events...
        /// <summary>
        /// A selection event has been fired. Some properties need to be updated
        /// </summary>
        /// <param name="sender"> The sender (display window) </param>
        /// <param name="e"> Event arguments </param>
        void updateSelection(object sender, Canguro.Model.Model.SelectionChangedEventArgs e)
        {
            // Send signal to update Model on next refresh
            for (int i = 0; i < 4; i++)
                views[i].SetUpdateModelFlag();

            // Invalidates the viewports
            wnd.Invalidate();
        }

        /// <summary>
        /// Response to model reset
        /// </summary>
        /// <param name="sender"> Parent window (display window) </param>
        /// <param name="e"> Event arguments </param>
        private void updateModelReset(object sender, System.EventArgs e)
        {
            // Call reset method
            reset(device, true);
            // Update model
            updateModel(sender, e);
        }

        /// <summary>
        /// Response to model update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateModel(object sender, System.EventArgs e)
        {
            // Send signal to update Model on next refresh
            for (int i = 0; i < 4; i++)
                views[i].SetUpdateModelFlag();

            // Invalidate the viewports
            wnd.Invalidate();
        }

        /// <summary>
        /// On update results, renderers need to be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateResults(object sender, System.EventArgs e)
        {
            activeView.ModelRenderer.RenderOptions.ShowDeformed = true;
            //activeView.ModelRenderer.RenderOptions.ShowDesigned = true;
            updateModel(sender, e);
        }
        #endregion

        /// <summary>
        /// Update views according to the chosen layout
        /// </summary>
        internal void UpdateAllViews()
        {
            // Check if we have valid parameters and also, valid states
            if ((!enabled) || (device == null) || presenter.RecoverFromDeviceLostState(device)) return;

            try
            {
                // Los Viewports asociados a los Layouts se definen al definir el Layout
                // Sólo es necesario mandar llamar al update de los Viewports activos            
                switch (layout)
                {
                    case ViewportsLayout.OneView:
                        views[0].Update(device);
                        break;
                    case ViewportsLayout.TwoViewsHorizontal:
                    case ViewportsLayout.TwoViewsVertical:
                        views[0].Update(device);
                        views[1].Update(device);
                        break;
                    case ViewportsLayout.ThreeViewsBigBottom:
                    case ViewportsLayout.ThreeViewsBigLeft:
                    case ViewportsLayout.ThreeViewsBigRight:
                    case ViewportsLayout.ThreeViewsBigTop:
                        views[0].Update(device);
                        views[1].Update(device);
                        views[2].Update(device);
                        break;
                    case ViewportsLayout.FourViews:
                        views[0].Update(device);
                        views[1].Update(device);
                        views[2].Update(device);
                        views[3].Update(device);
                        break;
                }

                // When picking surface is not been drawn...
                if (!DrawingPickingSurface)
                {
                    // Update the view
                    presenter.Present(null);
                }
            }
            catch (DeviceLostException e)   // El Device está en estado Lost y no es posible darle Reset en este momento
            {
                System.Console.WriteLine(e.StackTrace);
            }
            catch (DeviceNotResetException e) // El Device está en estado Lost y si es posible darle Reset en este momento
            {
                // TODO: Reset al Device y luego wnd.Invalidate()
                System.Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Updates and redraws the Active View
        /// </summary>
        public void UpdateView()
        {
            if (activeView != null)
                UpdateView(activeView);
        }

        /// <summary>
        /// Updates and redraws the view
        /// </summary>
        /// <param name="view">The view to be redrawn</param>
        public void UpdateView(GraphicView view)
        {
            updateView(view);
        }

        /// <summary>
        /// Updates view and checks if picking and tracking surfaces need to be updated
        /// </summary>
        /// <param name="rebuildSurfaces"></param>
        internal void updateView(bool rebuildSurfaces)
        {
            updateView(activeView);
        }
        
        /// <summary>
        /// Updates a single view
        /// </summary>
        /// <param name="view"> The view that needs to be updated </param>
        private void updateView(GraphicView view)
        {
            // Check if everything is ok
            if ((!enabled) || (device == null) || presenter.RecoverFromDeviceLostState(device)) return;

            // Update view
            view.Update(device);

            presenter.Present(view);
        }


        /// <summary>
        /// Public property: Gets the resource cache used by application
        /// </summary>
        internal ResourceManager ResourceManager
        {
            get
            {
                return resourceManager;
            }
        }

        /// <summary>
        /// Get the active view, according to a screen point
        /// </summary>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        /// <returns> Returns the active view found, and null otherwise </returns>
        public GraphicView getActiveViewFromPoint(int x, int y)
        {
            bool found = false;
            int i = 0;
            while (!found && i < 4)
            {
                if (((x >= views[i].Viewport.X) && (x <= views[i].Viewport.X + views[i].Viewport.Width)) &&
                    ((y >= views[i].Viewport.Y) && (y <= views[i].Viewport.Y + views[i].Viewport.Height)))
                    found = true;
                i++;
            }

            if (found)
                return views[i - 1];

            return null;
        }
        
        /// <summary>
        /// Set the active view from a screen point
        /// </summary>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        public void SetActiveViewFromPoint(int x, int y)
        {
            GraphicView view = getActiveViewFromPoint(x, y);

            if (view != null)
            {
                if (activeView != view)
                {
                    activeViewChange(view);
                    activeView = view;

                    if (activeView.ModelRenderer.RenderOptions.ShowAnimated)
                        AnimationStart();
                }
            }
        }


        // 
        /// <summary>
        /// This method saves a screenshot to the specified file.
        /// Should be called before Device.Present, because it copies the
        /// primary RenderTarget which is usually the BackBuffer
        /// </summary>
        /// <param name="filename"></param>
        public void SaveScreenshot(string filename, ImageFileFormat format)
        {
            using (Surface renderTarget = device.GetRenderTarget(0))
            {
                //SurfaceLoader.Save(@"c:\temp\test.bmp", ImageFileFormat.Bmp, renderTarget);
                SurfaceLoader.Save(filename, format, renderTarget);
            }            
        }

        /// <summary>
        /// This method uses a previously calculated PickingSurface in order to perform
        /// picking.
        /// </summary>
        /// <param name="x">Position of the X-coordinate of the mouse in window client area coordinates</param>
        /// <param name="y">Position of the Y-coordinate of the mouse in window client area coordinates</param>
        /// <returns>A list of picked Items</returns>
        public List<Canguro.Model.Item> PickItem(int x, int y)
        {
            if (x < 0 || x >= wnd.Width || y < 0 || y >= wnd.Height) return null;

            //x = (x < 0) ? 0 : ((x >= wnd.Width) ? wnd.Width - 1 : x);
            //y = (y < 0) ? 0 : ((y >= wnd.Height) ? wnd.Height - 1 : y);

            presenter.RegenHelperSurfaces(true);
            Surface ps = resourceManager.PickingSurface;
            int pickD = Canguro.Commands.View.Selection.PickHalfWidthPixels;
            int bgColor = System.Drawing.Color.Black.ToArgb();
            int pickLen = 2 * pickD + 1;

            // Obtain pixels inside picking box
            List<int> pixelList = new List<int>(pickLen * pickLen);
            try
            {
                GraphicsStream gs = ps.LockRectangle(LockFlags.ReadOnly);
                unsafe
                {
                    int* ptr = (int*)gs.InternalDataPointer;
                    int pixel = *(ptr + (y * wnd.Width) + x); // pixel at coordinate (x,y);

                    int posx, posy;
                    for (int i = 0; i < pickLen; i++)
                        for (int j = 0; j < pickLen; j++)
                        {
                            posx = x - pickD + j;
                            posy = y - pickD + i;

                            // if pixel is outside screen, don't add it
                            if (posx >= wnd.Width || posx < 0 || posy >= wnd.Height || posy < 0) continue;

                            pixelList.Add(*(ptr + (posy * wnd.Width) + posx));
                        }
                }
            }
            finally
            {
                ps.UnlockRectangle();
            }

            // Retrieve Items without duplicates
            pixelList.Sort(new Utility.ReverseComparer<int>());
            List<Canguro.Model.Item> list = new List<Canguro.Model.Item>(pixelList.Count);
            int lastPixel = bgColor;
            foreach (int pixel in pixelList)
                if (pixel != lastPixel && pixel != bgColor)
                {
                    Canguro.Model.Item item = resourceManager.PickedItem(pixel);
                    if (item != null)
                        list.Add(item);
                    lastPixel = pixel;
                }

            return list;
        }

        /// <summary>
        /// This method responds to the Controller.EndViewCommand.
        /// events, which are generated every time a ViewCommand series ends 
        /// (excepting SelectionCommand) or a ModelCommand ends.
        /// For example, if the user issues a Zoom command followed immediately by a Pan cmd
        /// then the ViewCommand series hasn't ended. If the user end that series, i.e. by
        /// right clicking with the mouse, then the ViewCommand series is ended, because the
        /// next ViewCommand will be Selection wich cannot change the Viewing current state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void controller_EndViewCommand(object sender, Canguro.Controller.Controller.EndViewCommandArgs e)
        {
            // Finish animation (if applicable)
            if (activeView.ModelRenderer.RenderOptions.ShowAnimated && e.Command == Controller.Controller.Instance.SelectionCommand)
                activeView.ModelRenderer.RenderOptions.ShowAnimated = false;
            // End of Finish animation

            presenter.RegenHelperSurfaces(true);
        }

        private bool printingHiResImage = false;

        public bool PrintingHiResImage
        {
            get { return printingHiResImage; }
            set { printingHiResImage = value; }
        }

        private bool drawWorldAxes = true;

        public bool DrawWorldAxes
        {
            get { return drawWorldAxes; }
            set { drawWorldAxes = value; }
        }

        /// <summary>
        /// Property: Tells if picking surface is being drawn
        /// </summary>
        private bool drawingPickingSurface;
        /// <summary>
        /// This property gets or sets the state that Renderers will query to decide the current drawing mode. If true, the rendered color should equal an Index provided by ResourceCache.GetNextPickIndex
        /// </summary>
        public bool DrawingPickingSurface
        {
            get { return drawingPickingSurface; }
            set { drawingPickingSurface = value; }
        }

        private Presenter presenter;

        internal Presenter Presenter
        {
            get
            {
                return presenter;
            }
        }

        internal GraphicView GetView(int id)
        {
            if (id >= 0 && id < 4)
                return views[id];

            return null;
        }

        public void SavePreviousActiveView()
        {
            // Get a copy of the present ArcBall
            Canguro.View.ArcBall clonedArcBall = (Canguro.View.ArcBall)activeView.ArcBallCtrl.Clone();

            // Push it in the ArcBall stack
            activeView.PushArcBall(clonedArcBall);
        }
    }
}
