using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.View.Renderer
{
    /// <summary>
    /// Base class for item (joint, line, area) rendering
    /// </summary>
    public abstract class ItemRenderer
    {
        /// <summary>
        /// Virtual method for updating any resources needed by a renderer
        /// </summary>
        public virtual void UpdateResources() {}

        /// <summary>
        /// Virtual methos for disposing any resources used by a renderer
        /// </summary>
        public virtual void DisposeResources() {}

        /// <summary>
        /// Paint some text over the active viewport at the indicated position and color
        /// </summary>
        /// <param name="text"> A string containing the text to show </param>
        /// <param name="pos"> Position in world coordinates where text muts be displayed </param>
        /// <param name="color"> Color for the text </param>
        public void DrawItemText(string text, Vector3 pos, System.Drawing.Color color)
        {
            // Get the active view
            GraphicView gv = GraphicViewManager.Instance.ActiveView;
            // Get the resource cache because the Font is needed
            ResourceManager rc = GraphicViewManager.Instance.ResourceManager;

            // Get a point in 2D as drawing position in screen
            System.Drawing.Point pos2D = System.Drawing.Point.Empty;

            // Project to screen the world position 
            Vector3 aux = pos;
            gv.Project(ref aux);
            pos2D.X = (int)aux.X + 8;
            pos2D.Y = (int)aux.Y;

            // Check if Font object has a valid value
            if (rc.LabelFont != null && !rc.LabelFont.Disposed)
                rc.LabelFont.DrawText(null, text, pos2D, GraphicViewManager.Instance.PrintingHiResImage ? System.Drawing.Color.Black : color);        // Draw text on the screen
        }

        protected void GetItemsInView(List<Canguro.Model.Item> itemsInView)
        {
            if (itemsInView != null)
            {
                Viewport vp = Canguro.View.GraphicViewManager.Instance.ActiveView.Viewport;
                Canguro.Controller.Controller.Instance.SelectionCommand.SelectWindow(Canguro.View.GraphicViewManager.Instance.ActiveView, vp.X, vp.Y, vp.X + vp.Width, vp.Y + vp.Height, itemsInView, Canguro.Controller.SelectionFilter.Joints | Canguro.Controller.SelectionFilter.Lines);
            }
        }

        /// <summary>
        /// Draws a colored side bar with values as the one for stress render mode
        /// </summary>
        /// <param name="device">The DirectX device</param>
        /// <param name="offsetX">The distance in the X direction from the right border of the viewport to the x coordinate of the value texts. Usually between 50 and 100.</param>
        /// <param name="verticalBarStart">A value between [0, 1[ that states the vertical START of the side bar in terms of the device viewport's height. This usually is 0 and has to be less than verticalBarEnd</param>
        /// <param name="verticalBarEnd">A value between ]0, 1] that states the vertical END of the side bar in terms of the device viewport's height. This usually is 1 and has to be greater that verticalBarStart</param>
        /// <param name="numberFormat">The format of the number as used by ToString. Usually "G5".</param>
        /// <param name="positions">An array of positions between [0, 1] where a ratio is reported and the color is calculated</param>
        /// <param name="minValue">The minimum value (reported at the start of the side bar)</param>
        /// <param name="maxValue">The maximum value (reported at the end of the side bar)</param>
        /// <param name="unit">The unit of the ratio and min/maxValue</param>
        /// <param name="ratio2ColorDelegate">A delegate to calculate an appropriate color based on the ratio at some position</param>
        /// <param name="optionalValues">An optional array of strings that represent the values to be displayed at each position. If null, these values are automatically calculated based on min/maxValue. If not null, the array has to be of the same size as positions.</param>
        internal void DrawColorSideBar(Device device, float offsetX, float verticalBarStart, float verticalBarEnd, string numberFormat, float[] positions, float minValue, float maxValue, Model.UnitSystem.Units unit, Utility.ColorUtils.GetColorFromRatioDelegate ratio2ColorDelegate, string[] optionalValues)
        {
            int numPts = positions.Length;

            // Draw stress scale bar
            CustomVertex.TransformedColored[] scaleBarVerts = new CustomVertex.TransformedColored[numPts * 2];
            GraphicViewManager gvm = GraphicViewManager.Instance;
            Viewport vp = gvm.ActiveView.Viewport;
            float vpY0 = vp.Y, vpHeight = vp.Height;
            vpY0 += verticalBarStart * vpHeight;
            vpHeight *= (verticalBarEnd - verticalBarStart);

            ShadeMode oldShadeMode = device.RenderState.ShadeMode;
            device.RenderState.ShadeMode = ShadeMode.Gouraud;
            try
            {
                // Set vertex positions
                for (int i = 0; i < numPts; i++)
                {
                    scaleBarVerts[2 * i].Position = new Vector4(vp.X + vp.Width - (offsetX + 5), vpY0 + 25 + (vpHeight - 50) * positions[i], 0, 1);
                    scaleBarVerts[2 * i + 1].Position = new Vector4(vp.X + vp.Width - (offsetX + 25), vpY0 + 25 + (vpHeight - 50) * positions[i], 0, 1);
                }

                // Set vertex colors
                for (int i = 0; i < numPts; i++)
                {
                    scaleBarVerts[2 * i].Color = ratio2ColorDelegate(minValue + (maxValue - minValue) * positions[i]);
                    scaleBarVerts[2 * i + 1].Color = ratio2ColorDelegate(minValue + (maxValue - minValue) * positions[i]);
                    //scaleBarVerts[2 * i].Color = ratio2ColorDelegate(values[i]);
                    //scaleBarVerts[2 * i + 1].Color = ratio2ColorDelegate(values[i]);
                }

                device.VertexFormat = CustomVertex.TransformedColored.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, numPts * 2 - 2, scaleBarVerts);
            }
            finally
            {
                device.RenderState.ShadeMode = ShadeMode.Flat;
            }

            // Draw Texts
            int fontHeight = -6, y0 = (int)(fontHeight + vpY0 + 25 + (vpHeight - 50) * (minValue / (minValue - maxValue)));
            Model.UnitSystem.UnitSystem unitSystem = Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;
            string unitText = unitSystem.UnitName(unit);
            if (!string.IsNullOrEmpty(unitText))
                unitText = " [" + unitText + "]";

            for (int i = 0; i < numPts; i++)
            {
                if (optionalValues == null)
                    gvm.ResourceManager.LabelFont.DrawText(null, unitSystem.FromInternational((minValue + (maxValue - minValue) * positions[i]), unit).ToString(numberFormat) + unitText,
                        new System.Drawing.Point((int)(vp.X + vp.Width - offsetX), (int)(fontHeight + vpY0 + 25 + (vpHeight - 50) * positions[i])),
                        GraphicViewManager.Instance.PrintingHiResImage ? System.Drawing.Color.Black : System.Drawing.Color.White);
                else
                    gvm.ResourceManager.LabelFont.DrawText(null, optionalValues[i] + unitText,
                        new System.Drawing.Point((int)(vp.X + vp.Width - offsetX), (int)(fontHeight + vpY0 + 25 + (vpHeight - 50) * positions[i])),
                        GraphicViewManager.Instance.PrintingHiResImage ? System.Drawing.Color.Black : System.Drawing.Color.White);
            }
        }
    }
}
