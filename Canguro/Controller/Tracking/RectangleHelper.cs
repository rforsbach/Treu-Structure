using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Controller.Tracking
{
    public class RectangleHelper : Canguro.Controller.Tracking.ITrackingHelper
    {
        CustomVertex.TransformedColored[] verts;
        CustomVertex.TransformedColored[] lineVertices;

        public RectangleHelper(Color colorFill, Color colorLine) 
        {
            verts = new CustomVertex.TransformedColored[4];
            lineVertices = new CustomVertex.TransformedColored[5];

            SetColor(colorFill, colorLine);
        }

        public void SetColor(Color colorFill, Color colorLine)
        {
            for (int i = 0; i < 4; ++i)
                verts[i].Color = colorFill.ToArgb();

            for (int i = 0; i < 5; ++i)
                lineVertices[i].Color = colorFill.ToArgb();
        }

        public void MouseMove(View.GraphicView graphicView, System.Drawing.Point startPt, System.Drawing.Point lastPt)
        {
            int minX = graphicView.Viewport.X, maxX = minX + graphicView.Viewport.Width;
            int minY = graphicView.Viewport.Y, maxY = minY + graphicView.Viewport.Height;
           
            lastPt.X = (lastPt.X < minX) ? minX : ((lastPt.X > maxX) ? maxX : lastPt.X);
            lastPt.Y = (lastPt.Y < minY) ? minY : ((lastPt.Y > maxY) ? maxY : lastPt.Y);

            // Set vertices positions
            verts[0].X = startPt.X; verts[0].Y = startPt.Y;
            verts[1].X = lastPt.X; verts[1].Y = startPt.Y;
            verts[2].X = startPt.X; verts[2].Y = lastPt.Y;
            verts[3].X = lastPt.X; verts[3].Y = lastPt.Y;

            // Outline...
            lineVertices[0] = verts[0];
            lineVertices[1] = verts[1];
            lineVertices[2] = verts[3];
            lineVertices[3] = verts[2];
            lineVertices[4] = verts[0];
        }

        public void Paint(Device device)
        {
            Cull cull = device.RenderState.CullMode;
            bool alphaEnable = device.RenderState.AlphaBlendEnable;
            ShadeMode shadeMode = device.RenderState.ShadeMode;

            device.RenderState.CullMode = Cull.None;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.ShadeMode = ShadeMode.Flat;

            device.RenderState.SourceBlend = Blend.BothSourceAlpha;
            device.RenderState.DestinationBlend = Blend.DestinationColor;

            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, verts);

            device.RenderState.AlphaBlendEnable = alphaEnable;
            device.RenderState.CullMode = cull;

            device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, lineVertices);

            device.RenderState.ShadeMode = shadeMode;
        }
    }
}
