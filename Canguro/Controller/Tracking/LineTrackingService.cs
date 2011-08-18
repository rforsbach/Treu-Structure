using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Controller.Tracking
{
    /// <summary>
    /// Pinta una línea que sigue al Mouse tras seleccionar algún Vertex3D.
    /// Primero se selecciona un Vertex3D y después otro que termina el ciclo.
    /// Utiliza el MouseMove para seguir y SetPoint para fijar el punto inicial.
    /// </summary>
    public class LineTrackingService : TrackingService
    {
        public static readonly LineTrackingService Instance = new LineTrackingService();

        private Vector3 startVec;
        private Point startPt, lastPt;
        CustomVertex.TransformedColored[] verts;

        private LineTrackingService() 
        {
            verts = new CustomVertex.TransformedColored[2];
            for (int i = 0; i < 2; ++i)
                verts[i].Color = Color.Aqua.ToArgb();
        }

        public override void SetPoint(Vector3 vecInternational)
        {
            startVec = vecInternational;
            reset();
        }

        protected override void reset()
        {
            Vector3 vec = startVec;
            graphicView.Project(ref vec);
            startPt.X = (int)vec.X;
            startPt.Y = (int)vec.Y;
        }

        public override void MouseMove(System.Drawing.Point pt)
        {
            int minX = graphicView.Viewport.X, maxX = minX + graphicView.Viewport.Width;
            int minY = graphicView.Viewport.Y, maxY = minY + graphicView.Viewport.Height;

            lastPt = pt;
            lastPt.X = (lastPt.X < minX) ? minX : ((lastPt.X > maxX) ? maxX : lastPt.X);
            lastPt.Y = (lastPt.Y < minY) ? minY : ((lastPt.Y > maxY) ? maxY : lastPt.Y);

            // Set vertices positions
            verts[0].X = startPt.X; verts[0].Y = startPt.Y; verts[0].Z = 0.1f;
            verts[1].X = lastPt.X;  verts[1].Y = lastPt.Y;  verts[1].Z = 0.1f;
        }

        public override void Paint(Device device)
        {
            Cull cull = device.RenderState.CullMode;
            //bool alphaEnable = device.RenderState.AlphaBlendEnable;

            device.RenderState.CullMode = Cull.None;
            //device.RenderState.AlphaBlendEnable = true;
            //    device.RenderState.SourceBlend = Blend.BothSourceAlpha;
            //    device.RenderState.DestinationBlend = Blend.DestinationColor;

                //device.SetStreamSource(0, m_roiVB, 0);
                device.VertexFormat = CustomVertex.TransformedColored.Format;
                //device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
                device.DrawUserPrimitives(PrimitiveType.LineList, 1, verts);
            //device.RenderState.AlphaBlendEnable = alphaEnable;
            device.RenderState.CullMode = cull;
        }    
    }
}
