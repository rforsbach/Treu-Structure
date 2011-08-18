using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View
{
    internal class Presenter
    {
        #region Fields
        private GraphicViewManager manager;
        private MainFrm mainWnd;
        private IntPtr wndHandle;

        public static readonly uint CM_PAINT;
        public static readonly uint CM_TRACKINGPAINT;

        private bool dirtyPickingSurface = true;
        private bool dirtyTrackingSurface = true;
        private bool dirtyVisibleSurface = false;

        bool sentTrackingPaintMsg = false;
        TrackingPaintArgs[] trackingMessage = new TrackingPaintArgs[4];
        #endregion

        static Presenter()
        {
            CM_PAINT = Utility.NativeMethods.RegisterWindowMessage("CM_PAINT");
            CM_TRACKINGPAINT = Utility.NativeMethods.RegisterWindowMessage("CM_TRACKINGPAINT");
        }

        public Presenter(GraphicViewManager manager, MainFrm mainWnd)
        {
            this.manager = manager;
            this.mainWnd = mainWnd;
            wndHandle = mainWnd.Handle;
        }

        #region Regen Surfaces
        private int numLostEvents = 0;
        /// <summary>
        /// If Device is in Lost state but CAN be reset, then Reset it and invalidate
        /// to try to paint again. If Device is in Lost state but CANNOT be reset, then 
        /// invalidate to try to render later
        /// </summary>
        /// <returns>True if the device is lost and no painting can be done at the moment.
        /// False otherwise.</returns>
        public bool RecoverFromDeviceLostState(Device device)
        {
            int isLost = (int)ResultCode.Success;

            if (device.Disposed) return true;

            device.CheckCooperativeLevel(out isLost);

            if (isLost == (int)ResultCode.DeviceLost)
            {
                if (manager.ActiveView.ModelRenderer.RenderOptions.ShowAnimated)
                {
                    manager.ActiveView.ModelRenderer.RenderOptions.ShowAnimated = false;
                    mainWnd.UpdateToolBar(Canguro.Model.Model.Instance);
                }
                if (++numLostEvents > 10)
                {
                    // Recreate device completely                    
                    manager.RestartGraphics();
                }

                System.Threading.Thread.Sleep(250);
                Application.DoEvents();
                mainWnd.ScenePanel.Invalidate();
                return true;
            }
            else if (isLost == (int)ResultCode.DeviceNotReset)
            {
                if (manager.ActiveView.ModelRenderer.RenderOptions.ShowAnimated)
                {
                    manager.ActiveView.ModelRenderer.RenderOptions.ShowAnimated = false;
                    mainWnd.UpdateToolBar(Canguro.Model.Model.Instance);
                }
                device.Reset(manager.PresentParams);
                System.Threading.Thread.Sleep(500);
                mainWnd.ScenePanel.Invalidate();
                numLostEvents = 0;
                return true;
            }

            numLostEvents = 0;
            return false;
        }

        public void RegenHelperSurfaces(bool regenPicking)
        {
            if (manager.Device == null || manager.Device.Disposed == true || RecoverFromDeviceLostState(manager.Device)) return;

            try
            {
                if (!dirtyVisibleSurface)
                {
                    if (dirtyTrackingSurface)
                        regenTrackingSurface();

                    if (dirtyPickingSurface && regenPicking)
                        regenPickingSurface();
                }
            }
            catch (DeviceLostException)
            {
                System.Threading.Thread.Sleep(500);
                mainWnd.ScenePanel.Invalidate();
            }
            catch (DeviceNotResetException)
            {
                manager.Device.Reset(manager.PresentParams);
                System.Threading.Thread.Sleep(500);
                mainWnd.ScenePanel.Invalidate();
            }
        }

        /// <summary>
        /// This method redraws the whole scene under one condition. The color of each Item
        /// is equal to an index provided by ResourceCache.GetNextPickIndex
        /// </summary>
        private void regenPickingSurface()
        {
            try
            {
                Surface oldRT = manager.Device.GetRenderTarget(0);

                manager.DrawingPickingSurface = true;
                manager.ResourceManager.ResetPickIndices();
                manager.UpdateAllViews();
                manager.Device.GetRenderTargetData(manager.Device.GetRenderTarget(0), manager.ResourceManager.PickingSurface);

                dirtyPickingSurface = false;
                //SaveScreenshot(@"c:\ppp.png", ImageFileFormat.Png);
            }
            finally
            {
                manager.DrawingPickingSurface = false;
            }
        }

        private void regenTrackingSurface()
        {
            if (manager.Layout != GraphicViewManager.ViewportsLayout.OneView)
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
                Viewport vp = manager.ActiveView.Viewport;
                rect.X = vp.X;
                rect.Y = vp.Y;
                rect.Width = vp.Width;
                rect.Height = vp.Height;
                SurfaceLoader.FromSurface(manager.ResourceManager.TrackingSurface, rect, manager.Device.GetBackBuffer(0, 0, BackBufferType.Mono), rect, Filter.None, 0);
            }
            else
                SurfaceLoader.FromSurface(manager.ResourceManager.TrackingSurface, manager.Device.GetBackBuffer(0, 0, BackBufferType.Mono), Filter.None, 0);

            Canguro.Controller.Controller.Instance.TrackingController.Reset(manager.ActiveView);
            dirtyTrackingSurface = false;
            //manager.SaveScreenshot(@"c:\ppp.png", ImageFileFormat.Png);
        }

        #endregion

        #region Present
        public void Present(GraphicView view)
        {
            if (!dirtyVisibleSurface)
            {
                sendCanguroPaintMessage(view);
            }
        }

        private void sendCanguroPaintMessage(GraphicView view)
        {
            IntPtr LParam;
    
            if (view == null)
                LParam = (IntPtr)(-1);
            else
                LParam = (IntPtr)(view.Id);
            
            Utility.NativeMethods.PostMessage(wndHandle, CM_PAINT, IntPtr.Zero, LParam);
            dirtyVisibleSurface = true;
        }

        public void PresentImmediately(int viewId)
        {
            if (viewId >= 0)
                PresentImmediately(manager.GetView(viewId));
            else
                PresentImmediately(null);

            manager.ResourceManager.ResetAnimationCache();
        }

        public void PresentImmediately(GraphicView view)
        {
            if (RecoverFromDeviceLostState(manager.Device)) return;

            if (view != null)
            {
                // Get the rectangle that needs to be presented
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(
                    view.Viewport.X,
                    view.Viewport.Y,
                    view.Viewport.Width,
                    view.Viewport.Height);

                // Present scene
                manager.Device.Present(rect, rect, null);
            }
            else
                manager.Device.Present();

            dirtyPickingSurface = true;
            dirtyTrackingSurface = true;
            dirtyVisibleSurface = false;
        }
        #endregion

        #region Tracking Paint
        internal class TrackingPaintArgs
        {
            public long timeStampTicks;
            public MouseEventArgs MouseEventArgs;
            public Canguro.View.GraphicView ActiveView;
            public Canguro.Controller.TrackingController Tracking;

            public TrackingPaintArgs(DateTime timeStamp)
            {
                timeStampTicks = timeStamp.Ticks;
            }
            public TrackingPaintArgs(DateTime timeStamp, MouseEventArgs e, Canguro.View.GraphicView activeView, Canguro.Controller.TrackingController tracking)
            {
                timeStampTicks = timeStamp.Ticks;
                this.MouseEventArgs = e;
                this.ActiveView = activeView;
                this.Tracking = tracking;
            }
        }

        public void TrackingPaint(object sender, MouseEventArgs e, Canguro.View.GraphicView activeView, Canguro.Controller.TrackingController tracking)
        {
            if (!sentTrackingPaintMsg || trackingMessage[activeView.Id] == null || (trackingMessage[activeView.Id] != null && DateTime.Now.Ticks > (trackingMessage[activeView.Id].timeStampTicks + 10e7)))
            {
                sendCanguroTrackingPaintMessage(new TrackingPaintArgs(DateTime.Now, e, activeView, tracking));
            }
        }

        private void sendCanguroTrackingPaintMessage(TrackingPaintArgs e)
        {
            lock (trackingMessage)
            {
                if (trackingMessage[e.ActiveView.Id] != null && e.timeStampTicks < trackingMessage[e.ActiveView.Id].timeStampTicks)
                    return;

                trackingMessage[e.ActiveView.Id] = e;
            }

            IntPtr LParam = IntPtr.Zero;

            if (e.ActiveView == null)
                throw new ArgumentException("Cannot paint tracking on an invalid view");
            else
                LParam = (IntPtr)(e.ActiveView.Id);

            Utility.NativeMethods.PostMessage(wndHandle, CM_TRACKINGPAINT, IntPtr.Zero, LParam);
            sentTrackingPaintMsg = true;
        }

        internal void TrackingPaintImmediately(int viewId)
        {
            TrackingPaintArgs e = null;
            lock (trackingMessage)
            {
                if (viewId >= 0 && viewId <= 4)
                {
                    e = trackingMessage[viewId];
                    trackingMessage[viewId] = null;
                }
            }

            if (e != null)
                TrackingPaintImmediately(this, e.MouseEventArgs, e.ActiveView, e.Tracking);

            sentTrackingPaintMsg = false;
        }
        
        internal void TrackingPaintImmediately(object sender, MouseEventArgs e, Canguro.View.GraphicView activeView, Canguro.Controller.TrackingController tracking)
        {
            if (RecoverFromDeviceLostState(manager.Device)) return;

            Device device = manager.Device;
            if (device == null || device.Disposed == true) return;
            if (Controller.Controller.Instance.ViewCommand != Controller.Controller.Instance.SelectionCommand) return;

            if (dirtyVisibleSurface)
                PresentImmediately(activeView);

            RegenHelperSurfaces(false);

            device.Clear(ClearFlags.ZBuffer, Canguro.Properties.Settings.Default.BackColor, 1.0f, 0);

            lock (device)
            {
                //Begin the scene
                device.BeginScene();

                try
                {
                    if (manager.Layout != GraphicViewManager.ViewportsLayout.OneView)
                    device.Viewport = manager.FullViewport;

                    SurfaceLoader.FromSurface(device.GetBackBuffer(0, 0, BackBufferType.Mono),
                        Canguro.View.GraphicViewManager.Instance.ResourceManager.TrackingSurface, Filter.None, 0);

                    if (manager.Layout != GraphicViewManager.ViewportsLayout.OneView)
                        device.Viewport = activeView.Viewport;

                    if (tracking.TrackingService != null)
                        tracking.TrackingService.Paint(device);
                    else if (tracking.HoverController.IsActive && !tracking.SnapController.IsActive && !activeView.ModelRenderer.RenderOptions.ShowAnimated)
                        tracking.HoverController.Paint(device);

                    if (tracking.SnapController.IsActive)
                        tracking.SnapController.Paint(device, activeView, e);
                }
                finally
                {
                    //End the scene
                    device.EndScene();
                }
            }

            device.Present();
            sentTrackingPaintMsg = false;
        }
        #endregion
        
        internal void AnimationPaintImmediately(object sender, Canguro.View.GraphicView activeView, int animationStep)
        {
            if (RecoverFromDeviceLostState(manager.Device)) return;

            Device device = manager.Device;
            if (device == null || device.Disposed == true) return;
            if (Controller.Controller.Instance.ViewCommand != Controller.Controller.Instance.SelectionCommand) return;

            Surface srcSurface = Canguro.View.GraphicViewManager.Instance.ResourceManager.AnimationCache[animationStep];

            if (srcSurface != null && !srcSurface.Disposed)
            {
                lock (device)
                {
                    //Begin the scene
                    device.BeginScene();

                    try
                    {
                        if (manager.Layout != GraphicViewManager.ViewportsLayout.OneView)
                            device.Viewport = manager.FullViewport;

                        //SurfaceLoader.FromSurface(device.GetBackBuffer(0, 0, BackBufferType.Mono), srcSurface, Filter.None, 0);
                        device.UpdateSurface(srcSurface, device.GetBackBuffer(0, 0, BackBufferType.Mono));

                        if (manager.Layout != GraphicViewManager.ViewportsLayout.OneView)
                            device.Viewport = activeView.Viewport;
                    }
                    finally
                    {
                        //End the scene
                        device.EndScene();
                    }
                }

                device.Present();
            }
        }

    }
}
