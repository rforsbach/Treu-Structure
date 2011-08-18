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
    public class VectorTrackingService : TrackingService
    {
        public static readonly VectorTrackingService Instance = new VectorTrackingService();
        private bool closeArrow = false;
        public const int ArrowSize = 8;

        private Vector3 startVec;
        private Vector2 startV, lastV;
        private Point startPt, lastPt;
        CustomVertex.TransformedColored[] verts;

        private VectorTrackingService() 
        {
            verts = new CustomVertex.TransformedColored[8];
            for (int i = 0; i < 8; ++i)
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

            // Set vertices positions (line)
            verts[0].X = startPt.X; verts[0].Y = startPt.Y; verts[0].Z = 0.1f;
            verts[1].X = lastPt.X;  verts[1].Y = lastPt.Y;  verts[1].Z = 0.1f;

            startV.X = startPt.X; startV.Y = startPt.Y;            
            lastV.X  = lastPt.X;  lastV.Y  = lastPt.Y;
            
            Vector2 vecc, vec1, vec2;
            Vector2 vecP, vec = lastV - startV;
            float len = vec.Length();

            // Calculate arrow coordinates
            vec.Normalize();
            vecP = new Vector2(-vec.Y, vec.X);
            vecP *= ArrowSize;

            vecc = startV + (vec * (len - ArrowSize));
            vec1 = vecc + vecP;
            vec2 = vecc - vecP;

            // Set vertices positions (arrow)
            verts[2] = verts[1];
            verts[3].X = vec1.X; verts[3].Y = vec1.Y; verts[3].Z = 0.1f;
            verts[4] = verts[1];
            verts[5].X = vec2.X; verts[5].Y = vec2.Y; verts[5].Z = 0.1f;
            if (closeArrow)
            {
                verts[6] = verts[3];
                verts[7] = verts[5];
            }
        }

        public override void Paint(Device device)
        {
            Cull cull = device.RenderState.CullMode;
            bool alphaEnable = device.RenderState.AlphaBlendEnable;

            device.RenderState.CullMode = Cull.None;
            //device.RenderState.AlphaBlendEnable = true;
            //    device.RenderState.SourceBlend = Blend.BothSourceAlpha;
            //    device.RenderState.DestinationBlend = Blend.DestinationColor;

                //device.SetStreamSource(0, m_roiVB, 0);
                device.VertexFormat = CustomVertex.TransformedColored.Format;
                //device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
                
                int numLines = (closeArrow) ? 4 : 3;
                device.DrawUserPrimitives(PrimitiveType.LineList, numLines, verts);
            //device.RenderState.AlphaBlendEnable = alphaEnable;
            device.RenderState.CullMode = cull;
        }    

        public bool CloseArrow
        {
            get
            {
                return closeArrow;
            }
            set
            {
                closeArrow = value;
            }
        }
    }
}
