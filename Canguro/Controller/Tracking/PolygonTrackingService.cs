using System;
using System.Drawing;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Controller.Tracking
{
    /// <summary>
    /// Pinta un polígono cerrado que sigue al Mouse tras seleccionar una lista de Vertex3D's.
    /// Se van seleccionando Vertex3D y siempre se pinta lo necesario para cerrar el contorno.
    /// Se van acumulando puntos y formando el contorno hasta que se desactive (o se le dé Reset).
    /// Utiliza el MouseMove para seguir y SetPoint para fijar puntos.
    /// </summary>
    public class PolygonTrackingService : TrackingService
    {
        CustomVertex.TransformedColored[] verts;
        CustomVertex.TransformedColored[] lineVertices;

        public static readonly PolygonTrackingService Instance = new PolygonTrackingService();

        Color defaultColorFill = Color.FromArgb(120, 173, 200, 255);
        Color defaultColorLine = Color.Aqua;

        Queue<Vector3> points;
        Point lastPt;
        private PolygonTrackingService()
        {
            points = new Queue<Vector3>();

            verts = new CustomVertex.TransformedColored[100];
            lineVertices = new CustomVertex.TransformedColored[100];
            SetColor();
        }

        public void SetColor(Color colorFill, Color colorLine)
        {
            for (int i = 0; i < 100; ++i)
                verts[i].Color = colorFill.ToArgb();

            for (int i = 0; i < 100; ++i)
                lineVertices[i].Color = colorFill.ToArgb();
        }

        public void SetColor()
        {
            SetColor(defaultColorFill, defaultColorLine);
        }

        public override void SetPoint(System.Drawing.Point pt)
        {
            Vector3 vec = new Vector3(pt.X, pt.Y, 0.5f);
            graphicView.Unproject(ref vec);

            if (points.Count == 0)
                MouseMove(pt);

            points.Enqueue(vec);
        }

        public override void SetPoint(Vector3 vecInternational)
        {
            Vector3 vec = vecInternational;
            graphicView.Project(ref vec);

            Point pt = new Point((int)vec.X, (int)vec.Y);
            if (points.Count == 0)
                MouseMove(pt);

            points.Enqueue(vecInternational);
        }

        public override void MouseMove(System.Drawing.Point pt)
        {
            lastPt = pt;
            int activeVertex = points.Count;

            int minX = graphicView.Viewport.X, maxX = minX + graphicView.Viewport.Width;
            int minY = graphicView.Viewport.Y, maxY = minY + graphicView.Viewport.Height;

            lastPt.X = (lastPt.X < minX) ? minX : ((lastPt.X > maxX) ? maxX : lastPt.X);
            lastPt.Y = (lastPt.Y < minY) ? minY : ((lastPt.Y > maxY) ? maxY : lastPt.Y);

            // Set vertices positions
            verts[activeVertex].X = lastPt.X; verts[activeVertex].Y = lastPt.Y;

            // Outline...
            lineVertices[activeVertex] = verts[activeVertex];
            lineVertices[activeVertex + 1] = verts[0];
        }

        public override void Paint(Device device)
        {
            int numPoints = points.Count;
            if (numPoints < 1) return; // Cannot paint polygon with less than 3 points (2 stored + mouse position)

            device.VertexFormat = CustomVertex.TransformedColored.Format;

            recalcPoints();

            if (numPoints > 1)
            {
                Cull cull = device.RenderState.CullMode;
                bool alphaEnable = device.RenderState.AlphaBlendEnable;
                ShadeMode shadeMode = device.RenderState.ShadeMode;

                // Draw Triangles
                device.RenderState.CullMode = Cull.None;
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.ShadeMode = ShadeMode.Flat;

                device.RenderState.SourceBlend = Blend.BothSourceAlpha;
                device.RenderState.DestinationBlend = Blend.DestinationColor;

                device.DrawUserPrimitives(PrimitiveType.TriangleFan, numPoints - 1, verts);

                device.RenderState.AlphaBlendEnable = alphaEnable;
                device.RenderState.CullMode = cull;
                device.RenderState.ShadeMode = shadeMode;
            }

            // Draw Polygon Contour
            device.DrawUserPrimitives(PrimitiveType.LineStrip, numPoints + ((numPoints > 1) ? 1 : 0), lineVertices);

        }

        private void recalcPoints()
        {
            int i=0, activeVertex = points.Count;

            foreach (Vector3 v in points)
            {
                Vector3 vv = v;
                graphicView.Project(ref vv);

                // Set vertices positions
                verts[i].X = vv.X; verts[i].Y = vv.Y;

                // Outline...
                lineVertices[i] = verts[i];
                i++;
            }
        }

        public override void Start()
        {
            points.Clear();
        }
    }
}
