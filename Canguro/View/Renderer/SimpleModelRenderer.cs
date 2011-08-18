using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View.Renderer
{
    public class SimpleModelRenderer : ModelRenderer
    {
        List<Model.Joint> joints2RenderList = null;

        public SimpleModelRenderer(JointRenderer jr, LineRenderer lir, AreaRenderer ar, LoadRenderer lor, ElementForcesRenderer efr, GadgetRenderer gr)
            : base()
        {
            JointRenderer = jr;
            LineRenderer = lir;
            AreaRenderer = ar;
            LoadRenderer = lor;
            ForcesRenderer = efr;
            GadgetRenderer = gr;
        }

        public SimpleModelRenderer() : 
            this(new WireframeJointRenderer(), new WireframeLineRenderer(), new WireframeAreaRenderer(), new SimpleLoadRenderer(), new ElementForcesRenderer(), new GadgetRenderer()) { }

        public override void Render(Microsoft.DirectX.Direct3D.Device device)
        {
            if (model == null) return;
            bool lastUndoEnabled = model.Undo.Enabled;
            bool lastUnitSystemEnabled = Model.UnitSystem.UnitSystemsManager.Instance.Enabled;

            try
            {
                if (model.IsLocked)
                    model.Undo.Enabled = false;

                ResourceManager rm = Canguro.View.GraphicViewManager.Instance.ResourceManager;
                Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;
                itemsInView.Clear();

                lock (device)
                {
                    device.BeginScene();
                    try
                    {
                        if (GraphicViewManager.Instance.DrawingPickingSurface)
                        {
                            //if (AreaRenderer != null)
                            //    AreaRenderer.Render(device, model.AreaList);

                            // Get Line and Joint list to render for picking
                            bool[] joints2Render = new bool[model.JointList.Count];

                            if (joints2RenderList == null)
                                joints2RenderList = new List<Model.Joint>(model.JointList.Count);
                            else
                            {
                                joints2RenderList.Clear();

                                if (joints2RenderList.Capacity < model.JointList.Count)
                                    joints2RenderList.Capacity = model.JointList.Count;
                            }

                            foreach (Model.AreaElement a in model.AreaList)
                                if (a != null && a.IsVisible)
                                {
                                    joints2Render[(int)a.J1.Id] = true;
                                    joints2Render[(int)a.J2.Id] = true;
                                    joints2Render[(int)a.J3.Id] = true;
                                    if(a.J4 != null)
                                        joints2Render[(int)a.J4.Id] = true;
                                }

                            foreach (Model.LineElement l in model.LineList)
                                if (l != null && l.IsVisible)
                                {
                                    joints2Render[(int)l.I.Id] = true;
                                    joints2Render[(int)l.J.Id] = true;
                                }
                            
                            // Render lines
                            if (LineRenderer != null)
                                LineRenderer.Render(device, model, null, RenderOptions, itemsInView);

                            // Clear Z Buffer to point joint over other objects
                            device.Clear(ClearFlags.ZBuffer, Properties.Settings.Default.BackColor, 1.0f, 0);

                            // Build list of joints to render
                            foreach (Model.Joint j in model.JointList)
                                if (j != null && (joints2Render[(int)j.Id] || (j.IsVisible && (RenderOptions.OptionsShown & RenderOptions.ShowOptions.ShowJoints) > 0)))
                                    joints2RenderList.Add(j);
                            
                            if (JointRenderer != null)
                                JointRenderer.Render(device, model, joints2RenderList, RenderOptions, false, true);
                        }
                        else
                        {
                            if (JointRenderer != null && (((RenderOptions.OptionsShown & RenderOptions.ShowOptions.JointDOFs) > 0) ||
                                (!RenderOptions.ShowShaded && ((RenderOptions.OptionsShown & RenderOptions.ShowOptions.ShowJoints) > 0))))
                                JointRenderer.Render(device, model, model.JointList, RenderOptions, true, false);

                            if (LineRenderer != null)
                            {
                                if (GraphicViewManager.Instance.PrintingHiResImage)
                                {
                                    LineRenderer.Render(device, model, Model.Model.Instance.LineList, RenderOptions, itemsInView);

                                    Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;
                                    LineRenderer.DrawTexts(device, model.LineList, RenderOptions, itemsInView);
                                    Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

                                    if ((RenderOptions.OptionsShown & RenderOptions.ShowOptions.LineLocalAxes) != 0)
                                        LineRenderer.DrawLocalAxes(device, model.LineList, itemsInView);
                                }
                                else
                                {
                                    LineRenderer.Render(device, model, null, RenderOptions, itemsInView);

                                    Model.UnitSystem.UnitSystemsManager.Instance.Enabled = true;
                                    LineRenderer.DrawTexts(device, null, RenderOptions, itemsInView);
                                    Model.UnitSystem.UnitSystemsManager.Instance.Enabled = false;

                                    if ((RenderOptions.OptionsShown & RenderOptions.ShowOptions.LineLocalAxes) != 0)
                                        LineRenderer.DrawLocalAxes(device, model.LineList, itemsInView);
                                }
                            }

                            if (AreaRenderer != null)
                                AreaRenderer.Render(device, model, model.AreaList, RenderOptions, itemsInView);

                            if (!model.HasResults) // Don't draw loads when results are displayed
                            {
                                if (LoadRenderer != null && (RenderOptions.OptionsShown & RenderOptions.ShowOptions.Loads) != 0)
                                    LoadRenderer.Render(device, model, RenderOptions, itemsInView);
                            }
                            else
                            {
                                if (LoadRenderer != null && (RenderOptions.OptionsShown & RenderOptions.ShowOptions.Reactions) != 0)
                                    LoadRenderer.RenderReactions(device, model, RenderOptions, itemsInView);
                            }

                            if (ForcesRenderer != null)
                                ForcesRenderer.Render(device, model, RenderOptions, itemsInView);

                            if (GadgetRenderer != null)
                                GadgetRenderer.Render(device, rm.GadgetManager.GadgetList);

                            if (RenderOptions.ShowDesigned && !GraphicViewManager.Instance.PrintingHiResImage)
                            {
                                LineRenderer.DrawColorSideBar(device, 50, 0f, 0.85f, "G2", new float[] { 0f, 0.25f, 0.5f, 0.76f, 1f }, 0, 0.99f, Model.UnitSystem.Units.NoUnit, Utility.ColorUtils.GetColorFromDesignRatio, null);
                                LineRenderer.DrawColorSideBar(device, 50, 0.8f, 1f, "G2", new float[] { 0f, 0.5f, 1f }, 1f, 1f, Model.UnitSystem.Units.NoUnit, Utility.ColorUtils.GetColorFromDesignRatio, new string[] { "", ">= 1", "" });
                            }
                        }

                        // Flush Buffers
                        rm.Flush(ResourceStreamType.Points);
                        rm.Flush(ResourceStreamType.Lines);
                        rm.Flush(ResourceStreamType.TriangleListPositionColored);
                        rm.Flush(ResourceStreamType.TriangleListPositionNormalColored);
                    }
                    finally
                    {
                        device.EndScene();
                    }
                }
            }
            finally
            {
                Model.UnitSystem.UnitSystemsManager.Instance.Enabled = lastUnitSystemEnabled;
                model.Undo.Enabled = lastUndoEnabled;
            }
        }

        public override void ReconfigureRenderers()
        {
            // Joint Renderer
            JointRenderer = (JointRenderer)Renderers["wj"];

            // Line Renderer
            if (RenderOptions.ShowShaded)
            {
                if (RenderOptions.ShowDeformed)
                    LineRenderer = (LineRenderer)Renderers["dsl"];
                else
                    LineRenderer = (LineRenderer)Renderers["sl"];
            }
            else if (RenderOptions.ShowStressed)
                LineRenderer = (LineRenderer)Renderers["fl"];
            else
            {
                if (RenderOptions.ShowDeformed)
                    LineRenderer = (LineRenderer)Renderers["dwl"];
                else
                    LineRenderer = (LineRenderer)Renderers["wl"];
            }

            #region  Area Renderer
            // Area Renderer
            if (RenderOptions.ShowShaded)
            {
            //    if (RenderOptions.ShowDeformed)
            //    {
            //        LineRenderer = (AreaRenderer)Renderers["dsa"];
            //    }
            //    else
            //    {
                AreaRenderer = (AreaRenderer)Renderers["dsa"];
            //    }
            }
            //else if (RenderOptions.ShowStressed)
            //    LineRenderer = (AreaRenderer)Renderers["fa"];
            else
            {
            //    if (RenderOptions.ShowDeformed)
            //    {
            //        LineRenderer = (AreaRenderer)Renderers["dwa"];
            //    }
            //    else
            //    {
                AreaRenderer = (AreaRenderer)Renderers["dwa"];
            //    }
            }
            #endregion

            // Load Renderer
            LoadRenderer = (LoadRenderer)Renderers["lr"];

            // Forces Renderer
            ForcesRenderer = (ElementForcesRenderer)Renderers["ef"];

            //GadgetRenderer
            GadgetRenderer = (GadgetRenderer)Renderers["gr"];

            // Repaint
            //invalidateView();
            Canguro.Controller.Controller.Instance.MainFrm.ScenePanel.Invalidate();
        }

    }
}
