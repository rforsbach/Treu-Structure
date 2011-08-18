using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.View;

namespace Canguro.Controller.Tracking
{
    class HoverPainter
    {
        CustomVertex.TransformedColored[] pointVerts = new CustomVertex.TransformedColored[1];

        /// <summary>
        /// Paint some text over the active viewport at the indicated position and color
        /// </summary>
        /// <param name="text"> A string containing the text to show </param>
        /// <param name="pos"> Position in world coordinates where text muts be displayed </param>
        /// <param name="color"> Color for the text </param>
        public void DrawText(string text, Rectangle rect, System.Drawing.Color color)
        {
            // Get the resource cache because the Font is needed
            Canguro.View.ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            // Check if Font object has a valid value
            if (rc.LabelFont != null && !rc.LabelFont.Disposed)
                rc.LabelFont.DrawText(null, text, rect, DrawTextFormat.Left, color);        // Draw text on the screen
        }

        public Rectangle MeasureText(string text)
        {
            // Get the resource cache because the Font is needed
            Canguro.View.ResourceManager rc = GraphicViewManager.Instance.ResourceManager;
            
            // Get bounding rectangle
            return rc.LabelFont.MeasureString(null, text, DrawTextFormat.Left, GraphicViewManager.Instance.PrintingHiResImage ? Color.Black : Color.White);
        }

        public void PaintPoint(Device device, Vector3 screenPosition)
        {
            Cull cull = device.RenderState.CullMode;
            bool alphaEnable = device.RenderState.AlphaBlendEnable;

            device.RenderState.CullMode = Cull.None;
            device.RenderState.AlphaBlendEnable = true;

            device.RenderState.SourceBlend = Blend.BothSourceAlpha;
            device.RenderState.DestinationBlend = Blend.DestinationColor;

            pointVerts[0].X = screenPosition.X;
            pointVerts[0].Y = screenPosition.Y;
            pointVerts[0].Color = Color.Red.ToArgb();

            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.DrawUserPrimitives(PrimitiveType.PointList, 1, pointVerts);

            device.RenderState.AlphaBlendEnable = alphaEnable;
            device.RenderState.CullMode = cull;
        }

        public void PaintLine(Device device, Canguro.Model.LineElement line)
        {
            PaintLine(device, line.I.Position, line.J.Position);
        }

        public void PaintLine(Device device, Vector3 iPos, Vector3 jPos)
        {
            Cull cull = device.RenderState.CullMode;
            bool alphaEnable = device.RenderState.AlphaBlendEnable;

            device.RenderState.CullMode = Cull.None;
            device.RenderState.AlphaBlendEnable = true;

            device.RenderState.SourceBlend = Blend.BothSourceAlpha;
            device.RenderState.DestinationBlend = Blend.DestinationColor;

            Line l1 = GraphicViewManager.Instance.ResourceManager.SnapLines[2];
            Line l2 = GraphicViewManager.Instance.ResourceManager.SnapLines[1];

                View.GraphicView gv = View.GraphicViewManager.Instance.ActiveView;
                gv.Project(ref iPos);
                gv.Project(ref jPos);

                if (GraphicViewManager.Instance.Layout != GraphicViewManager.ViewportsLayout.OneView)
                {
                    Viewport vp = device.Viewport;
                    iPos.X -= vp.X;
                    iPos.Y -= vp.Y;
                    jPos.X -= vp.X;
                    jPos.Y -= vp.Y;
                }

                l1.Begin();
                    l1.Draw(new Vector2[] {new Vector2(iPos.X, iPos.Y), new Vector2(jPos.X, jPos.Y)}, Color.FromArgb(192, Color.SteelBlue));
                l1.End();

                l2.Begin();
                    l2.Draw(new Vector2[] { new Vector2(iPos.X, iPos.Y), new Vector2(jPos.X, jPos.Y) }, Color.FromArgb(128, Color.White));
                l2.End();

            device.RenderState.AlphaBlendEnable = alphaEnable;
            device.RenderState.CullMode = cull;
        }
    }
}
