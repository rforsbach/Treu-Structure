using System;
namespace Canguro.Controller.Tracking
{
    interface ITrackingHelper
    {
        void MouseMove(Canguro.View.GraphicView graphicView, System.Drawing.Point startPt, System.Drawing.Point lastPt);
        void Paint(Microsoft.DirectX.Direct3D.Device device);
        void SetColor(System.Drawing.Color colorFill, System.Drawing.Color colorLine);
    }
}
