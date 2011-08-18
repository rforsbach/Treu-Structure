using System;
using System.Collections.Generic;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Canguro.View;

namespace Canguro.Controller.Snap
{
    public class SnapPainter
    {
        #region Point Symbol drawing callers...
        public void PaintPointSymbol (Device device, GraphicView activeView, Vector3 magnet, PointMagnetType type, byte alpha)
        {
            // Must be called within a Begin-EndScene block
            Vector3 point2D = magnet;
            
            activeView.Project(ref point2D);

            drawPointSymbol(device, point2D.X, point2D.Y, type, alpha);
        }

        public void PaintPointSymbol(Device device, GraphicView activeView, Point magnet, PointMagnetType type, byte alpha)
        {
            drawPointSymbol(device, magnet.X, magnet.Y, type, alpha);
        }
        #endregion

        #region Point symbol painters...
        private void drawPointSymbol(Device device, float x, float y, PointMagnetType type, byte alpha)
        {
            // Get color
            Color color;
            if (alpha == 255)
                color = Color.Gold;
            else
                color = Color.FromArgb(alpha, Color.OrangeRed);
            
            Cull cull = device.RenderState.CullMode;
            bool alphaEnable = device.RenderState.AlphaBlendEnable;

            device.RenderState.CullMode = Cull.None;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.BothSourceAlpha;
            device.RenderState.DestinationBlend = Blend.DestinationColor;

            switch (type)
            {
                case PointMagnetType.EndPoint:
                    drawEndPoint(device, x, y, color);
                    break;
                case PointMagnetType.Intersection:
                    drawIntersectPoint(device, x, y, color);
                    break;
                case PointMagnetType.MidPoint:
                    drawMidPoint(device, x, y, color);
                    break;
                case PointMagnetType.Perpendicular:
                    drawPerpPoint(device, x, y, color);
                    break;
                case PointMagnetType.SimplePoint:
                    drawSimplePoint(device, x, y, color);
                    break;
            }

            device.RenderState.AlphaBlendEnable = alphaEnable;
            device.RenderState.CullMode = cull;
        }

        private void drawSimplePoint(Device device, float x, float y, Color colorWithAlpha)
        {
            CustomVertex.TransformedColored[] verts = new CustomVertex.TransformedColored[1];

            verts[0].X = x;
            verts[0].Y = y;
            verts[0].Z = 0.1f;
            verts[0].Color = colorWithAlpha.ToArgb();

            float lastSize = device.RenderState.PointSize;
            device.RenderState.PointSize = 7;
            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.DrawUserPrimitives(PrimitiveType.PointList, 1, verts);
            device.RenderState.PointSize = lastSize;
        }

        private void drawEndPoint(Device device, float x, float y, Color colorWithAlpha)
        {
            Line line = GraphicViewManager.Instance.ResourceManager.SnapLines[0];
            int midWidth = 5;

            line.Begin();
                line.Draw(new Vector2[] { new Vector2(x - midWidth, y - midWidth), new Vector2(x + midWidth, y - midWidth) }, colorWithAlpha);
                line.Draw(new Vector2[] { new Vector2(x + midWidth, y - midWidth), new Vector2(x + midWidth, y + midWidth) }, colorWithAlpha);
                line.Draw(new Vector2[] { new Vector2(x + midWidth, y + midWidth), new Vector2(x - midWidth, y + midWidth) }, colorWithAlpha);
                line.Draw(new Vector2[] { new Vector2(x - midWidth, y + midWidth), new Vector2(x - midWidth, y - midWidth) }, colorWithAlpha);
            line.End();
        }

        private void drawIntersectPoint(Device device, float x, float y, Color colorWithAlpha)
        {
            Line line = GraphicViewManager.Instance.ResourceManager.SnapLines[0];
            int midWidth = 6;

            line.Begin();
                line.Draw(new Vector2[] { new Vector2(x - midWidth, y - midWidth), new Vector2(x + midWidth, y + midWidth) }, colorWithAlpha);
                line.Draw(new Vector2[] { new Vector2(x + midWidth, y - midWidth), new Vector2(x - midWidth, y + midWidth) }, colorWithAlpha);
            line.End();
        }

        private void drawMidPoint(Device device, float x, float y, Color colorWithAlpha)
        {
            Line line = GraphicViewManager.Instance.ResourceManager.SnapLines[0];
            int length = 10;
            float cos30 = 0.8666f;
            float sin30 = 0.5f;

            line.Begin();
                line.Draw(new Vector2[] { new Vector2(x, y - cos30 * length), new Vector2(x + cos30 * length, y + sin30 * length) }, colorWithAlpha);
                line.Draw(new Vector2[] { new Vector2(x + cos30 * length, y + sin30 * length), new Vector2(x - cos30 * length, y + sin30 * length) }, colorWithAlpha);
                line.Draw(new Vector2[] { new Vector2(x - cos30 * length, y + sin30 * length), new Vector2(x, y - cos30 * length) }, colorWithAlpha);
            line.End();
        }

        private void drawPerpPoint(Device device, float x, float y, Color colorWithAlpha)
        {
            Line line = GraphicViewManager.Instance.ResourceManager.SnapLines[0];
            int midWidth = 5;

            line.Begin();
                line.Draw(new Vector2[] { new Vector2(x - 2.0f * midWidth, y), new Vector2(x, y) }, colorWithAlpha);
                line.Draw(new Vector2[] { new Vector2(x, y), new Vector2(x, y + 2.0f * midWidth) }, colorWithAlpha);
                line.Draw(new Vector2[] { new Vector2(x - 2.0f * midWidth, y + 2.0f * midWidth), new Vector2(x + 1.5f * midWidth, y + 2.0f * midWidth) }, colorWithAlpha);
                line.Draw(new Vector2[] { new Vector2(x - 2.0f * midWidth, y + 2.0f * midWidth), new Vector2(x - 2.0f * midWidth, y - 1.5f * midWidth) }, colorWithAlpha);
            line.End();
        }
        #endregion

        #region Line drawing callers...
        public void PaintLineSegment(Device device, GraphicView activeView, Point startPoint, Point endPoint, LineMagnetType type)
        {
            drawSegment(device, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, type);
        }

        public void PaintLineSegment(Device device, GraphicView activeView, Vector3 startPos, Point endPoint, LineMagnetType type)
        {
            Vector3 pos2D = startPos;

            activeView.Project(ref pos2D);

            drawSegment(device, pos2D.X, pos2D.Y, endPoint.X, endPoint.Y, type);
        }

        public void PaintLineSegment(Device device, GraphicView activeView, Vector3 startPos, Vector3 endPos, LineMagnetType type)
        {
            activeView.Project(ref startPos);
            activeView.Project(ref endPos);

            drawSegment(device, startPos.X, startPos.Y, endPos.X, endPos.Y, type);
        }
        #endregion

        #region Line painters...
        private void drawSegment(Device device, float x0, float y0, float x1, float y1, LineMagnetType type)
        {
            int color = Color.FromArgb(128, Color.Gold).ToArgb();
            uint stipplePattern = 0xffffffff;

            switch (type)
            {
                case LineMagnetType.FollowProjection:
                    break;
                case LineMagnetType.FollowXAxis:
                    color = Color.FromArgb(128, Color.Red).ToArgb();
                    break;
                case LineMagnetType.FollowYAxis:
                    color = Color.FromArgb(128, Color.LightGreen).ToArgb();
                    break;
                case LineMagnetType.FollowZAxis:
                    color = Color.FromArgb(128, Color.Blue).ToArgb();
                    break;
                case LineMagnetType.FollowHelper:
                    color = Color.FromArgb(128, Color.SandyBrown).ToArgb();
                    break;
            }
            followAxis(device, x0, y0, x1, y1, color, (int)stipplePattern);
        }

        private void followAxis(Device device, float x0, float y0, float x1, float y1, int color, int stipplePattern)
        {
            Line line = GraphicViewManager.Instance.ResourceManager.SnapLines[1];
            line.Pattern = stipplePattern;
            float w = line.Width;

            line.Width = 2.0f;

            Cull cull = device.RenderState.CullMode;
            bool alphaEnable = device.RenderState.AlphaBlendEnable;

            device.RenderState.CullMode = Cull.None;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.BothSourceAlpha;
            device.RenderState.DestinationBlend = Blend.DestinationColor;

            line.Begin();
            line.Draw(new Vector2[] { new Vector2(x0, y0), new Vector2(x1, y1) }, color);
            line.End();

            device.RenderState.AlphaBlendEnable = alphaEnable;
            device.RenderState.CullMode = cull;

            line.Width = w;


        }
        #endregion
    }
}
