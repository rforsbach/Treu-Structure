using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.View.Gadgets;

using Canguro.Model;
using Canguro.Model.Section;

namespace Canguro.View.Renderer
{
    /// <summary>
    /// Renders lines in wireframe mode
    /// </summary>
    public class WireframeLineRenderer : LineRenderer
    {
        #region Legacy Render
        //public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<Canguro.Model.LineElement> lines, RenderOptions options)
        //{
        //    // Get resource cache instance
        //    ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
        //    // Get main vertex buffer
        //    VertexBuffer theVB = null; // rc.MainVB;
        //    // When no valid vertex buffer handle is gotten, do nothing
        //    if (theVB == null) return;

        //    // Check for possible bug: There is an update at the end, but we always start at the beginning of the vertex buffer
        //    int vbBase = 0;             // Get vertex buffer base pointer
        //    int vbFlush = 0; //rc.VbFlush;   // Get vertex buffer flush size
        //    int vbSize = 0; //rc.VbSize;     // Get vertex buffer size

        //    // Get vertex format size and set stream source->MainVB
        //    device.VertexFormat = CustomVertex.PositionColored.Format;
        //    device.SetStreamSource(0, theVB, 0);

        //    // Is picking mode active? We'll need this state later
        //    bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
        //    // Get the default painting color for joints
        //    int jointColor = Properties.Settings.Default.JointDefaultColor.ToArgb();
        //    // Get default painting color for selected joints
        //    int jointSelectedColor = Properties.Settings.Default.JointSelectedDefaultColor.ToArgb();
        //    // How much is the size of this kind of vertices?
        //    int vSize = CustomVertex.PositionColored.StrideSize;
        //    // Lock vertex buffer for start adding vertices to data stream. Just locks needed space, not the entire buffer
        //    GraphicsStream vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);

        //    int vertsSize = 0;
        //    int lineColor;

        //    // Inicia código NO SEGURO
        //    unsafe
        //    {
        //        // Get direct memory pointer to vertex data
        //        CustomVertex.PositionColored* vbArray = (CustomVertex.PositionColored*)vbData.InternalDataPointer;

        //        // Sweep every line in the model
        //        foreach (Model.LineElement l in lines)
        //        {
        //            // Lines just matter id they are visible
        //            if (l != null && l.IsVisible)
        //            {
        //                // Get line beginning and feed as starting vertex 
        //                vbArray->Position = l.I.Position;
        //                // Define line color according to the rendering state: picking mode, or display scene
        //                if (pickingMode)
        //                    lineColor = rc.GetNextPickIndex(l);
        //                else
        //                {
        //                    if (l.IsSelected == false && l.Properties is StraightFrameProps)
        //                    {
        //                        FrameSection sec = ((StraightFrameProps)l.Properties).Section;

        //                        if (sec.Material.DesignProperties is Canguro.Model.Material.SteelDesignProps)
        //                            lineColor = System.Drawing.Color.FromArgb(255, 75, 105, 185).ToArgb();
        //                        else
        //                            lineColor = System.Drawing.Color.FromArgb(255, (int)(0.52854 * 255), (int)(0.52854 * 255), (int)(0.50754 * 255)).ToArgb();
        //                    }
        //                    else
        //                        lineColor = jointSelectedColor;
        //                }

        //                // Set line color and increment the pointer to next data
        //                vbArray->Color = lineColor;
        //                vbArray++;

        //                // We have one more vertex
        //                vertsSize++;

        //                // Get line ending and feed as ending vertex
        //                vbArray->Position = l.J.Position;
        //                vbArray->Color = lineColor;
        //                vbArray++;

        //                // We have one more vertex
        //                vertsSize++;

        //                // Flush vertices to allow the GPU to start processing
        //                if (vertsSize >= vbFlush - 1)
        //                {
        //                    theVB.Unlock();
        //                    device.DrawPrimitives(PrimitiveType.LineList, vbBase, vertsSize / 2);
        //                    vbBase += vbFlush;

        //                    // If the space allocated in memory is over, discard buffer, else append data
        //                    if (vbBase >= vbSize)
        //                        vbBase = 0;

        //                    // Re-lock the VertexBuffer and obtain the unsafe pointer from the new lock
        //                    vbData = theVB.Lock(vbBase * vSize, vbFlush * vSize, (vbBase != 0) ? LockFlags.NoOverwrite : LockFlags.Discard);
        //                    vbArray = (CustomVertex.PositionColored*)vbData.InternalDataPointer;
        //                    vertsSize = 0;
        //                }
        //            }
        //        }
        //    }   // Termina código NO SEGURO

        //    // Flush remaining vertices
        //    theVB.Unlock();
        //    if (vertsSize != 0)
        //    {
        //        device.DrawPrimitives(PrimitiveType.LineList, vbBase, vertsSize / 2);

        //        vbBase += vbFlush;

        //        // If the space allocated in memory is over, discard buffer, else append data
        //        if (vbBase >= vbSize)
        //            vbBase = 0;
        //    }

        //    // Update vertex buffer base pointer
        //    //rc.VbBase = vbBase;
        //}
        #endregion

        private void renderLine(ResourceManager rc, LineElement l, bool pickingMode, ref PositionColoredPackage package, ref int vertsSize, RenderOptions options)
        {
            int lineColor;
            
            // Define line color according to the rendering state: picking mode, or display scene
            lineColor = getLineColor(rc, l, pickingMode, options.LineColoredBy);

            if (vertsSize + 2 >= package.NumVertices - 1)
            {
                rc.ReleaseBuffer(vertsSize, 0, package.Stream);
                package = (PositionColoredPackage)rc.CaptureBuffer(package.Stream, 0, 2, true);
                vertsSize = 0;
            }
            vertsSize += 2;

            float endI = 0, endJ = 0;
            Vector3 lineDir = l.J.Position - l.I.Position;
            lineDir.Normalize();

            l.GetEndOffsets(ref endI, ref endJ);

            // Inicia código NO SEGURO
            unsafe
            {
                // Get line beginning and feed as starting vertex 
                package.VBPointer->Position = l.I.Position + endI * lineDir;
                package.VBPointer->Color = lineColor;
                package.VBPointer++;

                // Get line ending and feed as ending vertex
                package.VBPointer->Position = l.J.Position - endJ * lineDir;
                package.VBPointer->Color = lineColor;
                package.VBPointer++;
            } // Termina código NO SEGURO
        }

        /// <summary> Main method for rendering lines in wireframe mode </summary>
        /// <param name="device"> The rendering device </param>
        /// <param name="lines"> Line collection in the model </param>
        /// <param name="options"> Rendering options </param>
        public override void Render(Device device, Model.Model model, System.Collections.Generic.IEnumerable<Canguro.Model.LineElement> lines, RenderOptions options, List<Item> itemsInView)
        {
            // Get resource cache instance
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;

            // Is picking mode active? We'll need this state later
            bool pickingMode = GraphicViewManager.Instance.DrawingPickingSurface;
            // Get default painting color for selected joints
            selectedColor = Properties.Settings.Default.SelectedDefaultColor.ToArgb();
            int vertsSize = 0;

            rc.ActiveStream = ResourceStreamType.Lines;
            PositionColoredPackage package = (PositionColoredPackage)rc.CaptureBuffer(ResourceStreamType.Lines, false, true);

            if (lines != null && ((IList<LineElement>)lines).Count > 0)
            {
                foreach (LineElement l in lines)
                {
                    // Lines just matter id they are visible
                    if (l != null && l.IsVisible)
                    {
                        renderLine(rc, l, pickingMode, ref package, ref vertsSize, options);
                        drawReleaseIfNeeded(rc, l, options);
                    }
                }
            }
            else if (itemsInView != null)
            {
                if (itemsInView.Count <= 0)
                    GetItemsInView(itemsInView);

                if (itemsInView.Count > 0)
                {
                    LineElement l;

                    foreach (Item item in itemsInView)
                    {
                        l = item as LineElement;
                        // Lines just matter id they are visible
                        if (l != null && l.IsVisible)
                        {
                            renderLine(rc, l, pickingMode, ref package, ref vertsSize, options);
                            if ((options.OptionsShown & RenderOptions.ShowOptions.Releases) != 0)
                                rc.GadgetManager.GadgetList.AddLast(new Gadget(l, GadgetType.Release));
                        }
                    }
                }
            } 

            // Flush remaining vertices
            rc.ReleaseBuffer(vertsSize, 0, ResourceStreamType.Lines);
            rc.Flush(ResourceStreamType.Lines);
        }
    }
}

#region Code used instead of new if-else block in Render method
//// Sweep every line in the model
//foreach (Model.LineElement l in lines)
//{
//    int lineColor;
//    // Lines just matter id they are visible
//    if (l != null && l.IsVisible)
//    {
//        // Define line color according to the rendering state: picking mode, or display scene
//        lineColor = getLineColor(rc, l, pickingMode);

//        if (vertsSize + 2 >= package.NumVertices - 1)
//        {
//            rc.ReleaseBuffer(vertsSize, 0, package.Stream);
//            package = (PositionColoredPackage)rc.CaptureBuffer(package.Stream, 0, 2, true);
//            vertsSize = 0;
//        }
//        vertsSize += 2;

//        // Inicia código NO SEGURO
//        unsafe
//        {
//            // Get line beginning and feed as starting vertex 
//            package.VBPointer->Position = l.I.Position;
//            package.VBPointer->Color = lineColor;
//            package.VBPointer++;

//            // Get line ending and feed as ending vertex
//            package.VBPointer->Position = l.J.Position;
//            package.VBPointer->Color = lineColor;
//            package.VBPointer++;
//        } // Termina código NO SEGURO
//    }
//}
#endregion