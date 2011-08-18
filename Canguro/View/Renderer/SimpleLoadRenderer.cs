using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.Model;
using Canguro.Model.Load;
using Canguro.Properties;

namespace Canguro.View.Renderer
{
    /// <summary>
    /// Draws loads as simple arrows
    /// </summary>
    public class SimpleLoadRenderer : LoadRenderer
    {
        /// <summary> Main method for rendering loads </summary>
        /// <param name="device"> The rendering device </param>
        /// <param name="model"> The model instance </param>
        /// <param name="options"> Rendering options </param>
        public override void Render(Device device, Model.Model model, RenderOptions options, List<Item> itemsInView)
        {
            if (itemsInView == null) return;

            if (itemsInView.Count <= 0)
                GetItemsInView(itemsInView);

            if (itemsInView.Count > 0)
            {
                // Get resource cache instance
                ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
                int numTriangVerticesDrawn = 0;
                int numLineVerticesDrawn = 0;

                // Turn off lighting for color rendering
                device.RenderState.Lighting = false;
                device.RenderState.CullMode = Cull.None;

                // Vertex Buffer capture depends on current "renderer" (joints, lines, areas) and must be updated by them according to the feeded vertices
                // For simplicity, any "renderer" takes as parameter the last captured VertexBuffer and the number of vertices drawn
                PositionColoredPackage linePack = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);
                PositionColoredPackage triangPack = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.TriangleListPositionColored, false, true);

                // First, render loads over joints
                renderJointLoads(itemsInView, options, ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                // Second, render loads over lines
                renderLineLoads(itemsInView, options, ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);
                // Third, render loads over areas
                renderAreaLoads(itemsInView, options, ref triangPack, ref linePack, ref numTriangVerticesDrawn, ref numLineVerticesDrawn);

                // Flush remaining vertices
                rc.ReleaseBuffer(numLineVerticesDrawn, 0, ResourceStreamType.Lines);
                rc.Flush(ResourceStreamType.Lines);
                rc.ReleaseBuffer(numTriangVerticesDrawn, 0, ResourceStreamType.TriangleListPositionColored);
                rc.Flush(ResourceStreamType.TriangleListPositionColored);

                //Turn on lighting
                device.RenderState.Lighting = true;
            }
        }
    }
    #region Render Legacy
    ///// <summary>
    ///// Draws loads as simple arrows
    ///// </summary>
    //public class SimpleLoadRenderer : LoadRenderer
    //{
    //    /// <summary> Main method for rendering loads </summary>
    //    /// <param name="device"> The rendering device </param>
    //    /// <param name="model"> The model instance </param>
    //    /// <param name="options"> Rendering options </param>
    //    protected override void render(Device device, Model.Model model, RenderOptions options)
    //    {
    //        // First, render loads over joints
    //        renderJointLoads(model.JointList, options);
    //        // Second, render loads over lines
    //        renderLineLoads(model.LineList, options);
    //        // Third, render loads over areas
    //        renderAreaLoads(model.AreaList, options);
    //    }
    //}
    #endregion
}
