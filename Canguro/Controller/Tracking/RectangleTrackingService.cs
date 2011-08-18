using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Canguro.Controller.Tracking
{
    /// <summary>
    /// Pinta un rectángulo como de selección en una ventana.
    /// </summary>
    public class RectangleTrackingService : TrackingService
    {
        public static readonly RectangleTrackingService Instance = new RectangleTrackingService();

        Color defaultColorFill = Color.FromArgb(120, 173, 200, 255);
        Color defaultColorLine = Color.Aqua;
        RectangleHelper rectHelper;

        private Point startPt;
        private RectangleTrackingService() 
        {
            rectHelper = new RectangleHelper(defaultColorFill, defaultColorLine);
        }

        public void SetColor()
        {
            rectHelper.SetColor(defaultColorFill, defaultColorLine);
        }

        public void SetColor(Color colorFill, Color colorLine)
        {
            rectHelper.SetColor(colorFill, colorLine);
        }

        public override void SetPoint(System.Drawing.Point pt)
        {
            startPt = pt;
            rectHelper.SetColor(defaultColorFill, defaultColorLine);
        }

        public override void MouseMove(System.Drawing.Point pt)
        {
            rectHelper.MouseMove(graphicView, startPt, pt);
        }

        public override void Paint(Device device)
        {
            rectHelper.Paint(device);
        }
    }
}
